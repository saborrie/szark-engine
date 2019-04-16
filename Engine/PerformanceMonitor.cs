using System.Diagnostics;

namespace Szark
{
    public sealed class PerformanceMonitor
    {
        public bool Active { get; set; } = true;
        public float FPS { get; private set; }
        public float UPS { get; private set; }

        private float lastUPS, lastFPS;
        private readonly Process process;
        private readonly Text text;

        internal PerformanceMonitor()
        {
            text = new Text("Arial", 16);
            process = Process.GetCurrentProcess();
        }

        internal void Update(float dt)
        {
            if (!Active) return;

            if ((lastUPS += dt) > 1)
            {
                UPS = (int)(1 / dt);
                lastUPS = 0;
            }
        }

        internal void Render(float dt)
        {
            if (!Active) return;

            if ((lastFPS += dt) > 1)
            {
                FPS = (int)(1 / dt);
                lastFPS = 0;
                process.Refresh();
            }

            text.DrawString($"FPS: {(int)FPS}, UPS: {(int)UPS}", 0, 0, 1, 1, -8);
            text.DrawString($"Memory Usage: {(int)(process.PrivateMemorySize64 / 1e+6)}MB", 0, 20, 1, 1, -8);
        }
    }
}
