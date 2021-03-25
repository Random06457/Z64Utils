using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace F3DZEX.Render
{
    public class RdpVertexDrawer : VertexDrawer
    {
        public RdpVertexDrawer()
        {
            _shader = new ShaderHandler("Shaders/rdpVtx.vert", "Shaders/rdpVtx.frag");
            _attrs = new VertexAttribs();
            // position
            //_attrs.LayoutAddFloat(3, VertexAttribPointerType.Short, false);
            _attrs.LayoutAddInt(3, VertexAttribIntegerType.UnsignedShort);
            //flag
            _attrs.LayoutAddInt(1, VertexAttribIntegerType.UnsignedShort);
            // tex coords
            _attrs.LayoutAddInt(2, VertexAttribIntegerType.Short);
            // color/normal
            _attrs.LayoutAddFloat(4, VertexAttribPointerType.UnsignedByte, true);
        }

        public void SetData(byte[] data, BufferUsageHint hint) => SetVertexData(data, true, hint);
        public void SetSubData(byte[] data, int off) => SetVertexSubData(data, off, true);

        public void SendProjViewMatrices(ref Matrix4 proj, ref Matrix4 view)
        {
            _shader.Send("u_Projection", proj);
            _shader.Send("u_View", view);
        }
        public void SendModelMatrix(Matrix4 model) => _shader.Send("u_Model", model);
        public void SendTexture(int tex) => _shader.Send("u_Tex", tex);
        public void SendHighlightColor(Color color) => _shader.Send("u_HighlightColor", color);
        public void SendPrimColor(Color color) => _shader.Send("u_PrimColor", color);
        public void SendTextureEnabled(bool enabled) => _shader.Send("u_TexEnabled", enabled);

    }
}
