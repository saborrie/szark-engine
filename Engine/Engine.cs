using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    /// <summary>
    /// Derive from this class to access the engine.
    /// Use this as a starting point for your application.
    /// </summary>
    public abstract class SzarkEngine
    {
        public static SzarkEngine Context { get; private set; }

        public string Title { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int FPS { get; private set; }

        public bool ShowFPS { get; set; } = true;
        public Color Background { get; set; }

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

        private double lastFPSCheck;
        private int renderOffsetX, renderOffsetY;
        private readonly GameWindow window;

        /// <summary>
        /// Creates a window and starts OpenGL.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelSize">Size of Each Pixel</param>
        public SzarkEngine(string title, int width, int height)
        {
            Title = title;
            window = new GameWindow(width, height,
                GraphicsMode.Default, title);

            window.WindowBorder = WindowBorder.Fixed;

            Width = window.Width;
            Height = window.Height;

            MakeContext();
            QuadData.CreateQuadData();
            Shader.CreateDefaultShader();

            window.Load += (s, f) => Start();
            window.RenderFrame += (s, f) => Render(f);
            window.Disposed += (s, f) => Destroyed();

            window.UpdateFrame += (s, f) =>
            {
                Update((float)f.Time);
                Input.Update();
            };

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha,
                BlendingFactor.OneMinusSrcAlpha);

            window.Run();
        }

        private void Render(FrameEventArgs e)
        {
            GL.Viewport(renderOffsetX, renderOffsetY, window.Width, window.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.red, Background.green, Background.blue, 1);

            Draw((float)e.Time);

            if ((lastFPSCheck += e.Time) > 1)
            {
                FPS = (int)(1 / e.Time);
                window.Title = $"{Title} " + (ShowFPS ?
                    $"| FPS: {FPS}" : "");
                lastFPSCheck = 0;
            }

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