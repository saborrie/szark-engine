using OpenTK;
using OpenTK.Input;

namespace PGE
{
    public class Input
    {
        private static GameWindow gameWindow;
        private static PixelGameEngine engine;

        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;

        /// <summary>
        /// Mouse X position on screen
        /// </summary>
        public static int MouseX
        {
            get => engine.IsFullscreen ? (gameWindow.Mouse.X -
                engine.RenderOffsetX) / engine.PixelSize :
                    gameWindow.Mouse.X / engine.PixelSize;
        }

        /// <summary>
        /// Mouse Y position on screen
        /// </summary>
        public static int MouseY
        {
            get => engine.IsFullscreen ? (gameWindow.Mouse.Y -
                engine.RenderOffsetY) / engine.PixelSize :
                    gameWindow.Mouse.Y / engine.PixelSize;
        }

        public static void SetContext(PixelGameEngine engine, GameWindow window) 
        {
            if (gameWindow != null)
            {
                gameWindow.KeyUp -= KeyUp;
                gameWindow.KeyDown -= KeyDown;
                gameWindow.MouseDown -= MouseDown;
                gameWindow.MouseUp -= MouseUp;
            }

            if (engine != null)
                engine.AdditionalUpdates -= Update;

            Input.engine = engine;
            gameWindow = window;

            engine.AdditionalUpdates += Update;

            gameWindow.KeyUp += KeyUp;
            gameWindow.KeyDown += KeyDown;
            gameWindow.MouseDown += MouseDown;
            gameWindow.MouseUp += MouseUp;
        }

        public static void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
        }

        #region Events

        private static void KeyDown(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        private static void KeyUp(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        private static void MouseDown(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        private static void MouseUp(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        #endregion

        /// <summary>
        /// Checks if a key is held
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Is Held?</returns>
        public static bool GetKey(Key key) => keyboardState[key];

        /// <summary>
        /// Check if a key is pressed once
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Was Pressed?</returns>
        public static bool GetKeyDown(Key key) => 
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        /// <summary>
        /// Checks if mouse button is pressed
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Down?</returns>
        public static bool GetMouseButton(MouseButton button) =>
            mouseState[button];

        /// <summary>
        /// Checks if mouse button is pressed down once
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Is Up?</returns>
        public static bool GetMouseButtonDown(MouseButton button) =>
            mouseState[button] && (mouseState[button] != lastMouseState[button]);
    }
}