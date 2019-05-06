using OpenTK;

namespace Szark
{
    public static partial class Game
    {
        public static string WindowTitle
        {
            get => window.Title;
            set => window.Title = value;
        }

        public static int ScreenWidth
        {
            get => window.Width;
            set => window.Width = value;
        }

        public static int ScreenHeight
        {
            get => window.Height;
            set => window.Height = value;
        }

        public static bool Resizeable
        {
            get => window.WindowBorder == WindowBorder.Resizable;
            set => window.WindowBorder = (WindowBorder)(value ? 0 : 1);
        }

        public static bool Fullscreen
        {
            get => window.WindowState == (WindowState)3;
            set
            {
                window.WindowState = (WindowState)(value ? 3 : 0);
                renderOffset.x = value ? (window.Width - ScreenWidth) / 2 : 0;
                renderOffset.y = value ? (window.Height - ScreenHeight) / 2 : 0;
            }
        }

        public static bool Vsync
        {
            get => window.VSync == VSyncMode.On;
            set => window.VSync = (VSyncMode)(value ? 1 : 0);
        }

        public static float TargetFramerate
        {
            get => (float)window.TargetRenderFrequency;
            set => window.TargetRenderFrequency = value;
        }

        public static bool Visible
        {
            get => window.Visible;
            set => window.Visible = value;
        }

        public static bool Focused => window.Focused;

        public static float Framerate { get; private set; }

        public static Color Background { get; set; }
    }
}