using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World.Scene
{
    public class SceneNodeAnimationController : SimulatorController
    {
        public SceneNode CameraAnchorNode { get; set; }
        
        public ITransformable TrackedObject { get; set; }

        public ITransformable CenterObject { get; set; }

        public float InnerRadius { get; set; }

        public float OuterRadius { get; set; }

        public override void Update(GameTime gameTime)       
        {
            if (TrackedObject != null && CenterObject != null && OuterRadius > 0)
            {
                //Get world location of tracked object.
                Transform wsTrackedObjectTransform = TrackedObject.ParentToWorld(TrackedObject.Transform);
                //Get the location of outer object in the center object's local space.
                Vector3 csTrackedObjectLocation = CenterObject.WorldToLocal(wsTrackedObjectTransform).Translation;
                //Project the location to the center object's y=0 plane.
                //{Projection of a Vector to the nearest point on a plane : B = A - ( A - P dot N ) * N.
                //{A is the vector (from plane's origin to the location) P is the plane's origin and N is the plane's normal.}
                //(See tmpearce's explanation at http://stackoverflow.com/questions/9605556/how-to-project-a-3d-point-to-a-3d-plane#comment12185786_9605695)
                //NOTE: Since we're projecting to the Y=0 plane, P = Vector3.Zero (therefore we simply omit P).
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
                CameraAnchorNode.Transform = new Transform(Matrix.Translation(newNodeLocation));
            }           
        }

        protected override void OnSimulationReset()
        {
            
        }
    }

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
