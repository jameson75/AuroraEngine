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

        public ITransformable CenterObject { get; set; }

        public float InnerRadius { get; set; }

        public float OuterRadius { get; set; }

        public override void Update(long gameTime, SceneNode node)
        {
            if (TrackedObject != null && CenterObject != null && OuterRadius > 0)
            {
                //Get world location of tracked object.
                Transform gTrackedObjectTransform = TrackedObject.ParentToWorld(TrackedObject.Transform);
                //Get the location of outer object in the center object's local space.
                Vector3 csTrackedObjectLocation = CenterObject.WorldToLocal(gTrackedObjectTransform).Translation;
                //Project the vector on to the center object's y=0 plane.
                //{Projection of a Vector on a plane : B = A ( A dot N ) * N, where A is the vector N is the plane's normal.}
                Vector3 pcsTrackedObjectLocation = csTrackedObjectLocation - (Vector3.Dot(csTrackedObjectLocation, Vector3.UnitY)) * Vector3.UnitY;
                //Get distance of projected location.
                float pcsTrackedObjectDistance = Vector3.Distance(Vector3.Zero, pcsTrackedObjectLocation);
                //Calc percentage of the outer radius is covered by the distance.
                float clampedOffsetPct = MathUtil.Clamp(pcsTrackedObjectDistance / OuterRadius, 0, 1);
                //Get the direction of the projected location.
                Vector3 pcsTrackedObjectDir = Vector3.Normalize(pcsTrackedObjectLocation);
                //Calculate the new location, multplying the direction by the portion of the radius to be covered.
                Vector3 newNodeLocation = pcsTrackedObjectDir * clampedOffsetPct * InnerRadius;
                //Transform this offset node to the new location.
                node.Transform = new Transform(Matrix.Translation(newNodeLocation));
            }
            //base.Update(gameTime);
        }
    }

    public class PivotSceneNodeEffector : SceneNodeEffector
    {
        public ITransformable TrackedObject { get; set; }
        public ITransformable PivotObject { get; set; }
        public Vector3 PivotAxis { get; set; }
        public float PivotAngle { get; set; }
        
        private KeyframeAnimationController pivotAnimationController = null;
        private int previousObjectSign = 0;

        public override void Update(long gameTime, SceneNode node)
        {  
            if (TrackedObject != null && PivotObject != null)
            {
                //Get world location of tracked object.
                Vector3 gTrackedObjectLocation = TrackedObject.ParentToWorld(TrackedObject.Transform).Translation;
                //Get the location of outer object in this node's parent space.
                Vector3 gPivotObjectLocation = PivotObject.ParentToWorld(PivotObject.Transform).Translation;                             
                
                int sign = Math.Sign(gPivotObjectLocation.X);
                if (previousObjectSign != sign)
                    pivotAnimationController = null;

                if (pivotAnimationController == null)
                {
                    pivotAnimationController = new KeyframeAnimationController();                
                    TransformAnimation animation = new TransformAnimation();
                    
                    //TOOD: Use the current position to 
                    //calculate the end time of the animation (with in-mind that a complete pivot is 1 second).

                    animation.SetKeyFrame(new AnimationKeyFrame(0, PivotObject.Transform));
                    Vector3 negatedTranslation = Vector3.Negate(PivotObject.Transform.Translation);
                    Matrix pivotObjectRotations = PivotObject.Transform.ToMatrix() * Matrix.Translation(negatedTranslation);
                    Matrix pivotMatrix = pivotObjectRotations * Matrix.RotationAxis(PivotAxis, PivotAngle * -sign);
                    animation.SetKeyFrame(new AnimationKeyFrame(1000, pivotMatrix));
                    pivotAnimationController.Target = node;
                    pivotAnimationController.Animation = animation;
                    pivotAnimationController.AnimationComplete+= (object sender, EventArgs args) =>
                        {
                            pivotAnimationController = null;
                        };
                }

                if (pivotAnimationController != null)
                    pivotAnimationController.UpdateAnimation(gameTime);

                previousObjectSign = sign;
            }
        }
    }  
}
