using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Extensions;
using CipherPark.Aurora.Core.World.Geometry;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
{
    public class ModelRenderer : IRenderer, IProvideBoundingContext
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
                var cameraNode = _game.GetRenderingCamera();               
                Model.Effect.World = container?.WorldTransform().ToMatrix() ?? Matrix.Identity;
                Model.Effect.View = cameraNode.RiggedViewMatrix;
                Model.Effect.Projection = cameraNode.ProjectionMatrix;
                Model.Draw();
            }
        }
     
        public void Update(GameTime gameTime) { }

        public BoundingBox? GetBoundingBox()
        {
            return Model?.BoundingBox;
        }
    }
}
