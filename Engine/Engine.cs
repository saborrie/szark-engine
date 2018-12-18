/*
 * Created by: Jakub P. Szarkowicz
 * Derivation of olcPixelGameEngine by Javidx9
 * Please check OLC-3.txt for Javidx9's license.
*/

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Szark
{
    /// <summary>
    /// The main engine with all Open GL and drawing
    /// methods. Derive from this class to access the engine.
    /// </summary>
    public abstract class SzarkEngine
    {
        public string WindowTitle { get; protected set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelSize { get; private set; }

        public int CurrentFPS { get; private set; }
        public int BaseShaderID { get; private set; }

        public bool ShowFPS { get; set; } = true;
        public bool IsFullscreen { get; private set; }

        public Action AdditionalUpdates { get; set; }

        private double lastFPSCheck;
        private int renderOffsetX, renderOffsetY;

        private GameWindow gameWindow;
        private SpriteRenderer pixelGraphics;
        private Graphics2D graphics;
        private Sprite background;

        private const string vertexShader = 
        @"
            #version 400 

            layout(location = 0) in vec2 pos;
            layout(location = 1) in vec2 tex;

            out vec2 texCoord;

            uniform mat4 projection;
            uniform mat4 model;
            uniform mat4 rotation;
            uniform mat4 scale;

            void main() 
            {
                texCoord = tex;
                gl_Position = projection * scale * model * rotation * vec4(pos.x, pos.y, 0, 1.0);
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
        /// <param name="width">Width of the Window</param>
        /// <param name="height">Height of the Window</param>
        /// <param name="pixelSize">Size of Each Pixel</param>
        public void Construct(int width = 800, int height = 800, int pixelSize = 8)
        {
            WindowWidth = width;
            WindowHeight = height;
            PixelSize = pixelSize;

            gameWindow = new GameWindow(WindowWidth, WindowHeight)
            {
                VSync = VSyncMode.Off,
                Title = WindowTitle
            };

            gameWindow.Load += Loaded;
            gameWindow.RenderFrame += Render;
            gameWindow.UpdateFrame += Update;
            gameWindow.Disposed += Disposed;

            ScreenWidth = WindowWidth / PixelSize;
            ScreenHeight = WindowHeight / PixelSize;

            background = new Sprite(ScreenWidth, ScreenHeight);
            graphics = new Graphics2D(ScreenWidth, ScreenHeight);
            background.Clear(Pixel.BLACK);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, 
                BlendingFactorDest.OneMinusSrcAlpha);

            BaseShaderID = ShaderLoader.CreateProgram(vertexShader, fragmentShader);
            pixelGraphics = new SpriteRenderer(this, graphics.DrawTarget, BaseShaderID);

            Audio.Init();
            Input.SetContext(this, gameWindow);

            gameWindow.WindowBorder = WindowBorder.Fixed;
            gameWindow.Run();
        }

        #region Events

        // On Window Loaded
        private void Loaded(object sender, EventArgs e) => Start();

        // On Window Disposed
        private void Disposed(object sender, EventArgs e) => Destroyed();

        // On Window Update
        private void Update(object sender, FrameEventArgs e) =>
            Update((float)e.Time);

        // On Window Render Frame
        private void Render(object sender, FrameEventArgs e)
        {
            GL.Viewport(renderOffsetX, renderOffsetY, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0, 0, 0, 1);

            background.CopyTo(graphics.DrawTarget);
            pixelGraphics.Render(0, 0, 0, 1, -99, true);

            Draw(graphics, (float)e.Time);
            pixelGraphics.Refresh();
            GPUDraw((float)e.Time);

            if ((lastFPSCheck += e.Time) > 1)
            {
                CurrentFPS = (int)(1 / e.Time);
                gameWindow.Title = $"{WindowTitle} " + (ShowFPS ? 
                    $"| FPS: {CurrentFPS}" : "");
                lastFPSCheck = 0;
            }

            AdditionalUpdates?.Invoke();
            gameWindow.SwapBuffers();
        }

        #endregion

        #region Extra

        /// <summary>
        /// Sets the mode for VSync
        /// </summary>
        /// <param name="isActive">Is On?</param>
        public void SetVSync(VSyncMode mode) => 
            gameWindow.VSync = mode;

        /// <summary>
        /// Sets the window to be fullscreen
        /// </summary>
        /// <param name="fullscreen">Is Fullscreen?</param>
        public void SetFullscreen(bool fullscreen)
        {
            IsFullscreen = fullscreen;
            gameWindow.WindowState = fullscreen ? WindowState.Fullscreen :
                WindowState.Normal;

            renderOffsetX = fullscreen ? (gameWindow.Width - WindowWidth) / 2 : 0;
            renderOffsetY = fullscreen ? (gameWindow.Height - WindowHeight) / 2 : 0;
            Input.UpdateOffsets();
        }

        /// <summary>
        /// Sets the background color
        /// </summary>
        /// <param name="color">The Color</param>
        public void SetBackgroundColor(Pixel color) =>
            background.Clear(color);

        #endregion

        #region Abstractions

        /// <summary>
        /// Called when window is opened, use for initialization
        /// </summary>
        protected virtual void Start() {}

        /// <summary>
        /// Called every tick, use for logic
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected virtual void Update(float deltaTime) {}

        /// <summary>
        /// Called every frame, use for drawing
        /// </summary>
        /// <param name="graphics">The Graphics</param>
        /// <param name="deltaTime">Delta Time</param>
        protected virtual void Draw(Graphics2D graphics, float deltaTime) {}

        /// <summary>
        /// Called every frame, used for drawing GPU Sprites, Shapes, etc.
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected virtual void GPUDraw(float deltaTime) {}

        /// <summary>
        /// Called when window is closing, use for cleanup
        /// </summary>
        protected virtual void Destroyed() {}

        #endregion
    }
}