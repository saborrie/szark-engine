using OpenTK.Graphics.OpenGL;
using System;

namespace Szark
{
    public static class ShaderLoader
    {
        /// <summary>
        /// Creates a Shader Program from a Vertex Shader and a Fragment Shader
        /// </summary>
        /// <param name="vertexShader">Vertex Shader Path</param>
        /// <param name="fragmentShader">Fragment Shader Path</param>
        /// <returns>Shader Program ID</returns>
        public static int CreateProgram(string vertexShader, string fragmentShader)
        {
            int vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderID, vertexShader);
            GL.CompileShader(vertexShaderID);

            string vertexInfo = GL.GetShaderInfoLog(vertexShaderID);
            if (vertexInfo != "") Console.WriteLine(vertexInfo);

            GL.ShaderSource(fragmentShaderID, fragmentShader);
            GL.CompileShader(fragmentShaderID);

            string fragmentInfo = GL.GetShaderInfoLog(fragmentShaderID);
            if (fragmentInfo != "") Console.WriteLine(fragmentInfo);

            int programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            GL.LinkProgram(programID);

            string programInfo = GL.GetProgramInfoLog(programID);
            if (programInfo != "") Console.WriteLine(programInfo);

            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);

            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);

            return programID;
        }
    }
}