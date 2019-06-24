namespace Szark
{
    public abstract class PixelWindow
    {
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelSize { get; private set; }

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

        public abstract void Start();
        public abstract void Update(float deltaTime);
        public abstract void Render(float deltaTime);
        public abstract void Destroyed();
    }
}