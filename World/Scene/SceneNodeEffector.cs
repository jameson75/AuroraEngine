using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public abstract class SceneNodeEffector
    {
        public abstract void Update(long gameTime, SceneNode node);
    }

    public class OffsetNodeEffector : SceneNodeEffector
    {
        //public OffsetCameraNodeEffector(IGameApp game)
        //    : base(game)
        //{ }

        //public OffsetCameraNodeEffector(IGameApp game, string name)
        //    : base(game, name)
        //{ }

        public ITransformable TrackedObject { get; set; }

        public float InnerRadius { get; set; }

        public float OuterRadius { get; set; }

        public override void Update(long gameTime, SceneNode node)
        {
            if (TrackedObject != null && OuterRadius > 0)
            {
                //Get world location of tracked object.
                Transform gTransform = TrackedObject.ParentToWorld(TrackedObject.Transform);
                //Get the location of outer object in this node's parent's space.
                Vector3 psLocation = node.WorldToParent(gTransform).Translation;
                //Project the vector on to the parent space y=0 plane.
                //{Projection of a Vector on a plane : B = A ( A dot N ) * N, where A is the vector N is the plane's normal.}
                Vector3 ppsLocation = psLocation - (Vector3.Dot(psLocation, Vector3.UnitY)) * Vector3.UnitY;
                //Get distance of projected location.
                float ppsLocationDistance = Vector3.Distance(Vector3.Zero, ppsLocation);
                //Calc percentage of the outer radius is covered by the distance.
                float clampedOffsetPct = MathUtil.Clamp(ppsLocationDistance / OuterRadius, 0, 1);
                //Get the direction of the projected location.
                Vector3 dir = Vector3.Normalize(ppsLocation);
                //Calculate the new location, multplying the direction by the portion of the radius to be covered.
                Vector3 offsetLocation = dir * clampedOffsetPct * InnerRadius;
                //Transform this offset node to the new location.
                node.Transform = new Transform(Matrix.Translation(offsetLocation));
            }
            //base.Update(gameTime);
        }
    }

    public class PivotSceneNodeEffector : SceneNodeEffector
    {
        public ITransformable TrackedObject { get; set; }

        public override void Update(long gameTime, SceneNode node)
        {
            if (TrackedObject != null)
            {
                //Get world location of tracked object.
                Transform gTransform = TrackedObject.ParentToWorld(TrackedObject.Transform);
                //Get the location of outer object in this node's parent space.
                Vector3 psLocation = node.WorldToParent(gTransform).Translation;
                //
                // float XDistance = Math.Abs(psLocation.X);                
            }
        }
    }  
}
