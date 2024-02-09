using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class RayExtensions
    {
        public static Ray Reverse(this Ray ray)
        {
            return new Ray(ray.Position, -ray.Direction);
        }
    }


}
