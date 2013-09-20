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
    //TODO: Refactor this class so that it 1. Inherits + Leverages ComplexModel and uses InstanceRenderer (class used to render instances).
    public abstract class Form : Model
    {
        private ObservableCollection<FormElement> _elements = new ObservableCollection<FormElement>();
        private Emitter _emitter = null;
        private Mesh _mesh = null;
        
        public Mesh Mesh 
        { 
            get { return _mesh; }
            set { 
                _mesh = value; 
                OnMeshChanged(); 
            }
        }
        
        public Emitter Emitter 
        { 
            get { return _emitter; } 
            set {
                OnEmitterChanging();
                _emitter = value; 
                OnEmitterChanged(); 
            }
        }

        public override BoundingBox BoundingBox
        {
            get { return (Mesh != null) ? Mesh.BoundingBox : BoundingBoxExtension.Empty; }
        }

        public ParticleRenderer ParticleRenderer { get; set; }

        protected ObservableCollection<FormElement> Elements 
        {
            get { return _elements; }
        }

        public Form(IGameApp game) : base(game)
        { }       

        public override void Draw(long gameTime)
        {            
            //Matrix formTransform = this.Effect.World;
            Matrix formTransform = ((ITransformable)this).ParentToWorld(this.Transform).ToMatrix();
            List<Matrix> particleInstanceWorldTransforms = new List<Matrix>();
            foreach (FormElement element in _elements)
            {                             
                this.Effect.World = formTransform * element.Transform;
                this.Effect.Apply();                
                this.Mesh.Draw(gameTime);
                
                if (Emitter != null)
                {
                    Transform cachedTransform = Emitter.Transform;
                    Emitter.Transform = new Transform(formTransform * element.Transform * cachedTransform.ToMatrix());
                    foreach (Particle p in Emitter.Particles)
                        particleInstanceWorldTransforms.Add(p.WorldTransform().ToMatrix());
                    Emitter.Transform = cachedTransform;
                }
                    
                if (Emitter != null && ParticleRenderer != null)
                {                    
                    ParticleRenderer.ParticleEffect.View = this.Effect.View;
                    ParticleRenderer.ParticleEffect.Projection = this.Effect.Projection;                  
                    //ParticleRenderer.Draw(gameTime, Emitter.Particles);     
                    ParticleRenderer.DrawInstanced(gameTime, particleInstanceWorldTransforms);               
                }
            }
            //this.Effect.World = formTransform;
            //this.Effect.Apply();
        }

        protected virtual void OnEmitterChanging()
        {
            if (this.Emitter != null)
                ((ITransformable)this.Emitter).TransformableParent = null;
        }

        protected virtual void OnEmitterChanged()
        {
            if (this.Emitter != null)
                ((ITransformable)this.Emitter).TransformableParent = this;
        }

        protected virtual void OnLayoutChanged()
        {
          
        }

        protected virtual void OnMeshChanged()
        { 

        }
    }   

    public class FormElement
    {
        public Form _form = null;
        public FormElement(Form form)
        {
            _form = form;
            Transform = Matrix.Identity;
        }
        public Form Form { get { return _form; } }
        public Matrix Transform { get; set; }
    }
}
