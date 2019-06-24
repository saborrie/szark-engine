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
        private static Vector mouseOffset;
        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;
        private static int pixelScale = 1;

        // -- Mouse Static Values --

        public static float MouseX => (Window.Fullscreen ? 
            (mouseState.X - mouseOffset.x) : mouseState.X) / pixelScale;

        public static float MouseY => (Window.Fullscreen ? 
            (mouseState.Y - mouseOffset.y) : mouseState.Y) / pixelScale;

        public static float MouseWheel { get; private set; }

        // -- Internal Static Methods --

        internal static void SetPixelScale(int scale) =>
            pixelScale = scale;

        internal static void Init()
        {
            Window.window.KeyUp += (o, a) => UpdateKeyboardState(a);
            Window.window.KeyDown += (o, a) => UpdateKeyboardState(a);
            Window.window.MouseDown += (o, a) => UpdateMouseState(a);
            Window.window.MouseUp += (o, a) => UpdateMouseState(a);
            Window.window.MouseMove += (o, a) => UpdateMouseState(a);
            Window.window.MouseWheel += (o, a) => UpdateMouseState(a);
        }

        internal static void Update()
        {
            mouseOffset.x = Window.Fullscreen ? Window.Width * 0.5f : 0;
            mouseOffset.y = Window.Fullscreen ? Window.Height * 0.5f : 0;

            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            MouseWheel = 0;
        }

        // -- Update Keyboard and Mouse States --

        private static void UpdateKeyboardState(KeyboardKeyEventArgs args) =>
            keyboardState = args.Keyboard;

        private static void UpdateMouseState(MouseEventArgs args) =>
            mouseState = args.Mouse;

        // -- Keyboard Static Methods --

        public static bool GetKey(string key) =>
            Enum.TryParse(key, out Key result) ? keyboardState[result] : false;

        public static bool GetKeyDown(string key) =>
            Enum.TryParse(key, out Key result) ? keyboardState[result] && 
                (keyboardState[result] != lastKeyboardState[result]) : false;

        public static bool GetKeyUp(string key) =>
            Enum.TryParse(key, out Key result) ? lastKeyboardState[result] &&
                (keyboardState[result] != lastKeyboardState[result]) : false;

        // -- Mouse Static Methods --

        public static bool GetMouseButton(int button) => mouseState[(MouseButton)button];

        public static bool GetMouseButtonDown(int button) => 
            mouseState[(MouseButton)button] && (mouseState[(MouseButton)button] != 
                lastMouseState[(MouseButton)button]);

        public static bool GetMouseButtonUp(int button) => 
            lastMouseState[(MouseButton)button] && (mouseState[(MouseButton)button] != 
                lastMouseState[(MouseButton)button]);
    }
}