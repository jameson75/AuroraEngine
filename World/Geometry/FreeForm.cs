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
    /// <summary>
    /// 
    /// </summary>
    public class Path
    {
        private List<PathNode> _nodes = new List<PathNode>();

        private List<float> _approximateSegmentLengths = null;

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
        /// Generates and stores the linear-approximate length of each curve segment.
        /// </summary>
        /// <param name="nSamplesPerSegment"></param>
        /// <returns></returns>
        public void GenerateLinearApproximation(int nSamplesPerSegment = 16)
        {
            if (nSamplesPerSegment <= 0)
                throw new ArgumentOutOfRangeException("nSamplesPerSegment must be a positive, non-zero integer");            
            
            _approximateSegmentLengths = new List<float>();

            if (SmoothingEnabled)
            {               
                float stepSize = 1.0f / nSamplesPerSegment;
                for (int i = 1; i < _nodes.Count; i++)
                {                   
                    PathNode n1 = _nodes[i - 1];
                    PathNode n2 = _nodes[i];
                    PathNode n0 = (n1 != _nodes.First()) ? _nodes[i - 2] : n1;
                    PathNode n3 = (n2 != _nodes.Last()) ? _nodes[i + 1] : n2;
                    Vector3 p0 = n1.Transform.Translation;
                    float step = stepSize;                   
                    float totalLength = 0;
                    for (int j = 0; j < nSamplesPerSegment; j++)
                    {
                        //**************************************************************
                        //NOTE: Important quirk for future reference. I've found that
                        //Vector3.CatmullRom does not step evenly for the first
                        //and last segments of a piece-wise curve. 
                        //**************************************************************
                        Vector3 p = Vector3.CatmullRom(n0.Transform.Translation,
                                                       n1.Transform.Translation,
                                                       n2.Transform.Translation,
                                                       n3.Transform.Translation, step);
                        totalLength += Vector3.Distance(p0, p);
                        System.Diagnostics.Trace.WriteLine(Vector3.Distance(p0, p));
                        step += stepSize;
                        p0 = p;
                    }
                    System.Diagnostics.Trace.WriteLine("-------");
                    _approximateSegmentLengths.Add(totalLength);
                }               
            }
            else
            {
                for (int i = 1; i < _nodes.Count; i++)
                {
                    _approximateSegmentLengths.Add(Vector3.Distance(_nodes[i-1].Transform .Translation,
                                                                    _nodes[i].Transform.Translation));
                }          
            }           
        }

        /// <summary>
        /// Gets the interpolated node at the specifed distance along this path.
        /// </summary>
        /// <remarks>
        /// The path node returned is always the result of an interpolation.
        /// Therefore, the Nodes property of this Path will never contain the
        /// PathNode instance returned by this method.
        /// </remarks>
        /// <param name="d"></param>
        /// <param name="linearApproximation"></param>
        /// <returns></returns>
        public PathNode GetNodeAtDistance(float d)
        {
            PathNode pathNode = null;

            if (_approximateSegmentLengths == null)
                GenerateLinearApproximation();

            //TODO: Handle looped path.
            //-------------------------
            
            if (d < 0 || d > _approximateSegmentLengths.Sum())
                return null;
            
            float accumLength = 0;
            float pct = 0;           

            for (int i = 0; i < _approximateSegmentLengths.Count; i++)
            {
                float delta = d - accumLength;
                if (delta <= _approximateSegmentLengths[i])
                {
                    pct = delta / _approximateSegmentLengths[i];                   
                    PathNode n1 = _nodes[i];
                    PathNode n2 = _nodes[i + 1];
                    PathNode n0 = (n1 != _nodes.First()) ? _nodes[i - 1] : n1;
                    PathNode n3 = (n2 != _nodes.Last()) ? _nodes[i + 2] : n2;
                    Vector3 vTranslation;
                    if (SmoothingEnabled)
                    {
                        vTranslation = Vector3.CatmullRom(n0.Transform.Translation,
                                                          n1.Transform.Translation,
                                                          n2.Transform.Translation,
                                                          n3.Transform.Translation,
                                                          pct);
                    }
                    else                                                
                        vTranslation = Vector3.Lerp(n1.Transform.Translation, 
                                                    n2.Transform.Translation,
                                                    pct);
                    Quaternion qRotation = Quaternion.Lerp(n1.Transform.Rotation, n2.Transform.Rotation, pct);
                    pathNode = new PathNode() { Transform = new Transform(qRotation, vTranslation) };
                    break;                    
                }                
                accumLength += _approximateSegmentLengths[i];
            }

            return pathNode;
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
            Vector3 renderedCellDimension = gridForm.CalculateRenderedCellDimensions();
            Vector3 renderedGridDimension = gridForm.CalculateRenderedDimensions();
            Vector3 gridDimension = gridForm.Dimensions;
            FreeForm newForm = new FreeForm(gridForm.Game);
            newForm.ElementMesh = gridForm.ElementMesh;
            newForm.ElementEffect = gridForm.ElementEffect;
            List<Particle> elements = newForm.CreateElements(gridForm.Particles.Count);
            for (int i = 0; i < gridDimension.Z; i++)
            {
                float distanceZ = renderedCellDimension.Z * i + renderedCellDimension.Z * 0.5f;
                PathNode node = path.GetNodeAtDistance(distanceZ);                
                for (int j = 0; j < gridDimension.X; j++)
                {
                    float xOffset = -0.5f * renderedGridDimension.X + j * renderedCellDimension.X;
                    elements[i * (int)gridDimension.Z + j].Transform = new Transform(node.Transform.ToMatrix() * Matrix.Translation(new Vector3(xOffset, 0, 0)));
                }
            }
            newForm.AddElements(elements);
            return newForm;
        }
    }
}
