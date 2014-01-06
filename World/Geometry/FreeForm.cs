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
    internal class PathNodeSamples
    {
        public PathNode Node { get; set; }
        public Vector3[] Points { get; set; }
        public float TotalDistance { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Path
    {
        private List<PathNode> _nodes = new List<PathNode>();

        private PathNodeSamples[] _linearApproximation = null;

        /// <summary>
        /// 
        /// </summary>
        public Path()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public List<PathNode> Nodes { get { return _nodes; } }

        /// <summary>
        /// 
        /// </summary>
        public bool SmoothingEnabled { get; set; }

        /// <summary>
        /// Creates a linear approximation of the path in its current state.
        /// </summary>
        /// <param name="nSamplesPerSegment"></param>
        /// <returns></returns>
        public void GenerateLinearApproximation(int nSamplesPerSegment = 16)
        {
            if (nSamplesPerSegment <= 0)
                throw new ArgumentOutOfRangeException("nSamplesPerSegment must be a positive, non-zero integer");

            List<PathNodeSamples> points = new List<PathNodeSamples>();
            
            if (SmoothingEnabled)
            {               
                float stepSize = 100.0f / nSamplesPerSegment;
                for (int i = 1; i < _nodes.Count; i++)
                {
                    PathNode n1 = _nodes[i - 1];
                    PathNode n2 = _nodes[i];
                    PathNode n0 = (n1 != _nodes.First()) ? _nodes[i - 2] : n1;
                    PathNode n3 = (n2 != _nodes.Last()) ? _nodes[i + 1] : n2;
                    float step = 0;
                    for (int j = 0; j < nSamplesPerSegment; j++)
                    {
                        Vector3 p = Vector3.CatmullRom(n0.Transform.Translation,
                                           n1.Transform.Translation,
                                           n2.Transform.Translation,
                                           n3.Transform.Translation, step);
                        step += stepSize;
                    }
                }

                //The loop above does not create a point for the very last node. We add it here.           
                if (_nodes.Count > 0)
                    points.Add(_nodes.Last().Transform.Translation);
            }
            else
                for (int i = 0; i < _nodes.Count; i++)
                    points.Add(_nodes[i].Transform.Translation);

            _linearApproximation = points.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="linearApproximation"></param>
        /// <returns></returns>
        public PathNode GetNodeAtDistance(float d)
        {
            if (SmoothingEnabled)
            {
                if(_linearApproximation == null)
                    GenerateLinearApproximation();

                if (d >= 0)
                {           
                    int index = -1;
                    for (int i = 0; i < _linearApproximation.Length; i++)
                    {
                        if (d >= _linearApproximation[i].TotalDistance)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index == -1)
                        _linearApproximation

                }
            }

            //Quaternion r = Quaternion.Lerp(frameVal0.Rotation, frameVal1.Rotation, pctStep);
            //Vector3 x = Vector3.Zero;
            //if (SmoothingEnabled)
            //{
            //    AnimationKeyFrame fa = GetPreviousKeyFrame(f0);
            //    if (fa == null)
            //        fa = f0;
            //    AnimationKeyFrame fb = GetNextKeyFrame(f1);
            //    if (fb == null)
            //        fb = f1;
            //    Transform frameVala = (fa.Value != null) ? (Transform)fa.Value : Transform.Identity;
            //    Transform frameValb = (fb.Value != null) ? (Transform)fb.Value : Transform.Identity;
            //    x = Vector3.CatmullRom(frameVala.Translation, frameVal0.Translation, frameVal1.Translation, frameValb.Translation, pctStep);
            //}
            //else
            //    x = Vector3.Lerp(frameVal0.Translation, frameVal1.Translation, pctStep);

            //return new Transform { Rotation = r, Translation = x };
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
