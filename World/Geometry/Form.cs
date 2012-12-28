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
    public abstract class Form
    {
        ObservableCollection<FormElement> _elements = new ObservableCollection<FormElement>();
        public ObservableCollection<FormElement> Elements { get { return _elements; } }

        public Form()
        {
            _elements.CollectionChanged+= Elements_CollectionChanged;
        }

        public void Draw(long gameTime)
        {
            foreach (FormElement element in _elements)
            {
                element.Model.Transform = this.Transform * element.Transform;
                element.Model.Draw(gameTime);
            }                
        }

        private void Elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.Action == NotifyCollectionChangedAction.Add)
                foreach(FormElement newElement in args.NewItems)
                    OnElementAdded(newElement);
            else if(args.Action == NotifyCollectionChangedAction.Remove)
                foreach(FormElement oldElement in args.OldItems)
                    OnElementRemoved(oldElement);
        }

        protected virtual void OnElementAdded(FormElement newElement)
        {

        }

        protected virtual void OnElementRemoved(FormElement oldElement)
        {

        }
    }

    public class GridForm : Form
    {
        public DrawingSize Dimensions { get; set; }
        public 

    }

    public class FormElement
    {
        public Model Model { get; set; }
        public Matrix Transform { get; set; }
    }
}
