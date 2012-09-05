using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.AngelJacket.Core.Utils
{
    public struct Range
    {
        public static Range Empty = new Range();
        public float Min { get; set; }
        public float Max { get; set; }
    }
}
