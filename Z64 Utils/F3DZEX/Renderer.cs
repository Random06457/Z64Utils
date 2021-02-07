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
using RDP;

using DList = System.Collections.Generic.List<System.Tuple<uint, F3DZEX.Command.CommandInfo>>;

namespace F3DZEX
{
    public class Renderer
    {
        public class Config
        {
            public bool RenderTextures { get; set; } = true;
        }


        public uint RenderErrorAddr { get; private set; } = 0xFFFFFFFF;
        public string ErrorMsg { get; private set; } = null;
        public Config CurrentConfig { get; set; }
        public Memory Memory { get; private set; }

        Vertex[] _vertices = new Vertex[32];
        Vector4[] _vtxCoords = new Vector4[32]; // temp
        int _curTexID;
        Enums.G_IM_SIZ _loadTexSiz;
        Enums.G_IM_FMT _renderTexFmt;
        Enums.G_IM_SIZ _renderTexSiz;
        uint _curImgAddr;
        byte[] _loadTex;
        byte[] _renderTex;
        byte[] _curTLUT;
        int _curTexW;
        int _curTexH;
        bool _mirrorV;
        bool _mirrorH;
        bool _reqDecodeTex = false;

        bool _texInitialized;



        public bool RenderFailed() => ErrorMsg != null;

        public Renderer(Z64Game game, Config cfg, int depth = 10) : this(new Memory(game), cfg, depth)
        {

        }
        public Renderer(Memory mem, Config cfg, int depth = 10)
        {
            Memory = mem;
            CurrentConfig = cfg;
        }
        
        public void ClearErrors() => ErrorMsg = null;

        public List<Command.CommandInfo> GetDlist(uint vaddr)
        {
            try
            {
                for (int off = 0; ; off += 8)
                {
                    byte[] ins = Memory.ReadBytes(vaddr + (uint)off, 8);
                    if (ins[0] == (byte)Command.OpCodeID.G_ENDDL)
                    {
                        return Command.DecodeDList(Memory.ReadBytes(vaddr, off + 8), 0);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public DList GetFullDlist(uint vaddr)
        {
            DList ret = new DList();
            Stack<uint> stack = new Stack<uint>();

            var a = GetDlist(vaddr);
            uint addr1 = 0;
            a.ForEach(e => { ret.Add(new Tuple<uint, Command.CommandInfo>(addr1, e)); addr1 += (uint)e.GetSize(); });
            return ret;

            try
            {
                for (int off = 0; ; off += 8)
                {
                    Command.OpCodeID id = (Command.OpCodeID)Memory.ReadBytes(vaddr + (uint)off, 1)[0];

                    if (id == Command.OpCodeID.G_DL)
                    {
                        var dlist = Command.DecodeDList(Memory.ReadBytes(vaddr, off + 8), 0);
                        uint addr = vaddr;
                        dlist.ForEach(e => { ret.Add(new Tuple<uint, Command.CommandInfo>(addr, e)); addr += (uint)e.GetSize(); });
                        stack.Push(vaddr + (uint)off);
                        off = 0;
                    }
                    if (id == Command.OpCodeID.G_ENDDL)
                    {
                        var dlist = Command.DecodeDList(Memory.ReadBytes(vaddr, off + 8), 0);
                        uint addr = vaddr;
                        dlist.ForEach(e => { ret.Add(new Tuple<uint, Command.CommandInfo>(addr, e)); addr += (uint)e.GetSize(); });
                        off = 0;
                        if (stack.Count > 0)
                        {
                            vaddr = stack.Last();
                            stack.Pop();
                        }
                        else return ret;

                    }
                }
            }
            catch
            {
                return null;
            }
        }


        public void RenderDList(DList dlist)
        {
            if (!_texInitialized)
                InitTex();

            if (RenderFailed())
                return;


            uint addr = 0xFFFFFFFF;
            try
            {
                foreach (var entry in dlist)
                {
                    addr = entry.Item1;
                    ProcessInstruction(entry.Item2);
                }
            }
            catch (Exception ex)
            {
                RenderErrorAddr = addr;
                ErrorMsg = ex.Message;
            }
        }

        private void InitTex()
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out _curTexID);
            GL.BindTexture(TextureTarget.Texture2D, _curTexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            
            _texInitialized = true;
        }

        private void RenderVtx(int idx)
        {
            Vertex vtx = _vertices[idx];
            Vector4 vec4 = _vtxCoords[idx];
            //GL.Color4(R, G, B, A);
            GL.Normal3(vtx.R, vtx.G, vtx.B);
            float x = (float)(vtx.TexX >> 5) / _curTexW;
            float y = (float)(vtx.TexY >> 5) / _curTexH;
            GL.TexCoord2(x, y);
            GL.Vertex3(vec4.X, vec4.Y, vec4.Z);
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

        private unsafe void ProcessInstruction(Command.CommandInfo info)
        {
            switch (info.ID)
            {
                case Command.OpCodeID.G_SETPRIMCOLOR:
                    {
                        var cmd = info.Convert<Command.GSetPrimColor>();

                        GL.Color4(cmd.R, cmd.G, cmd.B, cmd.A);
                    } break;
                case Command.OpCodeID.G_VTX:
                    {
                        var cmd = info.Convert<Command.GVtx>();

                        cmd.vaddr += (uint)cmd.vbidx * 0x10;
                        for (int i = 0; i < cmd.numv; i++, cmd.vaddr += 0x10)
                            _vertices[cmd.vbidx + i] = new Vertex(Memory.ReadBytes(cmd.vaddr, 0x10));

                        GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 curMtx);
                        for (int i = 0; i < cmd.numv; i++)
                        {
                            var vtx = _vertices[cmd.vbidx + i];
                            _vtxCoords[cmd.vbidx + i] = new Vector4(vtx.X, vtx.Y, vtx.Z, 1) * curMtx;
                        }
                    } break;
                case Command.OpCodeID.G_TRI1:
                    {
                        GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 curMtx);
                        GL.LoadIdentity();

                        var cmd = info.Convert<Command.GTri1>();
                        if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();

                            GL.Begin(PrimitiveType.Triangles);
                            RenderVtx(cmd.v0);
                            RenderVtx(cmd.v1);
                            RenderVtx(cmd.v2);
                            GL.End();
                        }
                        else
                        {
                            GL.Color3(0, 0, 0);
                            GL.Begin(PrimitiveType.Lines);
                            RenderVtx(cmd.v0);
                            RenderVtx(cmd.v1);
                            RenderVtx(cmd.v1);
                            RenderVtx(cmd.v2);
                            RenderVtx(cmd.v2);
                            RenderVtx(cmd.v0);

                            GL.End();
                        }

                        GL.LoadMatrix(ref curMtx);
                    }
                    break;
                case Command.OpCodeID.G_TRI2:
                    {
                        GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 curMtx);
                        GL.LoadIdentity();

                        var cmd = info.Convert<Command.GTri2>();
                        if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();

                            GL.Begin(PrimitiveType.Triangles);

                            RenderVtx(cmd.v00);
                            RenderVtx(cmd.v01);
                            RenderVtx(cmd.v02);
                            RenderVtx(cmd.v10);
                            RenderVtx(cmd.v11);
                            RenderVtx(cmd.v12);

                            GL.End();
                        }
                        else
                        {
                            GL.Color3(0, 0, 0);
                            GL.Begin(PrimitiveType.Lines);

                            RenderVtx(cmd.v00);
                            RenderVtx(cmd.v01);
                            RenderVtx(cmd.v01);
                            RenderVtx(cmd.v02);
                            RenderVtx(cmd.v02);
                            RenderVtx(cmd.v00);

                            RenderVtx(cmd.v10);
                            RenderVtx(cmd.v11);
                            RenderVtx(cmd.v11);
                            RenderVtx(cmd.v12);
                            RenderVtx(cmd.v12);
                            RenderVtx(cmd.v10);

                            GL.End();
                        }
                        GL.LoadMatrix(ref curMtx);
                    }
                    break;
                case Command.OpCodeID.G_SETTILESIZE:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var cmd = info.Convert<Command.GLoadTile>();

                        int w = (int)(cmd.lrs.Float() + 1 - cmd.uls.Float());
                        int h = (int)(cmd.lrt.Float() + 1 - cmd.ult.Float());

                        if (N64Texture.GetTexSize(w * h, _renderTexSiz) != _loadTex.Length)
                            return; // ??? (see object_en_warp_uzu)

                        _curTexW = w;
                        _curTexH = h;

                        _reqDecodeTex = true;
                    }
                    break;
                case Command.OpCodeID.G_LOADBLOCK:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var cmd = info.Convert<Command.GLoadBlock>();

                        if (cmd.tile != Enums.G_TX_tile.G_TX_LOADTILE)
                            throw new Exception("??");
                        int texels = cmd.texels + 1;

                        _loadTex = Memory.ReadBytes(_curImgAddr, N64Texture.GetTexSize(texels, _loadTexSiz)); //w*h*bpp
                        _reqDecodeTex = true;
                    }
                    break;
                case Command.OpCodeID.G_LOADTLUT:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var cmd = info.Convert<Command.GLoadTlut>();
                        _curTLUT = Memory.ReadBytes(_curImgAddr, (cmd.count+1)*2);
                        _reqDecodeTex = true;
                    }
                    break;
                case Command.OpCodeID.G_SETTIMG:
                    {
                        var cmd = info.Convert<Command.GSetTImg>();
                        _curImgAddr = cmd.imgaddr;
                        _reqDecodeTex = true;
                    }
                    break;
                case Command.OpCodeID.G_SETTILE:
                    {
                        if (!CurrentConfig.RenderTextures)
                            return;

                        var settile = info.Convert<Command.GSetTile>();

                        GL.BindTexture(TextureTarget.Texture2D, _curTexID);

                        _mirrorV = settile.cmT.HasFlag(Enums.ClampMirrorFlag.G_TX_MIRROR);
                        _mirrorH = settile.cmS.HasFlag(Enums.ClampMirrorFlag.G_TX_MIRROR);

                        var wrap = settile.cmS.HasFlag(Enums.ClampMirrorFlag.G_TX_CLAMP)
                            ? TextureWrapMode.ClampToEdge
                            : (_mirrorH ? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap);

                        wrap = settile.cmT.HasFlag(Enums.ClampMirrorFlag.G_TX_CLAMP)
                            ? TextureWrapMode.ClampToEdge
                            : (_mirrorV? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap);


                        if (settile.tile == Enums.G_TX_tile.G_TX_LOADTILE)
                        {
                            _loadTexSiz = settile.siz;
                        }
                        else if (settile.tile == Enums.G_TX_tile.G_TX_RENDERTILE)
                        {
                            _renderTexFmt = settile.fmt;
                            _renderTexSiz = settile.siz;
                        }
                        _reqDecodeTex = true;
                    }
                    break;
                    /*
                case Command.OpCodeID.G_DL:
                    {
                        var cmd = info.Convert<Command.GDl>();
                        BranchFrame(cmd.dl, !cmd.branch);
                        break;
                    }
                    */
                case Command.OpCodeID.G_POPMTX:
                    {
                        var cmd = info.Convert<Command.GPopMtx>();
                        for (uint i = 0; i < cmd.num; i++)
                            GL.PopMatrix();
                        break;
                    }
                case Command.OpCodeID.G_MTX:
                    {
                        var cmd = info.Convert<Command.GMtx>();
                        var mtx = new Mtx(Memory.ReadBytes(cmd.mtxaddr, Mtx.SIZE));
                        var mtxf = mtx.ToMatrix4();

                        if (cmd.param.HasFlag(Enums.G_MtxParams.G_MTX_PUSH))
                            GL.PushMatrix();

                        GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 curMtx);
                        if (!cmd.param.HasFlag(Enums.G_MtxParams.G_MTX_LOAD))
                            mtxf = curMtx * mtxf;

                        GL.LoadMatrix(ref mtxf);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
