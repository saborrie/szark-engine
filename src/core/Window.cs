﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Szark
{
    public static partial class Window
    {
        internal static GameWindow window;

        public static event Action Started, Disposed;
        public static event Action<float> Updated, Rendered;

        private static float lastFPSCheck;
        private static Vector renderOffset;

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