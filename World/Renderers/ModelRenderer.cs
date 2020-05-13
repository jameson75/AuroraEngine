using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.AngelJacket.Core.World.Systems;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class ModelRenderer : IRenderer
    {
        IGameApp _game = null;

        public ModelRenderer(IGameApp game)
        {
            _game = game;
        }

        public ModelRenderer(Model model)
        {
            Model = model;
            _game = model.Game;
        }

        public Model Model { get; set; }       

        public void Draw(ITransformable container)
        {
            if (Model != null)
            {
                var camera = _game.GetGameContext()
                                  .Scene
                                  .CameraNode
                                  .Camera;
                /*
                if (Model.IsDynamicAndInstanced())                                   
                    Model.Effect.World = Matrix.Identity;                
                else
                */
                Model.Effect.World = container.WorldTransform().ToMatrix();
                Model.Effect.View = camera.ViewMatrix;
                Model.Effect.Projection = camera.ProjectionMatrix;
                Model.Draw();
            }
        }

        public void Update(GameTime gameTime) { }
    }
}
