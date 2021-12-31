using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils
{
    public class MatrixStack
    {
        private List<Matrix> _innerList = new List<Matrix>();

        public Matrix Transform
        {
            get
            {
                Matrix t = Matrix.Identity;
                foreach (Matrix m in _innerList)
                    t *= m;
                return t;
            }
        }

        public Matrix ReverseTransform
        {
            get
            {
                Matrix t = Matrix.Identity;
                for (int i = _innerList.Count - 1; i >= 0; i--)
                    t *= _innerList[i];
                return t;
            }         
        }

        public void Push(Matrix m)
        {
            _innerList.Add(m);
        }

        public Matrix Pop()
        {
            if (_innerList.Count == 0)
                throw new InvalidOperationException("Matrix stack is empty.");

            int lastIndex = _innerList.Count - 1;
            Matrix m = _innerList[lastIndex];
            _innerList.RemoveAt(lastIndex);
            return m;
        }

        public Matrix Top
        {
            get
            {
                if (_innerList.Count == 0)
                    return Matrix.Identity;
                else
                    return _innerList.Last();
            }
        }
    }
}
