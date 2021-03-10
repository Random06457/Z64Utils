using OpenTK;
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
        public void SendTexture()
        {

        }
    }
}
