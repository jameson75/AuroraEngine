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
    public class Model
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
       
        //public BasicEffect Effect { get; set; }

        //public BasicEffectEx Effect { get; set; }

        public DofLightingEffect Effect { get; set; }

        //public Camera Camera { get; set; }

        //public Matrix Transform { get; set; }

        //public void ApplyEffect()
        //{
        //    _effect.Apply(_effectParameters);
        //}

        public virtual void Draw(long gameTime)
        {
            //if (Effect != null)
            //{
            //    Effect.SetWorld(Transform);
            //    Effect.SetView(Camera.ViewMatrix);
            //    Effect.SetProjection(Camera.ProjectionMatrix);
            //    Effect.Apply();
            //}
            //Effect.Apply();
            if (Mesh != null)
                Mesh.Draw(gameTime);
        } 
    }

   
}
