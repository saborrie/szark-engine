/*
 * Created by: Jakub P. Szarkowicz
 * Derivation of olcPixelGameEngine by Javidx9
 * Please check OLC-3.txt for Javidx9's license.
*/

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    /// <summary>
    /// The main engine with all Open GL and drawing
    /// methods. Derive from this class to access the engine.
    /// </summary>
    public abstract class SzarkEngine
    {
        public string WindowTitle { get; set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelSize { get; private set; }

        public int CurrentFPS { get; private set; }
        public int BaseShader { get; private set; }

        public bool ShowFPS { get; set; } = true;
        public Pixel Background { get; set; }
        public VSyncMode Vsync => gameWindow.VSync;

        public bool IsFullscreen
        {
            get => gameWindow.WindowState == WindowState.Fullscreen;
            set
            {
                gameWindow.WindowState = (WindowState)(value ? 3 : 0);
                renderOffsetX = value ? (gameWindow.Width - WindowWidth) / 2 : 0;
                renderOffsetY = value ? (gameWindow.Height - WindowHeight) / 2 : 0;
                Input.UpdateOffsets();
            }
        }

        private double lastFPSCheck;
        private int renderOffsetX, renderOffsetY;
        private GameWindow gameWindow;

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
            gameWindow = new GameWindow(width, height);
            gameWindow.Title = title;

            Audio.Init();
            Input.SetContext(this, gameWindow);

            // Set internal window events
            gameWindow.RenderFrame += (s, f) => Render(f);

            // Set abstract window events
            gameWindow.Load += (s, f) => Start();
            gameWindow.UpdateFrame += (s, f) => Update((float)f.Time);
            gameWindow.Disposed += (s, f) => Destroyed();

            // Calculate the internal screen dimensions
            ScreenWidth = WindowWidth / PixelSize;
            ScreenHeight = WindowHeight / PixelSize;

            // Do some OpenGL setup
            SetupOpenGL();

            // Set window as a fixed size
            gameWindow.WindowBorder = WindowBorder.Fixed;
            gameWindow.Run();
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
                gameWindow.Title = $"{WindowTitle} " + (ShowFPS ? 
                    $"| FPS: {CurrentFPS}" : "");
                lastFPSCheck = 0;
            }

            gameWindow.SwapBuffers();
        }

        /// <summary>
        /// Creates a Sprite Renderer
        /// </summary>
        /// <param name="sprite">The sprite to render</param>
        /// <returns></returns>
        public SpriteRenderer CreateRenderer(Sprite sprite) =>
            new SpriteRenderer(this, sprite, BaseShader);

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