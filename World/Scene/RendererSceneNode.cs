using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class RendererSceneNode : SceneNode
    {
        public IRenderer Renderer { get; set; }

        public RendererSceneNode(IGameApp game)
            : base(game)
        { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Renderer != null)
            {
                Renderer.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Renderer != null)
            {
                Renderer.Effect.View = Camera.TransformToViewMatrix(Scene.CameraNode.ParentToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                Renderer.Effect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;                
                Renderer.Draw(gameTime);               
            }
        }
    }   
}
