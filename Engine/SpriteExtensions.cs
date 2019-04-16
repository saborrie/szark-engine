using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    public static class SpriteExtensions
    {
        private static readonly float zRange = 100f;

        /// <summary>
        /// Renders a sprite on screen with the GPU
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Render(this Sprite sprite, float x, 
            float y, float rotation = 0, float scale = 1, int layer = 1)
        {
            GL.UseProgram(sprite.Shader.ID);

            var engine = SzarkEngine.Context;
            float ratioX = sprite.Width * 0.5f, ratioY = sprite.Height * 0.5f;
            float posX = x / ratioX, posY = y / ratioY;

            var mvp = Matrix4.Identity;
            mvp *= Matrix4.CreateRotationZ(rotation);
            mvp *= Matrix4.CreateScale(scale, scale, 1);
            mvp *= Matrix4.CreateTranslation(posX + 1, posY + 1, layer / zRange);
            mvp *= Matrix4.CreateOrthographicOffCenter(0, engine.Width / ratioX,
                engine.Height / ratioY, 0, -zRange, zRange);

            // Send matrices to the shader
            GL.UniformMatrix4(sprite.Shader.MVP, false, ref mvp);

            // Bind texture the the shader
            GL.BindTexture(TextureTarget.Texture2D, sprite.ID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadData.EBO);
            GL.BindVertexArray(QuadData.VAO);

            // Draw the sprite
            GL.DrawElements(PrimitiveType.Triangles,
                6, DrawElementsType.UnsignedInt, 0);
        }
    }
}
