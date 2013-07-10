using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

public class TransformStack
{
    private List<Transform> _innerList = new List<Transform>();

    public Transform Transform
    {
        get
        {
            Matrix m = Matrix.Identity;
            foreach (Transform t in _innerList)
                m *= t.ToMatrix();
            return new Transform(m);
        }
    }

    public Transform ReverseTransform
    {
        get
        {
            Matrix m = Matrix.Identity;
            for (int i = _innerList.Count - 1; i < 0; i--)            
                m *= _innerList[i].ToMatrix();
            return new Transform(m);
        }
    }

    public void Push(Transform t)
    {
        _innerList.Add(t);
    }

    public Transform Pop()
    {
        if (_innerList.Count == 0)
            throw new InvalidOperationException("Transform stack is empty.");

        int lastIndex = _innerList.Count - 1;
        Transform m = _innerList[lastIndex];
        _innerList.RemoveAt(lastIndex);
        return m;
    }

    public Transform Top
    {
        get
        {
            if (_innerList.Count == 0)
                return Transform.Identity;
            else
                return _innerList.Last();
        }
    }
}

