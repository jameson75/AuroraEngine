using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
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


}
