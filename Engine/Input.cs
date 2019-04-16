using OpenTK;
using OpenTK.Input;
using System;

namespace Szark
{
    /// <summary>
    /// Holds the states of the keyboard and the mouse.
    /// Use methods in the update thread for optimal accuracy.
    /// </summary>
    public static class Input
    {
        private static int offsetX, offsetY;
        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;

        private static GameWindow gameWindow;
        private static SzarkEngine engine;

        // -- Mouse Static Values --

        public static int MouseX => engine.Fullscreen ? (mouseState.X - offsetX) : mouseState.X;
        public static int MouseY => engine.Fullscreen ? (mouseState.Y - offsetY) : mouseState.Y;
        public static float MouseWheel { get; private set; }

        // -- Internal Static Methods --

        internal static void SetContext(GameWindow window, SzarkEngine engine)
        {
            if (gameWindow != null)
            {
                gameWindow.KeyUp -= KeyUp;
                gameWindow.KeyDown -= KeyDown;
                gameWindow.MouseDown -= MouseDown;
                gameWindow.MouseUp -= MouseUp;
                gameWindow.MouseMove -= OnMouseMoved;
                gameWindow.MouseWheel -= OnMouseWheel;
            }

            gameWindow = window;
            Input.engine = engine;

            gameWindow.KeyUp += KeyUp;
            gameWindow.KeyDown += KeyDown;
            gameWindow.MouseDown += MouseDown;
            gameWindow.MouseUp += MouseUp;
            gameWindow.MouseMove += OnMouseMoved;
            gameWindow.MouseWheel += OnMouseWheel;
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
            MouseWheel = 0;
        }

        // -- Window Input Events --

        private static void KeyDown(object sender, KeyboardKeyEventArgs e) => keyboardState = e.Keyboard;
        private static void KeyUp(object sender, KeyboardKeyEventArgs e) => keyboardState = e.Keyboard;
        private static void MouseDown(object sender, MouseButtonEventArgs args) => mouseState = args.Mouse;
        private static void MouseUp(object sender, MouseButtonEventArgs args) => mouseState = args.Mouse;
        private static void OnMouseWheel(object sender, MouseWheelEventArgs e) => MouseWheel = e.Value;
        private static void OnMouseMoved(object sender, MouseMoveEventArgs e) => mouseState = e.Mouse;

        // -- Keyboard Static Methods --

        public static bool GetKey(Key key) => keyboardState[key];

        public static bool GetKey(string key) =>
            Enum.TryParse(key, out Key result) ? keyboardState[result] : false;

        public static bool GetKeyDown(Key key) =>
            keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        public static bool GetKeyDown(string key) =>
            Enum.TryParse(key, out Key result) ? keyboardState[result] && 
                (keyboardState[result] != lastKeyboardState[result]) : false;

        public static bool GetKeyUp(Key key) =>
            lastKeyboardState[key] && (keyboardState[key] != lastKeyboardState[key]);

        public static bool GetKeyUp(string key) =>
            Enum.TryParse(key, out Key result) ? lastKeyboardState[result] &&
                (keyboardState[result] != lastKeyboardState[result]) : false;

        // -- Mouse Static Methods --

        public static bool GetMouseButton(MouseButton button) => mouseState[button];

        public static bool GetMouseButtonDown(MouseButton button) =>
            mouseState[button] && (mouseState[button] != lastMouseState[button]);

        public static bool GetMouseButtonUp(MouseButton button) =>
            lastMouseState[button] && (mouseState[button] != lastMouseState[button]);

        public static bool GetMouseButton(int button) => GetMouseButton((MouseButton)button);
        public static bool GetMouseButtonDown(int button) => GetMouseButtonDown((MouseButton)button);
        public static bool GetMouseButtonUp(int button) => GetMouseButtonUp((MouseButton)button);
    }
}