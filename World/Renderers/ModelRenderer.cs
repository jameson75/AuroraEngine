using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.KillScript.Core.World.Systems;
using CipherPark.KillScript.Core.Effects;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World
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
                var camera = _game.GetActiveModuleContext()
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
