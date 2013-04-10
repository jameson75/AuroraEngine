using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CoreEffect = CipherPark.AngelJacket.Core.Effects.Effect;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public abstract class Model : ITransformable
    {
        private IGameApp _game = null;

        public Model(IGameApp game)
        {
            _game = game;
            //Transform = Matrix.Identity;
        }

        public string Name { get; set; }

        public IGameApp Game { get { return _game; } }

        public Mesh Mesh { get; set; }

        public Transform Transform { get; set; }
        
        //public Camera Camera { get; set; }

        //public Matrix Transform { get; set; }

        //public BasicEffect Effect { get; set; }

        //public BasicEffectEx Effect { get; set; }

        public CoreEffect Effect { get; set; }

        //public void ApplyEffect()
        //{
        //    _effect.Apply(_effectParameters);
        //}

        public abstract void Draw(long gameTime);
    }

    public class BasicModel : Model
    { 
        public BasicModel(IGameApp game) : base(game)
        {
           
        }   

        public override void Draw(long gameTime)
        {
            if(Effect != null)
                Effect.Apply();
            
            if (Mesh != null)
                Mesh.Draw(gameTime);
        } 
    }

    public class CompositeModel : Model
    {
        private List<CompositeModel> _childModels = new List<CompositeModel>();

        public List<CompositeModel> ChildModels { get { return _childModels; } }

        public CompositeModel Parent { get; set; }

        public CompositeModel(IGameApp app)
            : base(app)
        { }      

        public override void Draw(long gameTime)
        {
            if (Effect != null)
                Effect.Apply();

            if (Mesh != null)
                Mesh.Draw(gameTime);

            foreach (CompositeModel childModel in _childModels)
            {
                if (Effect != null)
                {
                    childModel.Effect.World = childModel.LocalToWorld(childModel.Transform.ToMatrix());
                    childModel.Effect.View = Effect.View;
                    childModel.Effect.Projection = Effect.Projection;
                }
                childModel.Draw(gameTime);
            }
        }

        public Matrix LocalToWorld(Matrix localTransform)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(localTransform);
            CompositeModel model = this.Parent;
            while (model != null)
            {
                stack.Push(model.Transform.ToMatrix());
                model = model.Parent;
            }
            return stack.Transform;
        }

        public Transform LocalToWorld(Transform localTransform)
        {
            TransformStack stack = new TransformStack();
            stack.Push(localTransform);
            CompositeModel model = this.Parent;
            while (model != null)
            {
                stack.Push(model.Transform);
                model = model.Parent;
            }
            return stack.Transform;
        } 
    }
}
