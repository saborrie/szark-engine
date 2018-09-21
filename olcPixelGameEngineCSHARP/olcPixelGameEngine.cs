/*
	olcPixelGameEngine.cs

	+-------------------------------------------------------------+
	|           OneLoneCoder Pixel Game Engine v1.2               |
	| "Like the command prompt console one, but not..." - javidx9 |
	+-------------------------------------------------------------+

	What is this?
	~~~~~~~~~~~~~
	The olcConsoleGameEngine has been a surprsing and wonderful
	success for me, and I'm delighted how people have reacted so
	positively towards it, so thanks for that.
	However, there are limitations that I simply cannot avoid.
	Firstly, I need to maintain several different versions of
	it to accommodate users on Windows7, 8, 10, Linux, Mac,
	Visual Studio & Code::Blocks. Secondly, this year I've been
	pushing the console to the limits of its graphical capabilities
	and the effect is becoming underwhelming. The engine itself
	is not slow at all, but the process that Windows uses to
	draw the command prompt to the screen is, and worse still,
	it's dynamic based upon the variation of character colours
	and glyphs. Sadly I have no control over this, and recent
	videos that are extremely graphical (for a command prompt :P )
	have been dipping to unacceptable framerates. As the channel
	has been popular with aspiring game developers, I'm concerned
	that the visual appeal of the command prompt is perhaps
	limited to us oldies, and I dont want to alienate younger
	learners. Finally, I'd like to demonstrate many more
	algorithms and image processing that exist in the graphical
	domain, for which the console is insufficient.
	For this reason, I have created olcPixelGameEngine! The look
	and feel to the programmer is almost identical, so all of my
	existing code from the videos is easily portable, and the
	programmer uses this file in exactly the same way. But I've
	decided that rather than just build a command prompt emulator,
	that I would at least harness some modern(ish) portable
	technologies.
	As a result, the olcPixelGameEngine supports 32-bit colour, is
	written in a cross-platform style, uses modern(ish) C++
	conventions and most importantly, renders much much faster. I
	will use this version when my applications are predominantly
	graphics based, but use the console version when they are
	predominantly text based - Don't worry, loads more command
	prompt silliness to come yet, but evolution is important!!
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
	Relevant Videos
	~~~~~~
	I'd like to extend thanks to Eremiell, slavka, Phantim, JackOJC,
	KrossX, Huhlig, Dragoneye, Appa & MagetzUb for advice, ideas and testing,
	and I'd like to extend my appreciation to the 13K YouTube followers
	and 1K Discord server members who give me the motivation to keep
	going with all this :D
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

namespace olc
{
    struct Pixel
    {
        public byte r, g, b, a;

        public Pixel(byte r, byte g, byte b, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Pixel WHITE = new Pixel(255, 255, 255);
        public static Pixel GREY = new Pixel(100, 100, 100, 100);
        public static Pixel BLACK = new Pixel(0, 0, 0, 0);

        public static Pixel RED = new Pixel(255, 0, 0);
        public static Pixel GREEN = new Pixel(0, 255, 0);
        public static Pixel BLUE = new Pixel(0, 0, 255);

        public static Pixel YELLOW = new Pixel(255, 255, 0);
        public static Pixel MAGENTA = new Pixel(255, 0, 255);
        public static Pixel CYAN = new Pixel(0, 255, 255);

        public uint ToInt() =>
            (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));

        public void ToColor(uint i)
        {
            r = (byte)(i >> 0);
            g = (byte)(i >> 8);
            b = (byte)(i >> 16);
            g = (byte)(i >> 24);
        }

        public enum Mode
        {
            NORMAL,
            MASK,
            ALPHA
        }
    }

    class Sprite
    {
        public int width;
        public int height;

        private Pixel[] pixelData;

        public Sprite(int width, int height)
        {
            this.width = width;
            this.height = height;

            pixelData = new Pixel[width * height];
            for (var i = 0; i < pixelData.Length; i++)
                pixelData[i] = new Pixel();
        }

        public Sprite(string name)
        {
            Image image = Image.FromFile(name);
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

        public Pixel[] GetPixels() => pixelData;

        public uint[] GetData()
        {
            uint[] data = new uint[pixelData.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = pixelData[i].ToInt();
            return data;
        }

        public Pixel GetPixel(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return pixelData[x * width + y];
            else
                return new Pixel();
        }

        public void SetPixel(int x, int y, Pixel color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                pixelData[y * width + x] = color;
        }

        public Pixel Sample(float x, float y)
        {
            int sx = (int)(x * width);
            int sy = (int)(y * height);
            return GetPixel(sx, sy);
        }
    }

    class PixelGameEngine
    {
        public string sAppName = "";

        private Sprite drawTarget = null;
        private Sprite fontSprite = null;

        private Pixel.Mode pixelMode = Pixel.Mode.NORMAL;

        private float fBlendFactor = 1.0f;

        private int nScreenWidth = 256;
        private int nScreenHeight = 240;

        private int nPixelWidth = 4;
        private int nPixelHeight = 4;

        private int fps;
        private int fpsCheck;

        private bool bHasStarted = false;

        private string title;

        private int glBuffer;
        private GameWindow gameWindow;

        private KeyboardState keyboardState,
            lastKeyboardState;

        public bool IsFocused { get; }

        public bool Construct(int sWidth, int sHeight, int pWidth, int pHeight, int fps = -1)
        {
            if (bHasStarted) return false;

            nScreenWidth = sWidth;
            nScreenHeight = sHeight;
            nPixelWidth = pWidth;
            nPixelHeight = pHeight;
            this.fps = fps;

            title = "OLC Pixel Game Engine (C# Edition)";
            gameWindow = new GameWindow(sWidth * pWidth, sHeight * pHeight,
                GraphicsMode.Default, "");

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
            GL.BlendFunc(BlendingFactor.SrcAlpha,
                BlendingFactor.OneMinusSrcAlpha);

            // Setup Texture Parameters to have clean pixels with no filters

            GL.TexEnv(TextureEnvTarget.TextureEnv,
                TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return true;
        }

        public void Start()
        {
            bHasStarted = true;
            if (fps == -1) gameWindow.Run();
            else if (fps > 0) gameWindow.Run(fps);
        }

        private void Resize(object sender, EventArgs e)
        {
            gameWindow.Width = nScreenWidth * nPixelWidth;
            gameWindow.Height = nScreenHeight * nPixelHeight;
        }

        private void Loaded(object sender, EventArgs e) {
            OnUserCreate();
        }

        private void Disposed(object sender, EventArgs e) {
            OnUserDestroy();
        }

        private void Render(object sender, FrameEventArgs e)
        {
            if (!bHasStarted) return;

            keyboardState = Keyboard.GetState();

            GL.Viewport(0, 0, drawTarget.width * nPixelWidth, drawTarget.height * nPixelHeight);
            var targetPixels = drawTarget.GetData();

            if (fpsCheck++ * e.Time > 1)
            {
                gameWindow.Title = title + " - " + sAppName + 
                    " | FPS: " + (int)(1 / e.Time);
                fpsCheck = 0;
            }

            // Draw the Graphics on the Screen
            OnUserUpdate((float)e.Time);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, nScreenWidth, nScreenHeight,
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

        // Called once on application startup, use to load your resources
        protected virtual void OnUserCreate() { }

        // Called every frame, and provides you with a time per frame value
        protected virtual void OnUserUpdate(float fElapsedTime) { }

        // Called once on application termination, so you can be a clean coder
        protected virtual void OnUserDestroy() { }

        public bool GetKeyHeld(Key key) {
            return keyboardState[key];
        }

        public bool GetKeyDown(Key key) {
            return keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);
        }

        public bool GetMouseDown(MouseButton button) {
            return Mouse.GetState().IsButtonDown(button);
        }

        public bool GetMouseUp(MouseButton button) {
            return Mouse.GetState().IsButtonUp(button);
        }

        public int GetMouseX() { return Mouse.GetState().X; }
        public int GetMouseY() { return Mouse.GetState().Y; }

        public int ScreenWidth() {
            return nScreenWidth;
        }

        public int ScreenHeight() {
            return nScreenHeight;
        }

        public int GetDrawTargetWidth() {
            return drawTarget.width;
        }

        public int GetDrawTargetHeight() {
            return drawTarget.height;
        }

        public Sprite GetDrawTarget() {
            return drawTarget;
        }

        public void SetDrawTarget(Sprite target) {
            drawTarget = target;
        }

        public void SetPixelMode(Pixel.Mode mode) {
            pixelMode = mode;
        }

        public void SetPixelBlend(float fBlend)
        {
            fBlendFactor = fBlend;
            if (fBlendFactor < 0.0f) fBlendFactor = 0.0f;
            if (fBlendFactor > 1.0f) fBlendFactor = 1.0f;
        }

        public virtual void Draw(int x, int y, Pixel p) {
            drawTarget.SetPixel(x, y, p);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Pixel p)
        {
            int dx, dy;

            dx = x2 - x1;
            dy = y2 - y1;

            if (dx != 0)
            {
                for (int x = 0; x < dx; x++)
                    Draw(x + x1, y1 + dy * (x - x1) / dx, p);
            }
            else
            {
                for (int y = 0; y < dy; y++)
                    Draw(x1, y + y1, p);
            }
        }

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
            drawStraight = (int sx, int ex, int ny) =>
            {
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

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            DrawLine(x1, y1, x2, y2, p);
            DrawLine(x2, y2, x3, y3, p);
            DrawLine(x3, y3, x1, y1, p);
        }

        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {

        }

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

        public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h)
        {
            if (sprite == null)
                return;

            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    Draw(x + i, y + j, sprite.GetPixel(i + ox, j + oy));
        }

        public void DrawString(int x, int y, string text, Pixel col, int scale = 1)
        {
            int sx = 0;
            int sy = 0;
            Pixel.Mode m = pixelMode;
            if (col.a != 255) SetPixelMode(Pixel.Mode.ALPHA);
            else SetPixelMode(Pixel.Mode.MASK);
            foreach(var c in text)
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
                                    for (int k = 0; k < scale; k++)
                                        for (int l = 0; l < scale; l++)
                                            Draw(x + sx + (i * scale) + k, y + sy + (j * scale) + l, col);
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                                if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)
                                    Draw(x + sx + i, y + sy + j, col);
                    }
                    sx += 8 * scale;
                }
            }

            SetPixelMode(m);
        }

        public void Clear(Pixel p) {
            FillRect(0, 0, nScreenWidth, nScreenHeight, p);
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
            //int px = 0, py = 0;
            for (int b = 0; b < 1024; b += 4)
            {
                int sym1 = data[b + 0] - 48;
                int sym2 = data[b + 1] - 48;
                int sym3 = data[b + 2] - 48;
                int sym4 = data[b + 3] - 48;
                int r = sym1 << 18 | sym2 << 12 | sym3 << 6 | sym4;

                for (int i = 0; i < 24; i++)
                {
                    //int k = r & (1 << i) ? 255 : 0;
                    //fontSprite.SetPixel(px, py, new Pixel(k, k, k, k));
                    //if (++py == 48) { px++; py = 0; }
                }
            }
        }
    }
}
