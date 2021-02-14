using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core
{
    public static class ResourceViewExtension
    {
        public static Texture2DDescription GetTextureDescription(this ResourceView view)
        {
            using (var resource = view.ResourceAs<Texture2D>())
                return resource.Description;
        }
    }

    public static class Texture2DExtension
    {
        public static ShaderResourceView ToShaderResourceView(this Texture2D texture, SharpDX.Direct3D11.Device device)
        {
            return new ShaderResourceView(device, texture);
        }
    }
}

namespace CipherPark.KillScript.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class RectangleExtension
    {
        public static bool Contains(this Rectangle r, Point point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static Vector2 Position(this Rectangle r)
        {
            return new Vector2(r.X, r.Y);
        }       

        public static Size2 Size(this Rectangle r)
        {
            return new Size2(r.Right - r.Left, r.Bottom - r.Top);
        }      
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MouseStateExtension
    {
        public static ButtonState LeftButton(this MouseState ms)
        {
            return ms.Buttons[0] ? ButtonState.Down : ButtonState.Up;
        }

        public static ButtonState RightButton(this MouseState ms)
        {
            return ms.Buttons[1] ? ButtonState.Down : ButtonState.Up;
        }

        public static ButtonState MiddleButton(this MouseState ms)
        {
            return ms.Buttons[2] ? ButtonState.Down : ButtonState.Up;
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

        public static bool Contains(this RectangleF r, Vector2 point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static bool Contains(this RectangleF r, Point point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static Size2F Size(this RectangleF r)
        {
            return new Size2F(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Vector2 Position(this RectangleF r)
        {
            return new Vector2(r.Left, r.Top);
        }        

        public static void AlignRectangle(this RectangleF thisRect, ref RectangleF rectangle, RectangleAlignment alignment)
        {
            Size2F originalSize = rectangle.Size();

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

        public static Rectangle ToRectangle(this RectangleF rf)
        {
            return new Rectangle((int)rf.X, (int)rf.Y, (int)rf.Width, (int)rf.Height);
        }        
    }

    public static class RectangleDimensions
    {
        public static RectangleF FromCenter(float quadWidth, float quadHeight)
        {
            float halfWidth = quadWidth / 2.0f;
            float halfHeight = quadHeight / 2.0f;
            return new RectangleF(-halfWidth, halfHeight, quadWidth, -quadHeight);
        }
    }

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

    /// <summary>
    /// 
    /// </summary>
    public static class Size2Extension
    {
        private static Size2 _zero = new Size2(0, 0);

        public static Size2 Zero { get { return _zero; } }

        public static Size2 Add(this Size2 s, Size2 s2)
        {
            return new Size2(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static Size2F ToSize2F(this Size2 size)
        {
            return new Size2F(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static Vector2 ToVector2(this Size2 size)
        {
            return new Vector2(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static float AspectRatio(this Size2 sizeF)
        {
            return (float)sizeF.Width / (float)sizeF.Height;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PointExtension
    {
        private static Point _zero = new Point(0, 0);

        public static Point Zero { get { return _zero; } }

        public static Point Add(this Point s, Point s2)
        {
            return new Point(s.X + s2.X, s.Y + s2.Y);
        }

        /* Vector2 is deprecated */
        //
        //public static Vector2 ToVector2(this Point point)
        //{
        //    return new Vector2(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        //}

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Vector2Extension
    {
        private static Vector2 _zero = new Vector2(0, 0);

        public static Vector2 Zero { get { return _zero; } }

        public static Point ToPoint(this Vector2 pointF)
        {
            return new Point(Convert.ToInt32(Math.Floor(pointF.X)), Convert.ToInt32(Math.Floor(pointF.Y)));
        }

        public static Vector2 Add(this Vector2 s, Vector2 s2)
        {
            return new Vector2(s.X + s2.X, s.Y + s2.Y);
        }

        public static Vector2 Subtract(this Vector2 s, Vector2 s2)
        {
            return new Vector2(s.X - s2.X, s.Y - s2.Y);
        }

        public static Vector2 ToVector2(this Vector2 pointF)
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
        
        public static bool IsAnyComponentNaN(this Vector3 v)
        {
            return float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z);
        }
    }   

    public static class Vector4Extension
    {
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }

    public static class QuaternionExtension
    {
        /*
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
        */
        public static void GetYawPitchRoll(this Quaternion q, out float yaw, out float pitch, out float roll)
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

    public static class PlaneExtension
    {
        public static Vector3? ProjectPoint(this Plane p, Vector3 point, Vector3? alongVector = null)
        {
            if (alongVector == null)
                //{Projection of a Vector to the nearest point on a plane : B = A - Dot(A - P, N) * N.                
                //(See tmpearce's explanation at http://stackoverflow.com/questions/9605556/how-to-project-a-3d-point-to-a-3d-plane#comment12185786_9605695)            
                return point - Vector3.Dot(point - p.GetOrigin(), p.Normal) * p.Normal;
            else
            {
                Ray ray = new Ray(point, Vector3.Normalize(alongVector.Value));
                Vector3 intersection;
                if (p.Intersects(ref ray, out intersection))
                    return intersection;
                else
                    return null;
            }                
        }

        public static Vector3 GetOrigin(this Plane p)
        {
            return p.Normal * p.D;
        }
    }

    public static class CollisionExtension
    {
        public static bool LineIntersectLine(Vector3 pA1, Vector3 pA2, Vector3 pB1, Vector3 pB2, out Vector3 intersection)
        {
            Ray rA = new Ray(pA1, Vector3.Normalize(pA2 - pA1));
            Ray rB = new Ray(pB1, Vector3.Normalize(pB2 - pB1)); 
       
            //NOTE: When the two rays are coincident, the sharpdx library returns
            //true, and returns an intersection of Vector3.Zero. 
            //This is to ambiguous for our purposes.
            //As a rule in this engine, neither conicident rays nor lines are considered
            //to be intersecting,  since we cannot define a single point of intersection.    
                   
            //if (rA.Intersects(ref rB, out intersection) && !AreRaysCoincident(rA, rB))
            if (RayIntersectsRay(ref rA, ref rB, out intersection) == RayRayIntersection.Intersect)
            {               
                float dA = Vector3.DistanceSquared(pA1, pA2);
                float dB = Vector3.DistanceSquared(pB1, pB2);
                float dAI = Vector3.DistanceSquared(pA1, intersection);
                float dBI = Vector3.DistanceSquared(pB1, intersection);
                if (dAI <= dA && dBI <= dB)
                    return true;
            }
            intersection = Vector3.Zero;
            return false;
        }       

        public static RayRayIntersection RayIntersectsRay(ref Ray ray1, ref Ray ray2, out Vector3 point) 
        { 
            //Source: Real-Time Rendering, Third Edition 
            //Reference: Page 780  
             Vector3 cross; 
 
             Vector3.Cross(ref ray1.Direction, ref ray2.Direction, out cross); 
             float denominator = cross.Length(); 
 
             //Lines are parallel. 
             if (MathUtil.IsZero(denominator)) 
             { 
                 //Lines are parallel and on top of each other. 
                 if (MathUtil.NearEqual(ray2.Position.X, ray1.Position.X) && 
                     MathUtil.NearEqual(ray2.Position.Y, ray1.Position.Y) && 
                     MathUtil.NearEqual(ray2.Position.Z, ray1.Position.Z)) 
                 { 
                     point = Vector3.Zero; 
                     return RayRayIntersection.Overlap; 
                 } 
             }  
 
             denominator = denominator * denominator;  
 
             //3x3 matrix for the first ray. 
             float m11 = ray2.Position.X - ray1.Position.X; 
             float m12 = ray2.Position.Y - ray1.Position.Y; 
             float m13 = ray2.Position.Z - ray1.Position.Z; 
             float m21 = ray2.Direction.X; 
             float m22 = ray2.Direction.Y; 
             float m23 = ray2.Direction.Z; 
             float m31 = cross.X; 
             float m32 = cross.Y; 
             float m33 = cross.Z;  
 
             //Determinant of first matrix. 
             float dets = 
                 m11 * m22 * m33 + 
                 m12 * m23 * m31 + 
                 m13 * m21 * m32 - 
                 m11 * m23 * m32 - 
                 m12 * m21 * m33 - 
                 m13 * m22 * m31;  
 
             //3x3 matrix for the second ray. 
             m21 = ray1.Direction.X; 
             m22 = ray1.Direction.Y; 
             m23 = ray1.Direction.Z;  
 
             //Determinant of the second matrix. 
             float dett = 
                 m11 * m22 * m33 + 
                 m12 * m23 * m31 + 
                 m13 * m21 * m32 - 
                 m11 * m23 * m32 - 
                 m12 * m21 * m33 - 
                 m13 * m22 * m31;  
 
             //t values of the point of intersection. 
             float s = dets / denominator; 
             float t = dett / denominator;

             if (float.IsNaN(s) || float.IsNaN(t))
             {
                 point = Vector3.Zero;
                 return RayRayIntersection.None;
             }

             //The points of intersection. 
             Vector3 point1 = ray1.Position + (Math.Max(s, 0) * ray1.Direction); 
             Vector3 point2 = ray2.Position + (Math.Max(t, 0) * ray2.Direction);  
 
             //If the points are not equal, no intersection has occurred. 
             if (!MathUtil.NearEqual(point2.X, point1.X) || 
                 !MathUtil.NearEqual(point2.Y, point1.Y) || 
                 !MathUtil.NearEqual(point2.Z, point1.Z)) 
             { 
                 point = Vector3.Zero; 
                 return RayRayIntersection.None; 
             }  
 
             point = point1;
             return RayRayIntersection.Intersect;
         } 
    }

    public static class ResourceViewExtension
    {
        public static Size2 GetTexture2DSize(this ResourceView view)
        {          
            return new Size2(view.GetTextureDescription().Width, view.GetTextureDescription().Height);
        }
    }

    public enum RayRayIntersection
    {
        None,
        Intersect,
        Overlap
    }


}
