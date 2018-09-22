using System;
using olc;

namespace MyGame
{
    class Game : PixelGameEngine
    {
        Game()
        {
            sAppName = "Example";
        }

        protected override void OnUserCreate()
        {
            
        }

        protected override void OnUserUpdate(float fElapsedTime)
        {
            //Random r = new Random();
            //for (int i = 0; i < ScreenWidth(); i++)
            //    for (int j = 0; j < ScreenHeight(); j++)
            //        Draw(i, j, new Pixel((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255)));

            DrawString(0, 0, "the red fox", Pixel.RED);
            DrawString(0, 24, "jumps over the", Pixel.WHITE);
            DrawString(0, 48, "lazy brown dog", Pixel.BLUE);
        }

        protected override void OnUserDestroy()
        {
            
        }

        [STAThread]
        static void Main()
        {
            var game = new Game();
            if (game.Construct(256, 240, 4, 4))
                game.Start();
        }
    }
}
