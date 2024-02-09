using SharpDX.DirectInput;
using CipherPark.Aurora.Core.Utils;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MouseStateExtension
    {
        public static ButtonState LeftButton(this MouseState ms)
        {
            return ms.Buttons[0] ? ButtonState.Down : ButtonState.Up;
        }

        public static ButtonState RightButton(this MouseState ms)
        {
            return ms.Buttons[1] ? ButtonState.Down : ButtonState.Up;
        }

        public static ButtonState MiddleButton(this MouseState ms)
        {
            return ms.Buttons[2] ? ButtonState.Down : ButtonState.Up;
        }
    }


}
