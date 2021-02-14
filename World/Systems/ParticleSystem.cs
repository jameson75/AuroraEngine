﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Effects;
using CipherPark.KillScript.Core.Services;
using CipherPark.KillScript.Core.World;
using CipherPark.KillScript.Core.World.Scene;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Systems
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

        /// <summary>
        /// 
        /// </summary>
        public IGameApp Game { get { return _game; } }

        /// <summary>
        /// 
        /// </summary>
        public List<Emitter> Emitters { get { return _emitters; } }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<Particle> Particles { get { return _particles.ToList().AsReadOnly(); } }    
        
        /// <summary>
        /// 
        /// </summary>
        public bool EnableFustrumClipping { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="particleDescription"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Particle> Spawn(int index = 0, ParticleDescription particleDescription = null, int count = 0)
        {
            IGameStateService stateService = (IGameStateService)Game.Services.GetService(typeof(IGameStateService));           
            return _emitters[index].Spawn(particleDescription, stateService.GameTime, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="particleDescription"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Particle> Emit(int index = 0, ParticleDescription particleDescription = null, int count = 0)
        {
            List<Particle> pList = Spawn(index, particleDescription, count);
            Add(pList);
            return pList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="index"></param>
        public void Add(IEnumerable<Particle> particles, int? index = null)
        {
            if (index == null)
                _particles.AddRange(particles);
            else
                _particles.InsertRange(index.Value, particles);
            OnParticlesAdded(particles);
        }

        /// <summary>
        /// 
        /// </summary>
        public void KillAll()
        {
            List<Particle> auxParticles = new List<Particle>(_particles);
            _particles.Clear();
            OnParticlesReset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Kill(Particle p)
        {
            _particles.Remove(p);
            OnParticlesRemoved(new Particle[] { p });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pList"></param>
        public void Kill(IEnumerable<Particle> pList)
        {
            foreach (Particle p in pList)
                Kill(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        protected virtual void OnParticlesAdded(IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
                p.TransformableParent = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        protected virtual void OnParticlesRemoved(IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
                p.TransformableParent = null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnParticlesReset()
        {
            _particles.ForEach(p => p.TransformableParent = null);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw()
        {
            var particleGroupings = _particles.GroupBy(p => p.Description);
            foreach (IGrouping<ParticleDescription, Particle> particleGrouping in particleGroupings)
            {                
                if (particleGrouping.Key.Mesh.IsInstanced)
                    DrawInstancedParticles(particleGrouping.Key.Mesh, particleGrouping.Key.Effect, particleGrouping.ToArray());
                else
                    DrawParticles(particleGrouping.ToArray());                               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="instanceMesh"></param>
        /// <param name="instanceEffect"></param>
        /// <param name="particles"></param>
        private void DrawInstancedParticles(Mesh instanceMesh, SurfaceEffect instanceEffect, IEnumerable<Particle> particles)
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
                IEnumerable<InstanceVertexData> instanceData = particles.Where(p => p.IsVisible && !IsParticleClipped(p))
                                                                                .Select( p => new InstanceVertexData() 
                                                                                {                                                                                    
                                                                                   Matrix = Matrix.Transpose(p.Transform.ToMatrix())                                                                                   
                                                                                });
                instanceMesh.UpdateVertexStream<InstanceVertexData>(instanceData.ToArray());
                instanceEffect.World = this.WorldTransform().ToMatrix();
                instanceEffect.Apply();
                instanceMesh.Draw();
                instanceEffect.Restore();
            }
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="particles"></param>
        private void DrawParticles(IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
            {
                if (p.IsVisible && !IsParticleClipped(p))
                {
                    p.Description.Effect.World = p.WorldTransform().ToMatrix();
                    p.Description.Effect.Apply();                   
                    p.Description.Mesh.Draw();
                    p.Description.Effect.Restore();
                }             
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsParticleClipped(Particle p)
        {
            return EnableFustrumClipping && IsParticleInFustrum(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        private bool IsParticleInFustrum(Particle particle)
        {
            SceneGraph scene = Game.GetActiveModuleContext().Scene;
            Matrix viewMatrix = Camera.TransformToViewMatrix(scene.CameraNode.WorldTransform());
            Matrix projMatrix = scene.CameraNode.Camera.ProjectionMatrix;
            BoundingFrustum fustrum = new BoundingFrustum(viewMatrix * projMatrix);
            BoundingBox box = particle.Description.Mesh.BoundingBox.Transform(particle.WorldTransform().ToMatrix());
            return fustrum.Intersects(ref box);
        }
    }
}
