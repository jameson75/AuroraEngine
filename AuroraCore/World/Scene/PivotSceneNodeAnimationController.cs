using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Scene
{
    public class PivotSceneNodeAnimationController : SimulatorController
    { 
        public SceneNode Node { get; set; }
        public ITransformable TrackedObject { get; set; }
        public ITransformable PivotObject { get; set; }
        public Vector3 PivotAxis { get; set; }
        public float TrackingRange { get; set; }
        public float MaxPivotAngle { get; set; }
        public float PivotSpeed { get; set; }    

        public override void Update(GameTime gameTime)
        {
            if (TrackedObject != null && PivotObject != null && TrackingRange > 0)
            {
                //Get the location of outer object in wold space.
                Vector3 wsPivotObjectLocation = PivotObject.ParentToWorld(PivotObject.Transform).Translation;
                //Get the location of the tracked object in this world space.
                Vector3 wsTrackedObjectLocation = TrackedObject.ParentToWorld(TrackedObject.Transform).Translation;
                //Get the x-offset of the tracked object from the pivot object.
                float xOffset = (wsTrackedObjectLocation - wsTrackedObjectLocation).X;
                //Get the percentage of the tracking range covered by the xoffset (clamped between -1 and 1).
                float trackingRangePct = MathUtil.Clamp(xOffset / TrackingRange, -1, 1);
                //Determine the angle of the pivot object.
                float pivotAngle = -trackingRangePct * MaxPivotAngle;
                //Transform scene node (pivot about axis at determined angle).
                //NOTE: We make sure to combine rotations.
                Matrix nodeMatrix = Node.Transform.ToMatrix();                
                Matrix nodeRotations = nodeMatrix * Matrix.Translation(-nodeMatrix.TranslationVector);
                Node.Transform = new Transform(nodeRotations * Matrix.RotationAxis(PivotAxis, pivotAngle) * Matrix.Translation(nodeMatrix.TranslationVector));
            }              
        }

        protected override void OnSimulationReset()
        {

        }
    }  
}
