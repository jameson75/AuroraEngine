using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class SpriteFont
    {
        IGameApp _game = null;

        public SpriteFont(IGameApp game)
        {
            _game = game;
        }

        public Vector2 MeasureString(string text)
        {
            Size stringSize = UnsafeNativeMethods.MeasureString(_game.DeviceHwnd, text);
            return new Vector2((float)stringSize.X, (float)stringSize.Y);
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint = "MeasureString")]
            public static extern Size MeasureString(IntPtr hWnd, string text);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Size
        {
            public long X;
            public long Y;
        }
    }
}
