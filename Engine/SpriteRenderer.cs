using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Szark
{
    public sealed class SpriteRenderer
    {
        public Sprite Sprite
        {
            get { return sprite; }
            set
            {
                if (value == null) return;
                bool sameSize = value.Width == sprite.Width &&
                    value.Height == sprite.Height;

                sprite = value;
                Graphics.DrawTarget = value;

                if (!sameSize) CreateTexImage2D();
                else Refresh();
            }
        }

        public int Shader { get; set; }
        public Graphics2D Graphics { get; private set; }

        private Sprite sprite;
        private readonly int mvpLocation;
        private int textureID;

        public SpriteRenderer(string path) : this(new Sprite(path),
            SzarkEngine.Context.BaseShader)
        { }

        public SpriteRenderer(int width, int height) : this(new Sprite(width,
            height), SzarkEngine.Context.BaseShader)
        { }

        public SpriteRenderer(Sprite sprite) : this(sprite,
            SzarkEngine.Context.BaseShader)
        { }

        public SpriteRenderer(Sprite sprite, int shaderID)
        {
            this.sprite = sprite;
            Shader = shaderID;

            Graphics = new Graphics2D(Sprite);
            mvpLocation = GL.GetUniformLocation(shaderID, "mvp");
            CreateTexture();
        }

        // Creates the initial texture with parameters
        private void CreateTexture()
        {
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

            // Calculate the orthographic scale and position
            var engine = SzarkEngine.Context;
            float right = fillScreen ? 2 : engine.ScreenWidth / sprite.Width;
            float top = fillScreen ? 2 : engine.ScreenHeight / sprite.Height;
            float posX = x / scale / sprite.Width, posY = y / scale / sprite.Height;

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
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadData.EBO);
            GL.BindVertexArray(QuadData.VAO);

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