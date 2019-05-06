using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    public sealed class Sprite
    {
        public int ID { get; private set; }
        public Shader Shader { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Sprite(Texture texture)
        {
            ID = GL.GenTexture();
            Shader = Shader.Default;

            Width = texture.Width;
            Height = texture.Height;

            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    texture.Pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);
        }

        public void Refresh(Texture texture)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, texture.Width, texture.Height,
                PixelFormat.Rgba, PixelType.UnsignedByte, texture.Pixels);
        }
    }
}