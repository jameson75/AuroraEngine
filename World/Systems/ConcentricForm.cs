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

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class ConcentricForm : Form
    {
        float _radius = 0;
        int _steps = 0;

        public ConcentricForm(IGameApp game)
            : base(game)
        { }

        public ConcentricForm(IGameApp game, float radius, int steps) 
            : base(game)
        {
            _radius = radius;
            _steps = steps;
            GenerateElements();
        }

        public float Radius
        {
            get { return _radius; }
            set {
                _radius = value;
                OnLayoutChanged();
            }
        }

        public int Steps
        {
            get { return _steps; }
            set {
                _steps = value;
                GenerateElements();
            }
        }

        protected override void OnLayoutChanged()
        {            
            float step = ElementCount != 0 ? MathUtil.TwoPi / ElementCount : 0;
            float angle = MathUtil.PiOverTwo;
            //foreach (Particle element in Particles)
            foreach(Particle element in Particles)
            {
                Matrix rotation = Matrix.RotationY(angle);
                Matrix translation = Matrix.Translation(new Vector3(0, 0, _radius));
                element.Transform = new Animation.Transform(translation * rotation);
                angle += step;
            }
        }

        private void GenerateElements()
        {        
            ClearElements();          
            //AddElements(CreateElements(_steps));
            EmitElements(_steps);
            OnLayoutChanged();
        }
    }
}

