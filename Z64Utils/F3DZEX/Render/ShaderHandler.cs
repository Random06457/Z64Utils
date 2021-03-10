using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace F3DZEX.Render
{

    [Serializable]
    public class ShaderException : Exception
    {
        public ShaderException() { }
        public ShaderException(string message) : base(message) { }
        public ShaderException(string message, Exception inner) : base(message, inner) { }
        protected ShaderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class ShaderHandler
    {
        private int _program;

        public ShaderHandler(string vertPath, string fragPath)
        {
            int vertShader = CompileShader(vertPath, ShaderType.VertexShader);
            int fragShader = CompileShader(fragPath, ShaderType.FragmentShader);

            LinkShaders(vertShader, fragShader);
        }

        private int CompileShader(string path, ShaderType type)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, File.ReadAllText(path));
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);

            GL.GetShaderInfoLog(shader, out string info);
            if (!string.IsNullOrEmpty(info))
                throw new ShaderException($"Failed to compile \"{path}\" : \n{info}");

            return shader;
        }

        private void LinkShaders(params int[] shaders)
        {
            _program = GL.CreateProgram();

            foreach (var shader in shaders)
                GL.AttachShader(_program, shader);

            GL.LinkProgram(_program);

            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int status);
            GL.GetProgramInfoLog(_program, out string info);
            if (!string.IsNullOrEmpty(info))
                throw new ShaderException($"Failed to link shaders : \r{info}");

            foreach (var shader in shaders)
            {
                GL.DetachShader(_program, shader);
                GL.DeleteShader(shader);
            }
        }

        public void Use()
        {
            GL.UseProgram(_program);
        }

        public void Send(string name, float data) => GL.Uniform1(GL.GetUniformLocation(_program, name), data);
        public void Send(string name, int data) => GL.Uniform1(GL.GetUniformLocation(_program, name), data);
        public void Send(string name, float x, float y) => GL.Uniform2(GL.GetUniformLocation(_program, name), x, y);
        public void Send(string name, float x, float y, float z) => GL.Uniform3(GL.GetUniformLocation(_program, name), x, y, z);
        public void Send(string name, float x, float y, float z, float w) => GL.Uniform4(GL.GetUniformLocation(_program, name), x, y, z, w);
        public void Send(string name, Matrix4 mtx) => GL.UniformMatrix4(GL.GetUniformLocation(_program, name), false, ref mtx);
        public void Send(string name, Color color) => GL.Uniform4(GL.GetUniformLocation(_program, name), color);
        public void Send(string name, bool x) => GL.Uniform1(GL.GetUniformLocation(_program, name), x ? 1 : 0);
    }
}
