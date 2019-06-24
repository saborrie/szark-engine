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
        
        private Texture drawTarget;
        private Sprite screenSprite;

        private int hWidth, hHeight;

        public PixelWindow(string title, int width, int height, int pixelSize = 4)
        {
            Window.Started += () => 
            {
                ScreenWidth = Window.Width / pixelSize;
                ScreenHeight = Window.Height / pixelSize;
                PixelSize = pixelSize;

                hWidth = Window.Width / 2;
                hHeight = Window.Height / 2;

                drawTarget = new Texture(ScreenWidth, ScreenHeight);
                screenSprite = new Sprite(drawTarget);
                GFX = new Graphics2D(drawTarget);

                Start();
            };  

            Window.Updated += f => {
                Update(f);
            };

            Window.Rendered += f => 
            {
                Render(f);

                screenSprite.Render(new Transform(hWidth - ScreenWidth / 2, 
                    hHeight - ScreenHeight / 2, 0, pixelSize + 0.01f));
                screenSprite.Refresh(drawTarget);
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