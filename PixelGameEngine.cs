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
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace PGE
{
    /// <summary>
    /// A struct containing RGBA information for a pixel.
    /// This also can be used for representing color
    /// </summary>
    public struct Pixel
    {
        public byte r, g, b, a;

        /// <summary>
        /// The Constructor for a Pixel in RGBA Form.
        /// Alpha is not required and will just render
        /// completely opaque.
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <param name="a">Alpha</param>
        public Pixel(byte r, byte g, byte b, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        // Greyscale Colors
        public static Pixel BLANK = new Pixel(0, 0, 0, 0);
        public static Pixel WHITE = new Pixel(255, 255, 255);
        public static Pixel GREY = new Pixel(192, 192, 192);
        public static Pixel BLACK = new Pixel(0, 0, 0);

        public static Pixel DARK_GREY = new Pixel(128, 128, 128);

        public static Pixel VERY_DARK_GREY = new Pixel(64, 64, 64);

        // RGB Colors
        public static Pixel RED = new Pixel(255, 0, 0);
        public static Pixel GREEN = new Pixel(0, 255, 0);
        public static Pixel BLUE = new Pixel(0, 0, 255);

        public static Pixel DARK_RED = new Pixel(128, 0, 0);
        public static Pixel DARK_GREEN = new Pixel(0, 128, 0);
        public static Pixel DARK_BLUE = new Pixel(0, 0, 128);

        public static Pixel VERY_DARK_RED = new Pixel(64, 0, 0);
        public static Pixel VERY_DARK_GREEN = new Pixel(0, 64, 0);
        public static Pixel VERY_DARK_BLUE = new Pixel(0, 0, 64);

        // CYM Colors
        public static Pixel YELLOW = new Pixel(255, 255, 0);
        public static Pixel MAGENTA = new Pixel(255, 0, 255);
        public static Pixel CYAN = new Pixel(0, 255, 255);

        public static Pixel DARK_YELLOW = new Pixel(128, 128, 0);
        public static Pixel DARK_MAGENTA = new Pixel(128, 0, 128);
        public static Pixel DARK_CYAN = new Pixel(0, 128, 128);

        public static Pixel VERY_DARK_YELLOW = new Pixel(64, 64, 0);
        public static Pixel VERY_DARK_MAGENTA = new Pixel(64, 0, 64);
        public static Pixel VERY_DARK_CYAN = new Pixel(0, 64, 64);

        /// <summary>
        /// Interpolates Two Pixels based on T between 0 to 1
        /// </summary>
        /// <param name="a">First Pixel</param>
        /// <param name="b">Second Pixel</param>
        /// <param name="t">Alpha</param>
        /// <returns>The Blended Pixel</returns>
        public static Pixel Lerp(Pixel a, Pixel b, float t) =>
            new Pixel((byte)((1 - t) * a.r + t * b.r), (byte)((1 - t) * a.g + t * b.g),
                (byte)((1 - t) * a.b + t * b.b), (byte)((1 - t) * a.a + t * b.a));

        /// <summary>
        /// Converts a Pixel in a UInt
        /// </summary>
        /// <returns>A UInt that represents a Pixel</returns>
        public uint ToInt() =>
            (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));

        /// <summary>
        /// Converts a UInt to a Pixel
        /// </summary>
        /// <param name="i">UInt</param>
        /// <returns>Pixel</returns>
        public static Pixel ToPixel(uint i) =>
            new Pixel((byte)(i >> 0), (byte)(i >> 8), 
                (byte)(i >> 16), (byte)(i >> 24));

        public static Pixel operator+(Pixel f, Pixel p) =>
            new Pixel((byte)(f.r + p.r), (byte)(f.g + p.g), (byte)(f.b + p.b), (byte)(f.a + p.a));

        public static Pixel operator-(Pixel f, Pixel p) =>
            new Pixel((byte)(f.r - p.r), (byte)(f.g - p.g), (byte)(f.b - p.b), (byte)(f.a - p.a));

        public static Pixel operator*(Pixel f, float t) =>
            new Pixel((byte)(f.r * t), (byte)(f.g * t), (byte)(f.b * t), (byte)(f.a * t));

        /// <summary>
        /// Compares this pixel's color to another
        /// </summary>
        /// <param name="other">The other pixel</param>
        /// <returns>If both colors are the same</returns>
        public bool Compare(Pixel other) =>
            other.r == r && other.g == g && other.b == b && other.a == a;
    }

    /// <summary>
    /// The Alpha 'Blending' Modes
    /// Normal - Alpha has no Affect
    /// Mask - Any Alpha below 255 doesn't get rendered
    /// Alpha - 'Proper' Alpha Blending
    /// </summary>
    public enum OpacityMode
    {
        NORMAL,
        MASK,
        ALPHA
    }

    /// <summary>
    /// A Class that contains an Array of Pixels
    /// and may Represent an Image
    /// </summary>
    public class Sprite
    {
        public int Width { get; private set; }
        public int Height { get; private set; }        
        
        private Pixel[] pixelData;

        /// <summary>
        /// Constructor for a Blank Sprite
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Sprite(int width, int height)
        {
            Width = width;
            Height = height;

            pixelData = new Pixel[width * height];
            for (var i = 0; i < pixelData.Length; i++)
                pixelData[i] = new Pixel();
        }

        /// <summary>
        /// Constructor to make a Hard Copy of another Sprite
        /// </summary>
        /// <param name="sprite"></param>
        public Sprite(Sprite sprite)
        {
            if (sprite == null) return;

            pixelData = new Pixel[sprite.Width * sprite.Height];
            for (int i = 0; i < sprite.pixelData.Length; i++)
                pixelData[i] = sprite.pixelData[i];
        }

        /// <summary>
        /// Constructor for a Sprite that will
        /// be retrieved from a file on the computer
        /// </summary>
        /// <param name="path">The Path on the Computer</param>
        public Sprite(string path)
        {
            try
            {
                var image = System.Drawing.Image.FromFile(path);
                var b = new System.Drawing.Bitmap(image);

                Width = b.Width;
                Height = b.Height;

                pixelData = new Pixel[Width * Height];

                for (var x = 0; x < b.Width; x++)
                {
                    for (var y = 0; y < b.Height; y++)
                    {
                        var p = b.GetPixel(x, y);
                        var col = new Pixel(p.R, p.G, p.B, p.A);
                        SetPixel(x, y, col);
                    }
                }

                image.Dispose();
                b.Dispose();
            }
            catch(FileNotFoundException) {
                Console.WriteLine($"[ERROR]: Image / File not found at\n \"{path}\"");
            }
        }

        /// <summary>
        /// The Whole Array of Pixels
        /// </summary>
        /// <returns>A Pixel Array</returns>
        public Pixel[] GetPixels() => pixelData;

        /// <summary>
        /// The Whole Array of Pixels each converted
        /// to UInts
        /// </summary>
        /// <returns>A UInt Array</returns>
        public uint[] GetData()
        {
            uint[] data = new uint[pixelData.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = pixelData[i].ToInt();
            return data;
        }

        /// <summary>
        /// Retrieves a Pixel from the Sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>A Pixel</returns>
        public Pixel GetPixel(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return pixelData[y * Width + x];
            else
                return new Pixel();
        }

        /// <summary>
        /// Sets a Pixel in the Sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="color">Pixel Replacement</param>
        public void SetPixel(int x, int y, Pixel p)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                pixelData[y * Width + x] = p;
        }
    }

    /// <summary>
    /// The main engine with all Open GL and Drawing
    /// Methods. Derive from this class to access the engine.
    /// Make sure to construct the Engine and Call Start
    /// </summary>
    public class PixelGameEngine
    {
        public string WindowTitle = "";

        private int maxFPS, lastFPSCheck;
        private int renderOffsetX, renderOffsetY;
        private int glBuffer;

        private bool hasStarted;
        private bool isFullscreen;

        private Sprite drawTarget;
        private Sprite fontSprite;

        private GameWindow gameWindow;
        private KeyboardState keyboardState, lastKeyboardState;
        private MouseState mouseState, lastMouseState;

        public OpacityMode OpacityMode { get; protected set; }

        public int ScreenWidth { get; private set; } = 256;
        public int ScreenHeight { get; private set; } = 240;

        public int PixelWidth { get; private set; } = 4;
        public int PixelHeight { get; private set; } = 4;

        public int MouseX => isFullscreen ? (gameWindow.Mouse.X - renderOffsetX) / 
            PixelWidth : gameWindow.Mouse.X / PixelWidth; 

        public int MouseY => isFullscreen ? (gameWindow.Mouse.Y - renderOffsetY) / 
            PixelHeight : gameWindow.Mouse.Y / PixelHeight;

        private const string Characters = 
            @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVB" + 
            @"NM0123456789µ§½!""#¤%&/()=?^*@£€${[]}\~¨'-_.:,;<>|°©®±¥+";

        private System.Drawing.Bitmap[] alphabet;
        private Font font = new Font("Arial", 9);

        /// <summary>
        /// Creates a Window and Starts OpenGL. This needs to
        /// be called before calling start.
        /// </summary>
        /// <param name="screenWidth">Width of the Screen</param>
        /// <param name="screenHeight">Height of the Screen</param>
        /// <param name="pixelWidth">Width of Each Pixel</param>
        /// <param name="pixelHeight">Height of Each Pixel</param>
        /// <param name="fps">Max FPS</param>
        /// <returns>If Construction of Success</returns>
        public bool Construct(int screenWidth, int screenHeight, int pixelWidth, int pixelHeight, int fps = -1)
        {
            if (hasStarted) return false;
            if (screenWidth <= 0 | screenHeight <= 0 | pixelWidth < 1 | pixelHeight < 1 | fps == 0)
                return false;

            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;

            maxFPS = fps;

            gameWindow = new GameWindow(screenWidth * pixelWidth, screenHeight * pixelHeight,
                GraphicsMode.Default, WindowTitle);
            gameWindow.VSync = VSyncMode.Off;

            gameWindow.Load += Loaded;
            gameWindow.RenderFrame += Render;
            gameWindow.UpdateFrame += Update;
            gameWindow.Disposed += Disposed;
            gameWindow.KeyDown += KeyDown;
            gameWindow.KeyUp += KeyUp;

            gameWindow.MouseDown += MouseDown;
            gameWindow.MouseUp += MouseUp;

            drawTarget = new Sprite(screenWidth, screenHeight);
            for (int i = 0; i < drawTarget.Width; i++)
                for (int j = 0; j < drawTarget.Height; j++)
                    drawTarget.SetPixel(i, j, new Pixel(0, 0, 0, 0));

            // Make a Texture that will contain all graphics

            GL.Enable(EnableCap.Texture2D);
            GL.GenTextures(1, out glBuffer);
            GL.BindTexture(TextureTarget.Texture2D, glBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                screenWidth, screenHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    drawTarget.GetData());

            // Enable Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Setup Texture Parameters to have clean pixels with no filters

            GL.TexEnv(TextureEnvTarget.TextureEnv,
                TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GenerateAlphabet();
            ConstructFontSheet();

            // Disable Window Resizing
            gameWindow.WindowBorder = WindowBorder.Fixed;

            return true;
        }

        // On Window Update
        private void Update(object sender, FrameEventArgs e)
        {
            if (!hasStarted) return;
            OnUserUpdate((float)e.Time);
        }

        /// <summary>
        /// Starts the Engine. Requires Construct to be
        /// called before hand.
        /// </summary>
        public void Start()
        {
            hasStarted = true;
            if (maxFPS == -1) gameWindow.Run();
            else if (maxFPS > 0) gameWindow.Run(maxFPS);
        }

        // On Window Loaded
        private void Loaded(object sender, EventArgs e) => OnUserCreate();

        // On Window Disposed
        private void Disposed(object sender, EventArgs e) => OnUserDestroy();

        // On Window Render Frame
        private void Render(object sender, FrameEventArgs e)
        {
            if (!hasStarted) return;

            GL.Viewport(renderOffsetX, renderOffsetY, ScreenWidth * PixelWidth, 
                ScreenHeight * PixelHeight);

            if (lastFPSCheck++ * e.Time > 1)
            {
                gameWindow.Title = $"{WindowTitle} | FPS: {(int)(1 / e.Time)}";
                lastFPSCheck = 0;
            }

            OnUserRender((float)e.Time);

            // Use Open-GL to Draw Graphics to Screen
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, ScreenWidth, ScreenHeight,
                PixelFormat.Rgba, PixelType.UnsignedByte, drawTarget.GetData());

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(-1, -1);
            GL.TexCoord2(0, 0); GL.Vertex2(-1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, -1);
            GL.End();

            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;

            gameWindow.SwapBuffers();
        }

        // On Key Pressed Down
        private void KeyDown(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        // On Key Lifted Up
        private void KeyUp(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        /// <summary>
        /// Called once on application startup, use to load your resources
        /// </summary>
        protected virtual void OnUserCreate() { }

        /// <summary>
        /// Runs on a seperate Thread for game logic
        /// </summary>
        /// <param name="fElapsedTime">Update Time</param>
        protected virtual void OnUserUpdate(float fElapsedTime) { }

        /// <summary>
        /// Runs every frame, put graphics rendering here    
        /// </summary>
        /// <param name="fElapsedTime">Render Time</param>
        protected virtual void OnUserRender(float fElapsedTime) { }

        /// <summary>
        /// Called once on application termination, so you can be a clean coder
        /// </summary>
        protected virtual void OnUserDestroy() { }

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
            isFullscreen = fullscreen;
            gameWindow.WindowState = fullscreen ? WindowState.Fullscreen :
                WindowState.Normal;

            renderOffsetX = fullscreen ? gameWindow.Width / 2 - 
                ScreenWidth * PixelWidth / 2 : 0;
            renderOffsetY = fullscreen ? gameWindow.Height / 2 - 
                ScreenHeight * PixelHeight / 2 : 0;
        }

        /// <summary>
        /// Checks if a Key is Held
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Is Held?</returns>
        public bool GetKey(Key key) => keyboardState[key];

        /// <summary>
        /// Check if a Key is Pressed Once
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Was Pressed?</returns>
        public bool GetKeyDown(Key key) => 
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        private void MouseDown(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        private void MouseUp(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        /// <summary>
        /// Checks if Mouse Button is Pressed
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Down?</returns>
        public bool GetMouseButton(MouseButton button) =>
            mouseState[button];

        /// <summary>
        /// Checks if Mouse Button is pressed down once
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Up?</returns>
        public bool GetMouseButtonDown(MouseButton button) =>
            mouseState[button] && (mouseState[button] != lastMouseState[button]);

        /// <summary>
        /// Returns the current Frame
        /// </summary>
        /// <returns>Draw Target</returns>
        public Sprite GetDrawTarget(bool copy = false) =>
            !copy ? drawTarget : new Sprite(drawTarget);

        /// <summary>
        /// Set the Draw Target. If Target is null
        /// Creates a new Draw Target.
        /// </summary>
        /// <param name="target">The New Target</param>
        public void SetDrawTarget(Sprite target) =>
            drawTarget = target == null ? new Sprite(ScreenWidth, 
                ScreenHeight) : target;

        /// <summary>
        /// Draws a Pixel on the Screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="p">Color</param>
        public virtual void Draw(int x, int y, Pixel p)
        {
            if (OpacityMode == OpacityMode.ALPHA && p.a < 255)
            {
                var l = Pixel.Lerp(drawTarget.GetPixel(x, y), p, p.a / 255f);
                drawTarget.SetPixel(x, y, new Pixel(l.r, l.g, l.b));
                return;
            }

            if (OpacityMode == OpacityMode.MASK && p.a < 255)
                return;

            drawTarget.SetPixel(x, y, p);
        }

        /// <summary>
        /// Draws a Line
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="p">Color</param>
        /// <param name="thickness">Thickness</param>
        public void DrawLine(int x1, int y1, int x2, int y2, Pixel p, int thickness = 1)
        {
            float x, y, step;
            float dx = x2 - x1;
            float dy = y2 - y1;

            float absDX = Math.Abs(dx);
            float absDY = Math.Abs(dy);

            step = absDX >= absDY ? absDX : absDY;

            dx /= step;
            dy /= step;

            x = x1;
            y = y1;

            for (int i = 1; i <= step; i++)
            {
                Draw((int)x, (int)y, p);

                if (thickness > 1)
                {
                    for(int j = 1; j < thickness; j++)
                    {
                        Draw((int)x + j, (int)y, p);
                        Draw((int)x, (int)y + j, p);
                    }
                }

                x += dx;
                y += dy;
            }
        }

        /// <summary>
        /// Draws a Rectangle Outline
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="p">Color</param>
        public void DrawRect(int x, int y, int w, int h, Pixel p)
        {
            if (w < 0)
            {
                w *= -1;
                x -= w;
            }

            DrawLine(x, y, x + w, y, p);
            DrawLine(x + w - 1, y, x + w - 1, y + h, p);
            DrawLine(x, y + h - 1, x + w, y + h - 1, p);
            DrawLine(x, y, x, y + h, p);
        }

        /// <summary>
        /// Draws a Filled In Rectangle
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="p">Color</param>
        public void FillRect(int x, int y, int w, int h, Pixel p)
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    Draw(x + i, y + j, p);
        }

        /// <summary>
        /// Draws a Circle Outline
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="r">Radius</param>
        /// <param name="p">Color</param>
        public void DrawCircle(int x, int y, int r, Pixel p)
        {
            if (r == 0) return;

            if (r < 0)
            {
                r *= -1;
                x -= r;
            }

            int x0 = 0;
            int y0 = r / 2;
            int d = 3 - 2 * r;

            while (y0 >= x0)
            {
                Draw(x - x0, y - y0, p);
                Draw(x - y0, y - x0, p);
                Draw(x + y0, y - x0, p);
                Draw(x + x0, y - y0, p);
                Draw(x - x0, y + y0, p);
                Draw(x - y0, y + x0, p);
                Draw(x + y0, y + x0, p);
                Draw(x + x0, y + y0, p);
                if (d < 0) d += 4 * x0++ + 6;
                else d += 4 * (x0++ - y0--) + 10;
            }
        }

        /// <summary>
        /// Draws a Filled in Circle
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="r">Radius</param>
        /// <param name="p">Color</param>
        public void FillCircle(int x, int y, int r, Pixel p)
        {
            for (int i = x; i < x + r * 2; i++)
            {
                for (int j = y; j < y + r * 2; j++)
                {
                    var dist = Math.Sqrt((x + r - i) * (x + r - i) + (y + r - j) * (y + r - j));
                    if (dist < r) Draw(x - 1 + i, y - 1 + j, p);
                }
            }
        }

        /// <summary>
        /// Draw a Triangle Outline
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="x3">X3</param>
        /// <param name="y3">Y3</param>
        /// <param name="p">Color</param>
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            DrawLine(x1, y1, x2, y2, p);
            DrawLine(x2, y2, x3, y3, p);
            DrawLine(x1, y1, x3, y3, p);
        }

        /// <summary>
        /// Draws a Filled In Triangle
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="x3">X3</param>
        /// <param name="y3">Y3</param>
        /// <param name="p">Color</param>
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            var minX = Math.Min(Math.Min(x1, x2), x3);
            var maxX = Math.Max(Math.Max(x1, x2), x3);

            var minY = Math.Min(Math.Min(y1, y2), y3);
            var maxY = Math.Max(Math.Max(y1, y2), y3);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float d1, d2, d3;
                    bool hasNeg, hasPos;

                    d1 = Sign(x, y, x1, y1, x2, y2);
                    d2 = Sign(x, y, x2, y2, x3, y3);
                    d3 = Sign(x, y, x3, y3, x1, y1);

                    hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
                    hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

                    if (!(hasNeg && hasPos))
                    {
                        Draw(x, y, p);
                    }
                }
            }
        }

        private float Sign(int x1, int y1, int x2, int y2, int x3, int y3) =>
            (x1 - x3) * (y2 - y3) - (x2 - x3) * (y1 - y3);

        /// <summary>
        /// Gives you the Distance Between to Points
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <returns>The Distance</returns>
        public float Distance(int x1, int y1, int x2, int y2) =>
            (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

        /// <summary>
        /// Draws a Sprite on the Screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="scale">Scale</param>
        /// <param name="sprite">The Sprite</param>
        public void DrawSprite(int x, int y, Sprite sprite, int scale = 1)
        {
            if (sprite == null || scale <= 0) return;
            for (int i = 0; i < sprite.Width; i++)
                for (int j = 0; j < sprite.Height; j++)
                    FillRect((x + i) * scale, (y + j) * scale, 
                        scale, scale, sprite.GetPixel(i, j));
        }

        /// <summary>
        /// Partially Draws a Sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sprite">The Sprite</param>
        /// <param name="ox">Offset X</param>
        /// <param name="oy">Offset Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h)
        {
            if (sprite == null) return;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    Draw(x + i, y + j, sprite.GetPixel(i + ox, j + oy));
        }

        /// <summary>
        /// Draws a String on the Screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="text">The String</param>
        /// <param name="p">Color</param>
        /// <param name="spacing">Letter Spacing</param>
        /// <param name="spaceSize">Space Size</param>
        public void DrawString(int x, int y, string text, Pixel p, int spacing = -4, int spaceSize = 6)
        {
            int offset = 0;
            foreach (var c in text)
            {
                if (c == ' ')
                {
                    offset += spaceSize;
                    continue;
                }

                var index = Characters.IndexOf(c);
                var bitmap = alphabet[index];

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        if (pixel.R == 0) continue;
                        Draw(x + offset + i, y + j, p);
                    }
                }

                offset += spacing + bitmap.Width;
            }
        }

        /// <summary>
        /// Draws a String using Embeded Character Data.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="text">Text</param>
        /// <param name="p">Color</param>
        /// <param name="scale">Scale</param>
        public void DrawPixelString(int x, int y, string text, Pixel p, int scale = 1)
        {
            int sx = 0;
            int sy = 0;
            foreach (var c in text)
            {
                if (c == '\n')
                {
                    sx = 0; sy += 8 * scale;
                }
                else
                {
                    int ox = (c - 32) % 16;
                    int oy = (c - 32) / 16;

                    if (scale > 1)
                    {
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                                if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)
                                    for (int s = 0; s < scale; s++)
                                        for (int js = 0; js < scale; js++)
                                            Draw(x + sx + (i*scale) + s, y + sy + (j*scale) + js, p);
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                                if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)								
                                    Draw(x + sx + i, y + sy + j, p);
                    }	

                    sx += 8 * scale;
                }
            }
        }

        /// <summary>
        /// Fills the Whole Screen with a Color
        /// </summary>
        /// <param name="p">The Color</param>
        public void Clear(Pixel p)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(p.r / 255f, p.g / 255f, p.b / 255f, p.a / 255f);
        }

        // Makes a Bitmap out of a char
        private System.Drawing.Bitmap GenerateCharacter(Font font, char c)
        {
            var size = GetSize(font, c);
            var bmp = new System.Drawing.Bitmap((int)size.Width, (int)size.Height);

            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.FillRectangle(Brushes.Black, 0, 0, bmp.Height, bmp.Height);
                gfx.DrawString(c.ToString(), font, Brushes.White, 0, 0);
            }

            return bmp;
        }

        private System.Drawing.SizeF GetSize(Font font, char c)
        {
            using (var bmp = new System.Drawing.Bitmap(512, 512))
                using (var gfx = Graphics.FromImage(bmp))
                    return gfx.MeasureString(c.ToString(), font);
        }

        private void GenerateAlphabet()
        {
            alphabet = new System.Drawing.Bitmap[Characters.Length];
            for (int i = 0; i < alphabet.Length; i++)
                alphabet[i] = GenerateCharacter(font, Characters[i]);
        }

        private void ConstructFontSheet()
        {
            string data = "";
            data += "?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000";
            data += "O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400";
            data += "Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000";
            data += "720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000";
            data += "OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000";
            data += "ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000";
            data += "Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000";
            data += "70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000";
            data += "OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000";
            data += "00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000";
            data += "<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000";
            data += "O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000";
            data += "00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000";
            data += "Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0";
            data += "O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000";
            data += "?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020";

            fontSprite = new Sprite(128, 48);
            int px = 0, py = 0;
            for (int b = 0; b < 1024; b += 4)
            {
                uint sym1 = (uint)data[b + 0] - 48;
                uint sym2 = (uint)data[b + 1] - 48;
                uint sym3 = (uint)data[b + 2] - 48;
                uint sym4 = (uint)data[b + 3] - 48;
                uint r = sym1 << 18 | sym2 << 12 | sym3 << 6 | sym4;

                for (int i = 0; i < 24; i++)
                {
                    byte k = (byte)((r & (1 << i)) > 0 ? 255 : 0);
                    fontSprite.SetPixel(px, py, new Pixel(k, k, k, k));
                    if (++py == 48) { px++; py = 0; }
                }
            }
        }
    }
}