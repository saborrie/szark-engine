using System;
using olc;

namespace Example
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
            Clear(Pixel.VERY_DARK_GREY);
            DrawLine(0, 0, 16, 16, Pixel.WHITE);
            DrawRect(16, 0, 16, 16, Pixel.RED);
            FillRect(32, 0, 16, 16, Pixel.GREEN);
            DrawCircle(64, 64, 12, Pixel.YELLOW);
            FillCircle(64, 96, 12, Pixel.MAGENTA);

        }

        protected override void OnUserDestroy()
        {
            
        }

        [STAThread]
        static void Main()
        {
            var game = new Game();
            if (game.Construct(128, 128, 8, 8))
                game.Start();
        }
    }
}
