using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RectangleExtension
    {
        public static bool Contains(this Rectangle r, Point point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static Vector2 Position(this Rectangle r)
        {
            return new Vector2(r.X, r.Y);
        }       

        public static Size2 Size(this Rectangle r)
        {
            return new Size2(r.Right - r.Left, r.Bottom - r.Top);
        }      
    }


}
