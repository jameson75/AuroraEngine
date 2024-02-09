using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Size2FExtension
    {
        private static Size2F _zero = new Size2F(0, 0);

        public static Size2F Zero { get { return _zero; } }

        public static Size2F Add(this Size2F s, Size2F s2)
        {
            return new Size2F(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static Size2F Add(this Size2F s, float w, float h)
        {
            return Add(s, new Size2F(w, h));
        }

        public static Size2 ToSize2(this Size2F sizeF)
        {
            return new Size2(Convert.ToInt32(Math.Ceiling(sizeF.Width)), Convert.ToInt32(Math.Ceiling(sizeF.Height)));
        } 

        public static Vector2 ToVector2(this Size2F sizeF)
        {
            return new Vector2(sizeF.Width, sizeF.Height);
        }  
  
        public static float AspectRatio(this Size2F sizeF)
        {
            return sizeF.Width / sizeF.Height;
        }
    }


}
