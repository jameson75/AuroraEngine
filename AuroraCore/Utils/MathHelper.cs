namespace CipherPark.Aurora.Core.Utils
{
    public static class MathHelper
    {
        public static short Clamp(short value, short min, short max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
    }
}
