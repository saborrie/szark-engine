using OpenTK;
using OpenTK.Input;
using System;

namespace Szark
{
    public static class Input
    {
        private static int offsetX, offsetY;
        private static int mouseX, mouseY;

        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;

        private static GameWindow gameWindow;
        private static SzarkEngine engine;

        public static int MouseX =>
            engine.Fullscreen ? (mouseX - offsetX) : mouseX;

        public static int MouseY =>
            engine.Fullscreen ? (mouseY - offsetY) : mouseY;

        internal static void SetContext(GameWindow window, SzarkEngine engine)
        {
            if (gameWindow != null)
            {
                gameWindow.KeyUp -= KeyUp;
                gameWindow.KeyDown -= KeyDown;
                gameWindow.MouseDown -= MouseDown;
                gameWindow.MouseUp -= MouseUp;
                gameWindow.MouseMove -= MouseMoved;
            }

            gameWindow = window;
            Input.engine = engine;

            gameWindow.KeyUp += KeyUp;
            gameWindow.KeyDown += KeyDown;
            gameWindow.MouseDown += MouseDown;
            gameWindow.MouseUp += MouseUp;
            gameWindow.MouseMove += MouseMoved;
        }

        private static void MouseMoved(object sender, MouseMoveEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }

        internal static void UpdateOffsets()
        {
            offsetX = engine.Fullscreen ? gameWindow.Width / 2 : 0;
            offsetY = engine.Fullscreen ? gameWindow.Height / 2 : 0;
        }

        internal static void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
        }

        private static void KeyDown(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        private static void KeyUp(object sender, KeyboardKeyEventArgs e) =>
            keyboardState = e.Keyboard;

        private static void MouseDown(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        private static void MouseUp(object sender, MouseButtonEventArgs args) =>
            mouseState = args.Mouse;

        public static bool GetKey(Key key) => keyboardState[key];

        public static bool GetKeyDown(Key key) =>
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        public static bool GetKeyUp(Key key) =>
            lastKeyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

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

        public static bool GetMouseButton(MouseButton button) =>
            mouseState[button];

        public static bool GetMouseButtonDown(MouseButton button) =>
            mouseState[button] && (mouseState[button] != lastMouseState[button]);

        public static bool GetMouseButtonUp(MouseButton button) =>
            lastMouseState[button] && (mouseState[button] != lastMouseState[button]);

        public static bool GetMouseButton(int button) => GetMouseButton((MouseButton)button);
        public static bool GetMouseButtonDown(int button) => GetMouseButtonDown((MouseButton)button);
        public static bool GetMouseButtonUp(int button) => GetMouseButtonUp((MouseButton)button);
    }
}