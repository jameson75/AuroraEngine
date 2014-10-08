using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////


namespace CipherPark.AngelJacket.Core.Effects
{
    public class Light
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
