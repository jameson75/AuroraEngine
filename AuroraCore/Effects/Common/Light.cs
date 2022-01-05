using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public abstract class Light
    {
        public Color Diffuse { get; set; }        
    }

    public class PointLight : Light, ITransformable 
    {
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
    }

    public class DirectionalLight : Light
    {
        public Vector3 Direction { get; set; }
    }
}
