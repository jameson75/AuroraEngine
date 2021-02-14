using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Systems;

namespace CipherPark.KillScript.Core.World.Geometry
{
    public class ConcentricForm : Form
    {
        ConcentricFormPattern _pattern = null;

        public ConcentricForm(IGameApp game, float radius, int steps) 
            : base(game)
        {
            _pattern = new ConcentricFormPattern(radius, steps);
            _pattern.Initialize(this);
        }

        public override FormPattern Pattern
        {
            get
            {
                return _pattern;
            }
        }

        public float Radius
        {
            get { return _pattern.Radius; }
        }

        public int Steps
        {
           get { return _pattern.Steps; }
        }

        protected override void OnMeshChanged()
        {
            _pattern.Update(this);
            base.OnMeshChanged();
        }
    }
}

