using System;
using System.Collections.Generic;
using System.Reflection;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils
{
    public static class ColorParser
    {
        public static Color4 Parse(string name)
        {
            Type colorType = typeof(Color4);
            string propertyName = name.Replace("Color4.", string.Empty);
            PropertyInfo pi = colorType.GetProperty(propertyName);
            if (pi == null)
                throw new FormatException(string.Format("{0} is not a recognized Xna color", propertyName));
            return (Color4)pi.GetValue(null, null);
        }
    }
}
