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
    public class RiggedModel : BasicModel
    {
        private Bones _bones = new Bones();
       
        private List<TransformAnimationController> _animationControllers = new List<TransformAnimationController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public RiggedModel(IGameApp game)
            : base(game)
        { }    
        
        /// <summary>
        /// 
        /// </summary>
        public Bones Bones { get { return _bones; } }

        /// <summary>
        /// 
        /// </summary>
        public Frame FrameTree { get; set; }

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
            if (FrameTree != null)
            {
                List<Frame> frameList = FrameTree.FlattenToList();
                foreach (Frame frame in frameList)
                {
                    TransformAnimation transitionAnimation = new TransformAnimation();
                    if (transitionTimeSpan > 0)
                    {
                        AnimationKeyFrame startPoseKeyFrame = new AnimationKeyFrame(0, frame.Transform);
                        transitionAnimation.SetKeyFrame(startPoseKeyFrame);
                    }
                    Transform endPoseTransform = (Transform)Animation.Find(a => a.Target == frame).Animation.GetActiveKeyFrameAtT(poseKeyTime).Value;
                    AnimationKeyFrame endPoseKeyFrame = new AnimationKeyFrame(transitionTimeSpan, endPoseTransform);
                    transitionAnimation.SetKeyFrame(endPoseKeyFrame);
                    TransformAnimationController bonePoseController = new TransformAnimationController(transitionAnimation, frame);
                    result.Add(bonePoseController);
                }
            }
            return result;
        }

        public override void Draw(long gameTime)
        {
            base.Draw(gameTime);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Bone 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        public Transform Transform { get; set; }        
    }

    public class Bones : List<Bone>
    {  
        public Bone this[string name]
        {
            get
            {
                Bone result = this.Find(b => b.Name == name);
                if (result != null)
                    return result;
                throw new IndexOutOfRangeException();                
            }
        }      
    }   
        
    public class Frame : ITransformable
    {
        private Frames _children = null;
        private Frame _parent = null;        
       
        public string Name { get; set; }
        
        public object Reference { get; set; }
        
        public Frame Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null && _parent.Children.Contains(this))
                    _parent.Children.Remove(this);
                _parent = value;
                if (_parent != null && !_parent.Children.Contains(this))
                    _parent.Children.Add(this);
            }
        }

        public Frames Children { get { return _children; } }
        
        #region ITransformable implementation
        
        public Transform Transform { get; set; }

        #endregion

        /// <summary>
        /// Returns all the nodes in the frame tree as a list.
        /// </summary>        
        /// <returns></returns>
        public List<Frame> FlattenToList()
        {
            List<Frame> results = new List<Frame>();
            _BuildFlattenedTree(this, results);
            return results;
        }

        private static void _BuildFlattenedTree(Frame frame, List<Frame> results)
        {
            results.Add(frame);
            foreach (Frame child in frame.Children)
                _BuildFlattenedTree(child, results);
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Frame child in args.NewItems)
                        OnChildAdded(child);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (Frame child in args.OldItems)
                        OnChildRemoved(child);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnChildReset();
                    break;
            }
        }

        protected virtual void OnChildAdded(Frame child)
        {
            if (child.Parent != this)
                child.Parent = this;
        }

        protected virtual void OnChildRemoved(Frame child)
        {
            if (child.Parent == this)
                child.Parent = null;
        }

        protected virtual void OnChildReset()
        {

        }
    }

    public class Frames : ObservableCollection<Frame>
    {
        public Frame this[string name]
        {
            get
            {
                Frame result = this.FirstOrDefault(f => f.Name == name);
                if (result != null)
                    return result;
                throw new IndexOutOfRangeException();
            }
        }     
    }
}
