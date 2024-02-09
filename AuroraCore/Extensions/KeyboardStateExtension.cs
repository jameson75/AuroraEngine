using SharpDX.DirectInput;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class KeyboardStateExtension
    {
        public static bool IsKeyDown(this KeyboardState ks, Key key)
        {
            return ks.PressedKeys.Contains(key);
        }

        public static bool IsKeyUp(this KeyboardState ks, Key key)
        {
            return !IsKeyDown(ks, key);
        }
    }


}
