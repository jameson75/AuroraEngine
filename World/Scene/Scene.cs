using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Systems.Animation;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class Scene
    {
        private IGameApp _game = null;
        private SceneNodes _nodes = null;
        private MatrixStack _worldTransformStack = null;

        public Scene(IGameApp game)
        {
            _game = game;
            _nodes = new SceneNodes();
            _worldTransformStack = new MatrixStack();
        }

        public IGameApp Game { get { return _game; } }
        public SceneNodes Nodes { get { return _nodes; } }
        public Camera Camera { get; set; }

        public void Update(long gameTime)
        {
            OnBeginUpdate();
            foreach (SceneNode node in Nodes)
                _UpdateNodeHierarchy(gameTime, node);
            OnEndUpdate();
        }       

        public void Draw(long gameTime)
        {
            OnBeginDraw();
            foreach (SceneNode node in Nodes)
                _DrawNodeHierarchy(gameTime, node);
            OnEndDraw();
        }
        
        private void _UpdateNodeHierarchy(long gameTime, SceneNode node)
        {       
            node.Update(gameTime);            
            foreach (SceneNode child in node.Children)
                _UpdateNodeHierarchy(gameTime, child);           
        }

        private void _DrawNodeHierarchy(long gameTime, SceneNode node)
        {            
            node.Draw(gameTime);           
            foreach (SceneNode child in node.Children)
                _DrawNodeHierarchy(gameTime, child);
        }

        protected virtual void OnBeginDraw()
        {
            EventHandler handler = BeginDraw;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEndDraw()
        {
            EventHandler handler = EndDraw;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnBeginUpdate()
        {
            EventHandler handler = BeginUpdate;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEndUpdate()
        {
            EventHandler handler = EndUpdate;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler BeginDraw;
        public event EventHandler EndDraw;
        public event EventHandler BeginUpdate;
        public event EventHandler EndUpdate;
    }

    public class MatrixStack
    {
        private List<Matrix> _innerList = new List<Matrix>();

        public Matrix Transform
        {
            get
            {
                Matrix t = Matrix.Identity;
                foreach (Matrix m in _innerList)
                    t *= m;
                return t;
            }                   
        }

        public void Push(Matrix m)
        {
            _innerList.Add(m);
        }

        public Matrix Pop()
        {
            if (_innerList.Count == 0)
                throw new InvalidOperationException("Matrix stack is empty.");

            int lastIndex = _innerList.Count - 1;
            Matrix m = _innerList[lastIndex];
            _innerList.RemoveAt(lastIndex);
            return m;
        }
      
        public Matrix Top
        {
            get
            {
                if (_innerList.Count == 0)
                    return Matrix.Identity;
                else
                    return _innerList.Last();
            }
        }
    }
}
