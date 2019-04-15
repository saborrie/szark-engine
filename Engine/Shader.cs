using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    public struct Shader
    {
        /// <summary>
        /// Program ID for the Shader
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The MVP Location
        /// </summary>
        public int MVP { get; private set; }

        /// <summary>
        /// A Ordinary Sprite Shader
        /// </summary>
        public static Shader Default { get; private set; }

        private const string defaultVertex =
        @"
            #version 420 

            layout(location = 0) in vec2 pos;
            layout(location = 1) in vec2 tex;

            out vec2 texCoord;
            uniform mat4 mvp;

            void main() 
            {
                texCoord = tex;
                gl_Position = mvp * vec4(pos.x, pos.y, 0, 1.0);
            }
        ";

        private const string defaultFragment =
        @"
            #version 420

            out vec4 FragColor;
            in vec2 texCoord;
            uniform sampler2D tex;

            void main() {
                FragColor = texture(tex, texCoord);
            } 
        ";

        internal static void CreateDefaultShader()
        {
            Default = new Shader(defaultVertex, 
                defaultFragment, "mvp");
        }

        /// <summary>
        /// Creates a Shader Program from a Vertex Shader and a Fragment Shader
        /// </summary>
        /// <param name="vertexShader">Vertex Shader</param>
        /// <param name="fragmentShader">Fragment Shader</param>
        /// <returns>Shader Program ID</returns>
        public Shader(string vertexShader, string fragmentShader, string mvp)
        {
            int vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderID, vertexShader);
            GL.CompileShader(vertexShaderID);

            string vertexInfo = GL.GetShaderInfoLog(vertexShaderID);
            if (vertexInfo != "") Debug.Log(vertexInfo, LogLevel.WARNING);

            GL.ShaderSource(fragmentShaderID, fragmentShader);
            GL.CompileShader(fragmentShaderID);

            string fragmentInfo = GL.GetShaderInfoLog(fragmentShaderID);
            if (fragmentInfo != "") Debug.Log(fragmentInfo, LogLevel.WARNING);

            int programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            GL.LinkProgram(programID);

            string programInfo = GL.GetProgramInfoLog(programID);
            if (programInfo != "") Debug.Log(programInfo, LogLevel.WARNING);

            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);

            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);

            ID = programID;
            MVP = GL.GetUniformLocation(ID, mvp);
        }
    }
}
