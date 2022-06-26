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

        /* public List<MeshTextures> MeshTextures { get; set; }      */

        public override BoundingBox BoundingBox
        {
            get
            {
                Vector3 min = Meshes.Min(m => m.BoundingBox.Minimum);
                Vector3 max = Meshes.Max(m => m.BoundingBox.Maximum);
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
                    //*************************************************************************************************
                    //NOTES: It is expectected that the World (and Projection) matrix of the Effect has already been
                    //set before calling this method. 
                    //*************************************************************************************************
                       
                    //We need to combine the mesh's frame-tree transformation to the current world matrix.
                    //We cache the world matrix specified before this call and apply the mesh's frame transformation
                    //be fore drawing. We restore the orginal world matrix after drawing is complete.                        
                        
                    Matrix originalWorld = Effect.World;
                    Frame meshFrame = frameList.First(f => f.Name == mesh.Name);
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
