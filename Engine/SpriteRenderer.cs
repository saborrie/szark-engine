using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Szark
{
    public class SpriteRenderer
    {
        public Sprite Sprite { get; private set; }
        public int Shader { get; private set; }

        private readonly int VAO, EBO, textureID;
        private readonly int projLocation, modelLocation, 
            scaleLocation, rotLocation;

        private SzarkEngine engine;

        private const float orthoFactor = 0.0312f;

        private float[] vertices =
        {
            // Vertices    // Texture Coords
             1.0f,  1.0f,  1.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
            -1.0f,  1.0f,  0.0f, 1.0f
        };

        private int[] indices = 
        {
            0, 1, 3,
            1, 2, 3
        };

        public SpriteRenderer(SzarkEngine engine, Sprite sprite, int shaderID)
        {
            this.engine = engine;

            Shader = shaderID;
            Sprite = sprite;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, 
                vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4,
                indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 16, 8);
            GL.EnableVertexAttribArray(1);

            textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                sprite.Width, sprite.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    sprite.Pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Nearest);

            modelLocation = GL.GetUniformLocation(shaderID, "model");
            projLocation = GL.GetUniformLocation(shaderID, "projection");
            rotLocation = GL.GetUniformLocation(shaderID, "rotation");
            scaleLocation = GL.GetUniformLocation(shaderID, "scale");
        }

        /// <summary>
        /// Renders a sprite on screen with the GPU
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="rotation">Rotation (Radians)</param>
        /// <param name="scale">Scale</param>
        /// <param name="layer">Layer</param>
        /// <param name="fillScreen">Stretch to Screen?</param>
        public void Render(float x, float y, float rotation = 0, float scale = 1, 
            float layer = -1, bool fillScreen = false)
        {
            GL.UseProgram(Shader);

            float left = 0, right = fillScreen ? 2 : engine.ScreenWidth * orthoFactor;
            float bottom = 0, top = fillScreen ? 2 : engine.ScreenHeight * orthoFactor;
            float posX = x * orthoFactor / scale, posY = y * orthoFactor / scale;

            Matrix4.CreateTranslation(posX + 1, posY + 1, layer, out var model);
            Matrix4.CreateOrthographicOffCenter(left, right, top, bottom, 0.1f, 100f, out var projection);
            Matrix4.CreateRotationZ(rotation, out var rot);
            Matrix4.CreateScale(scale, out var sc);

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BindVertexArray(VAO);

            GL.UniformMatrix4(modelLocation, 1, false, ref model.Row0.X);
            GL.UniformMatrix4(projLocation, 1, false, ref projection.Row0.X);
            GL.UniformMatrix4(scaleLocation, 1, false, ref sc.Row0.X);
            GL.UniformMatrix4(rotLocation, 1, false, ref rot.Row0.X);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        /// <summary>
        /// Will refresh the texture if edited by the CPU
        /// </summary>
        public void Refresh()
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Sprite.Width, Sprite.Height,
                PixelFormat.Rgba, PixelType.UnsignedByte, Sprite.Pixels);
        }
    }
}