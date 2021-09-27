using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace F3DZEX.Render
{

    public class FrameBufferDrawer2
    {
        TexturedVertexDrawer _vtxDrawer;
        //TextureHandler _tex;

        public FrameBufferDrawer2()
        {
            _vtxDrawer = new TexturedVertexDrawer();
            //_tex = new TextureHandler();

            _vtxDrawer.SendTexture(0);
            _vtxDrawer.SendColor(Color.White);

            float[] vertices = new float[]
            {
                -1.0f, -1.0f, 0.0f,    0.0f, 0.0f,
                -1.0f,  1.0f, 0.0f,    0.0f, 1.0f,
                 1.0f,  1.0f, 0.0f,    1.0f, 1.0f,
                 1.0f, -1.0f, 0.0f,    1.0f, 0.0f
            };

            _vtxDrawer.SetData(vertices, BufferUsageHint.StaticDraw);
        }


        public void Draw()
        {
            //_tex.Use();

            Matrix4 id = Matrix4.Identity;
            _vtxDrawer.SendModelMatrix(id);
            _vtxDrawer.SendProjViewMatrices(ref id, ref id);
            _vtxDrawer.Draw(PrimitiveType.Quads);
        }

    }
    public class FrameBufferDrawer : VertexDrawer
    {
        public FrameBufferDrawer()
        {
            _shader = new(File.ReadAllText("Shaders/framebuffer.vert"), File.ReadAllText("Shaders/framebuffer.frag"));
            _attrs = new();

            // pos
            _attrs.LayoutAddFloat(2, VertexAttribPointerType.Float, true);
            // tex coords
            _attrs.LayoutAddFloat(2, VertexAttribPointerType.Float, true);

            float[] vertices = new float[]
            {
                -1.0f / 2,  1.0f,  0.0f, 1.0f,
                -1.0f / 2, -1.0f,  0.0f, 0.0f,
                 1.0f / 2, -1.0f,  1.0f, 0.0f,

                -1.0f / 2,  1.0f,  0.0f, 1.0f,
                 1.0f / 2, -1.0f,  1.0f, 0.0f,
                 1.0f / 2,  1.0f,  1.0f, 1.0f
            };

            SetVertexData(vertices, vertices.Length * sizeof(float));
        }

        public void SendTexture(int tex)
        {
            _shader.Send("u_FrameTexture", tex);
        }

        public void Draw()
        {
            _shader.Use();
            _attrs.Draw(PrimitiveType.Triangles);
        }
    }
}
