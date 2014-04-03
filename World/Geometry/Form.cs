using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Renderers;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.World.Geometry
{    
    public abstract class Form : ParticleSystem 
    {      
        private Emitter _elementEmitter = null;
        private ParticleDescription _elementDescription = null;

        protected Form(IGameApp game)
            : base(game)
        {
            _elementEmitter = new Emitter();
            _elementDescription = new ParticleDescription();
            _elementEmitter.DefaultParticleDescription = _elementDescription;
            Emitters.Add(_elementEmitter);
        }

        public Mesh ElementMesh
        {
            get { return _elementDescription.Mesh; }
            set
            {
                _elementDescription.Mesh = value;
                OnMeshChanged();
            }
        }

        public ForwardEffect ElementEffect
        {
            get { return _elementDescription.Effect; }
            set
            {
                _elementDescription.Effect = value;
                OnEffectChanged();
            }
        }      

        public BoundingBox BoundingBox
        {
            //TODO: Calculate Bounding Box size.
            get { return BoundingBoxExtension.Empty; }
        }              

        protected void ClearElements() { KillAll(); }

        protected List<Particle> EmitElements(int count) 
        {           
            return Emit(0, null, count);       
        }

        protected List<Particle> SpawnElements(int count)
        {
            return Spawn(0, null, count);
        }

        protected void AddElements(IEnumerable<Particle> elements, int? index = null)
        {
            Add(elements, index);
        }

        protected void KillElements(IEnumerable<Particle> elements)
        {
            Kill(elements);
        }

        protected int ElementCount { get { return Particles.Count; } }

        protected virtual void OnLayoutChanged() { }

        protected virtual void OnMeshChanged() { }

        protected virtual void OnEffectChanged() { }
    }  
}
