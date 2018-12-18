## Current Version: 1.0

## How to get started:
1. Download .Net Core 2.2 SDK or Higher
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
using PGE;

namespace Example
{
    class RandomExample : PixelGameEngine
    {
        RandomExample() =>
            WindowTitle = "Random Pixels Example";
    
        protected override void Start() {}

        protected override void Update(float deltaTime) {}

        protected override void Draw(Graphics2D graphics, float deltaTime)
        {
            var random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    graphics.Draw(i, j, new Pixel((byte)random.Next(255), 
                        (byte)random.Next(255), (byte)random.Next(255)));
        }

        protected override void Destroyed() {}

        static void Main() => 
            new RandomExample().Construct(800, 800, 8);
    }
}
```
## Built-in Overridable Methods
  * **Start** - Called when window is created (Use for initializing)
  * **Update** - Called every tick (Use for game logic like player movement, physics, etc.)
  * **Draw** - Called every frame (Use for rendering stuff and drawing)
  * **GPUDraw** - Called every frame (Use for rendering GPU sprites)
  * **Destroyed** - Called when window closes (Use for cleanup)
  
# Documention
You can find documentation either by going to the wiki tab or
by looking through the Engine folder.

* https://github.com/jakubshark/PixelGameEngine/wiki/Getting-Started

# Other Examples
<img src="https://i.imgur.com/SPTGHfe.gif" width="400"><img src="https://i.imgur.com/sgPtLmT.gif" width="400">

