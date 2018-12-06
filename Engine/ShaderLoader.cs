using OpenTK.Graphics.OpenGL;
using System.IO;
using System;

namespace PGE
{
    public class ShaderLoader
    {
        public static int LoadShader(string vertPath, string fragPath)
        {
            int vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            string vertexCode = File.ReadAllText(vertPath);
            string fragmentCode = File.ReadAllText(fragPath);

            GL.ShaderSource(vertexShaderID, vertexCode);
            GL.CompileShader(vertexShaderID);

            Console.WriteLine(GL.GetShaderInfoLog(vertexShaderID));

            GL.ShaderSource(fragmentShaderID, fragmentCode);
            GL.CompileShader(fragmentShaderID);

            Console.WriteLine(GL.GetShaderInfoLog(fragmentShaderID));

            int programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            GL.LinkProgram(programID);

            Console.WriteLine(GL.GetProgramInfoLog(programID));

            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);

            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);

            return programID;
        }
    }
}