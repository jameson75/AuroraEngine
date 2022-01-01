using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.Aurora.Core.World.Systems;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
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

        public void Dispose()
        {
            Model?.Dispose();
        }

        public void Draw(ITransformable container)
        {
            if (Model != null)
            {
                var cameraNode = _game.GetActiveModuleContext()
                                  .Scene
                                  .CameraNode;
               
                Model.Effect.World = container?.WorldTransform().ToMatrix() ?? Matrix.Identity;
                Model.Effect.View = cameraNode.RiggedViewMatrix;
                Model.Effect.Projection = cameraNode.ProjectionMatrix;
                Model.Draw();
            }
        }

        public void Update(GameTime gameTime) { }
    }
}
