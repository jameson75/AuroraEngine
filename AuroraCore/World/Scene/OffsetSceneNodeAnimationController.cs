﻿using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Scene
{
    public class OffsetSceneNodeAnimationController : SimulatorController
    {
        public SceneNode TargetNode { get; set; }
        
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
                TargetNode.Transform = new Transform(Matrix.Translation(newNodeLocation));
            }           
        }

        protected override void OnSimulationReset()
        {
            
        }
    }
}
