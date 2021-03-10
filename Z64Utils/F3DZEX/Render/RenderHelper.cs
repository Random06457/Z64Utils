using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace F3DZEX.Render
{
    public static class RenderHelper
    {
        public static void RenderGrid(int gridScale)
        {
            GL.LineWidth(1.0f);
            GL.Begin(PrimitiveType.Lines);

            GL.Color4(0.0f, 0.0f, 0.0f, 0.5f);

            int lineCount = 6;

            for (float x = -gridScale; x < gridScale + 1; x += gridScale / lineCount)
            {
                GL.Vertex3(x, 0, -gridScale);
                GL.Vertex3(x, 0, gridScale);
            }
            for (float z = -gridScale; z < gridScale + 1; z += gridScale / lineCount)
            {
                GL.Vertex3(-gridScale, 0, z);
                GL.Vertex3(gridScale, 0, z);
            }

            //GL.Color3(Color.Transparent);
            GL.End();
        }

        public static void RenderAxis(int gridScale)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.LineWidth(2.0f);
            GL.Begin(PrimitiveType.Lines);
            //X
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(gridScale / 10, 0, 0);
            //Y
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, gridScale / 10, 0);
            //Z
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, gridScale / 10);

            GL.Color3(Color.Transparent);
            GL.End();
            GL.LineWidth(1.0f);
        }
    }
}
