using System;

namespace Szark
{
    /// <summary>
    /// This window is a Window for drawing pixel graphics,
    /// easily with the CPU. All you need to do is inherit
    /// from it and implement the required methods.
    /// </summary>
    public abstract class PixelWindow
    {
        /// <summary>
        /// Width of the Screen in Pixels,
        /// Use this over Window.Width
        /// </summary>
        public int ScreenWidth { get; private set; }

        /// <summary>
        /// Height of the Screen in Pixels,
        /// Use this over Window.Height
        /// </summary>
        public int ScreenHeight { get; private set; }

        /// <summary>
        /// Density / Size of each Pixel
        /// </summary>
        public int PixelSize { get; private set; }

        /// <summary>
        /// Graphics2D Object for Drawing to the Screen,
        /// Use this in Render Method
        /// </summary>
        public Graphics2D GFX { get; private set; }

        /// <summary>
        /// Background Color
        /// </summary>
        public Color Background
        {
            get { return background.Get(0, 0); }
            set { background.Fill(value); }
        }

        private Texture drawTarget, background;
        private Sprite screenSprite;

        public PixelWindow(string title, int width, int height, int pixelSize = 4)
        {
            Window.Started += () => 
            {
                ScreenWidth = (int)Math.Ceiling(Window.Width / (float)pixelSize);
                ScreenHeight = (int)Math.Ceiling(Window.Height / (float)pixelSize);
                PixelSize = pixelSize;

                drawTarget = new Texture(ScreenWidth, ScreenHeight);
                background = new Texture(ScreenWidth, ScreenHeight);
                Background = Color.Black;

                screenSprite = new Sprite(drawTarget);
                GFX = new Graphics2D(drawTarget);

                Input.SetPixelScale(pixelSize);

                Start();
            };  

            Window.Updated += f => {
                Update(f);
            };

            Window.Rendered += f => 
            {
                Render(f);

                screenSprite.Render(new Transform(0, 0, 0, pixelSize));
                screenSprite.Refresh(drawTarget);

                Array.Copy(background.pixels, drawTarget.pixels, 
                    drawTarget.pixels.Length);
            };

            Window.Disposed += Destroyed;

            Window.Create(title, width, height);
        }

        /// <summary>
        /// Called when the Window opens,
        /// Use this for initialization
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Runs every tick,
        /// Use this for logic
        /// </summary>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Runs every frame,
        /// Use this for drawing
        /// </summary>
        public abstract void Render(float deltaTime);

        /// <summary>
        /// Called when Window closes,
        /// Use this for cleanup
        /// </summary>
        public abstract void Destroyed();
    }
}