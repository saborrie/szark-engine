using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using System;

namespace Szark
{
    /// <summary>
    /// Derive from this class to access the engine.
    /// Use this as a starting point for your application.
    /// </summary>
    public static partial class Game
    {
        public static event Action Started, Disposed;
        public static event Action<float> Updated, Rendered;

        private static float lastFPSCheck;
        private static Vector renderOffset;
        internal static GameWindow window;

        /// <summary>
        /// Creates a window and starts the Engine.
        /// </summary>
        public static void Start(string title, int width, int height)
        {
            if (window != null) return;

            window = new GameWindow(width, height);
            window.WindowBorder = (WindowBorder)1;
            window.Title = title;

            window.Load += (s, f) => Started?.Invoke();
            window.Disposed += (s, f) => Disposed?.Invoke();

            window.RenderFrame += (s, f) => Render(f);
            window.UpdateFrame += (s, f) => Update(f);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha,
                BlendingFactor.OneMinusSrcAlpha);

            Input.Init();
            window.Run();
        }

        private static void Update(FrameEventArgs e)
        {
            Updated?.Invoke((float)e.Time);
            Input.Update();
        }

        private static void Render(FrameEventArgs e)
        {
            GL.Viewport((int)renderOffset.x, (int)renderOffset.y, ScreenWidth, ScreenHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.r / 255f, Background.g / 255f, Background.b / 255f, 1);

            // Checks the Current Framerate
            if ((lastFPSCheck += (float)e.Time) >= 1)
            {
                Framerate = (float)(1f / e.Time);
                lastFPSCheck = 0;
            }

            Rendered?.Invoke((float)e.Time);
            window.SwapBuffers();
        }
    }
}