using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace F3DZEX.Render
{
    public class RdpVertexDrawer
    {
        public enum ModelRenderMode
        {
            Wireframe,
            Textured,
            Surface,
            Normal
        }

        private ModelRenderMode _mode;

        private ShaderHandler _nrmShader;
        private ShaderHandler _wireframeShader;
        private ShaderHandler _shader;
        private VertexAttribs _attrs;

        public RdpVertexDrawer()
        {
            _shader = new ShaderHandler("Shaders/rdpVtx.vert", "Shaders/rdpVtx.frag");
            _wireframeShader = new ShaderHandler("Shaders/rdpVtx.vert", "Shaders/wireframe.frag", "Shaders/wireframe.geom");
            _nrmShader = new ShaderHandler("Shaders/rdpvtx.vert", "Shaders/coloredVtx.frag", "Shaders/rdpVtxNrm.geom");
            _attrs = new VertexAttribs();
            // position
            //_attrs.LayoutAddFloat(3, VertexAttribPointerType.Short, false);
            _attrs.LayoutAddInt(3, VertexAttribIntegerType.UnsignedShort);
            //flag
            _attrs.LayoutAddInt(1, VertexAttribIntegerType.UnsignedShort);
            // tex coords
            _attrs.LayoutAddInt(2, VertexAttribIntegerType.Short);
            // color/normal
            _attrs.LayoutAddFloat(4, VertexAttribPointerType.Byte, true);
            // matrix
            _attrs.LayoutAddFloat(4, VertexAttribPointerType.Float, false);
            _attrs.LayoutAddFloat(4, VertexAttribPointerType.Float, false);
            _attrs.LayoutAddFloat(4, VertexAttribPointerType.Float, false);
            _attrs.LayoutAddFloat(4, VertexAttribPointerType.Float, false);
        }

        public void SetData(byte[] data, BufferUsageHint hint) => _attrs.SetData(data, true, hint);
        public void SetSubData(byte[] data, int off) => _attrs.SetSubData(data, off, true);

        #region uniform
        public void SendProjViewMatrices(ref Matrix4 proj, ref Matrix4 view)
        {
            _shader.Send("u_Projection", proj);
            _shader.Send("u_View", view);
            _nrmShader.Send("u_Projection", proj);
            _nrmShader.Send("u_View", view);
            _wireframeShader.Send("u_Projection", proj);
            _wireframeShader.Send("u_View", view);
        }
        public void SendModelMatrix(Matrix4 model)
        {
            _shader.Send("u_Model", model);
            _nrmShader.Send("u_Model", model);
            _wireframeShader.Send("u_Model", model);
        }
        public void SendTexture(int tex)
        {
            _shader.Send("u_Tex", tex);
            _wireframeShader.Send("u_Tex", tex);
        }
        public void SendHighlightEnabled(bool enabled)
        {
            _shader.Send("u_HighlightEnabled", enabled);
            _wireframeShader.Send("u_HighlightEnabled", enabled);
        }
        public void SendHighlightColor(Color color)
        {
            _shader.Send("u_HighlightColor", color);
            _wireframeShader.Send("u_HighlightColor", color);
        }
        public void SendPrimColor(Color color)
        {
            _shader.Send("u_PrimColor", color);
            _wireframeShader.Send("u_PrimColor", color);
        }
        public void SendWireFrameColor(Color color)
        {
            _wireframeShader.Send("u_WireFrameColor", color);
        }
        
        public void SetModelRenderMode(ModelRenderMode mode)
        {
            _mode = mode;
            _shader.Send("u_ModelRenderMode", (int)mode);
            _wireframeShader.Send("u_ModelRenderMode", (int)mode);
        }

        public void SendNormalColor(Color color)
        {
            _nrmShader.Send("u_NrmColor", color);
        }
        public void SendLightingEnabled(bool enabled)
        {
            _shader.Send("u_LigthingEnabled", enabled);
        }
        #endregion


        public void UseNormalShader() => _nrmShader.Use();
        public void UseVertexShader() => _shader.Use();
        public void UseWireFrameShader() => _wireframeShader.Use();

        public void Draw(PrimitiveType type, byte[] indices, bool drawNormals)
        {
            if (_mode == ModelRenderMode.Wireframe)
                UseWireFrameShader();
            else
                UseVertexShader();
            
            _attrs.Draw(type, indices);

            if (drawNormals)
            {
                UseNormalShader();
                _attrs.Draw(type, indices);
            }
        }
    }
}
