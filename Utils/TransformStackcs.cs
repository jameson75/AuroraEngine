using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;

public class TransformStack
{
    private List<Transform> _innerList = new List<Transform>();

    public Transform Transform
    {
        get
        {
            Transform t = Transform.Identity;
            foreach (Transform m in _innerList)
            {
                t.Rotation *= m.Rotation;
                t.Translation += m.Translation;
            }
            return t;
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
namespace CipherPark.AngelJacket.Core.Utils
{
    class TransformStackcs
    {
    }
}
