using OpenTK;

namespace Szark
{
    public static partial class Game
    {
        public static string WindowTitle
        {
            get => window != null ? window.Title : "";
            set => window.Title = value;
        }

        public static int ScreenWidth
        {
            get => window != null ? window.Width : 0;
            set => window.Width = value;
        }

        public static int ScreenHeight
        {
            get => window != null ? window.Height : 0;
            set => window.Height = value;
        }

        public static bool Resizeable
        {
            get => window != null ? window.WindowBorder == WindowBorder.Resizable : false;
            set => window.WindowBorder = (WindowBorder)(value ? 0 : 1);
        }

        public static bool Fullscreen
        {
            get => window?.WindowState == (WindowState)3;
            set
            {
                window.WindowState = (WindowState)(value ? 3 : 0);
                renderOffset.x = value ? (window.Width - ScreenWidth) / 2 : 0;
                renderOffset.y = value ? (window.Height - ScreenHeight) / 2 : 0;
            }
        }

        public static bool Vsync
        {
            get => window?.VSync == VSyncMode.On;
            set => window.VSync = (VSyncMode)(value ? 1 : 0);
        }

        public static float TargetFramerate
        {
            get => window != null ? (float)window.TargetRenderFrequency : 0;
            set => window.TargetRenderFrequency = value;
        }

        public static bool Visible
        {
            get => window != null ? window.Visible : false;
            set => window.Visible = value;
        }

        public static bool Focused => window != null ? window.Focused : false;

        public static float Framerate { get; private set; }

        public static Color Background { get; set; }
    }
}