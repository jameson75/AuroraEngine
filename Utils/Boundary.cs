using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Utils
{
    public struct BoundaryF
    {
        private static BoundaryF _zero = new BoundaryF();

        public BoundaryF(float l, float t, float r, float b) : this()
        {
            Left = l;
            Top = t;
            Right = r;
            Bottom = b;
        }
        public float Left { get; set; }
        public float Top { get; set; }        
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Width
        {
            get { return Left + Right; }
        }
        public float Height
        {
            get { return Top + Bottom; }
        }
        public static BoundaryF Zero
        {
            get { return _zero; }
        }
    }
}
