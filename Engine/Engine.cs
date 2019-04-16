﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;

namespace Szark
{
    /// <summary>
    /// Derive from this class to access the engine.
    /// Use this as a starting point for your application.
    /// </summary>
    public abstract class SzarkEngine
    {
        /// <summary>
        /// The Currently Focused Window / Engine
        /// </summary>
        public static SzarkEngine Context { get; private set; }

        public string Title { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color Background { get; set; }

        public float TargetUPS { get; set; }
        public float TargetFPS { get; set; }

        protected PerformanceMonitor Monitor { private set; get; }

        public bool Fullscreen
        {
            get => window.WindowState == WindowState.Fullscreen;
            set
            {
                window.WindowState = (WindowState)(value ? 3 : 0);
                renderOffsetX = value ? (window.Width - Width) / 2 : 0;
                renderOffsetY = value ? (window.Height - Height) / 2 : 0;
                Input.UpdateOffsets();
            }
        }

        public bool Vsync
        {
            get => window.VSync == VSyncMode.On;
            set => window.VSync = value ? 
                VSyncMode.On : VSyncMode.Off;
        }

        private int renderOffsetX, renderOffsetY;
        private readonly GameWindow window;

        /// <summary>
        /// Creates a window and starts OpenGL.
        /// </summary>
        public SzarkEngine(string title, int width, int height)
        {
            window = new GameWindow(width, height,
                GraphicsMode.Default, title);
            window.WindowBorder = WindowBorder.Fixed;

            Width = window.Width;
            Height = window.Height;
            Title = window.Title;

            MakeContext();

            window.Load += (s, f) => Start();
            window.RenderFrame += (s, f) => Render(f);
            window.UpdateFrame += (s, f) => Update(f);
            window.Disposed += (s, f) => Destroyed();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha,
                BlendingFactor.OneMinusSrcAlpha);

            Monitor = new PerformanceMonitor();
            window.Run();
        }

        private void Update(FrameEventArgs e)
        {
            window.TargetRenderFrequency = TargetFPS;
            window.TargetUpdateFrequency = TargetUPS;

            Update((float)e.Time);
            Monitor.Update((float)e.Time);
            Input.Update();
        }

        private void Render(FrameEventArgs e)
        {
            GL.Viewport(renderOffsetX, renderOffsetY, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.red, Background.green, Background.blue, 1);

            Draw((float)e.Time);
            Monitor.Render((float)e.Time);
            window.SwapBuffers();
        }

        /// <summary>
        /// Makes this instance of the Engine the
        /// prefered focused instance.
        /// </summary>
        public void MakeContext()
        {
            Context = this;
            Input.SetContext(window, this);
        }

        /// <summary>
        /// Called when window is opened.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Called every tick.
        /// </summary>
        protected abstract void Update(float deltaTime);

        /// <summary>
        /// Called every frame.
        /// </summary>
        protected abstract void Draw(float deltaTime);

        /// <summary>
        /// Called when window is closes.
        /// </summary>
        protected abstract void Destroyed();
    }
}