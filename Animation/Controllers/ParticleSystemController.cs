using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////    

namespace CipherPark.AngelJacket.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticleSystemController : AnimationController
    {
        private long? _animationStartTime = null;
        private long? _lastEmissionTime = null;
        private Transform[] _lastEmitterWorldTransform = null;

        public ParticleSystem System { get; set; }

        public ParticleSolver Solver { get; set; }

        public int EmissionRate { get; set; }

        public bool EnableDirectionalBlur { get; set; }

        public override void Reset()
        {
            if (Solver != null)
                Solver.Reset();
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();

            if (_lastEmissionTime == null)
                _lastEmissionTime = gameTime.GetTotalSimtime();

            if (_lastEmitterWorldTransform == null )            
                _lastEmitterWorldTransform = System.Emitters.Select(e => e.WorldTransform()).ToArray();                         

            if (gameTime.GetTotalSimtime() - _lastEmissionTime > EmissionRate)
            {
                for (int i = 0; i < System.Emitters.Count; i++)
                {
                    var emittedParticles =  System.Emit(i);
                    
                    //****************************************************************************
                    //NOTE: Directional Blur is used for effects light "streaks"/"light trails".
                    //While, motion blur would be used for effects light a "sparks".
                    //****************************************************************************

                    if (EnableDirectionalBlur)
                    {
                        Vector3 blurDirection = Vector3.Normalize(System.Emitters[i].WorldTransform().Translation - _lastEmitterWorldTransform[i].Translation);                        
                        emittedParticles.ForEach(p => p.InstanceData = new ParticleInstanceData() { DirectionalBlur = p.WorldToLocal(blurDirection) });                        
                    }
                }
            }
          
            if( Solver != null )
                Solver.Step(gameTime, System);          

            //TODO: Optionally, apply Motion blur to ALL particles here.
        }
    }
}