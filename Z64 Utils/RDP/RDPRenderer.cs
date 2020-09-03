using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK;
using System.Drawing;
using N64;
using Syroot.BinaryData;
using Z64;

namespace RDP
{
    public class RDPRenderer
    {
        public class Config
        {
            public bool RenderTextures { get; set; } = true;
        }

        public struct Segment
        {
            public const int COUNT = 16;

            public byte[] Data { get; set; }
            public uint Address { get; set; }
            public string Label { get; set; }

            public bool IsVram() => Data == null;

            public static Segment FromVram(uint addr, string label) => new Segment() { Data = null, Address = addr, Label = label, };
            public static Segment FromBytes(byte[] data, string label) => new Segment() { Data = data, Address = 0, Label = label, };
        }

        public Segment[] Segments { get; private set; }
        public uint RenderErrorAddr { get; private set; } = 0xFFFFFFFF;
        public string ErrorMsg { get; private set; } = null;
        public Config CurrentConfig { get; set; }



        List<F3DZEX.CommandInfo> _dlist;
        uint _vaddr;
        RDPVtx[] _vertices = new RDPVtx[32];
        int _curTexID;
        RDPEnum.G_IM_SIZ _loadTexSiz;
        RDPEnum.G_IM_FMT _renderTexFmt;
        RDPEnum.G_IM_SIZ _renderTexSiz;
        uint _curImgAddr;
        byte[] _loadTex;
        byte[] _renderTex;
        byte[] _curTLUT;
        int _curTexW;
        int _curTexH;
        bool _mirrorV;
        bool _mirrorH;
        Z64Game _game;
        bool _started;

        bool _reqDecodeTex = false;

        public bool RenderFailed() => ErrorMsg != null;

        public RDPRenderer(Z64Game game, Config cfg)
        {
            _started = false;
            _game = game;
            Segments = new Segment[Segment.COUNT];
            for (int i = 0; i < Segments.Length; i++)
                Segments[i] = Segment.FromVram(0, "[NULL]");
            CurrentConfig = cfg;
        }
        public void Start(uint vaddr)
        {
            List<F3DZEX.CommandInfo> dlist = GetDlist(vaddr);
            if (dlist == null)
            {
                dlist = new List<F3DZEX.CommandInfo>();
                ErrorMsg = "Error while Decoding DList";
                RenderErrorAddr = 0;
                return;
            }
            Start(vaddr, dlist);
        }
        public void Start(uint entrypoint, List<F3DZEX.CommandInfo> dlist)
        {
            _vaddr = entrypoint;
            _dlist = dlist;

            InitTex();
            _started = true;
        }

        public List<F3DZEX.CommandInfo> GetDlist(uint vaddr)
        {
            try
            {
                for (int off = 0; ; off += 8)
                {
                    byte[] ins = ReadBytes(vaddr + (uint)off, 8);
                    if (ins[0] == (byte)F3DZEX.OpCodeID.G_ENDDL)
                    {
                        return F3DZEX.DecodeDList(ReadBytes(vaddr, off + 8), 0);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public void UpdateErrors()
        {
            if (!_started)
                return;

            ErrorMsg = null;
            uint vaddr = _vaddr;
            foreach (var ins in _dlist)
            {
                try
                {
                    ProcessInstruction(ins);
                }
                catch (Exception ex)
                {
                    RenderErrorAddr = vaddr;
                    ErrorMsg = ex.Message;
                    break;
                }
                vaddr += (uint)ins.GetSize();
            }
        }

        public void Render()
        {
            if (!_started)
                return;

            if (RenderFailed())
                return;

            foreach (var ins in _dlist)
                ProcessInstruction(ins);
        }

        private void InitTex()
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out _curTexID);
            GL.BindTexture(TextureTarget.Texture2D, _curTexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        private void RenderVtx(RDPVtx vtx)
        {
            //GL.Color4(R, G, B, A);
            GL.Normal3(vtx.R, vtx.G, vtx.B);
            float x = (float)(vtx.TexX >> 5) / _curTexW;
            float y = (float)(vtx.TexY >> 5) / _curTexH;
            GL.TexCoord2(x, y);
            GL.Vertex3(vtx.X, vtx.Y, vtx.Z);
        }

        
        
        private byte[] ReadBytes(uint vaddr, int count)
        {
            SegmentedAddress addr = new SegmentedAddress(vaddr);
            string path = $"{vaddr:X8}";

            int resolveCount = 0;
            while (addr.Segmented && Segments[addr.SegmentId].IsVram() && addr.VAddr != 0)
            {
                if (resolveCount > 16)
                    throw new Exception($"Could not resolve address 0x{vaddr:X}. Path: {path}");

                path += $" -> {Segments[addr.SegmentId].Label}+0x{addr.SegmentOff:X}";
                addr = new SegmentedAddress(Segments[addr.SegmentId].Address + addr.SegmentOff);
                resolveCount++;
            }

            if (addr.VAddr == 0)
                throw new Exception($"Could not read 0x{count:X} bytes at address {path}");

            if (!addr.Segmented)
            {
                return _game.Memory.ReadBytes(vaddr, count);
            }
            else if (addr.SegmentId >= 0 && addr.SegmentId < Segment.COUNT)
            {
                var seg = Segments[addr.SegmentId];
                if (seg.IsVram())
                {
                    try
                    {
                        return _game.Memory.ReadBytes(seg.Address + addr.SegmentOff, count);
                    }
                    catch (Z64MemoryException ex)
                    {
                        throw new Exception($"Could not read 0x{count:X} bytes at address {path}");
                    }
                }
                else if (addr.SegmentOff + count <= seg.Data.Length)
                {
                    byte[] buff = new byte[count];
                    System.Buffer.BlockCopy(seg.Data, (int)addr.SegmentOff, buff, 0, count);
                    return buff;
                }
            }

            throw new Exception($"Could not read 0x{count:X} bytes at address {path}");
        }
        
        private void DecodeTexIfRequired()
        {
            if (_reqDecodeTex)
            {
                GL.Color3(Color.Transparent);
                GL.BindTexture(TextureTarget.Texture2D, _curTexID);
                _renderTex = N64Texture.Decode(_curTexW * _curTexH, _renderTexFmt, _renderTexSiz, _loadTex, _curTLUT);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _curTexW, _curTexH, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _renderTex);
                _reqDecodeTex = false;
            }
        }
        private unsafe void ProcessInstruction(F3DZEX.CommandInfo info)
        {
            switch (info.ID)
            {
                case F3DZEX.OpCodeID.G_SETPRIMCOLOR:
                    {
                        var cmd = info.Convert<F3DZEX.GSetPrimColor>();

                        GL.Color4(cmd.R, cmd.G, cmd.B, cmd.A);
                    } break;
                case F3DZEX.OpCodeID.G_VTX:
                    {
                        var cmd = info.Convert<F3DZEX.GVtx>();

                        cmd.vaddr += (uint)cmd.vbidx * 0x10;
                        for (int i = 0; i < cmd.numv; i++, cmd.vaddr += 0x10)
                            _vertices[cmd.vbidx + i] = new RDPVtx(ReadBytes(cmd.vaddr, 0x10));
                    } break;
                case F3DZEX.OpCodeID.G_TRI1:
                    {
                        var cmd = info.Convert<F3DZEX.GTri1>();
                        if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();

                            GL.Begin(PrimitiveType.Triangles);
                            RenderVtx(_vertices[cmd.v0]);
                            RenderVtx(_vertices[cmd.v1]);
                            RenderVtx(_vertices[cmd.v2]);
                            GL.End();
                        }
                        else
                        {
                            GL.Color3(0, 0, 0);
                            GL.Begin(PrimitiveType.Lines);
                            RenderVtx(_vertices[cmd.v0]);
                            RenderVtx(_vertices[cmd.v1]);
                            RenderVtx(_vertices[cmd.v1]);
                            RenderVtx(_vertices[cmd.v2]);
                            RenderVtx(_vertices[cmd.v2]);
                            RenderVtx(_vertices[cmd.v0]);

                            GL.End();
                        }
                    }
                    break;
                case F3DZEX.OpCodeID.G_TRI2:
                    {
                        var cmd = info.Convert<F3DZEX.GTri2>();
                        if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();

                            GL.Begin(PrimitiveType.Triangles);

                            RenderVtx(_vertices[cmd.v00]);
                            RenderVtx(_vertices[cmd.v01]);
                            RenderVtx(_vertices[cmd.v02]);
                            RenderVtx(_vertices[cmd.v10]);
                            RenderVtx(_vertices[cmd.v11]);
                            RenderVtx(_vertices[cmd.v12]);

                            GL.End();
                        }
                        else
                        {
                            GL.Color3(0, 0, 0);
                            GL.Begin(PrimitiveType.Lines);

                            RenderVtx(_vertices[cmd.v00]);
                            RenderVtx(_vertices[cmd.v01]);
                            RenderVtx(_vertices[cmd.v01]);
                            RenderVtx(_vertices[cmd.v02]);
                            RenderVtx(_vertices[cmd.v02]);
                            RenderVtx(_vertices[cmd.v00]);

                            RenderVtx(_vertices[cmd.v10]);
                            RenderVtx(_vertices[cmd.v11]);
                            RenderVtx(_vertices[cmd.v11]);
                            RenderVtx(_vertices[cmd.v12]);
                            RenderVtx(_vertices[cmd.v12]);
                            RenderVtx(_vertices[cmd.v10]);

                            GL.End();
                        }
                    }
                    break;
                case F3DZEX.OpCodeID.G_SETTILESIZE:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var cmd = info.Convert<F3DZEX.GLoadTile>();

                        int w = (int)(cmd.lrs.Float() + 1 - cmd.uls.Float());
                        int h = (int)(cmd.lrt.Float() + 1 - cmd.ult.Float());

                        if (N64Texture.GetTexSize(w * h, _renderTexSiz) != _loadTex.Length)
                            return; // ??? (see object_en_warp_uzu)

                        _curTexW = w;
                        _curTexH = h;

                        _reqDecodeTex = true;
                    }
                    break;
                case F3DZEX.OpCodeID.G_LOADBLOCK:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var cmd = info.Convert<F3DZEX.GLoadBlock>();

                        if (cmd.tile != RDPEnum.G_TX_tile.G_TX_LOADTILE)
                            throw new Exception("??");
                        int texels = cmd.texels + 1;

                        _loadTex = ReadBytes(_curImgAddr, N64Texture.GetTexSize(texels, _loadTexSiz)); //w*h*bpp
                        _reqDecodeTex = true;
                    }
                    break;
                case F3DZEX.OpCodeID.G_LOADTLUT:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var cmd = info.Convert<F3DZEX.GLoadTlut>();
                        _curTLUT = ReadBytes(_curImgAddr, (cmd.count+1)*2);
                        _reqDecodeTex = true;
                    }
                    break;
                case F3DZEX.OpCodeID.G_SETTIMG:
                    {
                        var cmd = info.Convert<F3DZEX.GSetTImg>();
                        _curImgAddr = cmd.imgaddr;
                        _reqDecodeTex = true;
                    }
                    break;
                case F3DZEX.OpCodeID.G_SETTILE:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var settile = info.Convert<F3DZEX.GSetTile>();

                        GL.BindTexture(TextureTarget.Texture2D, _curTexID);

                        _mirrorV = settile.cmT.HasFlag(RDPEnum.ClampMirrorFlag.G_TX_MIRROR);
                        _mirrorH = settile.cmS.HasFlag(RDPEnum.ClampMirrorFlag.G_TX_MIRROR);

                        var wrap = settile.cmS.HasFlag(RDPEnum.ClampMirrorFlag.G_TX_CLAMP)
                            ? TextureWrapMode.ClampToEdge
                            : (_mirrorH ? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap);

                        wrap = settile.cmT.HasFlag(RDPEnum.ClampMirrorFlag.G_TX_CLAMP)
                            ? TextureWrapMode.ClampToEdge
                            : (_mirrorV? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap);


                        if (settile.tile == RDPEnum.G_TX_tile.G_TX_LOADTILE)
                        {
                            _loadTexSiz = settile.siz;
                        }
                        else if (settile.tile == RDPEnum.G_TX_tile.G_TX_RENDERTILE)
                        {
                            _renderTexFmt = settile.fmt;
                            _renderTexSiz = settile.siz;
                        }
                        _reqDecodeTex = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
