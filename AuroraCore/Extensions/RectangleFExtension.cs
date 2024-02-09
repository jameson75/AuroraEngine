using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RectangleFExtension
    {
        private static RectangleF _empty = new RectangleF();

        public static RectangleF Empty { get { return _empty; } }

        public static bool Contains(this RectangleF r, Vector2 point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static bool Contains(this RectangleF r, Point point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static Size2F Size(this RectangleF r)
        {
            return new Size2F(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Vector2 Position(this RectangleF r)
        {
            return new Vector2(r.Left, r.Top);
        }        

        public static void AlignRectangle(this RectangleF thisRect, ref RectangleF rectangle, RectangleAlignment alignment)
        {
            Size2F originalSize = rectangle.Size();

            //LEFT            
            if (alignment.HasFlag(RectangleAlignment.Left))            
                rectangle.Left = thisRect.Left;         
            else            
                rectangle.Left = thisRect.Left + ((thisRect.Width - rectangle.Width) / 2.0f);            
            if(!alignment.HasFlag(RectangleAlignment.Right))
                rectangle.Right = rectangle.Left + originalSize.Width;

            //RIGHT            
            if (alignment.HasFlag(RectangleAlignment.Right))
                rectangle.Right = thisRect.Right;
            else             
                rectangle.Right = thisRect.Right - ((thisRect.Width - rectangle.Width) / 2.0f);                
            if (!alignment.HasFlag(RectangleAlignment.Left))
                rectangle.Left = rectangle.Right - originalSize.Width;            
            
            //TOP
            if (alignment.HasFlag(RectangleAlignment.Top))
                rectangle.Top = thisRect.Top;
            else
                rectangle.Top = thisRect.Top + ((thisRect.Height - rectangle.Height) / 2.0f);
            if (!alignment.HasFlag(RectangleAlignment.Bottom))
                rectangle.Bottom = rectangle.Top + originalSize.Height;

            //BOTTOM
            if (alignment.HasFlag(RectangleAlignment.Bottom))
                rectangle.Bottom = thisRect.Bottom;
            else
                rectangle.Bottom = thisRect.Bottom - ((thisRect.Height - rectangle.Height) / 2.0f);
            if (!alignment.HasFlag(RectangleAlignment.Top))
                rectangle.Top = rectangle.Bottom - originalSize.Height;
        }

        public static Rectangle ToRectangle(this RectangleF rf)
        {
            return new Rectangle((int)rf.X, (int)rf.Y, (int)rf.Width, (int)rf.Height);
        }        
    }


}
