using OpenTK;
using OpenTK.Input;

namespace PGE
{
    public class Input
    {
        private GameWindow gameWindow;
        private PixelGameEngine gameEngine;
        private KeyboardState keyboardState, lastKeyboardState;
        private MouseState mouseState, lastMouseState;

        public Input(PixelGameEngine p, GameWindow e) 
        {
            gameEngine = p;
            gameWindow = e;

            gameWindow.KeyUp += KeyUp;
            gameWindow.KeyDown += KeyDown;
            gameWindow.MouseDown += MouseDown;
            gameWindow.MouseUp += MouseUp;
        }

        public void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
        }

        // On Key Pressed Down
        private void KeyDown(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        // On Key Lifted Up
        private void KeyUp(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        /// <summary>
        /// Checks if a Key is Held
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Is Held?</returns>
        public bool GetKey(Key key) => keyboardState[key];

        /// <summary>
        /// Check if a Key is Pressed Once
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Was Pressed?</returns>
        public bool GetKeyDown(Key key) => 
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        private void MouseDown(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        private void MouseUp(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        /// <summary>
        /// Returns the Mouse's X Position
        /// </summary>
        public int GetMouseX() => gameEngine.IsFullscreen ? (gameWindow.Mouse.X - 
            gameEngine.RenderOffsetX) / gameEngine.PixelSize : 
                gameWindow.Mouse.X / gameEngine.PixelSize; 

        /// <summary>
        /// Returns the Mouse's Y Position
        /// </summary>
        public int GetMouseY() => gameEngine.IsFullscreen ? (gameWindow.Mouse.Y - 
            gameEngine.RenderOffsetY) / gameEngine.PixelSize : 
                gameWindow.Mouse.Y / gameEngine.PixelSize;

        /// <summary>
        /// Checks if Mouse Button is Pressed
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Down?</returns>
        public bool GetMouseButton(MouseButton button) =>
            mouseState[button];

        /// <summary>
        /// Checks if Mouse Button is pressed down once
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Up?</returns>
        public bool GetMouseButtonDown(MouseButton button) =>
            mouseState[button] && (mouseState[button] != lastMouseState[button]);
    }
}