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
            OnLayoutChanged();
        }

        protected virtual void OnLayoutChanged()
        { }
    }

    public class GridForm : Form
    {
        private Vector3 _dimensions = Vector3.Zero;
        private Vector3 _elementDistance = Vector3.Zero;

        public DrawingSize Dimensions 
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;
                OnLayoutChanged();
            }
        }

        protected override void OnLayoutChanged()
        {
            for(int i = 0; i < (int)Dimensions.X; i++ )
                for (int j = 0; j < (int)Dimensions.Y; j++)
                    for (int k = 0; k < (int)Dimensions.Z; k++)
                    {
                        Vector3 elementOrigin = new Vector3(i * _elementDistance.X + _elementDistance.X * 0.5f, 
                                                            j * _elementDistance.Y + _elementDistance.Y * 0.5f, 
                                                            k * _elementDistance.Z + _elementDistance.Z * 0.5f);
                    }
        } 
    }

    public class FormElement
    {
        public Model Model { get; set; }
        public Matrix Transform { get; set; }
    }
}
