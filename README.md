## How to get started:

**You must have .Net Core SDK**

1. Clone this project
2. Restore dependencies

    **or**

1. Download latest release
2. Create new project and link the dll
3. Add the dependencies to your project<br>
through Nuget Packet Manager

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
  
# Documention
You can find documentation by going to the wiki tab.

# Other Examples
<img src="https://i.imgur.com/SPTGHfe.gif" width="400"><img src="https://i.imgur.com/sgPtLmT.gif" width="400">
<img src="https://i.imgur.com/MqgCckl.gif" width="800">

## Acknowledgments

Check out the C++ inspiration of this engine, the [olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine) by [Javidx9](https://www.youtube.com/channel/UC-yuWVUplUJZvieEligKBkA) (OneLoneCoder.com). 
He uses the OLC-3 License in his original project.
