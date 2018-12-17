## How to get started:
1. Download .Net Core 2.2 or Higher

2. Clone or download this project

3. Open the project in your favorite IDE like VS Code

4. Make sure to restore the project (download the dependencies with Nuget Package Manager if necessary)

5. Your good to go!

## Dependencies
* OpenTK.NetCore

* CoreCompact.System.Drawing

* NAudio

## Getting Starting Example
```
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

        protected override void Draw(float deltaTime)
        {
            var random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    Graphics.Draw(i, j, new Pixel((byte)random.Next(255), 
                        (byte)random.Next(255), (byte)random.Next(255)));
        }

        protected override void Destroyed() {}

        static void Main() => 
            new RandomExample().Construct(800, 800, 8);
    }
}
```
## What am I looking at?
* Getting started with this engine the first thing you'll need to do is to use the namespace PGE to get access to all the engine's features. 

* Then simply just create a class that inherits from the PixelGameEngine. This is the class where you will do most of your work in.

* To get a Window to show up simply create a main method in your CS file and instantiate the class you just created. 

* After this call the Construct function

* If you would like to change things like the window's title use the start function and change the string property WindowTitle.

* You may want to override a few methods in your class like:
  * **Start** - Called when window is created (Use for initializing)
  * **Update** - Called every tick (Use for game logic like player movement, physics, etc.)
  * **Draw** - Called every frame (Use for rendering stuff and drawing)
  * **GPUDraw** - Called every frame (Use for rendering GPU sprites)
  * **Destroyed** - Called when window closes (Use for cleanup)

* Lastly if you want to draw something, in the Draw function you can use the plethora of draw functions given in the PixelGameEngine class.

# Documention / Getting Started
You can find documentation either by going to the wiki tab or
by looking through the Engine folder.

* https://github.com/jakubshark/PixelGameEngine/wiki/Getting-Started

# Examples
![alt text](https://i.imgur.com/SPTGHfe.gif)
![alt text](https://i.imgur.com/sgPtLmT.gif)
