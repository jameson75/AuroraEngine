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
using CipherPark.AngelJacket.Core.Systems;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class Form : ParticleSystem
    {
        private Emitter _elementEmitter = null;
        private ParticleDescription _elementDescription = null;

        public Form(IGameApp game)
            : base(game)
        {
            _elementEmitter = new Emitter();
            _elementDescription = new ParticleDescription();
            _elementEmitter.DefaultParticleDescription = _elementDescription;
            Emitters.Add(_elementEmitter);
        }      

        public virtual FormPattern Pattern { get { return null; } }

        public Mesh ElementMesh
        {
            get { return _elementDescription.Mesh; }
            set
            {
                _elementDescription.Mesh = value;
                OnMeshChanged();
            }
        }

        public SurfaceEffect ElementEffect
        {
            get { return _elementDescription.Effect; }
            set
            {
                _elementDescription.Effect = value;
                OnEffectChanged();
            }
        }

        public virtual BoundingBox BoundingBox
        {
            get { return BoundingBoxExtension.Empty; }
        }

        public void ClearElements()
        { KillAll(); }

        public List<Particle> EmitElements(int count)
        {
            return Emit(0, null, count);
        }

        public List<Particle> SpawnElements(int count)
        {
            return Spawn(0, null, count);
        }

        public void AddElements(IEnumerable<Particle> elements, int? index = null)
        {
            Add(elements, index);
        }

        public void KillElements(IEnumerable<Particle> elements)
        {
            Kill(elements);
        }

        public int ElementCount { get { return Particles.Count; } }

        public event EventHandler MeshChanged;

        public event EventHandler EffectChanged;       

        protected virtual void OnMeshChanged() { FireHandler(MeshChanged); }

        protected virtual void OnEffectChanged() { FireHandler(EffectChanged); }

        private void FireHandler(EventHandler handler)
        {            
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
