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

        protected override void OnLayoutChanged()
        {
            //**************************************************************************
            //NOTES: 
            //The first element is the {left-most, top-most, front-most} element.
            //The last element is the {righ-most, bottom-most, back-most} element.
            //So... First Row is the front/near side, Last Row is the back/far side.
            //**************************************************************************
            int i = 0;
            foreach (Particle element in Particles)
            {
                if (ElementMesh != null)
                {
                    int xIndex = i % (int)Dimensions.X;
                    int zIndex = i / (int)Dimensions.X;
                    int yIndex = i / ((int)Dimensions.X * (int)Dimensions.Z);
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
            ClearElements();
            int elementCount = (int)(Dimensions.X * Dimensions.Y * Dimensions.Z);
            //List<Particle> elements = CreateElements(elementCount);
            EmitElements(elementCount);
            //AddElements(elements);
            OnLayoutChanged();
        }

        public Vector3 CalculateRenderedSize()
        {
            if (ElementMesh == null)
                return Vector3.Zero;
            else
            {
                return new Vector3((ElementMesh.BoundingBox.GetLengthX() + CellPadding.X * 2) * Dimensions.X,
                                   (ElementMesh.BoundingBox.GetLengthY() + CellPadding.Y * 2) * Dimensions.Y,
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

        public List<Particle> AddFrontRow()
        {
            AlterDimensions(Dimensions = new Vector3(Dimensions.X, Dimensions.Y, Dimensions.Z + 1));            
            //******************************************************************************
            //Simply changing the Dimension's Z component results in the appearing 
            //to extend it's back row. To give the illusion of extending the front row, we
            //transform the entire grid forward by the size of a row.            
            //******************************************************************************
            Vector3 newPosition = this.Transform.Translation;
            newPosition.Z += CalculateRenderedCellSize().Z;          
            this.Transform = new Animation.Transform(this.Transform.Rotation, newPosition);
            return GetRow(0);
        }     

        public List<Particle> AddBackRow()
        {            
            AlterDimensions(new Vector3(Dimensions.X, Dimensions.Y, Dimensions.Z + 1));
            return GetRow((int)(Dimensions.Z - 1));
        }

        public List<Particle> RemoveBackRow()
        {
            AlterDimensions(new Vector3(Dimensions.X, Dimensions.Y, Dimensions.Z - 0));
            return GetRow((int)(Dimensions.Z - 1));
        }

        public List<Particle> RemoveFrontRow()
        {
            AlterDimensions(new Vector3(Dimensions.X, Dimensions.Y, Dimensions.Z - 0));
            //******************************************************************************
            //Simply changing the Dimension's Z component results in the appearing 
            //to extend it's back row. To give the illusion of extending the front row, we
            //transform the entire grid backward by the size of a row.            
            //******************************************************************************
            Vector3 newPosition = this.Transform.Translation;
            newPosition.Z -= CalculateRenderedCellSize().Z;
            this.Transform = new Animation.Transform(this.Transform.Rotation, newPosition);    
            return GetRow(0);
        }

        public List<Particle> GetRow(int index)
        {

        }

        private public AlterDimensions(Vector3 vector3)
        {
            _true = 
            OnLayoutChanged();
            throw new NotImplementedException();   
        }     
    }
}
