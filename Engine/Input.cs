using OpenTK;
using OpenTK.Input;

namespace PGE
{
    public class Input
    {
        private GameWindow gameWindow;
        private PixelGameEngine engine;

        private KeyboardState keyboardState, lastKeyboardState;
        private MouseState mouseState, lastMouseState;

        /// <summary>
        /// Mouse X position on screen
        /// </summary>
        public int MouseX
        {
            get => engine.IsFullscreen ? (gameWindow.Mouse.X -
                engine.RenderOffsetX) / engine.PixelSize :
                    gameWindow.Mouse.X / engine.PixelSize;
        }

        /// <summary>
        /// Mouse Y position on screen
        /// </summary>
        public int MouseY
        {
            get => engine.IsFullscreen ? (gameWindow.Mouse.Y -
                engine.RenderOffsetY) / engine.PixelSize :
                    gameWindow.Mouse.Y / engine.PixelSize;
        }

        public Input(PixelGameEngine engine, GameWindow window) 
        {
            this.engine = engine;
            gameWindow = window;

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

        #region Events

        private void KeyDown(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        private void KeyUp(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        private void MouseDown(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        private void MouseUp(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        #endregion

        /// <summary>
        /// Checks if a key is held
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Is Held?</returns>
        public bool GetKey(Key key) => keyboardState[key];

        /// <summary>
        /// Check if a key is pressed once
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Was Pressed?</returns>
        public bool GetKeyDown(Key key) => 
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        /// <summary>
        /// Checks if mouse button is pressed
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Down?</returns>
        public bool GetMouseButton(MouseButton button) =>
            mouseState[button];

        /// <summary>
        /// Checks if mouse button is pressed down once
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Up?</returns>
        public bool GetMouseButtonDown(MouseButton button) =>
            mouseState[button] && (mouseState[button] != lastMouseState[button]);
    }
}