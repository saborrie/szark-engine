using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace Szark
{
    public sealed class Sprite : IDisposable
    {
        public readonly int id;
        public readonly int width, height;

        private const float Clipping = 100f;

        public Shader Shader { get; set; }

        public Sprite(Texture texture)
        {
            id = GL.GenTexture();
            Shader = Shader.Default;

            width = texture.Width;
            height = texture.Height;

            GL.BindTexture(TextureTarget.Texture2D, id);
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

        public void Dispose()
        {
            GL.DeleteTexture(id);
            GC.SuppressFinalize(this);
        }

        public void Render(Transform transform, Action extra = null)
        {
            GL.UseProgram(Shader.id);

            GetMVP(this, transform, out var mvp);

            // Send matrices to the shader
            GL.UniformMatrix4(Shader.mvpLocation, false, ref mvp);

            // Bind texture the the shader
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadData.EBO);
            GL.BindVertexArray(QuadData.VAO);

            extra?.Invoke();

            // Draw the sprite
            GL.DrawElements(PrimitiveType.Triangles,
                6, DrawElementsType.UnsignedInt, 0);
        }

        public void RenderBatch(Transform[] transforms, Action<int> extra = null)
        {
            GL.UseProgram(Shader.id);

            // Bind texture the the shader
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadData.EBO);
            GL.BindVertexArray(QuadData.VAO);

            for (int i = 0; i < transforms.Length; i++)
            {
                GetMVP(this, transforms[i], out var mvp);

                // Send matrices to the shader
                GL.UniformMatrix4(Shader.mvpLocation, false, ref mvp);

                extra?.Invoke(i);

                // Draw the sprite
                GL.DrawElements(PrimitiveType.Triangles,
                    6, DrawElementsType.UnsignedInt, 0);
            }
        }

        private static void GetMVP(Sprite sprite, Transform transform, out Matrix4 mvp)
        {
            float ratioX = sprite.width * 0.5f, ratioY = sprite.height * 0.5f;
            float posX = transform.position.x / ratioX, posY = transform.position.y / ratioY;

            mvp = Matrix4.Identity;

            if (transform.rotation != 0)
                mvp *= Matrix4.CreateRotationZ(transform.rotation);

            mvp *= Matrix4.CreateTranslation(posX + 1, posY + 1, transform.layer / Clipping);
            
            if (transform.scale != 1)
                mvp *= Matrix4.CreateScale(transform.scale, transform.scale, 1);

            mvp *= Matrix4.CreateOrthographicOffCenter(0, Window.Width / ratioX,
                Window.Height / ratioY, 0, -Clipping, Clipping);
        }
    }
}