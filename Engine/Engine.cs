/* Created by: Jakub P. Szarkowicz */

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Szark
{
    /// <summary>
    /// Derive from this class to access the engine.
    /// Use this as a starting point for your application.
    /// </summary>
    public abstract class SzarkEngine
    {
        public static SzarkEngine Context { get; private set; }
        public static event Action ContextChanged;

        public event Action FullscreenChanged;
        public event Action<float> WindowRendered;
        public event Action<float> WindowUpdated;

        public string Title { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int FPS { get; private set; }

        public bool ShowFPS { get; set; } = true;

        public bool Fullscreen
        {
            get => Window.WindowState == WindowState.Fullscreen;
            set
            {
                Window.WindowState = (WindowState)(value ? 3 : 0);
                renderOffsetX = value ? (Window.Width - Width) / 2 : 0;
                renderOffsetY = value ? (Window.Height - Height) / 2 : 0;
                FullscreenChanged?.Invoke();
            }
        }

        public Color Background { get; set; }
        public GameWindow Window { get; private set; }

        public VSyncMode Vsync
        {
            get => Window.VSync;
            set => Window.VSync = value;
        }

        private double lastFPSCheck;
        private int renderOffsetX, renderOffsetY;

        /// <summary>
        /// Creates a window and starts OpenGL.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelSize">Size of Each Pixel</param>
        public SzarkEngine(string title, int width, int height)
        {
            Width = width;
            Height = height;
            Title = title;

            Window = new GameWindow(width, height);
            Window.Title = title;

            MakeContext();
            Input.SetContext(this);
            Audio.Init();

            Window.Load += (s, f) => Start();
            Window.RenderFrame += (s, f) => Render(f);
            Window.Disposed += (s, f) => Destroyed();

            Window.UpdateFrame += (s, f) =>
            {
                Update((float)f.Time);
                WindowUpdated?.Invoke((float)f.Time);
            };

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha,
                BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            Window.WindowBorder = WindowBorder.Fixed;
            Window.Run();
        }

        private void Render(FrameEventArgs e)
        {
            WindowRendered?.Invoke((float)e.Time);

            GL.Viewport(renderOffsetX, renderOffsetY, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.red, Background.green, Background.blue, 1);

            Draw((float)e.Time);

            if ((lastFPSCheck += e.Time) > 1)
            {
                FPS = (int)(1 / e.Time);
                Window.Title = $"{Title} " + (ShowFPS ?
                    $"| FPS: {FPS}" : "");
                lastFPSCheck = 0;
            }

            Window.SwapBuffers();
        }

        public void MakeContext()
        {
            Context = this;
            ContextChanged?.Invoke();
        }

        /// <summary>
        /// Called when window is opened, use for initialization
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Called every tick, use for logic
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected abstract void Update(float deltaTime);

        /// <summary>
        /// Called every frame, used for drawing GPU Sprites, Shapes, etc.
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected abstract void Draw(float deltaTime);

        /// <summary>
        /// Called when window is closing, use for cleanup
        /// </summary>
        protected abstract void Destroyed();
    }
}