using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils
{
    public struct RangeF
    {
        public static RangeF Empty = new RangeF();
        public float Min { get; set; }
        public float Max { get; set; }
        public RangeF(float min, float max) : this()
        {
            Min = min;
            Max = max;
        }
    }

    public struct Range
    {
        public static Range Empty = new Range();
        public int Min { get; set; }
        public int Max { get; set; }
        public Range(int min, int max)
            : this()
        {
            Min = min;
            Max = max;
        }
    }
}
