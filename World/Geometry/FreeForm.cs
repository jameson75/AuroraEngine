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
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class Path
    {
        public PathNode GetNodeAtDistance(float d)
        {
            throw new NotImplementedException();
        }
    }

    public class PathNode
    {
        public PathNode()
        {
            Transform = Transform.Identity;
        }

        public Transform Transform { get; set; }
    }

    public class FreeForm : Form
    {
        public FreeForm(IGameApp game)
            : base(game)
        { }

        public static FreeForm CreateFrom(GridForm gridForm, Path path)
        {
            Vector3 gridCellDimension = gridForm.CalculateRenderedCellDimensions();
            Vector3 gridDimension = gridForm.CalculateRenderedDimensions();
            FreeForm newForm = new FreeForm(gridForm.Game);
            newForm.ElementMesh = gridForm.ElementMesh;
            newForm.ElementEffect = gridForm.ElementEffect;
            List<Particle> elements = newForm.CreateElements(gridForm.Particles.Count);
            for (int i = 0; i < gridDimension.Z; i++)
            {
                float distanceZ = gridCellDimension.Z * i + gridCellDimension.Z * 0.5f;
                PathNode node = path.GetNodeAtDistance(distanceZ);                
                for (int j = 0; j < gridDimension.X; i++)
                {
                    float xOffset = -0.5f * gridDimension.X + j * gridCellDimension.X;
                    elements[i * (int)gridDimension.Z + j].Transform = new Transform(node.Transform.ToMatrix() * Matrix.Translation(new Vector3(xOffset, 0, 0)));
                }
            }
            newForm.AddElements(elements);
            return newForm;
        }
    }
}
