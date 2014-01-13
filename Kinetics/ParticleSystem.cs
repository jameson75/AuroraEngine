﻿using System;
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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Kinetics
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

        public IGameApp Game { get { return _game; } }

        public List<Emitter> Emitters { get { return _emitters; } }

        public ReadOnlyCollection<Particle> Particles { get { return _particles.ToList().AsReadOnly(); } }

        #region ITransformable Members
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
        #endregion

        //public List<Particle> Emit(int index)
        //{
        //    ParticleDescription desc = _emitters[index].DefaultParticleDescription;
        //    return Emit(index, desc);
        //}

        public List<Particle> Emit(int index = 0, ParticleDescription particleDescription = null, int count = 0)
        {
            List<Particle> pList = _emitters[index].Spawn(particleDescription, count);
            Add(pList);
            return pList;
        }

        public void Add(IEnumerable<Particle> particles)
        {
            _particles.AddRange(particles);
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

        public virtual void Draw(long gameTime)
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

        private void DrawInstancedParticles(long gameTime, Mesh instanceMesh, Effect instanceEffect, IEnumerable<Particle> particles)
        {
            if (instanceMesh.IsInstanced == false || instanceMesh.IsDynamic == false)
                throw new InvalidOperationException("Particle mesh is not dynamic and instanced.");

            if (particles.Count() > 0)
            {
                IEnumerable<Matrix> instanceData = particles.Select(p => p.WorldTransform().ToMatrix());
                instanceMesh.Update<Matrix>(instanceData.ToArray());
                instanceEffect.Apply();
                instanceMesh.Draw(gameTime);
            }
        }

        private void DrawParticles(long gameTime, IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
            {               
                p.Description.Effect.World = p.WorldTransform().ToMatrix();
                p.Description.Effect.Apply();
                p.Description.Mesh.Draw(gameTime);                
            }
        }
    }

    public class PlexSystem : ParticleSystem
    {
        public PlexSystem(IGameApp game)
            : base(game)
        { }

        private List<ParticleLink> _links = new List<ParticleLink>();
        
        public ReadOnlyCollection<ParticleLink> Links { get { return _links.AsReadOnly(); } }    

        public void Link(Particle particle1, Particle particle2)
        {
            if (!_links.Exists(e => (e.P1 == particle1 && e.P2 == particle2) || (e.P1 == particle2 && e.P2 == particle2)))
                _links.Add(new ParticleLink(particle1, particle2));
        }

        public void Link(IEnumerable<Particle> particles)
        {
            Particle[] pArray = particles.ToArray();
            for (int i = 0; i < pArray.Length; i += 2)
                Link(pArray[i], pArray[i + 1]);
        }

        public void Unlink(Particle particle1, Particle particle2)
        {
            _links.RemoveAll(e => (e.P1 == particle1 && e.P2 == particle2) || (e.P1 == particle2 && e.P2 == particle2));
        }

        public void Unlink(Particle p)
        {
            _links.RemoveAll(e => e.P1 == p || e.P2 == p);
        }

        public void Unlink(IEnumerable<Particle> particles)
        {
            foreach (Particle p in particles)
                Unlink(p);
        }

        protected override void OnParticlesRemoved(IEnumerable<Particle> particles)
        {
            Unlink(particles);
            base.OnParticlesRemoved(particles);
        }

        protected override void OnParticlesReset()
        {
            _links.Clear();
            base.OnParticlesReset();
        }

        public override void Draw(long gameTime)
        {
            //TODO: Implement wire frame rendering.
            //-------------------------------------

            //List<BasicVertexPositionColor> linkVertices = new List<BasicVertexPositionColor>();
            //foreach (ParticleLink link in pLinks)
            //{
            //    BasicVertexPositionColor v1 = new BasicVertexPositionColor();
            //    v1.Color = Color.White.ToVector4();
            //    v1.Position = new Vector4((emitterTransform * link.P1.Transform.ToMatrix()).TranslationVector, 1.0f);
            //    BasicVertexPositionColor v2 = new BasicVertexPositionColor();
            //    v2.Color = Color.White.ToVector4();
            //    v2.Position = new Vector4((emitterTransform * link.P2.Transform.ToMatrix()).TranslationVector, 1.0f);
            //    linkVertices.Add(v1);
            //    linkVertices.Add(v2);
            //}

            //if (linkVertices.Count > 0)
            //{
            //    linkEffect.World = Matrix.Identity;
            //    linkEffect.View = view;
            //    linkEffect.Projection = projection;
            //    linkEffect.Apply();
            //    _linkMesh.Update<BasicVertexPositionColor>(linkVertices.ToArray());
            //    _linkMesh.Draw(gameTime);
            //}

            base.Draw(gameTime);
        }
    }
}
