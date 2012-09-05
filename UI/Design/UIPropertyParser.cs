using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using SharpDX;
using SharpDX.Direct3D;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public static class UIControlPropertyParser
    {
        private const string vector3Pattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*,\s*(?:(?:z\s*=\s*(?<z>[0-9]))|(?<z>[0-9]))?\s*}$";
        private const string vector2Pattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*}$";
        private const string vector4Pattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*,\s*(?:(?:z\s*=\s*(?<z>[0-9]))|(?<z>[0-9]))?\s*,\s*(?:(?:w\s*=\s*(?<w>[0-9]))|(?<w>[0-9]))?\s*}$";
        private const string rectanglePattern =    @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*,\s*(?:(?:(?:width|w)\s*=\s*(?<width>[0-9]))|(?<width>[0-9]))?\s*,\s*(?:(?:(?:height|h)\s*=\s*(?<height>[0-9]))|(?<height>[0-9]))?\s*}$";

        public static Vector4 ParseVector4(string value)
        {            
            Vector4 vector = new Vector4();
            Match match = Regex.Match(value, vector4Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                vector.X = UIControlPropertyParser.SafeParseFloat(match.Groups["x"].Value);
                vector.Y = UIControlPropertyParser.SafeParseFloat(match.Groups["y"].Value);
                vector.Z = UIControlPropertyParser.SafeParseFloat(match.Groups["z"].Value);
                vector.W = UIControlPropertyParser.SafeParseFloat(match.Groups["w"].Value);
            }
            else
                throw new IOException("Invalid format for type Vector4");
            return vector;
        }

        public static Vector3 ParseVector3(string value)
        {
            Vector3 vector = new Vector3();
            Match match = Regex.Match(value, vector3Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                vector.X = UIControlPropertyParser.SafeParseFloat(match.Groups["x"].Value);
                vector.Y = UIControlPropertyParser.SafeParseFloat(match.Groups["y"].Value);
                vector.Z = UIControlPropertyParser.SafeParseFloat(match.Groups["z"].Value);               
            }
            else
                throw new IOException("Invalid format for type Vector3");
            return vector;
        }

        public static Vector2 ParseVector2(string value)
        {
            Vector2 vector = new Vector2();
            Match match = Regex.Match(value, vector2Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                vector.X = UIControlPropertyParser.SafeParseFloat(match.Groups["x"].Value);
                vector.Y = UIControlPropertyParser.SafeParseFloat(match.Groups["y"].Value);
            }
            else
                throw new IOException("Invalid format for type Vector2");
            return vector;
        }

        public static Rectangle ParseRectangle(string value)
        {
            Rectangle r = new Rectangle();
            Match match = Regex.Match(value, rectanglePattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                r.Left = UIControlPropertyParser.SafeParseInt(match.Groups["left"].Value);
                r.Top = UIControlPropertyParser.SafeParseInt(match.Groups["top"].Value);
                r.Right = UIControlPropertyParser.SafeParseInt(match.Groups["right"].Value);
                r.Bottom = UIControlPropertyParser.SafeParseInt(match.Groups["bottom"].Value);
            }
            else
                throw new IOException("Invalid format for type Rectangle");
            return r;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static Color4 ParseColor(string value)
        {
            Type colorType = typeof(Color4);
            string propertyName = value.Replace("Color4.", string.Empty);
            PropertyInfo pi = colorType.GetProperty(propertyName);
            if (pi == null)
                throw new FormatException(string.Format("{0} is not a recognized Xna color", propertyName));
            return (Color4)pi.GetValue(null, null);
        }

        private static float SafeParseFloat(string value)
        {
            float fVal = 0;
            if(!string.IsNullOrEmpty(value))
                float.TryParse(value, out fVal);
             return fVal;            
        }

        private static int SafeParseInt(string value)
        {
            int iVal = 0;
            if (!string.IsNullOrEmpty(value))
                int.TryParse(value, out iVal);
            return iVal;
        }
    }
}
