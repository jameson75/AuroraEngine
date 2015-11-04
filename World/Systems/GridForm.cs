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
        private Vector3 _dimensions = Vector3.Zero;
        private Vector3 _cellPadding = Vector3.Zero;
        private Range? _rowClipRange = null;

        public GridForm(IGameApp game)
            : base(game)
        { }

        public GridForm(IGameApp game, Vector3 dimensions, Vector3 cellSpacing)
            : base(game)
        {
            _dimensions = dimensions;
            _cellPadding = cellSpacing;
            GenerateElements();
        }

        public Vector3 Dimensions
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;
                GenerateElements();
            }
        }

        public Vector3 CellPadding
        {
            get { return _cellPadding; }
            set
            {
                _cellPadding = value;
                OnLayoutChanged();
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

        protected override void OnLayoutChanged()
        {
            //**************************************************************************
            //NOTES: 
            //The first element is the {left-most, top-most, front-most} element.
            //The last element is the {righ-most, bottom-most, back-most} element.
            //So... First Row is the front/near side, Last Row is the back/far side.
            //**************************************************************************
            int i = 0;
            //foreach (Particle element in Particles)
            foreach(Particle element in Particles)
            {
                if (ElementMesh != null)
                {
                    int xIndex = i % (int)Dimensions.X;
                    int zIndex = i / (int)Dimensions.X;
                    int yIndex = 0; // i / ((int)Dimensions.X * (int)Dimensions.Z);
                    Vector3 elementOrigin = new Vector3();
                    elementOrigin.X = (xIndex * ((CellPadding.X * 2) + ElementMesh.BoundingBox.GetLengthX())) + (CellPadding.X + (ElementMesh.BoundingBox.GetLengthX() * 0.5f));
                    elementOrigin.Y = (yIndex * ((CellPadding.Y * 2) + ElementMesh.BoundingBox.GetLengthY())) + (CellPadding.Y + (ElementMesh.BoundingBox.GetLengthY() * 0.5f));
                    elementOrigin.Z = (zIndex * ((CellPadding.Z * 2) + ElementMesh.BoundingBox.GetLengthZ())) + (CellPadding.Z + (ElementMesh.BoundingBox.GetLengthZ() * 0.5f));
                    element.Transform = new Animation.Transform(Matrix.Translation(elementOrigin));
                }
                else
                    element.Transform = Animation.Transform.Identity;
                i++;
            }
        }

        protected override void OnMeshChanged()
        {
            OnLayoutChanged();
            base.OnMeshChanged();
        }

        private void GenerateElements()
        {          
            //Remove existing elements
            ClearElements();
            
            //Emit elements (particles) into the particle system.           
            int elementCount = (int)(Dimensions.X * Dimensions.Y * Dimensions.Z);           
            EmitElements(elementCount);
            
            //Layout particles into rows.                
            OnLayoutChanged();
        }

        public Vector3 CalculateRenderedSize()
        {
            if (ElementMesh == null)
                return Vector3.Zero;
            else
            {
                return new Vector3((ElementMesh.BoundingBox.GetLengthX() + CellPadding.X * 2) * Dimensions.X,
                                   (ElementMesh.BoundingBox.GetLengthY() + CellPadding.Y * 2)/* * Dimensions.Y*/,
                                   (ElementMesh.BoundingBox.GetLengthZ() + CellPadding.Z * 2) * Dimensions.Z);
            }
        }

        public Vector3 CalculateRenderedCellSize()
        {
            if (ElementMesh == null)
                return Vector3.Zero;
            else
            {
                return new Vector3((ElementMesh.BoundingBox.GetLengthX() + CellPadding.X * 2),
                                   (ElementMesh.BoundingBox.GetLengthY() + CellPadding.Y * 2),
                                   (ElementMesh.BoundingBox.GetLengthZ() + CellPadding.Z * 2));
            }
        }        

        public List<Particle> GetRowParticles(int index)
        {           
            return Particles.Skip(index * (int)Dimensions.X).Take((int)Dimensions.X).ToList();
        }

        /*
        public List<FormNode> GetRowNodes(int index)
        {
            return Nodes.Skip(index * (int)Dimensions.X).Take((int)Dimensions.X).ToList();
        }
        */

        public void HideRow(int index)
        {
            GetRowParticles(index).ForEach(p => p.IsVisible = false);        
        }

        public void ShowRow(int index)
        {
            GetRowParticles(index).ForEach(p => p.IsVisible = false);
        }

        public Range? RowClipRange
        {
            get { return _rowClipRange; }
            set
            {
                _rowClipRange = value;
                OnRowClipRangeChanged();
            }
        }

        protected void OnRowClipRangeChanged()
        {
            int i = 0;
            foreach (Particle element in Particles)
            {                              
                int zIndex = i / (int)Dimensions.X;
                if ( _rowClipRange == null ||
                    (_rowClipRange.Value.Min <= zIndex && _rowClipRange.Value.Max >= zIndex))
                    element.IsVisible = true;
                else
                    element.IsVisible = false;
                i++;
            }
        }
    }
}
