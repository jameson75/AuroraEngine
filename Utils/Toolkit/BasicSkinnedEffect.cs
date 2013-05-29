using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Utils.Toolkit
{
    public class BasicSkinnedEffect
    {
        private IntPtr _nativeObject = IntPtr.Zero;

        public IntPtr NativeObject
        {
            get { return _nativeObject; }
        }

        private static class UnsafeNativeMethods
        {
            
        }
    }
}
