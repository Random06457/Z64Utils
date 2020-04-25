using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK;

namespace Z64.Forms
{
    public partial class ModelViewerControl : GLControl
    {
        public class Config
        {
            public float GridScale { get; set; } = 5000;
            public bool ShowGrid { get; set; } = true;
            public bool ShowAxis { get; set; } = true;
            public bool DiffuseLight { get; set; } = false;
        }

        Vector3 _camPos;
        Vector3 _angle;
        Point _oldPos = Point.Empty;
        Point _oldAnglePos = Point.Empty;
        Action _render;
        bool _init = false;

        public Config CurrentConfig { get; set; } = new Config();
        public Action RenderCallback { get => _render; set { _render = value; Render(); } }

        public ModelViewerControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _init = true;
            _camPos = new Vector3(0, 0, -CurrentConfig.GridScale);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _camPos.Z += e.Delta * 4 * (Math.Max(0.01f, Math.Abs(_camPos.Z) / 10000));
            Render();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _oldPos = Point.Empty;
            _oldAnglePos = Point.Empty;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Right)
            {
                if (!_oldPos.IsEmpty)
                {
                    _camPos.X += (e.Location.X - _oldPos.X) * (Math.Abs(_camPos.Z) / (SystemInformation.MouseWheelScrollDelta * 4));
                    _camPos.Y -= (e.Location.Y - _oldPos.Y) * (Math.Abs(_camPos.Z) / (SystemInformation.MouseWheelScrollDelta * 4));
                }

                _oldPos = e.Location;
            }
            if (e.Button == MouseButtons.Left)
            {
                if (!_oldAnglePos.IsEmpty)
                {
                    _angle.Y += (e.Location.X - _oldAnglePos.X) / 1.5f;
                    _angle.X += (e.Location.Y - _oldAnglePos.Y) / 1.5f;
                }

                _oldAnglePos = e.Location;
            }
            Render();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Render();
        }


        public void Render()
        {
            if (!_init || RenderCallback == null ||Width == 0) return;
            try
            {
                MakeCurrent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            GL.Enable(EnableCap.DepthTest);
            //GL.DepthMask(false);
            //glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_BLEND)
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            if (CurrentConfig.DiffuseLight)
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

            if (Height == 0)
                ClientSize = new Size(Width, 1);

            GL.Viewport(0, 0, Width, Height);


            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 500000);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(BackColor);

            GL.Translate(_camPos.X, _camPos.Y, _camPos.Z);
            GL.Rotate(_angle.X, 1.0f, 0.0f, 0.0f);
            GL.Rotate(_angle.Y, 0.0f, 1.0f, 0.0f);

            if (CurrentConfig.ShowGrid)
                RenderGrid();
            RenderCallback?.Invoke();

            if (CurrentConfig.ShowAxis)
                RenderAxis();

            SwapBuffers();
        }
        void RenderGrid()
        {
            GL.LineWidth(1.0f);
            GL.Begin(PrimitiveType.Lines);

            GL.Color4(0.0f, 0.0f, 0.0f, 0.5f);

            int lineCount = 6;

            for (float x = -CurrentConfig.GridScale; x < CurrentConfig.GridScale + 1; x += CurrentConfig.GridScale / lineCount)
            {
                GL.Vertex3(x, 0, -CurrentConfig.GridScale);
                GL.Vertex3(x, 0, CurrentConfig.GridScale);
            }
            for (float z = -CurrentConfig.GridScale; z < CurrentConfig.GridScale + 1; z += CurrentConfig.GridScale / lineCount)
            {
                GL.Vertex3(-CurrentConfig.GridScale, 0, z);
                GL.Vertex3(CurrentConfig.GridScale, 0, z);
            }

            //GL.Color3(Color.Transparent);
            GL.End();
        }

        void RenderAxis()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.LineWidth(2.0f);
            GL.Begin(PrimitiveType.Lines);
            //X
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(CurrentConfig.GridScale / 10, 0, 0);
            //Y
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, CurrentConfig.GridScale / 10, 0);
            //Z
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, CurrentConfig.GridScale / 10);

            GL.Color3(Color.Transparent);
            GL.End();
            GL.LineWidth(1.0f);
        }


        public Bitmap CaptureScreen()
        {
            Invalidate();
            Update();
            Refresh();

            int w = ClientSize.Width;
            int h = ClientSize.Height;
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
    }
}
