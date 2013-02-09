using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public abstract class Form : Model
    {
        private ObservableCollection<FormElement> _elements = new ObservableCollection<FormElement>();            

        public Form(IGameApp game) : base(game)
        {
            
        }

        public override void Draw(long gameTime)
        {
            //this.Effect.SetView(Camera.ViewMatrix);
            //this.Effect.SetProjection(Camera.ProjectionMatrix);
            Matrix formTransform = this.Effect.World;
            foreach (FormElement element in _elements)
            {
                //this.Effect.SetWorld(this.Transform * element.Transform);               
                this.Effect.World = formTransform * element.Transform;
                this.Effect.Apply();
                Mesh.Draw(gameTime);
            }
            this.Effect.World = formTransform;
            this.Effect.Apply();
        }    
        
        protected ObservableCollection<FormElement> Elements { get { return _elements; } }      

        protected virtual void OnLayoutChanged()
        { }
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
