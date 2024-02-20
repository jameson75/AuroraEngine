using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Animation.Controllers;
using CipherPark.Aurora.Core.Extensions;

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
    public class SkinnedMeshModel : StaticMeshModel, IAnimatedModel
    {
        private SkinOffsets _bones = new SkinOffsets();
       
        private List<KeyframeAnimationController> _animationControllers = new List<KeyframeAnimationController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public SkinnedMeshModel(IGameApp game)
            : base(game)
        { }    
        
        /// <summary>
        /// 
        /// </summary>
        public SkinOffsets SkinOffsets { get { return _bones; } }

        #region IAnimatedModel
        
        /// <summary>
        /// 
        /// </summary>
        public Frame FrameTree { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<KeyframeAnimationController> AnimationRig
        { get { return _animationControllers; } }
        #endregion      

        protected override void OnApplyingEffect()
        {
            ISkinEffect skinEffect = this.Effect as ISkinEffect;
            if (skinEffect != null)
            {
                if (SkinOffsets != null)
                {                    
                    Matrix[] finalBoneMatrices = new Matrix[SkinOffsets.Count];
                    for (int i = 0; i < SkinOffsets.Count; i++ )
                    {
                        Frame bone = SkinOffsets[i].BoneReference;
                        finalBoneMatrices[i] = SkinOffsets[i].Transform.ToMatrix() * bone.WorldTransform().ToMatrix(); // /*REPLACED WITH SHORT CUT TO SAME BEHAVIOR* bone.ParentToWorld(bone.Transform.ToMatrix());
                    }
                    skinEffect.BoneTransforms = finalBoneMatrices;     
                }
            }
            base.OnApplyingEffect();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SkinOffset 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public Frame BoneReference { get; set; }
    }

    public class SkinOffsets : List<SkinOffset>
    {  
        public SkinOffset this[string name]
        {
            get
            {
                SkinOffset result = this.Find(b => b.Name == name);
                if (result != null)
                    return result;
                throw new IndexOutOfRangeException();                
            }
        }      
    }   
}
