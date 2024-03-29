﻿using System.Linq;
using CipherPark.Aurora.Core.Systems;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////    

namespace CipherPark.Aurora.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticleSystemController : SimulatorController
    {
        private long? _animationStartTime = null;
        private long? _lastSimulationTime = null;
        private Transform[] _lastEmitterWorldTransform = null;

        public ParticleSystem System { get; set; }

        public ParticleSolver Solver { get; set; }

        public float SimulationRate { get; set; }

        public ParticleSystemController()
        {
            
        }

        protected override void OnSimulationReset()
        {
            if (Solver != null)
                Solver.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();

            if (_lastSimulationTime == null)
                _lastSimulationTime = gameTime.GetTotalSimtime();

            if (_lastEmitterWorldTransform == null )            
                _lastEmitterWorldTransform = System.Emitters.Select(e => e.WorldTransform()).ToArray();                    

            //Simulating a particle system (especially ones with physically-realistic solvers) can
            //be computationally expensive. We allow the user of the this class to controll the
            //rate at which simulation is performed.
            if (gameTime.GetTotalSimtime() - _lastSimulationTime > SimulationRate)
            {            
                for (int i = 0; i < System.Emitters.Count; i++)
                {
                    //Kill dead particles.
                    var deadParticles = System.Particles.Where(p => p.Age(gameTime.GetTotalSimtime()) > (long)p.Life).ToArray();
                    System.Kill(deadParticles);

                    //Emit new particles.                    
                    System.Emit(i);       
                }

                if( Solver != null && !Solver.IsComplete )
                    Solver.Step(gameTime, System); 
            }              
        }
    }

    public static class ParticleExtension
    {
        public static long Age(this Particle p, long currentSimTime)
        {
            return ( p.Life == 0 ) ? -1 : currentSimTime - p.Birth;
        }
    }
}