using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CipherPark.AngelJacket.Core.Utils.Toolkit
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
                IntPtr cursor = new IntPtr(ptr.ToInt64() + (i * sizeOfTypeT));
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
                IntPtr cursor = new IntPtr(ptr.ToInt64() + (i * sizeofTypeT));
                Marshal.PtrToStructure(ptr, structures[i]);
            }
            return structures;
        }
    }
}
