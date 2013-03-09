using System;
using System.Collections.Generic;
using System.Reflection;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
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
