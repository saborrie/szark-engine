using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    public static class SpriteExtensions
    {
        private const float Clipping = 100f;

        public static void Render(this Sprite sprite, Transform transform, Action extra = null)
        {
            GL.UseProgram(sprite.Shader.id);

            GetMVP(sprite, transform, out var mvp);

            // Send matrices to the shader
            GL.UniformMatrix4(sprite.Shader.mvpLocation, false, ref mvp);

            // Bind texture the the shader
            GL.BindTexture(TextureTarget.Texture2D, sprite.id);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadData.EBO);
            GL.BindVertexArray(QuadData.VAO);

            extra?.Invoke();

            // Draw the sprite
            GL.DrawElements(PrimitiveType.Triangles,
                6, DrawElementsType.UnsignedInt, 0);
        }

        public static void RenderBatch(this Sprite sprite, Transform[] transforms, Action<int> extra = null)
        {
            GL.UseProgram(sprite.Shader.id);

            // Bind texture the the shader
            GL.BindTexture(TextureTarget.Texture2D, sprite.id);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadData.EBO);
            GL.BindVertexArray(QuadData.VAO);

            for (int i = 0; i < transforms.Length; i++)
            {
                GetMVP(sprite, transforms[i], out var mvp);

                // Send matrices to the shader
                GL.UniformMatrix4(sprite.Shader.mvpLocation, false, ref mvp);

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

            if (transform.scale != 1)
                mvp *= Matrix4.CreateScale(transform.scale, transform.scale, 1);

            mvp *= Matrix4.CreateTranslation(posX + 1, posY + 1, transform.layer / Clipping);
            mvp *= Matrix4.CreateOrthographicOffCenter(0, Window.Width / ratioX,
                Window.Height / ratioY, 0, -Clipping, Clipping);
        }
    }
}
