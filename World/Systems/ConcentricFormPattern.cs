using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Systems;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class ConcentricFormPattern : FormPattern
    {
        float _radius = 0;
        int _steps = 0;       

        public ConcentricFormPattern()
        { }     

        public ConcentricFormPattern(float radius, int steps)
        {
            _radius = radius;
            _steps = steps;
        }

        public float Radius
        {
            get { return _radius; }
            set {
                _radius = value;               
            }
        }

        public int Steps
        {
            get { return _steps; }
            set {
                _steps = value;               
            }
        }

        public SurfaceEffect ElementEffect { get; set; }
        
        public Mesh ElementMesh { get; set; }

        public override void Initialize(Form form)
        {
            form.EmitElements(_steps);
            this.Update(form);
        }

        public override void Update(Form form)
        {       
            float step = form.ElementCount != 0 ? MathUtil.TwoPi / form.ElementCount : 0;
            float angle = MathUtil.PiOverTwo;
            //foreach (Particle element in Particles)
            foreach(Particle element in form.Particles)
            {
                Matrix rotation = Matrix.RotationY(angle);
                Matrix translation = Matrix.Translation(new Vector3(0, 0, _radius));
                element.Transform = new Animation.Transform(translation * rotation);
                angle += step;
            } 
        }      
    }
}

