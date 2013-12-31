using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CipherPark.AngelJacket.Core.Animation;

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
        private List<Particle> _particles = new List<Particle>();
       
        public List<Emitter> _emitters = new List<Emitter>();
        
        public List<Emitter> Emitters { get; set; }      
        public ReadOnlyCollection<Particle> Particles { get { return _particles.ToList().AsReadOnly(); } }        
        
        #region ITransformable Members
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
        #endregion    

        public void Emit(int index)
        {
            ParticleDescription desc = _emitters[index].DefaultParticleDescription;
            Emit(index, desc);
        }

        public void Emit(int index, ParticleDescription customParticleDescription)
        {
            List<Particle> pList = _emitters[index].Spawn(customParticleDescription);
            _particles.AddRange(pList);
            OnParticlesAdded(pList);            
        }

        public List<Particle> Emit(IEnumerable<Particle> particles)
        {
            _particles.AddRange(particles);
            OnParticlesAdded(particles);
            return particles.ToList();
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
    }

    public class PlexSystem : ParticleSystem
    {
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

        protected override void OnParticlesRemoved(IEnumerable<Particle> particles)
        {
            Unlink(p);
            base.OnParticlesRemoved(particles);
        }

        protected override void OnParticlesReset()
        {
            _links.Clear();
            base.OnParticlesReset();
        }
    }
}
