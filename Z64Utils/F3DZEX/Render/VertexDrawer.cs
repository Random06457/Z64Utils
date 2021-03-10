using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace F3DZEX.Render
{
    public class VertexDrawer
    {
        ShaderHandler _shader;
        VertexAttribs _attrs;

        public VertexDrawer(ShaderHandler shader, VertexAttribs attrs)
            => (_shader, _attrs) = (shader, attrs);

        public VertexDrawer(string vertShader, string fragShader, VertexAttribs attrs) :
            this(new ShaderHandler(vertShader, fragShader), attrs)
        {

        }

        public void SendProjViewMatrices(Matrix4 proj, Matrix4 view)
        {
            _shader.Send("u_Projection", proj);
            _shader.Send("u_View", view);
        }
        public void SendModelMatrix(Matrix4 model)
        {
            _shader.Send("u_Model", model);
        }
        public void SendTexture(int tex)
        {
            _shader.Send("u_Tex", tex);
        }

        public void SetVertexData(byte[] buffer, bool bigEndian = false, BufferUsageHint hint = BufferUsageHint.StaticDraw)
            => _attrs.SetData(buffer, bigEndian, hint);

        public void SetVertexData<T>(T[] buffer, int size, BufferUsageHint hint = BufferUsageHint.StaticDraw)
            where T : struct, IComparable
            => _attrs.SetData(buffer, size, hint);

        public void Draw(PrimitiveType type, byte[] indices)
        {
            _shader.Use();
            _attrs.Draw(type, indices);
        }
        public void Draw(PrimitiveType type, uint[] indices)
        {
            _shader.Use();
            _attrs.Draw(type, indices);
        }
    }
}
