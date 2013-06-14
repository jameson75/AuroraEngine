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

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class ModelSceneNode : SceneNode
    {
        Transform _cachedTransform = Transform.Identity;       

        public ModelSceneNode(IGameApp game)
            : base(game)
        {
            HitTestable = true;
        }

        public ModelSceneNode(Model model, string name = null)
            : base(model.Game, name)
        {
            Model = model;
            HitTestable = true;
        }

        public Model Model { get; set; }

        public bool HitTestable { get; set; }

        public override Transform Transform 
        { 
            get 
            {
                if (Model != null)
                    return Model.Transform;
                else
                    return Transform.Identity;
            }
            set 
            {
                if (Model != null)
                    Model.Transform = value;
                else
                    _cachedTransform = value;
            }
        }

        public override BoundingBox Bounds
        {
            get
            {
                return Model.Mesh.BoundingBox;
            }
        }

        public override void Draw(long gameTime)
        {
            if (Model != null)
            {
                Model.Effect.World = LocalToWorld(this.Transform).ToMatrix();
                Model.Effect.View = Camera.TransformToView(Scene.CameraNode.LocalToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                Model.Effect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;                
                Model.Draw(gameTime);               
            }
        }
    }
}
