using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class PathTracker
    {
        private Path _path = null;

        public ITransformable Target { get; set; }

        public Path Path { get { return _path; } }

        public float PathNodeMinDistance { get; set; }

        public PathTracker()
        {
            _path = new Path();
            _path.GenerateLinearApproximation();
            _path.SmoothingEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            PathNode lastNode = Path.Nodes.LastOrDefault();            
            if (lastNode == null ||
                PathNodeMinDistance < Vector3.Distance(Target.Transform.Translation, lastNode.Transform.Translation) )
            {
                Path.Nodes.Add(new PathNode() { Transform = Target.Transform });
                Path.UpdateLinearApproximation(0);
                if (Path.Nodes.Count > 50)
                {
                    Path.Nodes.RemoveAt(0);
                    Path.UpdateLinearApproximation(1);
                }
            }            
        }       
    }
}
