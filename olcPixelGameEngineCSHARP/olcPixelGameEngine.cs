/*
  	Whats This?
	~~~~~~~~~~~
    A Port for the olcPixelGameEngine from C++ to C#
  
	olcPixelGameEngine.cs

	License (OLC-3)
	~~~~~~~~~~~~~~~
	Copyright 2018 OneLoneCoder.com
	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions
	are met:
	1. Redistributions or derivations of source code must retain the above
	copyright notice, this list of conditions and the following disclaimer.
	2. Redistributions or derivative works in binary form must reproduce
	the above copyright notice. This list of conditions and the following
	disclaimer must be reproduced in the documentation and/or other
	materials provided with the distribution.
	3. Neither the name of the copyright holder nor the names of its
	contributors may be used to endorse or promote products derived
	from this software without specific prior written permission.
	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
	"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
	LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
	A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
	LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
	DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
	THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
	OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

	Links
	~~~~~
	YouTube:	https://www.youtube.com/javidx9
	Discord:	https://discord.gg/WhwHUMV
	Twitter:	https://www.twitter.com/javidx9
	Twitch:		https://www.twitch.tv/javidx9
	GitHub:		https://www.github.com/onelonecoder
	Homepage:	https://www.onelonecoder.com

	Author
	~~~~~~
	David Barr, aka javidx9, ©OneLoneCoder 2018
    Ported By: Jakub P. Szarkowicz / JakubSzark
*/

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Drawing;
using System;
using System.IO;

namespace olc
{
    /// <summary>
    /// A struct containing RGBA information for a pixel.
    /// This also can be used for representing color
    /// </summary>
    struct Pixel
    {
        public byte r, g, b, a;

        /// <summary>
        /// The Constructor for a Pixel in RGBA Form
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

        // YMK Colors
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
        /// <param name="i">The UInt</param>
        public void ToPixel(uint i)
        {
            r = (byte)(i >> 0);
            g = (byte)(i >> 8);
            b = (byte)(i >> 16);
            g = (byte)(i >> 24);
        }

        /// <summary>
        /// The Alpha 'Blending' Modes
        /// Normal - Alpha has no Affect
        /// Mask - Any Alpha below 255 doesn't get rendered
        /// Alpha - 'Proper' Alpha Blending
        /// </summary>
        public enum Mode
        {
            NORMAL,
            MASK,
            ALPHA
        }
    }

    /// <summary>
    /// A Class that contains an Array of Pixels
    /// and may Represent an Image
    /// </summary>
    class Sprite
    {
        public int width, height;
        private Pixel[] pixelData;

        /// <summary>
        /// Constructor for a Blank Sprite
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Sprite(int width, int height)
        {
            this.width = width;
            this.height = height;

            pixelData = new Pixel[width * height];
            for (var i = 0; i < pixelData.Length; i++)
                pixelData[i] = new Pixel();
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
                Image image = Image.FromFile(path);
                Bitmap b = new Bitmap(image);
                width = b.Width;
                height = b.Height;

                pixelData = new Pixel[width * height];

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
            if (x >= 0 && x < width && y >= 0 && y < height)
                return pixelData[y * width + x];
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
            if (x >= 0 && x < width && y >= 0 && y < height)
                pixelData[y * width + x] = p;
        }

        /// <summary>
        /// Samples a Position of the Sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>A Pixel</returns>
        public Pixel Sample(float x, float y)
        {
            int sx = (int)(x * width);
            int sy = (int)(y * height);
            return GetPixel(sx, sy);
        }
    }

    /// <summary>
    /// The main engine with all Open GL and Drawing
    /// Methods. Derive from this class to access the engine.
    /// Make sure to construct the Engine and Call Start
    /// </summary>
    class PixelGameEngine
    {
        public string sAppName = "";
        private Sprite drawTarget = null;
        private Pixel.Mode pixelMode = 
            Pixel.Mode.NORMAL;

        private float blendFactor = 0.5f;

        private int screenWidth = 256;
        private int screenHeight = 240;

        private int pixelWidth = 4;
        private int pixelHeight = 4;

        private int fps, fpsCheck;

        private bool bHasStarted;

        private string title;

        private int glBuffer;

        private GameWindow gameWindow;
        private KeyboardState keyboardState,
            lastKeyboardState;

        private Bitmap[] alphabet;
        private const string Characters = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVB" + 
            @"NM0123456789µ§½!""#¤%&/()=?^*@£€${[]}\~¨'-_.:,;<>|°©®±¥+";
        Font font = new Font("Arial", 9);

        public bool IsFocused { get; }

        /// <summary>
        /// Creates a Window and Starts OpenGL. This needs to
        /// be called before calling start.
        /// </summary>
        /// <param name="sWidth">Width of the Screen</param>
        /// <param name="sHeight">Height of the Screen</param>
        /// <param name="pWidth">Width of Each Pixel</param>
        /// <param name="pHeight">Height of Each Pixel</param>
        /// <param name="fps">FPS to Run At</param>
        /// <returns>If Construction of Success</returns>
        public bool Construct(int sWidth, int sHeight, int pWidth, int pHeight, int fps = -1)
        {
            if (bHasStarted) return false;

            screenWidth = sWidth;
            screenHeight = sHeight;
            pixelWidth = pWidth;
            pixelHeight = pHeight;
            this.fps = fps;

            title = "OLC Pixel Game Engine (C# Edition)";
            gameWindow = new GameWindow(sWidth * pWidth, sHeight * pHeight,
                GraphicsMode.Default, title + " - " + sAppName);

            gameWindow.RenderFrame += Render;
            gameWindow.Load += Loaded;
            gameWindow.Disposed += Disposed;
            gameWindow.Resize += Resize;

            gameWindow.VSync = VSyncMode.Off;
            drawTarget = new Sprite(sWidth, sHeight);

            for (int i = 0; i < drawTarget.width; i++)
                for (int j = 0; j < drawTarget.height; j++)
                    drawTarget.SetPixel(i, j, new Pixel(0, 0, 0, 0));

            // Make a Texture that will contain all graphics

            GL.Enable(EnableCap.Texture2D);
            GL.GenTextures(1, out glBuffer);
            GL.BindTexture(TextureTarget.Texture2D, glBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                sWidth, sHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    drawTarget.GetData());

            // Enable Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Setup Texture Parameters to have clean pixels with no filters

            GL.TexEnv(TextureEnvTarget.TextureEnv,
                TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GenerateAlphabet();

            return true;
        }

        /// <summary>
        /// Starts the Engine. Requires Construct to be
        /// called before hand.
        /// </summary>
        public void Start()
        {
            bHasStarted = true;
            if (fps == -1) gameWindow.Run();
            else if (fps > 0) gameWindow.Run(fps);
        }

        // Keeps the Window the Same Size
        private void Resize(object sender, EventArgs e)
        {
            gameWindow.Width = screenWidth * pixelWidth;
            gameWindow.Height = screenHeight * pixelHeight;
        }

        // On Window Loaded
        private void Loaded(object sender, EventArgs e) => OnUserCreate();

        // On Window Disposed
        private void Disposed(object sender, EventArgs e) => OnUserDestroy();

        // On Window Render Frame
        private void Render(object sender, FrameEventArgs e)
        {
            if (!bHasStarted) return;

            keyboardState = Keyboard.GetState();

            GL.Viewport(0, 0, drawTarget.width * pixelWidth, drawTarget.height * pixelHeight);
            var targetPixels = drawTarget.GetData();

            if (fpsCheck++ * e.Time > 1)
            {
                gameWindow.Title = title + " - " + sAppName + 
                    " | FPS: " + (int)(1 / e.Time);
                fpsCheck = 0;
            }

            // Draw the Graphics on the Screen
            OnUserUpdate((float)e.Time);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, screenWidth, screenHeight,
                PixelFormat.Rgba, PixelType.UnsignedByte, targetPixels);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(-1, -1);
            GL.TexCoord2(0, 0); GL.Vertex2(-1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, -1);
            GL.End();

            gameWindow.SwapBuffers();

            lastKeyboardState = keyboardState;
        }

        /// <summary>
        /// Called once on application startup, use to load your resources
        /// </summary>
        protected virtual void OnUserCreate() { }

        /// <summary>
        /// Called every frame, and provides you with a time per frame value
        /// </summary>
        /// <param name="fElapsedTime">Engine Time</param>
        protected virtual void OnUserUpdate(float fElapsedTime) { }

        /// <summary>
        /// Called once on application termination, so you can be a clean coder
        /// </summary>
        protected virtual void OnUserDestroy() { }

        /// <summary>
        /// Checks if a Key is Held
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Is Held?</returns>
        public bool GetKeyHeld(Key key) => keyboardState[key];

        /// <summary>
        /// Check if a Key is Pressed Once
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Was Pressed?</returns>
        public bool GetKeyDown(Key key) =>
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        /// <summary>
        /// Checks if Mouse is Down
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Down?</returns>
        public bool GetMouseDown(MouseButton button) =>
            Mouse.GetState().IsButtonDown(button);

        /// <summary>
        /// Checks if Mouse is Up
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Up?</returns>
        public bool GetMouseUp(MouseButton button) =>
            Mouse.GetState().IsButtonUp(button);

        /// <summary>
        /// Gets the Mouse Relative X Position
        /// </summary>
        /// <returns>Relative X Position</returns>
        public int GetMouseX() => Mouse.GetState().X;

        /// <summary>
        /// Gets the Mouse Relative Y Position
        /// </summary>
        /// <returns>Relative Y Position</returns>
        public int GetMouseY() => Mouse.GetState().Y;

        /// <summary>
        /// Gets the Screen Width
        /// </summary>
        /// <returns>Screen Width</returns>
        public int ScreenWidth() => screenWidth;

        /// <summary>
        /// Gets the Screen Height
        /// </summary>
        /// <returns>Screen Height</returns>
        public int ScreenHeight() => screenHeight;

        /// <summary>
        /// Returns the Draw Target Width
        /// </summary>
        /// <returns>Draw Target Width</returns>
        public int GetDrawTargetWidth() => drawTarget.width;

        /// <summary>
        /// Return the Draw Target Height
        /// </summary>
        /// <returns>Draw Target Height</returns>
        public int GetDrawTargetHeight() => drawTarget.height;

        /// <summary>
        /// Returns the Actual Draw Target
        /// </summary>
        /// <returns>Draw Target</returns>
        public Sprite GetDrawTarget() => drawTarget;

        /// <summary>
        /// Set the Draw Target. If Target is null
        /// Creates a new Draw Target.
        /// </summary>
        /// <param name="target">The New Target</param>
        public void SetDrawTarget(Sprite target)
        {
            if (target != null) drawTarget = target;
            else drawTarget = new Sprite(screenWidth, screenHeight);
        }

        /// <summary>
        /// Sets the Alpha Blending Mode
        /// </summary>
        /// <param name="mode">The Mode</param>
        public void SetPixelMode(Pixel.Mode mode) => 
            pixelMode = mode;

        /// <summary>
        /// Sets the Alpha Blend Factor
        /// </summary>
        /// <param name="fBlend">The Factor</param>
        public void SetPixelBlend(float fBlend) =>
            blendFactor = fBlend < 0 ? 0 : fBlend > 1 ? 1 : fBlend;

        /// <summary>
        /// Draws a Pixel on the Screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="p">Color</param>
        public virtual void Draw(int x, int y, Pixel p)
        {
            if (pixelMode == Pixel.Mode.ALPHA && p.a < 255)
            {
                var d = drawTarget.GetPixel(x, y);
                var n = Pixel.Lerp(p, d, blendFactor * 1 - (p.a / 255f));
                drawTarget.SetPixel(x, y, new Pixel(n.r, n.g, n.b, 
                    (byte)((d.a + p.a) / 2f)));
                return;
            }

            if (pixelMode == Pixel.Mode.MASK && p.a < 255)
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
            float x, y;
            float step;

            float dx = x2 - x1;
            float dy = y2 - y1;

            if (Math.Abs(dx) >= Math.Abs(dy))
                step = Math.Abs(dx);
            else
                step = Math.Abs(dy);

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
            int x2 = x + w;
            int y2 = y + h;

            if (x < 0) x = 0;
            else if (x > drawTarget.width)
                x = drawTarget.width;

            if (y < 0) y = 0;
            else if (y > drawTarget.height)
                y = drawTarget.height;

            for (int i = x; i < x2; i++)
                for (int j = y; j < y2; j++)
                    Draw(i, j, p);
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
            if (r == 0) return;

            if (r < 0)
            {
                r *= -1;
                x -= r;
            }

            int x0 = 0;
            int d = 3 - 2 * r;
            int y0 = r / 2;

            Action<int, int, int> drawStraight;
            drawStraight = (int sx, int ex, int ny) => {
                for (int i = sx; i <= ex; i++) Draw(i, ny, p);
            };

            while (y0 >= x0)
            {
                // Modified to draw scan-lines instead of edges
                drawStraight(x - x0, x + x0, y - y0);
                drawStraight(x - y0, x + y0, y - x0);
                drawStraight(x - x0, x + x0, y + y0);
                drawStraight(x - y0, x + y0, y + x0);
                if (d < 0) d += 4 * x0++ + 6;
                else d += 4 * (x0++ - y0--) + 10;
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
            DrawTriangle(x1, y1, x2, y2, x3, y3, p);

            float xL, yL, xS, yS;
            float sideLength = Distance(x2, y2, x3, y3);

            for (int i = 0; i < (int)sideLength; i++)
            {
                float d = i / sideLength;

                xS = (1 - d) * x1 + d * x3;
                yS = (1 - d) * y1 + d * y3;

                xL = (1 - d) * x2 + d * x3;
                yL = (1 - d) * y2 + d * y3;

                DrawLine((int)xS, (int)yS, (int)xL, (int)yL, p, 2);
            }
        }

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
        /// <param name="sprite">The Sprite</param>
        public void DrawSprite(int x, int y, Sprite sprite)
        {
            if (sprite == null)
                return;

            for (int j = 0; j < sprite.width; j++)
            {
                for (int i = 0; i < sprite.height; i++)
                {
                    if (sprite.GetPixel(j, i).a == 255)
                        Draw(x + i, y + j, sprite.GetPixel(j, i));
                }
            }
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
            if (sprite == null)
                return;

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
        /// Fills the Whole Screen with a Color
        /// </summary>
        /// <param name="p">The Color</param>
        public void Clear(Pixel p) =>
            FillRect(0, 0, screenWidth, screenHeight, p);

        // Makes a Bitmap out of a char
        private Bitmap GenerateCharacter(Font font, char c)
        {
            var size = GetSize(font, c);
            var bmp = new Bitmap((int)size.Width, (int)size.Height);

            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.FillRectangle(Brushes.Black, 0, 0, bmp.Height, bmp.Height);
                gfx.DrawString(c.ToString(), font, Brushes.White, 0, 0);
            }

            return bmp;
        }

        private SizeF GetSize(Font font, char c)
        {
            using (var bmp = new Bitmap(512, 512))
                using (var gfx = Graphics.FromImage(bmp))
                    return gfx.MeasureString(c.ToString(), font);
        }

        private void GenerateAlphabet()
        {
            alphabet = new Bitmap[Characters.Length];
            for (int i = 0; i < alphabet.Length; i++)
                alphabet[i] = GenerateCharacter(font, Characters[i]);
        }
    }
}
