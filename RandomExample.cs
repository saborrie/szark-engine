using System;
using Szark;

namespace Example
{
    class RandomExample : SzarkEngine
    {
        private SpriteRenderer renderer;

        RandomExample() =>
            WindowTitle = "Random Pixels Example";

        protected override void Start()
        {
            renderer = new SpriteRenderer(this, 
                new Sprite(ScreenWidth, ScreenHeight), BaseShaderID);
        }

        protected override void Update(float deltaTime) { }

        protected override void Draw(float deltaTime)
        {
            var random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    renderer.Graphics.Draw(i, j, new Pixel((byte)random.Next(255),
                        (byte)random.Next(255), (byte)random.Next(255)));

            renderer.Render(0, 0, 0, 1, -1, true);
            renderer.Refresh();
        }

        protected override void Destroyed() { }

        static void Main() => 
            new RandomExample().Construct(800, 800, 8);
    }
}