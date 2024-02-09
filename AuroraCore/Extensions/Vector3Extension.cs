using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class Vector3Extension
    {
        public static Vector3 CatmullRomEx(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            float correctedAmount = amount;
            return Vector3.CatmullRom(value1, value2, value3, value4, correctedAmount);
        }
        
        public static bool IsAnyComponentNaN(this Vector3 v)
        {
            return float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z);
        }

        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }   


}
