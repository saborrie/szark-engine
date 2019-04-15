﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    public static class SpriteExtensions
    {
        private static float zRange = 100f;

        /// <summary>
        /// Renders a sprite on screen with the GPU
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Render(this Sprite sprite, float x, 
            float y, float rotation = 0, float scale = 1, int layer = 1)
        {
            GL.UseProgram(sprite.ID);

            var engine = SzarkEngine.Context;

            var mvp = Matrix4.CreateRotationZ(rotation);
            mvp *= Matrix4.CreateTranslation(x / scale / sprite.Width, 
                    y / scale / sprite.Height, layer / zRange);
            mvp *= Matrix4.CreateScale(scale, scale, 1);
            mvp *= Matrix4.CreateOrthographic(engine.Width * 2.0f / sprite.Width,
                engine.Height * 2.0f / sprite.Height, -zRange, zRange);

            // Send matrices to the shader
            GL.UniformMatrix4(sprite.Shader.MVP, 1, false, ref mvp.Row0.X);

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
