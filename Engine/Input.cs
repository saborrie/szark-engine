using OpenTK;
using OpenTK.Input;
using System;

namespace Szark
{
    public static class Input
    {
        private static int offsetX, offsetY;
        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;

        private static GameWindow gameWindow;
        private static SzarkEngine engine;

        /// <summary>
        /// Mouse X position on screen
        /// </summary>
        public static int MouseX =>
            engine.IsFullscreen ? (gameWindow.Mouse.X - offsetX) / 
                engine.PixelSize : gameWindow.Mouse.X / engine.PixelSize;

        /// <summary>
        /// Mouse Y position on screen
        /// </summary>
        public static int MouseY =>
            engine.IsFullscreen ? (gameWindow.Mouse.Y - offsetY) / 
                engine.PixelSize : gameWindow.Mouse.Y / engine.PixelSize;

        /// <summary>
        /// Sets the current active window and engine for input
        /// </summary>
        /// <param name="engine">The Engine</param>
        public static void SetContext(SzarkEngine engine) 
        {
            if (engine == null)
                throw new NullReferenceException();

            if (gameWindow != null)
            {
                gameWindow.KeyUp -= KeyUp;
                gameWindow.KeyDown -= KeyDown;
                gameWindow.MouseDown -= MouseDown;
                gameWindow.MouseUp -= MouseUp;
            }

            if (engine != null)
            {
                engine.WindowStateChanged -= OnWindowStateChanged;
                engine.WindowUpdated -= OnWindowUpdated;
            }

            gameWindow = engine.GameWindow;
            Input.engine = engine;

            gameWindow.KeyUp += KeyUp;
            gameWindow.KeyDown += KeyDown;
            gameWindow.MouseDown += MouseDown;
            gameWindow.MouseUp += MouseUp;

            engine.WindowStateChanged += OnWindowStateChanged;
            engine.WindowUpdated += OnWindowUpdated;
        }

        private static void OnWindowStateChanged()
        {
            offsetX = engine.IsFullscreen ? (gameWindow.Width - 
                engine.WindowWidth) / 2 : 0;
            offsetY = engine.IsFullscreen ? (gameWindow.Height - 
                engine.WindowHeight) / 2 : 0;
        }

        private static void OnWindowUpdated(float deltaTime)
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

        #region Keyboard

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
        /// Checks if a key was released
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Was Released?</returns>
        public static bool GetKeyUp(Key key) =>
            lastKeyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        /// <summary>
        /// Checks if a key is held
        /// </summary>
        /// <param name="key">Name of the key</param>
        public static bool GetKey(string key)
        {
            if (Enum.TryParse(key, out Key result))
                return keyboardState[result];
            else
            {
                Debug.Log($"Key with value '{key}' does not exist!",
                    LogLevel.WARNING);
                return false;
            }
        }

        /// <summary>
        /// Checks is key was pressed
        /// </summary>
        public static bool GetKeyDown(string key)
        {
            if (Enum.TryParse(key, out Key result))
            {
                return keyboardState[result] && (keyboardState[result] != 
                    lastKeyboardState[result]);
            }
            else
            {
                Debug.Log($"Key with value '{key}' does not exist!", 
                    LogLevel.WARNING);
                return false;
            }
        }

        /// <summary>
        /// Checks if key was released
        /// </summary>
        public static bool GetKeyUp(string key)
        {
            if (Enum.TryParse(key, out Key result))
            {
                return lastKeyboardState[result] && (keyboardState[result] !=
                    lastKeyboardState[result]);
            }
            else
            {
                Debug.Log($"Key with value '{key}' does not exist!", 
                    LogLevel.WARNING);
                return false;
            }
        }

        #endregion

        #region Mouse

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

        /// <summary>
        /// Checks if mouse button was released
        /// </summary>
        /// <param name="button">The Button</param>
        /// <returns>Was Released?</returns>
        public static bool GetMouseButtonUp(MouseButton button) =>
            lastMouseState[button] && (mouseState[button] != lastMouseState[button]);

        public static bool GetMouseButton(int button) => GetMouseButton((MouseButton)button);
        public static bool GetMouseButtonDown(int button) => GetMouseButtonDown((MouseButton)button);
        public static bool GetMouseButtonUp(int button) => GetMouseButtonUp((MouseButton)button);

        #endregion
    }
}