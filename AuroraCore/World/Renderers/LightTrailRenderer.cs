using System;
using SharpDX;
using CipherPark.Aurora.Core.World.Systems;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
{
    public class LightTrailRenderer : IRenderer
    {
        IGameApp _game = null;

        public LightTrailRenderer(IGameApp game)
        {
            _game = game;
        }

        public LightTrail LightTrail { get; set; }

        public void Draw(ITransformable container)
        {
            if (LightTrail != null)
            {
                if (LightTrail.Effect == null)
                    throw new InvalidOperationException("Effect not set for light trail.");
                var cameraNode = _game.GetActiveModuleContext()
                                      .Scene
                                      .CameraNode;

                LightTrail.Effect.World = Matrix.Identity; //TODO: Verifty whether I should be using the world transform here.
                LightTrail.Effect.View = cameraNode.RiggedViewMatrix;
                LightTrail.Effect.Projection = cameraNode.ProjectionMatrix;                
                LightTrail.Draw();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (LightTrail != null)
                LightTrail.Update(gameTime);
        }

        public void Dispose()
        {
            LightTrail?.Dispose();
        }

        public SurfaceEffect Effect
        {
            get { return LightTrail?.Effect; }
        }       
    }

    public class StreakRenderer : IRenderer
    {
        IGameApp _game = null;

        public StreakRenderer(IGameApp game)
        {
            _game = game;
        }

        public Streak Streak { get; set; }

        public void Draw(ITransformable container)
        {
            if (Streak != null)
            {
                if (Streak.Effect == null)
                    throw new InvalidOperationException("Effect not set for light trail.");
                var cameraNode = _game.GetActiveModuleContext()
                                      .Scene
                                      .CameraNode;
                Streak.Effect.World = Matrix.Identity; //TODO: Verifty whether I should be using the world transform here.
                Streak.Effect.View = cameraNode.RiggedViewMatrix;
                Streak.Effect.Projection = cameraNode.ProjectionMatrix;
                Streak.Draw();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Streak != null)
                Streak.Update(gameTime);
        }

        public void Dispose()
        {
            Streak?.Dispose();
        }

        public SurfaceEffect Effect
        {
            get { return Streak?.Effect; }
        }
    }
}
