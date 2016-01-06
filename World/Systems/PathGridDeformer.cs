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
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry.Modifiers
{
 
    /*
    public abstract class Deformer
    {
        public abstract void Deform(Form form);      
    }
    */

    public class PathGridDeformer : ParticleSolver
    {
        private bool _deformed;

        public Path Path { get; set; }
        
        public float GridRoll { get; set; }     
        
        public double StartDistance { get; set; }   

        public PathGridDeformer()            
        { }

        public override void Reset()
        { }

        public override void Step(GameTime time, ParticleSystem system)
        {            
            GridFormPattern gridForm = (GridFormPattern)system;
            Vector3 renderedGridSize = gridForm.CalculateRenderedSize();
            Vector3 gridDimension = gridForm.Dimensions;
            //ReadOnlyCollection<FormNode> elements = gridForm.Nodes;
            ReadOnlyCollection<Particle> elements = gridForm.Particles;
            float rowOffset = -0.5f * renderedGridSize.X;
            float heightOffset = -0.5f * renderedGridSize.Y;
            for (int i = 0; i < gridDimension.Z; i++)
            {
                int zIndex = i;
                float zOrigin = (zIndex * ((gridForm.CellPadding.Z * 2) + gridForm.ElementMesh.BoundingBox.GetLengthZ())) + (gridForm.CellPadding.Z + (gridForm.ElementMesh.BoundingBox.GetLengthZ() * 0.5f));
                PathNode node = Path.EvaluateNodeAtDistance(zOrigin);
                for (int j = 0; j < gridDimension.X; j++)
                {
                    int k = i * (int)gridDimension.X + j;
                    int xIndex = j;
                    int yIndex = k / ((int)gridDimension.X * (int)gridDimension.Z);
                    float yOrigin = heightOffset + (yIndex * ((gridForm.CellPadding.Y * 2) + gridForm.ElementMesh.BoundingBox.GetLengthY())) + (gridForm.CellPadding.Y + (gridForm.ElementMesh.BoundingBox.GetLengthY() * 0.5f));
                    float xOrigin = rowOffset + (xIndex * ((gridForm.CellPadding.X * 2) + gridForm.ElementMesh.BoundingBox.GetLengthX())) + (gridForm.CellPadding.X + (gridForm.ElementMesh.BoundingBox.GetLengthX() * 0.5f));
                    elements[k].Transform = new Transform(Matrix.Translation(new Vector3(xOrigin, yOrigin, 0)) * node.Transform.ToMatrix());
                }
            }
            IsComplete = true;
        }
    }   
}
