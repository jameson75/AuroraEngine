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
using CipherPark.KillScript.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World.Geometry
{
    public abstract class FormPattern
    {
        public abstract void Initialize(Form form);
        public abstract void Update(Form form);
    }

    public class GridFormPattern : FormPattern
    {
        private Vector3 _dimensions = Vector3.Zero;
        private Vector3 _cellPadding = Vector3.Zero;     
       

        public GridFormPattern()
        { }        

        public GridFormPattern(Vector3 dimensions, Vector3 cellSpacing) 
        {
            _dimensions = dimensions;
            _cellPadding = cellSpacing;            
        }
        
        public Vector3 Dimensions
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;              
            }
        }

        public Vector3 CellPadding
        {
            get { return _cellPadding; }
            set
            {
                _cellPadding = value;               
            }
        }
   
        public BoundingBox CalculateBoundingBox(Form form)
        {
            Vector3 renderedSize = CalculateRenderedSize(form);
            Vector3 min = new Vector3(0, 0, 0);
            Vector3 max = new Vector3(renderedSize.X, renderedSize.Y, renderedSize.Z);
            return new BoundingBox(min, max);
        }

        public override void Initialize(Form form)
        {
            //Emit elements (particles) into the particle system.           
            int elementCount = (int)(Dimensions.X * Dimensions.Y * Dimensions.Z);
            form.EmitElements(elementCount);            
        }

        public override void Update(Form form)
        {  
            //**************************************************************************
            //NOTES: 
            //The first element is the {left-most, top-most, front-most} element.
            //The last element is the {righ-most, bottom-most, back-most} element.
            //So... First Row is the front/near side, Last Row is the back/far side.
            //**************************************************************************
            int i = 0;            
            foreach(Particle element in form.Particles)
            {
                if (form.ElementMesh != null)
                {
                    int xIndex = i % (int)Dimensions.X;
                    int zIndex = i / (int)Dimensions.X;
                    int yIndex = 0; // i / ((int)Dimensions.X * (int)Dimensions.Z);
                    Vector3 elementOrigin = new Vector3();
                    elementOrigin.X = (xIndex * ((CellPadding.X * 2) + form.ElementMesh.BoundingBox.GetLengthX())) + (CellPadding.X + (form.ElementMesh.BoundingBox.GetLengthX() * 0.5f));
                    elementOrigin.Y = (yIndex * ((CellPadding.Y * 2) + form.ElementMesh.BoundingBox.GetLengthY())) + (CellPadding.Y + (form.ElementMesh.BoundingBox.GetLengthY() * 0.5f));
                    elementOrigin.Z = (zIndex * ((CellPadding.Z * 2) + form.ElementMesh.BoundingBox.GetLengthZ())) + (CellPadding.Z + (form.ElementMesh.BoundingBox.GetLengthZ() * 0.5f));
                    element.Transform = new Animation.Transform(Matrix.Translation(elementOrigin));
                }
                else
                    element.Transform = Animation.Transform.Identity;
                i++;
            }                            
        }    
        
        public void Deform(Form form, Path path)
        {            
            Vector3 renderedGridSize = this.CalculateRenderedSize(form);
            Vector3 gridDimension = this.Dimensions;            
            ReadOnlyCollection<Particle> elements = form.Particles;
            float rowOffset = -0.5f * renderedGridSize.X;
            float heightOffset = -0.5f * renderedGridSize.Y;
            for (int i = 0; i < gridDimension.Z; i++)
            {
                int zIndex = i;
                float zOrigin = (zIndex * ((this.CellPadding.Z * 2) + form.ElementMesh.BoundingBox.GetLengthZ())) + (this.CellPadding.Z + (form.ElementMesh.BoundingBox.GetLengthZ() * 0.5f));
                PathNode node = path.EvaluateNodeAtDistance(zOrigin);
                for (int j = 0; j < gridDimension.X; j++)
                {
                    int k = i * (int)gridDimension.X + j;
                    int xIndex = j;
                    int yIndex = k / ((int)gridDimension.X * (int)gridDimension.Z);
                    float yOrigin = heightOffset + (yIndex * ((this.CellPadding.Y * 2) + form.ElementMesh.BoundingBox.GetLengthY())) + (this.CellPadding.Y + (form.ElementMesh.BoundingBox.GetLengthY() * 0.5f));
                    float xOrigin = rowOffset + (xIndex * ((this.CellPadding.X * 2) + form.ElementMesh.BoundingBox.GetLengthX())) + (this.CellPadding.X + (form.ElementMesh.BoundingBox.GetLengthX() * 0.5f));
                    elements[k].Transform = new Animation.Transform(Matrix.Translation(new Vector3(xOrigin, yOrigin, 0)) * node.Transform.ToMatrix());
                }
            }            
        }
      
        public Vector3 CalculateRenderedSize(Form form)
        {
            if (form.ElementMesh == null)
                return Vector3.Zero;
            else
            {
                return new Vector3((form.ElementMesh.BoundingBox.GetLengthX() + CellPadding.X * 2) * Dimensions.X,
                                   (form.ElementMesh.BoundingBox.GetLengthY() + CellPadding.Y * 2)/* * Dimensions.Y*/,
                                   (form.ElementMesh.BoundingBox.GetLengthZ() + CellPadding.Z * 2) * Dimensions.Z);
            }
        }

        public Vector3 CalculateRenderedCellSize(Form form)
        {
            if (form.ElementMesh == null)
                return Vector3.Zero;
            else
            {
                return new Vector3((form.ElementMesh.BoundingBox.GetLengthX() + CellPadding.X * 2),
                                   (form.ElementMesh.BoundingBox.GetLengthY() + CellPadding.Y * 2),
                                   (form.ElementMesh.BoundingBox.GetLengthZ() + CellPadding.Z * 2));
            }
        }        

        public List<Particle> GetRowElements(Form form, int index)
        {           
            return form.Particles.Skip(index * (int)Dimensions.X).Take((int)Dimensions.X).ToList();
        }     

        /*
        public void HideRow(Form form, int index)
        {
            GetRowElements(form, index).ForEach(p => p.IsVisible = false);        
        }

        public void ShowRow(Form form, int index)
        {
            GetRowElements(form, index).ForEach(p => p.IsVisible = false);
        }
        */     
    }
}
