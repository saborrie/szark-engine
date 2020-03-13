using System;

namespace Szark
{
    /// <summary>
    /// This window is for drawing pixel graphics,
    /// easily with the CPU. All you need to do is inherit
    /// from it, implement the required methods and run.
    /// To draw in the window, use the canvas object.
    /// </summary>
    public abstract class PixelWindow
    {
        /// <summary>
        /// Width of the screen in pixels,
        /// </summary>
        public int ScreenWidth { get; private set; }

        /// <summary>
        /// Height of the screen in pixels,
        /// </summary>
        public int ScreenHeight { get; private set; }

        /// <summary>
        /// Size of each pixel
        /// </summary>
        public int PixelSize { get; private set; }

        /// <summary>
        /// Canvas for drawing to the screen
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        /// Background color
        /// </summary>
        public Color Background
        {
            get { return background[0, 0]; }
            set { background.ClearToColor(value); }
        }

        private Texture drawTarget, background;
        private Sprite screenSprite;

        public void Run(string title, int width, int height, int pixelSize = 4)
        {
            Window.Started += () => 
            {
                ScreenWidth = (int)Math.Ceiling(Window.Width / (float)pixelSize);
                ScreenHeight = (int)Math.Ceiling(Window.Height / (float)pixelSize);
                PixelSize = pixelSize;

                drawTarget = Texture.Create(ScreenWidth, ScreenHeight);
                background = Texture.Create(ScreenWidth, ScreenHeight);
                Background = Color.Black;

                screenSprite = new Sprite(drawTarget);
                Canvas = drawTarget.GetCanvas();

                Input.SetPixelScale(pixelSize);

                Start();
            };  

            Window.Rendered += f => 
            {
                Render(f);

                screenSprite.Render(new Transform(0, 0, 0, pixelSize));
                screenSprite.Refresh(drawTarget);

                Array.Copy(background.Pixels, drawTarget.Pixels, 
                    drawTarget.Pixels.Length);
            };

            Window.Updated += Update;
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