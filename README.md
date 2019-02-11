## How to get started:
1. Download .Net Core 2.1 SDK or Higher
2. Clone or download this project
3. Open the project in your favorite IDE
4. Restore the project if necessary
5. Your good to go!

## Dependencies
* OpenTK.NetCore
* CoreCompact.System.Drawing
* NAudio

## Getting Starting Example
```c#
using System;
using Szark;

namespace Example
{
    class RandomExample : SzarkEngine
    {
        private Random random;
        private SpriteRenderer renderer;

        RandomExample() : base("Random Pixels Example", 
            800, 800, 8) { }

        protected override void Start()
        {
            random = new Random();
            renderer = CreateRenderer(new Sprite(ScreenWidth, ScreenHeight));
        }

        protected override void Update(float deltaTime) { }

        protected override void Draw(float deltaTime)
        {
            for (int i = 0; i < ScreenWidth; i++)
            {
                for (int j = 0; j < ScreenHeight; j++)
                {
                    renderer.Graphics.Draw(i, j, new Pixel((byte)random.Next(255),
                        (byte)random.Next(255), (byte)random.Next(255)));
                }
            }

            renderer.Render(0, 0, 0, 1, -1, true);
            renderer.Refresh();
        }

        protected override void Destroyed() { }

        static void Main(string[] theArgs) => 
            new RandomExample();
    }
}
```
## Built-in Overridable Methods
  * **Start** - Called when window is created (Use for initializing)
  * **Update** - Called every tick (Use for game logic like player movement, physics, etc.)
  * **Draw** - Called every frame (Use for rendering stuff and drawing)
  * **Destroyed** - Called when window closes (Use for cleanup)
  
# Documention
You can find documentation either by going to the wiki tab or
by looking through the Engine folder.

# Other Examples
<img src="https://i.imgur.com/SPTGHfe.gif" width="400"><img src="https://i.imgur.com/sgPtLmT.gif" width="400">
<img src="https://i.imgur.com/MqgCckl.gif" width="800">

## Acknowledgments

Check out the C++ inspiration of this engine, the [olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine) by [Javidx9](https://www.youtube.com/channel/UC-yuWVUplUJZvieEligKBkA) (OneLoneCoder.com). 
He uses the OLC-3 License in his original project.
