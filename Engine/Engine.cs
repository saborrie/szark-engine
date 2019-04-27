using OpenTK;
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
        public Color Background { get; set; }

        protected PerformanceMonitor Monitor { private set; get; }

        public WindowBorder WindowBorder
        {
            get => window.WindowBorder;
            set => window.WindowBorder = value;
        }

        public int Width
        {
            get => window.Width;
            set => window.Width = value;
        }

        public int Height
        {
            get => window.Height;
            set => window.Height = value;
        }

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

        public float TargetFPS
        {
            get => (float)window.TargetRenderFrequency;
            set => window.TargetRenderFrequency = value;
        }

        private int renderOffsetX, renderOffsetY;
        private readonly GameWindow window;

        /// <summary>
        /// Creates a window and starts OpenGL.
        /// </summary>
        public SzarkEngine(string title, int width, int height, bool resizable = false)
        {
            window = new GameWindow(width, height,
                GraphicsMode.Default, title);
            window.WindowBorder = resizable ? WindowBorder.Resizable :
                WindowBorder.Fixed;

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
            Update((float)e.Time);
            Input.Update();
        }

        private void Render(FrameEventArgs e)
        {
            GL.Viewport(renderOffsetX, renderOffsetY, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.red / 255f, Background.green / 255f, Background.blue / 255f, 1);

            Draw((float)e.Time);
            Monitor.Render((float)e.Time);
            window.SwapBuffers();
        }

        /// <summary>
        /// Makes this instance of the Engine the
        /// preferred focused instance.
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