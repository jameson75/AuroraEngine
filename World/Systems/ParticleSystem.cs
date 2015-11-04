using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Systems
{
    public class ParticleSystem : ITransformable
    {
        private IGameApp _game = null;
        private List<Emitter> _emitters = new List<Emitter>();
        private List<Particle> _particles = new List<Particle>();

        public ParticleSystem(IGameApp game)
        {
            _game = game;
        }
        
        #region ITransformable Members
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
        #endregion       

        public IGameApp Game { get { return _game; } }

        public List<Emitter> Emitters { get { return _emitters; } }

        public ReadOnlyCollection<Particle> Particles { get { return _particles.ToList().AsReadOnly(); } }     

        public List<Particle> Spawn(int index = 0, ParticleDescription particleDescription = null, int count = 0)
        {
            IGameStateService stateService = (IGameStateService)Game.Services.GetService(typeof(IGameStateService));           
            return _emitters[index].Spawn(particleDescription, stateService.GameTime, count);
        }

        public List<Particle> Emit(int index = 0, ParticleDescription particleDescription = null, int count = 0)
        {
            List<Particle> pList = Spawn(index, particleDescription, count);
            Add(pList);
            return pList;
        }

        public void Add(IEnumerable<Particle> particles, int? index = null)
        {
            if (index == null)
                _particles.AddRange(particles);
            else
                _particles.InsertRange(index.Value, particles);
            OnParticlesAdded(particles);
        }

        public void KillAll()
        {
            List<Particle> auxParticles = new List<Particle>(_particles);
            _particles.Clear();
            OnParticlesReset();
        }

        public void Kill(Particle p)
        {
            _particles.Remove(p);
            OnParticlesRemoved(new Particle[] { p });
        }

        public void Kill(IEnumerable<Particle> pList)
        {
            foreach (Particle p in pList)
                Kill(p);
        }

        protected virtual void OnParticlesAdded(IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
                p.TransformableParent = this;
        }

        protected virtual void OnParticlesRemoved(IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
                p.TransformableParent = null;
        }

        protected virtual void OnParticlesReset()
        {
            _particles.ForEach(p => p.TransformableParent = null);
        }
       
        public virtual void Draw(GameTime gameTime)
        {
            var particleGroupings = _particles.GroupBy(p => p.Description);
            foreach (IGrouping<ParticleDescription, Particle> particleGrouping in particleGroupings)
            {                
                if (particleGrouping.Key.Mesh.IsInstanced)
                    DrawInstancedParticles(gameTime, particleGrouping.Key.Mesh, particleGrouping.Key.Effect, particleGrouping.ToArray());
                else
                    DrawParticles(gameTime, particleGrouping.ToArray());                               
            }
        }

        private void DrawInstancedParticles(GameTime gameTime, Mesh instanceMesh, SurfaceEffect instanceEffect, IEnumerable<Particle> particles)
        {
            if (instanceMesh.IsInstanced == false || instanceMesh.IsDynamic == false)
                throw new InvalidOperationException("Particle mesh is not both dynamic and instanced.");

            if (particles.Count() > 0)
            {            
                //*************************************************************************************
                //NOTE: It is expected that each particle being rendered is an immediate child of this 
                //particle system.                                                                                 
                //**************************************************************************************    
                
                //TODO: Ensure that the correct buffer size of instance data is being determined.                
                IEnumerable<InstanceVertexData> instanceData = particles.Where(p => p.IsVisible)
                                                                                .Select( p => new InstanceVertexData() 
                                                                                {                                                                                    
                                                                                   Matrix = Matrix.Transpose(p.Transform.ToMatrix())                                                                                   
                                                                                });
                instanceMesh.UpdateVertexStream<InstanceVertexData>(instanceData.ToArray());
                instanceEffect.World = this.WorldTransform().ToMatrix();
                instanceEffect.Apply();
                instanceMesh.Draw(gameTime);
                instanceEffect.Restore();
            }
        }       

        private void DrawParticles(GameTime gameTime, IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
            {
                if (p.IsVisible)
                {
                    p.Description.Effect.World = p.WorldTransform().ToMatrix();
                    p.Description.Effect.Apply();                   
                    p.Description.Mesh.Draw(gameTime);
                    p.Description.Effect.Restore();
                }             
            }
        }
    }
}
