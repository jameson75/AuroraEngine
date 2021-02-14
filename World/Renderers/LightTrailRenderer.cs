using System;
using SharpDX;
using CipherPark.KillScript.Core.World.Systems;
using CipherPark.KillScript.Core.Effects;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World
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
                var camera = _game.GetActiveModuleContext()
                                      .Scene
                                      .CameraNode
                                      .Camera;
                LightTrail.Effect.World = Matrix.Identity; //TODO: Verifty whether I should be using the world transform here.
                LightTrail.Effect.View = camera.ViewMatrix;
                LightTrail.Effect.Projection = camera.ProjectionMatrix;                
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
                var camera = _game.GetActiveModuleContext()
                                      .Scene
                                      .CameraNode
                                      .Camera;
                Streak.Effect.World = Matrix.Identity; //TODO: Verifty whether I should be using the world transform here.
                Streak.Effect.View = camera.ViewMatrix;
                Streak.Effect.Projection = camera.ProjectionMatrix;
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
