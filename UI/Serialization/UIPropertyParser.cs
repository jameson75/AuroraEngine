using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using SharpDX;
using SharpDX.Direct3D;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public static class UIControlPropertyParser
    {
        private const string vector3Pattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*,\s*(?:(?:z\s*=\s*(?<z>[0-9]))|(?<z>[0-9]))?\s*}$";
        private const string vector2Pattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*}$";
        private const string vector4Pattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*,\s*(?:(?:z\s*=\s*(?<z>[0-9]))|(?<z>[0-9]))?\s*,\s*(?:(?:w\s*=\s*(?<w>[0-9]))|(?<w>[0-9]))?\s*}$";
        private const string rectanglePattern = @"^{\s*(?:(?:x\s*=\s*(?<x>[0-9]))|(?<x>[0-9]))?\s*,\s*(?:(?:y\s*=\s*(?<y>[0-9]))|(?<y>[0-9]))?\s*,\s*(?:(?:(?:width|w)\s*=\s*(?<width>[0-9]))|(?<width>[0-9]))?\s*,\s*(?:(?:(?:height|h)\s*=\s*(?<height>[0-9]))|(?<height>[0-9]))?\s*}$";
        private const string boundaryPattery = @"^{\s*(?:(?:l\s*=\s*(?<l>[0-9]))|(?<l>[0-9]))?\s*,\s*(?:(?:t\s*=\s*(?<t>[0-9]))|(?<t>[0-9]))?\s*,\s*(?:(?:(?:r)\s*=\s*(?<r>[0-9]))|(?<r>[0-9]))?\s*,\s*(?:(?:(?:b)\s*=\s*(?<b>[0-9]))|(?<b>[0-9]))?\s*}$";
        private const string Size2Pattern = @"^{\s*(?:(?:w\s*=\s*(?<w>[0-9]))|(?<w>[0-9]))?\s*,\s*(?:(?:h\s*=\s*(?<h>[0-9]))|(?<h>[0-9]))?\s*}$";

        public static Vector4 ParseVector4(string value)
        {            
            Vector4 vector = new Vector4();
            Match match = Regex.Match(value, vector4Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                vector.X = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["x"].Value);
                vector.Y = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["y"].Value);
                vector.Z = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["z"].Value);
                vector.W = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["w"].Value);
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
                vector.X = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["x"].Value);
                vector.Y = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["y"].Value);
                vector.Z = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["z"].Value);               
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
                vector.X = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["x"].Value);
                vector.Y = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["y"].Value);
            }
            else
                throw new IOException("Invalid format for type Vector2");
            return vector;
        }

        public static Size2 ParseSize2(string value)
        {
            Size2 size = new Size2();
            Match match = Regex.Match(value, Size2Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                size.Width = UIControlPropertyParser.ParseOrDefaultInt(match.Groups["w"].Value);
                size.Height = UIControlPropertyParser.ParseOrDefaultInt(match.Groups["h"].Value);
            }
            else
                throw new IOException("Invalid format for type Size2");
            return size;
        }

        public static Size2F ParseSize2F(string value)
        {
            Size2F size = new Size2F();
            Match match = Regex.Match(value, Size2Pattern, RegexOptions.IgnoreCase);
            if (match.Success)  
            {
                size.Width = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["w"].Value);
                size.Height = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["h"].Value);
            }
            else
                throw new IOException("Invalid format for type Size2F");
            return size;
        }

        public static Point ParsePoint(string value)
        {
            Point size = new Point();
            Match match = Regex.Match(value, vector2Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                size.X = UIControlPropertyParser.ParseOrDefaultInt(match.Groups["x"].Value);
                size.Y = UIControlPropertyParser.ParseOrDefaultInt(match.Groups["y"].Value);
            }
            else
                throw new IOException("Invalid format for type Point");
            return size;
        }

        public static Vector2 ParseDrawingPointF(string value)
        {
            Vector2 size = new Vector2();
            Match match = Regex.Match(value, vector2Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                size.X = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["x"].Value);
                size.Y = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["y"].Value);
            }
            else
                throw new IOException("Invalid format for type Point");
            return size;
        }

        public static Rectangle ParseRectangle(string value)
        {
            Rectangle r = new Rectangle();
            Match match = Regex.Match(value, rectanglePattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                r.Left = UIControlPropertyParser.ParseOrDefaultInt(match.Groups["x"].Value);
                r.Top = UIControlPropertyParser.ParseOrDefaultInt(match.Groups["y"].Value);
                r.Right = r.Left + UIControlPropertyParser.ParseOrDefaultInt(match.Groups["width"].Value);
                r.Bottom = r.Top + UIControlPropertyParser.ParseOrDefaultInt(match.Groups["height"].Value);
            }
            else
                throw new IOException("Invalid format for type Rectangle");
            return r;
        }

        public static RectangleF ParseRectangleF(string value)
        {
            RectangleF r = new RectangleF();
            Match match = Regex.Match(value, rectanglePattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                r.Left = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["x"].Value);
                r.Top = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["y"].Value);
                r.Right = r.Left + UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["width"].Value);
                r.Bottom = r.Top + UIControlPropertyParser.ParseOrDefaultFloat (match.Groups["height"].Value);
            }
            else
                throw new IOException("Invalid format for type RectangleF");
            return r;
        }

        public static BoundaryF ParseBoundaryF(string value)
        {
            BoundaryF b = new BoundaryF();
            Match match = Regex.Match(value, rectanglePattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                b.Left = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["l"].Value);
                b.Top = UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["t"].Value);
                b.Right = b.Left + UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["r"].Value);
                b.Bottom = b.Top + UIControlPropertyParser.ParseOrDefaultFloat(match.Groups["b"].Value);
            }
            else
                throw new IOException("Invalid format for type BoundaryF");
            return b;
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
                throw new FormatException(string.Format("{0} is not a recognized SharpDX color", propertyName));
            return (Color4)pi.GetValue(null, null);
        }

        private static float ParseOrDefaultFloat(string value)
        {
            float fVal = 0;
            if(!string.IsNullOrEmpty(value))
                float.TryParse(value, out fVal);
             return fVal;            
        }

        private static int ParseOrDefaultInt(string value)
        {
            int iVal = 0;
            if (!string.IsNullOrEmpty(value))
                int.TryParse(value, out iVal);
            return iVal;
        }
    }
}
