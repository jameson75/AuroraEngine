using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace CipherPark.AngelJacket.Core.Utils
{
    public static class XnaExtensions
    {
        public static Vector2 Size(this Rectangle r)
        {
            return new Vector2(r.Width, r.Height);
        }
    }
}
