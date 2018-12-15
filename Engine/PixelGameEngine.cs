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

        private double lastFPSCheck;
        private GameWindow gameWindow;
        private Sprite background;

        private int quadVAO, textureID, shaderID;

        private float[] quadVerts = 
        {
             1.0f,  1.0f, 1.0f, 0.0f,
             1.0f, -1.0f, 1.0f, 1.0f,
            -1.0f,  1.0f, 0.0f, 0.0f,
            -1.0f, -1.0f, 0.0f, 1.0f,
        };

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

            background = new Sprite(ScreenWidth, ScreenHeight);
            background.Clear(Pixel.BLACK);

            GL.Enable(EnableCap.Texture2D);

            // Setup quad VAO
            quadVAO = GL.GenVertexArray();
            int quadVBO = GL.GenBuffer();
            GL.BindVertexArray(quadVAO);
            
            // Bind Data
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVerts.Length * 4, 
                quadVerts, BufferUsageHint.StaticDraw);

            // Setup Vertex Array Pointer
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 16, 8);

            // Create Texture
            textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                ScreenWidth, ScreenHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    Graphics.GetDrawTarget().GetData());

            // Configure Texture
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Nearest);

            // Load Base Shader
            shaderID = ShaderLoader.LoadShader("Engine/Shaders/base.vert", 
                "Engine/Shaders/base.frag");
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
            // Offsets the rendered frame to the center of the screen
            GL.Viewport(RenderOffsetX, RenderOffsetY, WindowWidth, WindowHeight);

            // Replaces current frame with a single-color background
            background.CopyTo(Graphics.GetDrawTarget());

            // Where user generated graphics will be drawn
            Draw((float)e.Time);

            if ((lastFPSCheck += e.Time) > 1)
            {
                CurrentFPS = (int)(1 / e.Time);
                gameWindow.Title = $"{WindowTitle} " + (ShowFPS ? 
                    $"| FPS: {CurrentFPS}" : "");
                lastFPSCheck = 0;
            }

            // Use Open-GL to Draw Graphics to Screen
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, ScreenWidth, ScreenHeight,
                PixelFormat.Rgba, PixelType.UnsignedByte, Graphics.GetDrawTarget().Pixels);

            // Load Shader
            GL.UseProgram(shaderID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Draw Quad to Screen
            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

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