/*
 * Created by: Jakub P. Szarkowicz
 * Derivation of olcPixelGameEngine by Javidx9
 * Please check OLC-3.txt for Javidx9's license.
*/

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Szark
{
    /// <summary>
    /// The main engine with all Open GL and drawing
    /// methods. Derive from this class to access the engine.
    /// </summary>
    public abstract class SzarkEngine
    {
        #region Variables

        public string WindowTitle { get; set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelSize { get; private set; }

        public int CurrentFPS { get; private set; }
        public int BaseShader { get; private set; }

        public bool ShowFPS { get; set; } = true;
        public GameWindow GameWindow { get; private set; }
        public Color Background { get; set; }

        public VSyncMode Vsync
        {
            get => GameWindow.VSync;
            set => GameWindow.VSync = value;
        }

        private double lastFPSCheck;
        private int renderOffsetX, renderOffsetY;

        public bool IsFullscreen
        {
            get => GameWindow.WindowState == WindowState.Fullscreen;
            set
            {
                GameWindow.WindowState = (WindowState)(value ? 3 : 0);
                renderOffsetX = value ? (GameWindow.Width - WindowWidth) / 2 : 0;
                renderOffsetY = value ? (GameWindow.Height - WindowHeight) / 2 : 0;
                WindowStateChanged?.Invoke();
            }
        }

        #endregion

        #region Context

        /// <summary>
        /// This determines what engine is the current focused
        /// </summary>
        public static SzarkEngine Context { get; private set; }

        /// <summary>
        /// Called when the context variable changes
        /// </summary>
        public static event Action ContextChanged;

        #endregion

        #region Events

        /// <summary>
        /// Called when fullscreen property is changed
        /// </summary>
        public event Action WindowStateChanged;

        /// <summary>
        /// Called when the window renders a frame
        /// </summary>
        public event Action<float> WindowRendered;

        /// <summary>
        /// Called when the window updates
        /// </summary>
        public event Action<float> WindowUpdated;

        #endregion

        #region Shaders

        private const string vertexShader =
        @"
            #version 400 

            layout(location = 0) in vec2 pos;
            layout(location = 1) in vec2 tex;

            out vec2 texCoord;
            uniform mat4 mvp;

            void main() 
            {
                texCoord = tex;
                gl_Position = mvp * vec4(pos.x, pos.y, 0, 1.0);
            }
        ";

        private const string fragmentShader =
        @"
            #version 400

            out vec4 FragColor;
            in vec2 texCoord;
            uniform sampler2D tex;

            void main() {
                FragColor = texture(tex, texCoord);
            } 
        ";

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Creates a window and starts OpenGL.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelSize">Size of Each Pixel</param>
        public SzarkEngine(string title, int width, int height, int pixelSize = 8)
        {
            WindowWidth = width;
            WindowHeight = height;
            WindowTitle = title;
            PixelSize = pixelSize;

            // Create the window
            GameWindow = new GameWindow(width, height) {
                Title = title
            };

            MakeContext();
            Input.SetContext(this);
            Audio.Init();

            GameWindow.Load += (s, f) => Start();
            GameWindow.Disposed += (s, f) => Destroyed();

            GameWindow.RenderFrame += (s, f) =>
            {
                Render(f);
                WindowRendered?.Invoke((float)f.Time);
            };

            GameWindow.UpdateFrame += (s, f) =>
            {
                Update((float)f.Time);
                WindowUpdated?.Invoke((float)f.Time);
            };

            // Calculate the internal screen dimensions
            ScreenWidth = WindowWidth / PixelSize;
            ScreenHeight = WindowHeight / PixelSize;

            // Do some OpenGL setup
            SetupOpenGL();

            // Set window as a fixed size
            GameWindow.WindowBorder = WindowBorder.Fixed;
            GameWindow.Run();
        }

        private void SetupOpenGL()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha,
                BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            BaseShader = ShaderLoader.CreateProgram(vertexShader,
                fragmentShader);
        }

        #endregion

        #region Rendering

        private void Render(FrameEventArgs e)
        {
            // Clear screen the a single color
            GL.Viewport(renderOffsetX, renderOffsetY, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background.red, Background.green, Background.blue, 1);

            Draw((float)e.Time);

            // Calculate framerate
            if ((lastFPSCheck += e.Time) > 1)
            {
                CurrentFPS = (int)(1 / e.Time);
                GameWindow.Title = $"{WindowTitle} " + (ShowFPS ?
                    $"| FPS: {CurrentFPS}" : "");
                lastFPSCheck = 0;
            }

            GameWindow.SwapBuffers();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Makes this Engine the Current Context
        /// </summary>
        public void MakeContext()
        {
            Context = this;
            ContextChanged?.Invoke();
        }

        #endregion

        #region Abstractions

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

        #endregion
    }
}