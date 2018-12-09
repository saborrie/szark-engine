using System;
using PGE;

namespace Example
{
    class RandomExample : PixelGameEngine
    {
        protected override void Start() {
            WindowTitle = "Random Pixels Example";
        }

        protected override void Update(float deltaTime) {}

        protected override void Draw(float deltaTime)
        {
            var random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    Graphics.Draw(i, j, new Pixel((byte)random.Next(255), 
                        (byte)random.Next(255), (byte)random.Next(255)));
        }

        protected override void Destroyed() {}

        static void Main() => 
            new RandomExample().Construct(800, 800, 8);
    }
}