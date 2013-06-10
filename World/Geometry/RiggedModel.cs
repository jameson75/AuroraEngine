using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public class SkinnedModel : BasicModel
    {
        private Bones _bones = new Bones();
        private List<TransformAnimationController> _animationControllers = new List<TransformAnimationController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public SkinnedModel(IGameApp game)
            : base(game)
        { }    
        
        /// <summary>
        /// 
        /// </summary>
        public Bones Bones { get { return _bones; } }
       
        /// <summary>
        /// 
        /// </summary>
        public List<TransformAnimationController> Animation
        { get { return _animationControllers; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poseTime"></param>
        /// <returns></returns>
        public List<TransformAnimationController> CreatePoseAnimation(ulong poseKeyTime, ulong transitionTimeSpan = 0)
        {           
            List<TransformAnimationController> result = new List<TransformAnimationController>();
            List<Bone> boneList = Bones.FlattenHierarchy();
            foreach (Bone bone in boneList)
            {
                TransformAnimation transitionAnimation = new TransformAnimation();
                if (transitionTimeSpan > 0)
                {
                    AnimationKeyFrame startPoseKeyFrame = new AnimationKeyFrame(0, bone.Transform);
                    transitionAnimation.SetKeyFrame(startPoseKeyFrame);
                }
                Transform endPoseTransform = (Transform)Animation.Find(c => c.Target == bone).Animation.GetActiveKeyFrameAtT(poseKeyTime).Value;
                AnimationKeyFrame endPoseKeyFrame = new AnimationKeyFrame(transitionTimeSpan, endPoseTransform);             
                transitionAnimation.SetKeyFrame(endPoseKeyFrame);
                TransformAnimationController bonePoseController = new TransformAnimationController(transitionAnimation, bone);
                result.Add(bonePoseController);
            }
            return result;
        }       
    }

    /// <summary>
    /// 
    /// </summary>
    public class Bone : ITransformable
    {
        private Bones _children = new Bones();

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public Bone Parent { get; set; }
        public Bones Children { get { return _children; } }
    }

    public class Bones :  ObservableCollection<Bone>
    {   
        public Bones()
        { }    
       
        public void AddRange(IEnumerable<Bone> bones)
        {
            foreach( Bone bone in bones )
                this.Add(bone);
        }

        public Bone this[string name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Name == name)
                        return this[i];
                return null;
            }
        }

        public List<Bone> FlattenHierarchy()
        {
            List<Bone> list = new List<Bone>();
            foreach (Bone bone in this)
                BuildList(bone, list);
            return list;
        }

        private void BuildList(Bone bone, List<Bone> list)
        {
            list.Add(bone);
            foreach (Bone childBone in bone.Children)
                BuildList(childBone, list);          
        }
    }  
}
