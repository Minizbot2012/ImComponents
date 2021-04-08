using System.Runtime.InteropServices;

namespace ImComponents
{
    public static class Keyboard
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);
        public static byte[] GetKeyboard()
        {
            var array = new byte[256];
            GetKeyboardState(array);
            return array;
        }
        public static bool IsPressed(byte ch)
        {
            return (ch & 0x80) != 0;
        }
    }
}