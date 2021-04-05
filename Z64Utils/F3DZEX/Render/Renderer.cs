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
using System.Diagnostics;

namespace F3DZEX.Render
{
    public class Renderer
    {
        public class Config
        {
            public event EventHandler OnGridScaleChanged;

            private float _gridScale = 5000;


            public float GridScale {
                get => _gridScale;
                set {
                    _gridScale = value;
                    OnGridScaleChanged?.Invoke(this, new EventArgs());
                }
            }
            public bool ShowGrid { get; set; } = true;
            public bool ShowAxis { get; set; } = true;
            public bool ShowGLInfo { get; set; } = false;
            public RdpVertexDrawer.ModelRenderMode RenderMode { get; set; } = RdpVertexDrawer.ModelRenderMode.Textured;
            public bool EnabledLighting { get; set; } = true;
            public bool DrawNormals { get; set; } = false;
            public Color NormalColor { get; set; } = Color.Yellow;
            public Color HighlightColor { get; set; } = Color.Red;
            public Color WireframeColor { get; set; } = Color.Black;
        }


        public uint RenderErrorAddr { get; private set; } = 0xFFFFFFFF;
        public string ErrorMsg { get; private set; } = null;
        public Config CurrentConfig { get; set; }
        public Memory Memory { get; private set; }

        Enums.G_IM_SIZ _loadTexSiz;
        Enums.G_IM_FMT _renderTexFmt;
        Enums.G_IM_SIZ _renderTexSiz;
        uint _curImgAddr;
        byte[] _loadTexData;
        byte[] _renderTexData;
        byte[] _curTLUT;
        int _curTexW;
        int _curTexH;
        bool _mirrorV;
        bool _mirrorH;
        bool _reqDecodeTex = false;

        bool _initialized;
        RdpVertexDrawer _rdpVtxDrawer;
        SimpleVertexDrawer _gridDrawer;
        ColoredVertexDrawer _axisDrawer;
        TextDrawer _textDrawer;
        TextureHandler _curTex;
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

        #region Matrix Stack

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
        
        #endregion

        private void SendModelMatrix()
        {
            _gridDrawer.SendModelMatrix(_mtxStack.Peek());
            _axisDrawer.SendModelMatrix(_mtxStack.Peek());
            
            /* The vertices coordinates are multiplied by the model matrix during the G_VTX command processing */
            //_rdpVtxDrawer.SendModelMatrix(_mtxStack.Peek());
        }


        private void CheckGLErros()
        {
            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                throw new Exception($"GL.GetError() -> {err}");
        }
        private void GLWrapper(Action callback)
        {
            callback();
            CheckGLErros();
        }

        public void SetHightlightEnabled(bool enabled)
        {
            _rdpVtxDrawer.SendHighlightEnabled(enabled);
        }


        private void Init()
        {
            /* Init Texture */
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            _curTex = new TextureHandler();

            /* Init Drawers */
            _rdpVtxDrawer = new RdpVertexDrawer();
            _gridDrawer = new SimpleVertexDrawer();
            _axisDrawer = new ColoredVertexDrawer();
            _textDrawer = new TextDrawer();

            float[] vertices = RenderHelper.GenerateGridVertices(CurrentConfig.GridScale, 6, false);
            _gridDrawer.SetData(vertices, BufferUsageHint.StaticDraw);

            vertices = RenderHelper.GenerateAxisvertices(CurrentConfig.GridScale);
            _axisDrawer.SetData(vertices, BufferUsageHint.StaticDraw);

            _rdpVtxDrawer.SetData(new byte[32 * (Vertex.SIZE + 4*4*4)], BufferUsageHint.DynamicDraw);

            CurrentConfig.OnGridScaleChanged += (o, e) =>
            {
                float[] vertices = RenderHelper.GenerateGridVertices(CurrentConfig.GridScale, 6, false);
                _gridDrawer.SetSubData(vertices, 0);
            };

            CheckGLErros();
            _initialized = true;
        }

        public void RenderStart(Matrix4 proj, Matrix4 view)
        {
            if (!_initialized)
                Init();

            if (RenderFailed())
                return;

            _mtxStack = new Stack<Matrix4>();
            PushMatrix(Matrix4.Identity);

            
            _gridDrawer.SendProjViewMatrices(ref proj, ref view);
            _axisDrawer.SendProjViewMatrices(ref proj, ref view);
            _rdpVtxDrawer.SendProjViewMatrices(ref proj, ref view);
            Matrix4 id = Matrix4.Identity;
            _textDrawer.SendProjViewMatrices(ref id, ref id);

            _rdpVtxDrawer.SendHighlightColor(CurrentConfig.HighlightColor);
            _rdpVtxDrawer.SendHighlightEnabled(false);
            _rdpVtxDrawer.SetModelRenderMode(CurrentConfig.RenderMode);
            _rdpVtxDrawer.SendNormalColor(CurrentConfig.NormalColor);
            _rdpVtxDrawer.SendWireFrameColor(CurrentConfig.WireframeColor);
            _rdpVtxDrawer.SendLightingEnabled(CurrentConfig.EnabledLighting);
            


            GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Lequal);
            //GL.DepthMask(false);
            //glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_BLEND)
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);

            if (CurrentConfig.ShowGrid)
                RenderHelper.DrawGrid(_gridDrawer);

            if (CurrentConfig.ShowAxis)
                RenderHelper.DrawAxis(_axisDrawer);

            if (CurrentConfig.ShowGLInfo)
            {
                _textDrawer.DrawString(
                    //$"Extensions: {GL.GetString(StringName.Extensions)}\n" + 
                    $"Shading Language Version: {GL.GetString(StringName.ShadingLanguageVersion)}\n" +
                    $"Version: {GL.GetString(StringName.Version)}\n" +
                    $"Renderer: {GL.GetString(StringName.Renderer)}\n" +
                    $"Vendor: {GL.GetString(StringName.Vendor)}");
            }

            CheckGLErros();
        }

        public Dlist GetDlist(uint vaddr)
        {
            return new Dlist(Memory, vaddr);
        }

        public void RenderDList(Dlist dlist)
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
                    
                    addr = entry.addr;
                    ProcessInstruction(entry.cmd);
                }
            }
            catch (Exception ex)
            {
                RenderErrorAddr = addr;
                ErrorMsg = ex.Message;
            }
        }



        static int TexDecodeCount = 0;
        private void DecodeTexIfRequired()
        {
            if (_reqDecodeTex)
            {
                //Debug.WriteLine($"Decoding texture... {TexDecodeCount++}");

                _renderTexData = N64Texture.Decode(_curTexW * _curTexH, _renderTexFmt, _renderTexSiz, _loadTexData, _curTLUT);                
                _curTex.SetDataRGBA(_renderTexData, _curTexW, _curTexH);
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

                        _rdpVtxDrawer.SendPrimColor(Color.FromArgb(cmd.A, cmd.R, cmd.G, cmd.B));
                    }
                    break;

                case Command.OpCodeID.G_VTX:
                    {
                        var cmd = info.Convert<Command.GVtx>();

                        /* We have to multiply the vertex coordinates with the model view matrix here */

                        Matrix4 curMtx = _mtxStack.Peek();


                        using (MemoryStream ms = new MemoryStream())
                        {
                            BinaryWriter bw = new BinaryWriter(ms);
                            for (int i = 0; i < cmd.numv; i++)
                            {
                                byte[] data = Memory.ReadBytes(cmd.vaddr + (uint)(Vertex.SIZE * i), Vertex.SIZE);
                                bw.Write(data);

                                for (int y = 0; y < 4; y++)
                                    for (int x = 0; x < 4; x++)
                                        bw.Write(curMtx[y, x]);
                            }

                            _rdpVtxDrawer.SetSubData(ms.ToArray(), cmd.vbidx * (Vertex.SIZE + 4 * 4 * 4));
                        }


                        /*
                        byte[] data = Memory.ReadBytes(cmd.vaddr, Vertex.SIZE * cmd.numv);

                        for (int off = 0; off < cmd.numv * Vertex.SIZE; off += Vertex.SIZE)
                        {
                            Vector4 pos = new Vector4(
                                (short)(data[off + 0] << 8 | data[off + 1]),
                                (short)(data[off + 2] << 8 | data[off + 3]),
                                (short)(data[off + 4] << 8 | data[off + 5]),
                                1
                                ) * curMtx;

                            short x = (short)pos.X;
                            short y = (short)pos.Y;
                            short z = (short)pos.Z;

                            data[off + 0] = (byte)((x >> 8) & 0xFF);
                            data[off + 1] = (byte)((x >> 0) & 0xFF);

                            data[off + 2] = (byte)((y >> 8) & 0xFF);
                            data[off + 3] = (byte)((y >> 0) & 0xFF);
                            
                            data[off + 4] = (byte)((z >> 8) & 0xFF);
                            data[off + 5] = (byte)((z >> 0) & 0xFF);
                        }

                        _rdpVtxDrawer.SetSubData(data, cmd.vbidx * Vertex.SIZE);*/



                    }
                    break;
                case Command.OpCodeID.G_TRI1:
                    {
                        var cmd = info.Convert<Command.GTri1>();
                        
                        if (CurrentConfig.RenderMode == RdpVertexDrawer.ModelRenderMode.Textured)
                        {
                            DecodeTexIfRequired();
                            _rdpVtxDrawer.SendTexture(0);
                        }

                        byte[] indices = new byte[] { cmd.v0, cmd.v1, cmd.v2 };
                        _rdpVtxDrawer.Draw(PrimitiveType.Triangles, indices, CurrentConfig.DrawNormals);

                        /*if (CurrentConfig.RenderTextures && !CurrentConfig.WireFrame)
                        {
                            DecodeTexIfRequired();
                            _rdpVtxDrawer.SendTexture(0);

                            byte[] indices = new byte[] { cmd.v0, cmd.v1, cmd.v2 };
                            _rdpVtxDrawer.Draw(PrimitiveType.Triangles, indices, CurrentConfig.DrawNormals);
                        }
                        else
                        {
                            byte[] indices = new byte[] { cmd.v0, cmd.v1, cmd.v2, cmd.v0 };
                            _rdpVtxDrawer.Draw(PrimitiveType.LineStrip, indices, CurrentConfig.DrawNormals);
                        }*/
                    }
                    break;
                case Command.OpCodeID.G_TRI2:
                    {
                        var cmd = info.Convert<Command.GTri2>();

                        /*if (CurrentConfig.RenderTextures)
                        {
                            DecodeTexIfRequired();
                            _rdpVtxDrawer.SendTexture(0);

                            byte[] indices = new byte[] { cmd.v00, cmd.v01, cmd.v02, cmd.v10, cmd.v11, cmd.v12 };
                            _rdpVtxDrawer.Draw(PrimitiveType.Triangles, indices, CurrentConfig.DrawNormals);

                        }
                        else
                        {
                            //byte[] indices = new byte[] { cmd.v00, cmd.v01, cmd.v02, cmd.v00, cmd.v10, cmd.v11, cmd.v12, cmd.v10 };
                            //_rdpVtxDrawer.Draw(PrimitiveType.LineStrip, indices);
                            byte[] indices = new byte[] { cmd.v00, cmd.v01, cmd.v02 };
                            _rdpVtxDrawer.Draw(PrimitiveType.LineLoop, indices, CurrentConfig.DrawNormals);
                            indices = new byte[] { cmd.v10, cmd.v11, cmd.v12 };
                            _rdpVtxDrawer.Draw(PrimitiveType.LineLoop, indices, CurrentConfig.DrawNormals);
                        }*/

                        if (CurrentConfig.RenderMode == RdpVertexDrawer.ModelRenderMode.Textured)
                        {
                            DecodeTexIfRequired();
                            _rdpVtxDrawer.SendTexture(0);
                        }

                        byte[] indices = new byte[] { cmd.v00, cmd.v01, cmd.v02, cmd.v10, cmd.v11, cmd.v12 };
                        _rdpVtxDrawer.Draw(PrimitiveType.Triangles, indices, CurrentConfig.DrawNormals);


                    }
                    break;


                case Command.OpCodeID.G_SETTILESIZE:
                    {
                        if (CurrentConfig.RenderMode != RdpVertexDrawer.ModelRenderMode.Textured)
                            return;

                        var cmd = info.Convert<Command.GLoadTile>();

                        int w = (int)(cmd.lrs.Float() + 1 - cmd.uls.Float());
                        int h = (int)(cmd.lrt.Float() + 1 - cmd.ult.Float());

                        if (N64Texture.GetTexSize(w * h, _renderTexSiz) != _loadTexData.Length)
                            return; // ??? (see object_en_warp_uzu)

                        _curTexW = w;
                        _curTexH = h;

                        _reqDecodeTex = true;
                    }
                    break;
                case Command.OpCodeID.G_LOADBLOCK:
                    {
                        if (CurrentConfig.RenderMode != RdpVertexDrawer.ModelRenderMode.Textured)
                            return;

                        var cmd = info.Convert<Command.GLoadBlock>();

                        if (cmd.tile != Enums.G_TX_tile.G_TX_LOADTILE)
                            throw new Exception("??");
                        int texels = cmd.texels + 1;

                        _loadTexData = Memory.ReadBytes(_curImgAddr, N64Texture.GetTexSize(texels, _loadTexSiz)); //w*h*bpp
                        _reqDecodeTex = true;
                    }
                    break;
                case Command.OpCodeID.G_LOADTLUT:
                    {
                        if (CurrentConfig.RenderMode != RdpVertexDrawer.ModelRenderMode.Textured)
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
                        if (CurrentConfig.RenderMode != RdpVertexDrawer.ModelRenderMode.Textured)
                            return;

                        var settile = info.Convert<Command.GSetTile>();

                        _mirrorV = settile.cmT.HasFlag(Enums.ClampMirrorFlag.G_TX_MIRROR);
                        _mirrorH = settile.cmS.HasFlag(Enums.ClampMirrorFlag.G_TX_MIRROR);

                        var wrapS = settile.cmS.HasFlag(Enums.ClampMirrorFlag.G_TX_CLAMP)
                            ? TextureWrapMode.ClampToEdge
                            : (_mirrorH ? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);

                        var wrapT = settile.cmT.HasFlag(Enums.ClampMirrorFlag.G_TX_CLAMP)
                            ? TextureWrapMode.ClampToEdge
                            : (_mirrorV ? TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat);

  
                        _curTex.SetTextureWrap((int)wrapS, (int)wrapT);

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
