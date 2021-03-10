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
using System.Diagnostics;

namespace F3DZEX.Render
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

        bool _initialized;
        byte[] _vtxBuffer = new byte[32 * Vertex.SIZE];
        ShaderHandler _rdpVtxShader;
        ShaderHandler _colorVtxShader;
        VertexAttribs _rdpVtxAttrs;
        VertexAttribs _colorVtxAttrs;
        Stack<Matrix4> _mtxStack = new Stack<Matrix4>();


        public bool RenderFailed() => ErrorMsg != null;

        public Renderer(Z64Game game, Config cfg, int depth = 10) : this(new Memory(game), cfg, depth)
        {

        }
        public Renderer(Memory mem, Config cfg, int depth = 10)
        {
            Memory = mem;
            CurrentConfig = cfg;

            _mtxStack.Push(Matrix4.Identity);
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



        public Matrix4 TopMatrix()
        {
            return _mtxStack.Peek();
        }
        public void PushMatrix()
        {
            _mtxStack.Push(_mtxStack.Peek());
        }
        public void PushMatrix(Matrix4 mtx)
        {
            _mtxStack.Push(mtx);
            SendModelMatrix();
        }
        public Matrix4 PopMatrix()
        {
            var ret = _mtxStack.Pop();
            SendModelMatrix();
            return ret;
        }
        public void LoadMatrix(Matrix4 mtx)
        {
            _mtxStack.Pop();
            PushMatrix(mtx);
        }

        public void SendHighlightColor(Color color) => _rdpVtxShader.Send("u_HighlightColor", color);

        private void SendPrimColor(Color color) => _rdpVtxShader.Send("u_PrimColor", color);
        private void SendTexture() => _rdpVtxShader.Send("u_Tex", 0);
        private void SendTextureEnabled(bool enabled) => _rdpVtxShader.Send("u_TexEnabled", enabled);
        private void SendModelMatrix() => _rdpVtxShader.Send("u_Model", _mtxStack.Peek());
        private void SendProjViewMatrices(Matrix4 proj, Matrix4 view)
        {
            _rdpVtxShader.Send("u_Projection", proj);
            _rdpVtxShader.Send("u_View", view);
        }

        private void CheckGLErros()
        {
            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                throw new Exception($"GL.GetError() -> {err}");
        }


        public void RenderStart(Matrix4 proj, Matrix4 view)
        {
            if (!_initialized)
                Init();

            if (RenderFailed())
                return;

            _rdpVtxShader.Use();

            _mtxStack = new Stack<Matrix4>();
            PushMatrix(Matrix4.Identity);

            SendProjViewMatrices(proj, view);
            SendHighlightColor(Color.Transparent);

            SendTextureEnabled(CurrentConfig.RenderTextures);


            GL.Enable(EnableCap.DepthTest);
            //GL.DepthMask(false);
            //glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_BLEND)
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            if (false/*CurrentConfig.DiffuseLight*/)
            {
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.ColorMaterial);
                GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f });
                GL.Enable(EnableCap.Light0);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.ColorMaterial);
                GL.Disable(EnableCap.Light0);
            }

            //RenderHelper.RenderAxis(5000);
            //RenderHelper.RenderGrid(5000);
            CheckGLErros();
        }
        private void GLWrapper(Action callback)
        {
            callback();
            CheckGLErros();
        }

        public void RenderDList(DList dlist)
        {
            if (!_initialized)
                Init();

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

        private void Init()
        {
            /* Init Texture */
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out _curTexID);
            GL.BindTexture(TextureTarget.Texture2D, _curTexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            /* Init Shaders */
            _rdpVtxShader = new ShaderHandler("Shaders/rdpVtx.vert", "Shaders/rdpVtx.frag");
            //_colorVtxShader = new ShaderHandler("Shaders/coloredVtx.vert", "Shaders/color.frag");


            /* Init RDP Vertex attributes */
            _rdpVtxAttrs = new VertexAttribs();
            // position
            _rdpVtxAttrs.LayoutAddFloat(3, VertexAttribPointerType.Short, false);
            //flag
            _rdpVtxAttrs.LayoutAddInt(1, VertexAttribIntegerType.UnsignedShort);
            // tex coords
            _rdpVtxAttrs.LayoutAddInt(2, VertexAttribIntegerType.Short);
            // color/normal
            _rdpVtxAttrs.LayoutAddFloat(4, VertexAttribPointerType.UnsignedByte, true);

            /* Init simple vertex attributes */
            /*_colorVtxAttrs = new VertexAttribs();
            // position
            _colorVtxAttrs.LayoutAddFloat(3, VertexAttribPointerType.Float, false);
            // color
            _colorVtxAttrs.LayoutAddFloat(4, VertexAttribPointerType.UnsignedByte, true);
*/
            CheckGLErros();
            _initialized = true;
        }

        static int TexDecodeCount = 0;
        private void DecodeTexIfRequired()
        {
            if (_reqDecodeTex)
            {
                Debug.WriteLine($"Decoding texture... {TexDecodeCount++}");

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

                        SendPrimColor(Color.FromArgb(cmd.A, cmd.R, cmd.G, cmd.B));
                    }
                    break;

                case Command.OpCodeID.G_VTX:
                    {
                        var cmd = info.Convert<Command.GVtx>();

                        byte[] data = Memory.ReadBytes(cmd.vaddr, Vertex.SIZE * cmd.numv);

                        System.Buffer.BlockCopy(data, 0, _vtxBuffer, cmd.vbidx * Vertex.SIZE, data.Length);

                        _rdpVtxAttrs.SetData(data, true, BufferUsageHint.DynamicDraw);

                    } break;
                case Command.OpCodeID.G_TRI1:
                    {
                        var cmd = info.Convert<Command.GTri1>();
                        
                        if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();
                            SendTexture();

                            byte[] indices = new byte[] { cmd.v0, cmd.v1, cmd.v2 };
                            _rdpVtxAttrs.Draw(PrimitiveType.Triangles, indices);
                        }
                        else
                        {
                            byte[] indices = new byte[] { cmd.v0, cmd.v1, cmd.v2, cmd.v0 };
                            _rdpVtxAttrs.Draw(PrimitiveType.LineStrip, indices);
                        }
                    }
                    break;
                case Command.OpCodeID.G_TRI2:
                    {
                        var cmd = info.Convert<Command.GTri2>();

                        if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();
                            SendTexture();

                            byte[] indices = new byte[] { cmd.v00, cmd.v01, cmd.v02, cmd.v10, cmd.v11, cmd.v12 };
                            _rdpVtxAttrs.Draw(PrimitiveType.Triangles, indices);

                        }
                        else
                        {
                            byte[] indices = new byte[] { cmd.v00, cmd.v01, cmd.v02, cmd.v00, cmd.v10, cmd.v11, cmd.v12, cmd.v10 };
                            _rdpVtxAttrs.Draw(PrimitiveType.LineStrip, indices);
                        }
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
                        _curTLUT = Memory.ReadBytes(_curImgAddr, (cmd.count + 1) * 2);
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
                            : (_mirrorV ? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);
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
                            PopMatrix();

                        break;
                    }
                case Command.OpCodeID.G_MTX:
                    {
                        var cmd = info.Convert<Command.GMtx>();
                        var mtx = new Mtx(Memory.ReadBytes(cmd.mtxaddr, Mtx.SIZE));
                        var mtxf = mtx.ToMatrix4();

                        if (cmd.param.HasFlag(Enums.G_MtxParams.G_MTX_PUSH))
                            _mtxStack.Push(mtxf);

                        // check G_MTX_MUL
                        if (!cmd.param.HasFlag(Enums.G_MtxParams.G_MTX_LOAD))
                            //mtxf = curMtx * mtxf;
                            mtxf *= _mtxStack.Peek();

                        LoadMatrix(mtxf);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
