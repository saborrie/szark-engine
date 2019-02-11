using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Szark
{
    public class SpriteRenderer
    {
        public Sprite Sprite
        {
            get { return sprite; }
            set
            {
                if (value == null) return;
                sprite = value;
                Graphics.DrawTarget = value;
                CreateTexImage2D();
            }
        }

        private Sprite sprite;

        public int Shader { get; set; }
        public Graphics2D Graphics { get; private set; }

        private readonly int VAO, EBO, textureID;
        private readonly int mvpLocation;
        private readonly SzarkEngine engine;

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
            this.sprite = sprite;
            this.engine = engine;
            Shader = shaderID;

            Graphics = new Graphics2D(Sprite);
            mvpLocation = GL.GetUniformLocation(shaderID, "mvp");

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
            CreateTexImage2D();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Nearest);
        }

        // Creates and binds a texture
        private void CreateTexImage2D()
        {
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                sprite.Width, sprite.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    sprite.Pixels);
        }

        /// <summary>
        /// Renders a sprite on screen with the GPU
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation">Radians</param>
        /// <param name="scale"></param>
        /// <param name="layer"></param>
        /// <param name="fillScreen">Stretch to Screen</param>
        public void Render(float x, float y, float rotation = 0, float scale = 1, float layer = -1, bool fillScreen = false)
        {
            GL.UseProgram(Shader);

            // Get the sprite's scale
            float scaleX = 1, scaleY = 1;
            if (sprite.Width > sprite.Height) scaleX = sprite.Width / sprite.Height;
            if (sprite.Width < sprite.Height) scaleY = sprite.Height / sprite.Width;

            // Calculate the orthographic scale and position
            float right = fillScreen ? 2 : (float)engine.ScreenWidth / engine.PixelSize / scaleX;
            float top = fillScreen ? 2 : (float)engine.ScreenHeight / engine.PixelSize / scaleY;
            float posX = x / scale / scaleX / engine.PixelSize, posY = y / scale / scaleY / engine.PixelSize;

            // Create matrices for the shader
            Matrix4.CreateTranslation(posX + 1, posY + 1, layer, out var mvp);
            Matrix4.CreateOrthographicOffCenter(0, right, top, 0, -100f, 100f, out var projection);
            Matrix4.CreateRotationZ(rotation, out var rot);
            Matrix4.CreateScale(scale, out var sc);

            // Send matrices to the shader
            mvp *= projection * rot * sc;
            GL.UniformMatrix4(mvpLocation, 1, false, ref mvp.Row0.X);

            // Bind texture the the shader
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BindVertexArray(VAO);

            // Draw the sprite
            GL.DrawElements(PrimitiveType.Triangles, 
                6, DrawElementsType.UnsignedInt, 0);
        }

        /// <summary>
        /// Will refresh the texture if edited by the CPU
        /// </summary>
        public void Refresh()
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, sprite.Width, sprite.Height,
                PixelFormat.Rgba, PixelType.UnsignedByte, sprite.Pixels);
        }
    }
}