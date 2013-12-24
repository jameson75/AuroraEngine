﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Utils;

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

        protected void OnLayoutChanged()
        {            
            float step = Elements.Count != 0 ? MathUtil.TwoPi / Elements.Count : 0;
            float angle = MathUtil.PiOverTwo;
            foreach (FormElement element in Elements)
            {
                Matrix rotation = Matrix.RotationY(angle);
                Matrix translation = Matrix.Translation(new Vector3(0, 0, _radius));
                element.Transform = translation * rotation;
                angle += step;
            }
        }

        private void GenerateElements()
        {
            Elements.Clear();
            for (int i = 0; i < _steps; i++)
                Elements.Add(new FormElement(this));
            OnLayoutChanged();
        }
    }
}
