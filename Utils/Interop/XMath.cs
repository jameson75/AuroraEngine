using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;

namespace CipherPark.AngelJacket.Core.Utils.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XMFLOAT2
    {
        public float X;
        public float Y;
        
        public XMFLOAT2(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public XMFLOAT2(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }
        
        static XMFLOAT2()
        {
            _zero = new XMFLOAT2(0,0);
            _unit = new XMFLOAT2(1,1);
        }
        
        private static XMFLOAT2 _zero;
        private static XMFLOAT2 _unit;

        public static XMFLOAT2 Zero { get { return _zero; } }
        public static XMFLOAT2 Unit { get { return _unit; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XVECTOR4
    {
        public float C1;
        public float C2;
        public float C3;
        public float C4;

        public XVECTOR4(float c1, float c2, float c3, float c4)
        {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
        }

        public XVECTOR4(Vector4 v)
        {
            C1 = v.X;
            C2 = v.Y;
            C3 = v.Z;
            C4 = v.W;
        }

        public XVECTOR4(Color4 c)
        {
            C1 = c.Red;
            C2 = c.Green;
            C3 = c.Blue;
            C4 = c.Alpha;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WIN32_RECT
    {
        long Left;
        long Top;
        long Right;
        long Bottom;

        public WIN32_RECT(long l, long t, long r, long b)
        {
            Left = l;
            Top = t;
            Right = r;
            Bottom = b;
        }

        public WIN32_RECT(Rectangle r)
        {
            Left = r.Left;
            Top = r.Top;
            Right = r.Right;
            Bottom = r.Bottom;
        }
    }
}