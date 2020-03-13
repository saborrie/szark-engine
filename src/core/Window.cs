using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Szark
{
    public static partial class Window
    {
        public static string Title
        {
            get => window != null ? window.Title : "";
            set => window.Title = value;
        }

        public static int Width
        {
            get => window != null ? window.Width : 0;
            set => window.Width = value;
        }

        public static int Height
        {
            get => window != null ? window.Height : 0;
            set => window.Height = value;
        }

        public static bool Resizeable
        {
            get => window != null ? window.WindowBorder == WindowBorder.Resizable : false;
            set => window.WindowBorder = (WindowBorder)(value ? 0 : 1);
        }

        public static bool Fullscreen
        {
            get => window?.WindowState == (WindowState)3;
            set
            {
                window.WindowState = (WindowState)(value ? 3 : 0);
                renderOffset.x = value ? (window.Width - Width) / 2 : 0;
                renderOffset.y = value ? (window.Height - Height) / 2 : 0;
            }
        }

        public static bool Vsync
        {
            get => window?.VSync == VSyncMode.On;
            set => window.VSync = (VSyncMode)(value ? 1 : 0);
        }

        public static float TargetFramerate
        {
            get => window != null ? (float)window.TargetRenderFrequency : 0;
            set => window.TargetRenderFrequency = value;
        }

        public static bool Visible
        {
            get => window != null ? window.Visible : false;
            set => window.Visible = value;
        }

        public static bool Focused => 
            window != null ? window.Focused : false;

        public static float Framerate { get; private set; }
        public static Color Background { get; set; }

        internal static GameWindow window;

        public static event Action Started, Disposed;
        public static event Action<float> Updated, Rendered;

        private static float lastFPSCheck;
        private static Vector2 renderOffset;

        public static void Create(string title, int width, int height)
        {
            if (window != null) return;

            window = new GameWindow(width, height);
            window.WindowBorder = (WindowBorder)1;
            window.Title = title;

            window.Load += (s, f) => Started?.Invoke();
            window.Disposed += (s, f) => Disposed?.Invoke();

            window.RenderFrame += (s, f) => PreRender(f);
            window.UpdateFrame += (s, f) => PreUpdate(f);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha,
                BlendingFactor.OneMinusSrcAlpha);

            Input.Init();
            window.Run();
        }

        public static void Stop()
        {
            if (window != null)
            {
                window.Exit();
                window = null;
            }
        }

        private static void PreUpdate(FrameEventArgs e)
        {
            Updated?.Invoke((float)e.Time);
            Input.Update();
        }

        private static void PreRender(FrameEventArgs e)
        {
            GL.Viewport((int)renderOffset.x, (int)renderOffset.y, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.r / 255f, Background.g / 255f, Background.b / 255f, 1);

            // Checks the Current Framerate
            if ((lastFPSCheck += (float)e.Time) >= 1)
            {
                Framerate = (float)(1f / e.Time);
                lastFPSCheck = 0;
            }

            Rendered?.Invoke((float)e.Time);
            window?.SwapBuffers();
        }
    }
}