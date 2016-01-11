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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class GridForm : Form
    {
        private GridFormPattern _pattern = null;

        public GridForm(IGameApp game, Vector3 dimensions, Vector3 cellPadding) : base(game)
        {
            GridFormPattern pattern = new GridFormPattern(dimensions, cellPadding);
            pattern.Initialize(this);
            _pattern = pattern;
        }

        public override FormPattern Pattern
        {
            get
            {
                return _pattern;
            }
        }

        public override BoundingBox BoundingBox
        {
            get
            {
                Vector3 renderedSize = CalculateRenderedSize();
                Vector3 min = new Vector3(0, 0, 0);              
                Vector3 max = new Vector3(renderedSize.X, renderedSize.Y, renderedSize.Z);
                return new BoundingBox(min, max);
            }
        }       

        public Vector3 Dimensions
        {
            get { return _pattern.Dimensions; }
        }

        public Vector3 Cellpadding
        {
            get { return _pattern.CellPadding; }
        }

        protected override void OnMeshChanged()
        {
            _pattern.Update(this);
            base.OnMeshChanged();
        }   

        public Vector3 CalculateRenderedSize()
        {
            return _pattern.CalculateRenderedSize(this);
        }

        public Vector3 CalculateRenderedCellSize()
        {
            return _pattern.CalculateRenderedCellSize(this);
        }        

        public void Deform(Path path)
        {
            _pattern.Deform(this, path);
        }

        public List<Particle> GetRowElements(int index)
        {
            return _pattern.GetRowElements(this, index);
        }              
    }
}
