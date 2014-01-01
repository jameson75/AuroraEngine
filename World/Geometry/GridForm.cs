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
using CipherPark.AngelJacket.Core.Kinetics;


namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class GridForm : Form
    {
        private Vector3 _dimensions = Vector3.Zero;
        private Vector3 _cellSpacining = Vector3.Zero;       

        public GridForm(IGameApp game)
            : base(game)
        { }

        public GridForm(IGameApp game, Vector3 dimensions, Vector3 cellSpacing)
            : base(game)
        {
            _dimensions = dimensions;
            _cellSpacining = cellSpacing;
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

        public Vector3 CellSpacing
        {
            get { return _cellSpacining; }
            set
            {
                _cellSpacining = value;
                //GenerateElements();
            }
        }

        protected override void OnLayoutChanged()
        {
            int i = 0;
            foreach (Particle element in Particles)
            {
                if (ElementMesh != null)
                {
                    int xIndex = i % (int)Dimensions.X;
                    int zIndex = i / (int)Dimensions.X;
                    int yIndex = i / ((int)Dimensions.X * (int)Dimensions.Z);
                    Vector3 elementOrigin = new Vector3();
                    elementOrigin.X = (xIndex * ((CellSpacing.X * 2) + ElementMesh.BoundingBox.GetLengthX())) + (CellSpacing.X + (ElementMesh.BoundingBox.GetLengthX() * 0.5f));
                    elementOrigin.Y = (yIndex * ((CellSpacing.Y * 2) + ElementMesh.BoundingBox.GetLengthY())) + (CellSpacing.Y + (ElementMesh.BoundingBox.GetLengthY() * 0.5f));
                    elementOrigin.Z = (zIndex * ((CellSpacing.Z * 2) + ElementMesh.BoundingBox.GetLengthZ())) + (CellSpacing.Z + (ElementMesh.BoundingBox.GetLengthZ() * 0.5f));
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
            //Elements.Clear();
            //int elementCount = (int)(Dimensions.X * Dimensions.Y * Dimensions.Z);
            //for (int i = 0; i < elementCount; i++)
            //    Elements.Add(new FormElement(this));
            ClearElements();
            int elementCount = (int)(Dimensions.X * Dimensions.Y * Dimensions.Z);
            List<Particle> elements = CreateElements(elementCount);
            AddElements(elements);
            OnLayoutChanged();
        }

        public Vector3 CalculateRenderedDimensions()
        {
            if (ElementMesh == null)
                return Vector3.Zero;
            else
            {
                return new Vector3((ElementMesh.BoundingBox.GetLengthX() + CellSpacing.X * 2) * Dimensions.X,
                                   (ElementMesh.BoundingBox.GetLengthY() + CellSpacing.Y * 2) * Dimensions.Y,
                                   (ElementMesh.BoundingBox.GetLengthZ() + CellSpacing.Z * 2) * Dimensions.Z);
            }
        }
    }
}
