﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.Aurora.Core.Toolkit
{
    static class MarshalHelper
    {
        public static IntPtr StructuresToPtr<T>(IEnumerable<T> structures, int length) where T : struct
        {
            int sizeOfTypeT = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(sizeOfTypeT * length);            
            int i = 0;
            foreach (T structure in structures)
            {
                IntPtr cursor = IntPtr.Add(ptr, i * sizeOfTypeT);
                Marshal.StructureToPtr(structure, cursor, false);
                i++;
            }
            return ptr;
        }

        public static T[] PtrToStructures<T>(IntPtr ptr, int length) where T : struct
        {
            T[] structures = new T[length];
            int sizeofTypeT = Marshal.SizeOf(typeof(T));
            for (int i = 0; i < length; i++)
            {
                IntPtr cursor = IntPtr.Add(ptr, i * sizeofTypeT);
                structures[i] = (T)Marshal.PtrToStructure(cursor, typeof(T));
            }
            return structures;
        }

        public static void CopyToDataPointer<T>(T[] source, IntPtr dataPointer, int destinationOffset, int sourceOffset, int sourceLength) where T : struct
        {
            var sourceBytes = ToByteArray(source);
            IntPtr destinationPointer = IntPtr.Add(dataPointer, Marshal.SizeOf(typeof(T)) * destinationOffset);
            Marshal.Copy(sourceBytes,
                         sourceOffset * Marshal.SizeOf(typeof(T)),
                         destinationPointer,
                         sourceLength * Marshal.SizeOf(typeof(T)));
        }

        private static byte[] ToByteArray<T>(T[] source) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                byte[] destination = new byte[source.Length * Marshal.SizeOf(typeof(T))];
                Marshal.Copy(pointer, destination, 0, destination.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        private static T[] FromByteArray<T>(byte[] source) where T : struct
        {
            T[] destination = new T[source.Length / Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                Marshal.Copy(source, 0, pointer, source.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }
    }

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
            _zero = new XMFLOAT2(0, 0);
            _unit = new XMFLOAT2(1, 1);
        }

        private static XMFLOAT2 _zero;
        private static XMFLOAT2 _unit;

        public static XMFLOAT2 Zero { get { return _zero; } }
        public static XMFLOAT2 Unit { get { return _unit; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XMFLOAT3
    {
        public float X;
        public float Y;
        public float Z;

        public XMFLOAT3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public XMFLOAT3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        static XMFLOAT3()
        {
            _zero = new XMFLOAT3(0, 0, 0);
            _unit = new XMFLOAT3(1, 1, 1);
        }

        private static XMFLOAT3 _zero;
        private static XMFLOAT3 _unit;

        public static XMFLOAT3 Zero { get { return _zero; } }
        public static XMFLOAT3 Unit { get { return _unit; } }
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
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public WIN32_RECT(int l, int t, int r, int b)
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
