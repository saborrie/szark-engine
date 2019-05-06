using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    public sealed class Sprite
    {
        public readonly int id;
        public readonly int width, height;

        public Shader Shader { get; set; }

        public Sprite(Texture texture)
        {
            id = GL.GenTexture();
            Shader = Shader.Default;

            width = texture.width;
            height = texture.height;

            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                texture.width, texture.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    texture.pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);
        }

        public void Refresh(Texture texture)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, texture.width, texture.height,
                PixelFormat.Rgba, PixelType.UnsignedByte, texture.pixels);
        }
    }
}