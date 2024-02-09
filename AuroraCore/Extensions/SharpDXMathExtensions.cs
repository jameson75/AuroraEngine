using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class SharpDXMathExtensions
    {
        static public BoundingBox ExpandedAlongY(this BoundingBox box)
        {
            return new BoundingBox(new Vector3(box.Minimum.X, float.MinValue, box.Minimum.Z),
                                   new Vector3(box.Maximum.X, float.MaxValue, box.Maximum.Z));
        }

        static public Vector2 XY(this Vector3 v) { return new Vector2(v.X, v.Y); }

        static public Vector2 XZ(this Vector3 v) { return new Vector2(v.X, v.Z); }

        static public Vector2 YZ(this Vector3 v) { return new Vector2(v.Y, v.Z); }

        static public Vector3 MapToXZ(this Vector2 v, float y) { return new Vector3(v.X, y, v.Y); }
    }
}
