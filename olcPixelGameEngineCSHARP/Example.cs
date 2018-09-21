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
            Random r = new Random();
            for (int i = 0; i < ScreenWidth(); i++)
                for (int j = 0; j < ScreenHeight(); j++)
                    Draw(i, j, new Pixel((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255)));
        }

        protected override void OnUserDestroy()
        {
            
        }

        [STAThread]
        static void Main()
        {
            var game = new Game();
            if (game.Construct(128, 128, 8, 8, 60))
                game.Start();
        }
    }
}
