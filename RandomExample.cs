using System;
using Szark;

namespace Example
{
    class RandomExample : SzarkEngine
    {
        private Random random;
        private SpriteRenderer renderer;

        private Sprite testSprite;
        private Sprite randomSprite;

        RandomExample() : base("Random Pixels Example", 
            800, 800, 8) { }

        protected override void Start()
        {
            random = new Random();
            randomSprite = new Sprite(ScreenWidth, ScreenHeight);
            renderer = CreateRenderer(randomSprite);
            testSprite = new Sprite(@"C:\Users\Jakub\Documents\RTSGame\RTSGame\Sprites\Ground-Sandstone.png");
        }

        protected override void Update(float deltaTime)
        {
            if (Input.GetKeyDown("Space"))
                renderer.Sprite = testSprite;
        }

        protected override void Draw(float deltaTime)
        {
            for (int i = 0; i < ScreenWidth; i++)
            {
                for (int j = 0; j < ScreenHeight; j++)
                {
                    renderer.Graphics.Draw(i, j, new Pixel((byte)random.Next(255),
                        (byte)random.Next(255), (byte)random.Next(255)));
                }
            }

            renderer.Render(0, 0, 0, 1, -1, true);
            renderer.Refresh();
        }

        protected override void Destroyed() { }

        static void Main(string[] theArgs) => 
            new RandomExample();
    }
}