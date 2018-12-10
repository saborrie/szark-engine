/*
	ShaderLoader.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using OpenTK.Graphics.OpenGL;
using System.IO;
using System;

namespace PGE
{
    public class ShaderLoader
    {
        /// <summary>
        /// Creates a Shader Program from a Vertex Shader and a Fragment Shader
        /// </summary>
        /// <param name="vertPath">Vertex Shader Path</param>
        /// <param name="fragPath">Fragment Shader Path</param>
        /// <returns>Shader Program ID</returns>
        public static int LoadShader(string vertPath, string fragPath)
        {
            int vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            string vertexCode = File.ReadAllText(vertPath);
            string fragmentCode = File.ReadAllText(fragPath);

            GL.ShaderSource(vertexShaderID, vertexCode);
            GL.CompileShader(vertexShaderID);

            string vertexInfo = GL.GetShaderInfoLog(vertexShaderID);
            if (vertexInfo != "") Console.WriteLine(vertexInfo);

            GL.ShaderSource(fragmentShaderID, fragmentCode);
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