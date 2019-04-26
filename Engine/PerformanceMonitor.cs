namespace Szark
{
    public sealed class PerformanceMonitor
    {
        public bool Active { get; set; } = true;
        public float FPS { get; private set; }

        private float lastFPS;
        private readonly Text text;

        internal PerformanceMonitor() {
            text = new Text("", "Arial", 16);
        }

        internal void Render(float dt)
        {
            if (!Active) return;

            if ((lastFPS += dt) > 1)
            {
                FPS = (int)(1 / dt);
                lastFPS = 0;
                text.Set($"FPS: {(int)FPS}", "Arial", 16);
            }

            text.Render();
        }
    }
}
