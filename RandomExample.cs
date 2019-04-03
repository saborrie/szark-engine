using System;
using Szark;

namespace Example
{
    class RandomExample : SzarkEngine
    {
        private Random random;
        private readonly int pixelSize = 4;
        private SpriteRenderer renderer;

        RandomExample() : base("Random Pixels Example",
            800, 800) { }

        protected override void Start()
        {
            random = new Random();
            renderer = new SpriteRenderer(ScreenWidth / pixelSize, 
                ScreenHeight / pixelSize);
        }

        protected override void Update(float deltaTime) { }

        protected override void Draw(float deltaTime)
        {
            for (int i = 0; i < renderer.Sprite.Width; i++)
            {
                for (int j = 0; j < renderer.Sprite.Height; j++)
                {
                    renderer.Graphics.Draw(i, j, new Color((byte)random.Next(255),
                        (byte)random.Next(255), (byte)random.Next(255)));
                }
            }

            renderer.Render(0, 0, 0, pixelSize, -1, true);
            renderer.Refresh();
        }

        protected override void Destroyed() { }

        static void Main() => new RandomExample();
    }
}