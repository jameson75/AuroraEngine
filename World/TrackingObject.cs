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
    public class TrackingObject : ITransformable
    {
        private Path _path = null;

        public ITransformable Source { get; set; }

        public Path Path { get { return _path; } }

        public float PathNodeMinDistance { get; set; }

        public TrackingObject()
        {
            _path = new Path();           
        }

        public void Update(GameTime gameTime)
        {
            PathNode lastNode = Path.Nodes.LastOrDefault();
            Transform sourceTransform = this.WorldToParent(Source.WorldTransform());
            if (lastNode == null ||
                PathNodeMinDistance < Vector3.Distance(sourceTransform.Translation, lastNode.Transform.Translation) )
            {
                Path.Nodes.Add(new PathNode() { Transform = sourceTransform });
                if (Path.Nodes.Count > 1)
                    Path.GenerateLinearApproximation(16, Path.Nodes.Count - 2);
            }            
        }
    }
}
