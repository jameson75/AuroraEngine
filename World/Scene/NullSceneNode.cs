﻿using System;
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

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class NullSceneNode : SceneNode
    {
        public NullSceneNode(IGameApp game)
            : base(game)
        { }

        public NullSceneNode(IGameApp game, string name)
            : base(game, name)
        { }
    }

    public class CameraOffsetSceneNode : SceneNode
    {
        public CameraOffsetSceneNode(IGameApp game)
            : base(game)
        { }

        public CameraOffsetSceneNode(IGameApp game, string name)
            : base(game, name)
        { }

        public ITransformable OuterObject { get; set; }

        public float InnerRadius { get; set; }

        public float OuterRadius { get; set; }

        public override void Update(long gameTime)
        {
            if (OuterObject != null && OuterRadius > 0)
            {
                //Get world location of outer object.
                Transform gTransform = OuterObject.ParentToWorld(OuterObject.Transform);
                //Get the location of outer object in this node's parent's space.
                Vector3 psLocation = this.WorldToParent(gTransform).Translation;
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
                this.Transform = new Transform(Matrix.Translation(offsetLocation));
            }
            base.Update(gameTime);
        }
    }
}
