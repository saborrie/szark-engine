/*
	PixelGameEngine.cs
        By: Jakub P. Szarkowicz / JakubSzark
	Credit: One Lone Coder
	
	Please Check Github Page for Liscense
*/

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;

using System;
using System.IO;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

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

        private int lastFPSCheck;
        private int glBuffer;

        private GameWindow gameWindow;

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

            GL.Enable(EnableCap.Texture2D);
            GL.GenTextures(1, out glBuffer);
            GL.BindTexture(TextureTarget.Texture2D, glBuffer);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                ScreenWidth, ScreenHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    Graphics.GetDrawTarget().GetData());

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.TexEnv(TextureEnvTarget.TextureEnv,
                TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
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
            GL.Viewport(RenderOffsetX, RenderOffsetY, WindowWidth, WindowHeight);

            Draw((float)e.Time);

            if (lastFPSCheck++ > 120)
            {
                gameWindow.Title = $"{WindowTitle} | FPS: {(int)(1 / e.Time)}";
                lastFPSCheck = 0;
            }

            // Use Open-GL to Draw Graphics to Screen
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, ScreenWidth, ScreenHeight,
                PixelFormat.Rgba, PixelType.UnsignedByte, Graphics.GetDrawTarget().GetPixels());

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(-1, -1);
            GL.TexCoord2(0, 0); GL.Vertex2(-1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, -1);
            GL.End();

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

        #endregion

        #region Abstractions

        /// <summary>
        /// Called when Window is Opened, use for Initialization
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Called every Tick, use for Logic
        /// </summary>
        /// <param name="deltaTime">Update Time</param>
        protected abstract void Update(float deltaTime);

        /// <summary>
        /// Called every Frame, use for Drawing
        /// </summary>
        /// <param name="deltaTime">Render Time</param>
        protected abstract void Draw(float deltaTime);

        /// <summary>
        /// Called when Window is Closed, use for Cleanup
        /// </summary>
        protected abstract void Destroyed();

        #endregion
    }
}