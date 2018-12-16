/*
	PixelGameEngine.cs
        By: Jakub P. Szarkowicz / JakubSzark
	
	Please Check Github Page for Liscense
*/

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

using System;

namespace PGE
{
    /// <summary>
    /// The main engine with all Open GL and Drawing
    /// Methods. Derive from this class to access the engine.
    /// Make sure to construct the Engine and Call Start
    /// </summary>
    public abstract class PixelGameEngine
    {
        public string WindowTitle { get; protected set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public int PixelSize { get; private set; }

        public bool IsFullscreen { get; private set; }

        public Audio Audio { get; private set; }
        public Graphics2D Graphics { get; private set; }
        public Input Input { get; private set; }

        public int RenderOffsetX { get; private set; }
        public int RenderOffsetY { get; private set; }

        public bool ShowFPS { get; set; } = true;
        public int CurrentFPS { get; private set; }

        public int BaseShaderID { get; private set; }

        private double lastFPSCheck;
        private GameWindow gameWindow;
        private SpriteRenderer pixelGraphics;
        private Sprite background;

        /// <summary>
        /// Creates a Window and Starts OpenGL.
        /// </summary>
        /// <param name="width">Width of the Screen</param>
        /// <param name="height">Height of the Screen</param>
        /// <param name="pixelSize">Size of Each Pixel</param>
        public void Construct(int width = 512, int height = 512, int pixelSize = 8)
        {
            WindowWidth = width;
            WindowHeight = height;
            PixelSize = pixelSize;

            InitializeWindow();
            SetupGL();

            Audio = new Audio();
            Input = new Input(this, gameWindow);
            gameWindow.WindowBorder = WindowBorder.Fixed;
            gameWindow.Run();
        }

        private void InitializeWindow()
        {
            gameWindow = new GameWindow(WindowWidth, 
                WindowHeight, GraphicsMode.Default, WindowTitle);
            gameWindow.VSync = VSyncMode.Off;

            gameWindow.Load += Loaded;
            gameWindow.RenderFrame += Render;
            gameWindow.UpdateFrame += Update;
            gameWindow.Disposed += Disposed;
        }

        private void SetupGL()
        {
            ScreenWidth = WindowWidth / PixelSize;
            ScreenHeight = WindowHeight / PixelSize;

            Graphics = new Graphics2D(new Sprite(ScreenWidth, ScreenHeight));
            Graphics.GetDrawTarget().Clear(Pixel.RED);

            background = new Sprite(ScreenWidth, ScreenHeight);
            background.Clear(Pixel.BLACK);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Load Base Shader
            BaseShaderID = ShaderLoader.LoadShader("Engine/Shaders/sprite.vert", 
                "Engine/Shaders/sprite.frag");
            pixelGraphics = new SpriteRenderer(this, Graphics.GetDrawTarget(), BaseShaderID);
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0, 0, 0, 1);

            // Offsets the rendered frame to the center of the screen
            GL.Viewport(RenderOffsetX, RenderOffsetY, WindowWidth, WindowHeight);

            // Replaces current frame with a single-color background
            background.CopyTo(Graphics.GetDrawTarget());

            // Drawing the Pixel Background
            pixelGraphics.Render(0, 0, 0, 1, -99, true);

            // Where user generated graphics will be drawn
            Draw((float)e.Time);

            pixelGraphics.Refresh();

            // Draws GPU based Graphics
            GPUDraw((float)e.Time);

            if ((lastFPSCheck += e.Time) > 1)
            {
                CurrentFPS = (int)(1 / e.Time);
                gameWindow.Title = $"{WindowTitle} " + (ShowFPS ? 
                    $"| FPS: {CurrentFPS}" : "");
                lastFPSCheck = 0;
            }

            Input.Update();
            gameWindow.SwapBuffers();
        }

        #endregion

        #region Extra

        /// <summary>
        /// Sets the Mode for VSync
        /// </summary>
        /// <param name="isActive">Is On?</param>
        public void SetVSync(VSyncMode mode) => 
            gameWindow.VSync = mode;

        /// <summary>
        /// Sets the Window to be Fullscreen
        /// </summary>
        /// <param name="fullscreen">Is Fullscreen?</param>
        public void SetFullscreen(bool fullscreen)
        {
            IsFullscreen = fullscreen;
            gameWindow.WindowState = fullscreen ? WindowState.Fullscreen :
                WindowState.Normal;

            RenderOffsetX = fullscreen ? (gameWindow.Width - WindowWidth) / 2 : 0;
            RenderOffsetY = fullscreen ? (gameWindow.Height - WindowHeight) / 2 : 0;
        }

        /// <summary>
        /// Sets the Background Color
        /// </summary>
        /// <param name="p">The Color</param>
        public void SetBackground(Pixel p) =>
            background.Clear(p);

        #endregion

        #region Abstractions

        /// <summary>
        /// Called when Window is Opened, use for Initialization
        /// </summary>
        protected virtual void Start() {}

        /// <summary>
        /// Called every Tick, use for Logic
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected virtual void Update(float deltaTime) {}

        /// <summary>
        /// Called every Frame, use for Drawing
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected virtual void Draw(float deltaTime) {}

        /// <summary>
        /// Called every Frame, used for drawing GPU Sprites, Shapes, etc.
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        protected virtual void GPUDraw(float deltaTime) {}

        /// <summary>
        /// Called when Window is Closed, use for Cleanup
        /// </summary>
        protected virtual void Destroyed() {}

        #endregion
    }
}