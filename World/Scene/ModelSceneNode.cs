using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class ModelSceneNode : SceneNode
    {
        public ModelSceneNode(Scene scene)
            : base(scene)
        {
            HitTestable = true;
        }

        public ModelSceneNode(Scene scene, Model model)
            : base(scene)
        {
            Model = model;
            HitTestable = true;
        }

        public Model Model { get; set; }

        public bool HitTestable { get; set; }

        public override Transform Transform { get { return Model.Transform; } set { Model.Transform = value; } }

        public override void Draw(long gameTime)
        {
            if (Model != null)
            {
                Model.Effect.World = LocalToWorld(this.Transform.ToMatrix());
                Model.Effect.View = Scene.Camera.ViewMatrix;
                Model.Effect.Projection = Scene.Camera.ProjectionMatrix;
                Model.Effect.Apply();
                Model.Draw(gameTime);
            }
        }
    }
}
