using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class RectangleExtension
    {
        public static bool Contains(this Rectangle r, DrawingPoint point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static Vector2 Position(this Rectangle r)
        {
            return new Vector2(r.X, r.Y);
        }       

        public static DrawingSize Size(this Rectangle r)
        {
            return new DrawingSize(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Rectangle CreateLTWH(int l, int t, int width, int height)
        {
            return new Rectangle(l, t, l + width, t + height);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MouseStateExtension
    {
        public static ButtonState LeftButton(this MouseState ms)
        {
            return ms.Buttons[0] ? ButtonState.Pressed : ButtonState.Released;
        }

        public static ButtonState RightButton(this MouseState ms)
        {
            return ms.Buttons[1] ? ButtonState.Pressed : ButtonState.Released;
        }

        public static ButtonState MiddleButton(this MouseState ms)
        {
            return ms.Buttons[2] ? ButtonState.Pressed : ButtonState.Released;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class KeyboardStateExtension
    {
        public static bool IsKeyDown(this KeyboardState ks, Key key)
        {
            return ks.PressedKeys.Contains(key);
        }

        public static bool IsKeyUp(this KeyboardState ks, Key key)
        {
            return !IsKeyDown(ks, key);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class RectangleFExtension
    {
        private static RectangleF _empty = new RectangleF();

        public static RectangleF Empty { get { return _empty; } }

        public static bool Contains(this RectangleF r, DrawingPointF point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static bool Contains(this RectangleF r, DrawingPoint point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static DrawingSizeF Size(this RectangleF r)
        {
            return new DrawingSizeF(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Vector2 Position(this RectangleF r)
        {
            return new Vector2(r.Left, r.Top);
        }

        public static RectangleF CreateLTWH(float l, float t, float width, float height)
        {
            return new RectangleF(l, t, l + width, t + height);
        }

        public static void AlignRectangle(this RectangleF thisRect, ref RectangleF rectangle, RectangleAlignment alignment)
        {
            DrawingSizeF originalSize = rectangle.Size();

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
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DrawingSizeFExtension
    {
        private static DrawingSizeF _zero = new DrawingSizeF(0, 0);

        public static DrawingSizeF Zero { get { return _zero; } }

        public static DrawingSizeF Add(this DrawingSizeF s, DrawingSizeF s2)
        {
            return new DrawingSizeF(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static DrawingSizeF Add(this DrawingSizeF s, float w, float h)
        {
            return Add(s, new DrawingSizeF(w, h));
        }

        public static DrawingSize ToDrawingSize(this DrawingSizeF sizeF)
        {
            return new DrawingSize(Convert.ToInt32(Math.Ceiling(sizeF.Width)), Convert.ToInt32(Math.Ceiling(sizeF.Height)));
        } 

        public static Vector2 ToVector2(this DrawingSizeF sizeF)
        {
            return new Vector2(sizeF.Width, sizeF.Height);
        }  
  
        public static float AspectRatio(this DrawingSizeF sizeF)
        {
            return sizeF.Width / sizeF.Height;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DrawingSizeExtension
    {
        private static DrawingSize _zero = new DrawingSize(0, 0);

        public static DrawingSize Zero { get { return _zero; } }

        public static DrawingSize Add(this DrawingSize s, DrawingSize s2)
        {
            return new DrawingSize(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static DrawingSizeF ToDrawingSizeF(this DrawingSize size)
        {
            return new DrawingSizeF(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static Vector2 ToVector2(this DrawingSize size)
        {
            return new Vector2(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static float AspectRatio(this DrawingSize sizeF)
        {
            return (float)sizeF.Width / (float)sizeF.Height;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DrawingPointExtension
    {
        private static DrawingPoint _zero = new DrawingPoint(0, 0);

        public static DrawingPoint Zero { get { return _zero; } }

        public static DrawingPoint Add(this DrawingPoint s, DrawingPoint s2)
        {
            return new DrawingPoint(s.X + s2.X, s.Y + s2.Y);
        }

        public static DrawingPointF ToDrawingPointF(this DrawingPoint point)
        {
            return new DrawingPointF(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }

        public static Vector2 ToVector2(this DrawingPoint point)
        {
            return new Vector2(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DrawingPointFExtension
    {
        private static DrawingPointF _zero = new DrawingPointF(0, 0);

        public static DrawingPointF Zero { get { return _zero; } }

        public static DrawingPoint ToDrawingPoint(this DrawingPointF pointF)
        {
            return new DrawingPoint(Convert.ToInt32(Math.Floor(pointF.X)), Convert.ToInt32(Math.Floor(pointF.Y)));
        }

        public static DrawingPointF Add(this DrawingPointF s, DrawingPointF s2)
        {
            return new DrawingPointF(s.X + s2.X, s.Y + s2.Y);
        }

        public static DrawingPointF Subtract(this DrawingPointF s, DrawingPointF s2)
        {
            return new DrawingPointF(s.X - s2.X, s.Y - s2.Y);
        }

        public static Vector2 ToVector2(this DrawingPointF pointF)
        {
            return new Vector2(pointF.X, pointF.Y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class BoundingBoxExtension
    {
        private static BoundingBox _empty = new BoundingBox(Vector3.Zero, Vector3.Zero);

        public static BoundingBox Transform(this BoundingBox boundingBox, Matrix matrix)
        {
            Vector4 vMin = Vector3.Transform(boundingBox.Minimum, matrix);
            Vector4 vMax = Vector3.Transform(boundingBox.Maximum, matrix);
            return new BoundingBox(new Vector3(vMin.X, vMin.Y, vMin.Z), new Vector3(vMax.X, vMax.Y, vMax.Z));
        }

        public static float GetLengthX(this BoundingBox boundingBox)
        {
            return boundingBox.Maximum.X - boundingBox.Minimum.X;
        }

        public static float GetLengthY(this BoundingBox boundingBox)
        {
            return boundingBox.Maximum.Y - boundingBox.Minimum.Y;
        }

        public static float GetLengthZ(this BoundingBox boundingBox)
        {
            return boundingBox.Maximum.Z - boundingBox.Minimum.Z;
        }

        public static BoundingBox Empty { get { return _empty; } }    
    }

    public static class Vector3Extension
    {
        public static Vector3 CatmullRomEx(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            float correctedAmount = amount;
            return Vector3.CatmullRom(value1, value2, value3, value4, correctedAmount);
        }
    }

    public static class QuaternionExtension
    {
        public static void GetYawPitchRoll(this Quaternion q, out float yaw, out float pitch, out float roll)
        {            
            yaw = (float)Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            pitch = (float)Math.Asin(-2.0 * (q.X * q.Z - q.W * q.Y));
            roll = (float)Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);
        }

        public static void GetYawPitchRoll2(this Quaternion q, out float yaw, out float pitch, out float roll)
        {
            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.499f * unit)
            {
                // Singularity at north pole
                yaw = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitch = (float)Math.PI * 0.5f;                         // Pitch
                roll = 0f;                                // Roll
            }
            else if (test < -0.499f * unit)
            {
                // Singularity at south pole
                yaw = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitch = -(float)Math.PI * 0.5f;                        // Pitch
                roll = 0f;                               // Roll               
            }
            else
            {
                yaw = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
                pitch = (float)Math.Asin(2 * test / unit);                                             // Pitch
                roll = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll
            }           
        }

        public static Quaternion RotationYawPitchRoll2(float yaw, float pitch, float roll)
        {   
            yaw *= 0.5f;
            pitch *= 0.5f;
            roll *= 0.5f;

            // Assuming the angles are in radians.
            float cy = (float)Math.Cos(yaw);
            float sy = (float)Math.Sin(yaw);
            float cp = (float)Math.Cos(pitch);
            float sp = (float)Math.Sin(pitch);
            float cr = (float)Math.Cos(roll);
            float sr = (float)Math.Sin(roll);
            float cycp = cy * cp;
            float sysp = sy * sp;

            Quaternion quaternion = new Quaternion();
            quaternion.W = cycp * cr - sysp * sr;
            quaternion.X = cycp * sr + sysp * cr;
            quaternion.Y = sy * cp * cr + cy * sp * sr;
            quaternion.Z = cy * sp * cr - sy * cp * sr;

            return quaternion;
        }

        public static void GetYawPitchRoll3(this Quaternion q, out float yaw, out float pitch, out float roll)
        {
            Vector3 v = Vector3.Zero;

            v.X = (float)Math.Atan2
            (
                2 * q.Y * q.W - 2 * q.X * q.Z,
                   1 - 2 * Math.Pow(q.Y, 2) - 2 * Math.Pow(q.Z, 2)
            );

            v.Z = (float)Math.Asin
            (
                2 * q.X * q.Y + 2 * q.Z * q.W
            );

            v.Y = (float)Math.Atan2
            (
                2 * q.X * q.W - 2 * q.Y * q.Z,
                1 - 2 * Math.Pow(q.X, 2) - 2 * Math.Pow(q.Z, 2)
            );

            if (q.X * q.Y + q.Z * q.W == 0.5)
            {
                v.X = (float)(2 * Math.Atan2(q.X, q.W));
                v.Y = 0;
            }

            else if (q.X * q.Y + q.Z * q.W == -0.5)
            {
                v.X = (float)(-2 * Math.Atan2(q.X, q.W));
                v.Y = 0;
            }

            yaw = v.X;
            pitch = v.Y;
            roll = v.Z;
        }   

    }

    public static class MathUtilExtension
    {
        public static short Clamp(short value, short min, short max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum RectangleAlignment
    {
        Centered = 0,
        Middle = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }
}
