using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Animation.Controllers;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiMeshModel : Model, IAnimatedModel
    {
        private List<Mesh> _meshes = new List<Mesh>();

        private List<KeyframeAnimationController> _animationControllers = new List<KeyframeAnimationController>();       

        public List<Mesh> Meshes { get { return _meshes; } }
        
        public override BoundingBox BoundingBox
        {
            get
            {
                Vector3 min = new Vector3
                {
                    X = Meshes.Min(m => m.BoundingBox.Minimum.X),
                    Y = Meshes.Min(m => m.BoundingBox.Minimum.Y),
                    Z = Meshes.Min(m => m.BoundingBox.Minimum.Z),
                };

                Vector3 max = new Vector3
                {
                    X = Meshes.Max(m => m.BoundingBox.Maximum.X),
                    Y = Meshes.Max(m => m.BoundingBox.Maximum.Y),
                    Z = Meshes.Max(m => m.BoundingBox.Maximum.Z),
                };

                return new BoundingBox(min, max);
            }
        }    

        #region IAnimatedModel

        /// <summary>
        /// 
        /// </summary>
       
        public Frame FrameTree { get; set; }
     
        public List<KeyframeAnimationController> AnimationRig { get { return _animationControllers; } }

        #endregion

        public MultiMeshModel(IGameApp game)
            : base(game)
        { }

        public override void Draw()
        {
            List<Frame> frameList = null;
            
            if (FrameTree != null)
                frameList = FrameTree.FlattenToList();

            if (Effect != null)
            {
                OnApplyingEffect();
                Effect.Apply();
              
                foreach (Mesh mesh in Meshes)
                {                  
                    Matrix originalWorld = Effect.World;
                    Frame meshFrame = frameList?.First(f => f.Name == mesh.Name || (f.MeshNames?.Contains(mesh.Name) == true));
                    Effect.World = meshFrame != null ? Effect.World * meshFrame.WorldTransform().ToMatrix() : Effect.World;
                    mesh.Draw();
                    Effect.World = originalWorld;                
                }                
                
                Effect.Restore();
            }
        }
      
        protected override void OnApplyingEffect()
        {
            base.OnApplyingEffect();
        }

        protected override void OnDispose()
        {
            Meshes?.ForEach(x => x?.Dispose());

            base.OnDispose();
        }
    }   
}
