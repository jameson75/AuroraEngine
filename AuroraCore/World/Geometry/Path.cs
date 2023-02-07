using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
////////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
{
    /// <summary>
    /// 
    /// </summary>
    public class Path
    {
        private List<PathNode> _nodes = new List<PathNode>();
        private List<float> _approximateSegmentLengths = new List<float>();
        private int _lastCalculatedNodeIndex = 0;             
      

        /// <summary>
        /// 
        /// </summary>
        public const int DefaultSamplesPerSegment = 16;                

        /// <summary>
        /// 
        /// </summary>
        public Path()
        {
            SamplesPerSegment = DefaultSamplesPerSegment;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SamplesPerSegment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SmoothingEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float Distance
        {
            get
            {
                if (this.RequiresApproximation)
                    throw new InvalidOperationException("Length not approximated. Must call GenerateLinearApproximation() at least once before calling this method");
                float d = 0.0f;
                for (int i = 0; i < _approximateSegmentLengths.Count; i++)
                    d += _approximateSegmentLengths[i];
                return d;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NodeCount
        {
            get { return _nodes.Count(); }
        }

        /// <summary>
        /// Returns a shallow copy of path nodes in the path in FIFO order.
        /// </summary>
        public List<PathNode> GetNodes() { return _nodes.ToList(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="index"></param>
        public void RemoveNodes(int count, int index = 0)
        {
            _nodes.RemoveRange(index, count);
            _approximateSegmentLengths.RemoveRange(index, count);
            _lastCalculatedNodeIndex = Math.Max(0, _lastCalculatedNodeIndex - count);
            OnPathChanged();
        }      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(PathNode node, bool generateLinearApprox = false)
        {
            AddNodes(new[] { node }, generateLinearApprox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        public void AddNodes(IEnumerable<PathNode> nodes, bool generateLinearApprox = false)
        {
            _nodes.AddRange(nodes);
            if (generateLinearApprox)
                GenerateLinearApproximation();
            OnPathChanged();  
        }

        /// <summary>
        /// Generates and stores the linear-approximate length of each curve segment.
        /// </summary>
        /// <param name="nSamplesPerSegment">Number of samples taken between each node</param>        
        /// <returns></returns>
        public void GenerateLinearApproximation()
        {           
            if (SamplesPerSegment <= 0)
                throw new ArgumentOutOfRangeException("The SamplesPerSegment property must be a positive, non-zero integer");          

            //Determine the starting node index from which a linear approximation will be generated.
            int si = _lastCalculatedNodeIndex + 1; 

            if (SmoothingEnabled)
            {
                float stepSize = 1.0f / SamplesPerSegment;
                for (int i = si; i < _nodes.Count; i++)
                {
                    PathNode n1 = _nodes[i - 1];
                    PathNode n2 = _nodes[i];
                    PathNode n0 = (n1 != _nodes.First()) ? _nodes[i - 2] : n1;
                    PathNode n3 = (n2 != _nodes.Last()) ? _nodes[i + 1] : n2;
                    Vector3 p0 = n1.Transform.Translation;
                    float step = stepSize;
                    float totalLength = 0;
                    for (int j = 0; j < SamplesPerSegment; j++)
                    {
                        //**************************************************************
                        //NOTE: Important for future reference - the catmull algorithm
                        //does not produce equidistant points.
                        //**************************************************************
                        Vector3 p = Vector3.CatmullRom(n0.Transform.Translation,
                                                       n1.Transform.Translation,
                                                       n2.Transform.Translation,
                                                       n3.Transform.Translation, step);
                        totalLength += Vector3.Distance(p0, p);
                        step += stepSize;
                        p0 = p;
                    }
                    _approximateSegmentLengths.Add(totalLength);
                    _lastCalculatedNodeIndex = i;
                }
            }
            else
            {
                for (int i = si; i < _nodes.Count; i++)
                {
                    _approximateSegmentLengths.Add(Vector3.Distance(_nodes[i - 1].Transform.Translation,
                                                                    _nodes[i].Transform.Translation));
                    _lastCalculatedNodeIndex = i;
                }
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RequiresApproximation
        {
            get { return _nodes.Count > 1 && _lastCalculatedNodeIndex != _nodes.Count() - 1; }
        }

        /// <summary>
        /// Uses interpoation to evaluate a node at the specifed distance along this path and
        /// returns a new instance of that node.
        /// </summary>      
        /// <param name="d"></param>
        /// <param name="linearApproximation"></param>
        /// <returns></returns>
        public PathNode EvaluateNodeAtDistance(float d)
        {
            PathNode pathNode = null;

            if (this.NodeCount < 2)
                throw new InvalidOperationException("Path is undefined. A path requires at least two nodes to be defined");

            if (this.RequiresApproximation)
                throw new InvalidOperationException("Length not approximated. Must call GenerateLinearApproximation() after adding or removing nodes from the path.");          

            //TODO: Handle looped path.
            //-------------------------

            if (d >= 0 && d <= Distance)
            {
                float accumLength = 0;
                float pct = 0;

                for (int i = 0; i < _approximateSegmentLengths.Count; i++)
                {
                    if (d >= accumLength && d <= accumLength + _approximateSegmentLengths[i] || i == _approximateSegmentLengths.Count - 1)
                    {
                        pct = (d - accumLength) / _approximateSegmentLengths[i];
                        PathNode n1 = _nodes[i];
                        PathNode n2 = _nodes[i + 1];
                        PathNode n0 = (n1 != _nodes.First()) ? _nodes[i - 1] : n1;
                        PathNode n3 = (n2 != _nodes.Last()) ? _nodes[i + 2] : n2;
                        Vector3 vTranslation;
                        if (SmoothingEnabled)
                        {
                            //**************************************************************
                            //NOTE: Important for future reference - the catmull algorithm
                            //does not produce equidistant points.
                            //**************************************************************
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
                        pathNode = new PathNode() { Transform = new Transform(qRotation, vTranslation, 1) };
                        break;
                    }
                    accumLength += _approximateSegmentLengths[i];
                }
            }            
            else
                throw new InvalidOperationException("Distance, d, is out of range.");

            return pathNode;
        }

        /// <summary>
        /// Returns a list of path nodes that are approximately equidistant.
        /// </summary>
        /// <param name="stepSize"></param>
        /// <param name="firstNode"></param>
        /// <param name="lastNode"></param>
        /// <returns></returns>
        /// <remarks>This distance between the nodes may actually be greater or less than the specified step size. 
        /// The stepSize parameter is used as an approximation.
        /// The SamplesPerSegment property determines the level of accuracy. The two are positively correlated.
        /// </remarks>
        public PathNode[] EvaluateEquidistantNodes(float stepSize, PathNode firstNode = null, PathNode lastNode = null)
        {
            List<PathNode> result = new List<PathNode>();
            float step = 0;

            //Ensure step size is rational number, greater than zero.
            if (stepSize <= 0)
                throw new ArgumentException("stepSize is not greater than zero.");
            
            //
            while (step < Distance)
            {
                result.Add(EvaluateNodeAtDistance(step));
                step += stepSize;
            }

            if (Distance > 0 && step < Distance)
                result.Add(EvaluateNodeAtDistance(Distance));

            return result.ToArray();
        }       

        /// <summary>
        /// 
        /// </summary>
        public void ClearNodes()
        {
            _nodes.Clear();
            _approximateSegmentLengths.Clear();
            _lastCalculatedNodeIndex = 0;
            OnPathChanged();
        }     

        /// <summary>
        /// 
        /// </summary>
        protected void OnPathChanged()
        {
            EventHandler handler = PathChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }       

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler PathChanged;        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class PathNode
    {
        public PathNode()
        {
            Transform = Transform.Identity;           
        }

        public Transform Transform { get; set; }       
    }
}
