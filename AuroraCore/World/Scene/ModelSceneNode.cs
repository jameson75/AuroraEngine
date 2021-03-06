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
        private Transform _transform = Transform.Identity;

        public ModelSceneNode(IGameApp game)
            : base(game)
        { }

        public ModelSceneNode(Model model, string name = null)
            : base(model.Game, name)
        {
            Model = model;
        }

        public Model Model { get; set; }      

        public override void Draw(GameTime gameTime)
        {
            if (Model != null)
            {
                Model.Effect.World = this.WorldTransform().ToMatrix();
                Model.Effect.View = Camera.TransformToViewMatrix(Scene.CameraNode.ParentToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                Model.Effect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;
                Model.Draw(gameTime);
            }
            base.Draw(gameTime);
        }       
    }
}
