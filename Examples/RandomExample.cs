using System;
using PGE;
using OpenTK.Graphics.OpenGL;

namespace Example
{
    class RandomExample : PixelGameEngine
    {
        private SpriteRenderer brickSprite;
        private float rotation;

        RandomExample() =>
            WindowTitle = "Random Pixels Example";

        protected override void Start() 
        {
            var sprite = new Sprite("Examples/Assets/Brick.png");
            brickSprite = new SpriteRenderer(this, sprite, BaseShaderID);
        }

        protected override void Update(float deltaTime) {}

        protected override void Draw(float deltaTime)
        {
            // Draws the Background Random Pixels
            var random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    Graphics.Draw(i, j, new Pixel((byte)random.Next(255), 
                        (byte)random.Next(255), (byte)random.Next(255)));
        }

        protected override void GPUDraw(float deltaTime) 
        {
            // Draws the Brick Sprite in the Middle of the Screen
            rotation += (50 * deltaTime) * (float)Math.PI / 180f;
            brickSprite.Render(ScreenWidth / 2f - 16, ScreenHeight / 2f - 16, rotation, 0.5f);
            if (rotation >= 360) rotation = 0;
        }

        protected override void Destroyed() {}

        static void Main() => 
            new RandomExample().Construct(800, 800, 8);
    }
}