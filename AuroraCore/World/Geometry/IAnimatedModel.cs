using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Effects;
using CipherPark.KillScript.Core.Animation.Controllers;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////


namespace CipherPark.KillScript.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnimatedModel
    {
        Frame FrameTree { get; set; }
        List<KeyframeAnimationController> AnimationRig { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class AnimatedModelExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="poseKeyTime"></param>
        /// <param name="transitionTimeSpan"></param>
        /// <returns></returns>
        public static CompositeAnimationController CreatePoseAnimation(this IAnimatedModel model, ulong poseKeyTime, ulong transitionTimeSpan = 0)
        {
            List<KeyframeAnimationController> result = new List<KeyframeAnimationController>();
            if (model.FrameTree != null)
            {
                List<Frame> frameList = model.FrameTree.FlattenToList();
                foreach (Frame frame in frameList)
                {
                    TransformAnimation transitionAnimation = new TransformAnimation();
                    if (transitionTimeSpan > 0)
                    {
                        AnimationKeyFrame startPoseKeyFrame = new AnimationKeyFrame(0, frame.Transform);
                        transitionAnimation.SetKeyFrame(startPoseKeyFrame);
                    }
                    Transform endPoseTransform = (Transform)model.AnimationRig.Find(a => a.Target == frame).Animation.GetActiveKeyFrameAtT(poseKeyTime).Value;
                    AnimationKeyFrame endPoseKeyFrame = new AnimationKeyFrame(transitionTimeSpan, endPoseTransform);
                    transitionAnimation.SetKeyFrame(endPoseKeyFrame);
                    KeyframeAnimationController frameController = new KeyframeAnimationController(transitionAnimation, frame);
                    result.Add(frameController);
                }
            }
            return new CompositeAnimationController(result);
        }
    }

    public class Frame : ITransformable
    {
        private Frames _children = null;
        private Frame _parent = null;
        private ITransformable _transformableParent = null;

        public Frame()
        {
            _children = new Frames();
            _children.CollectionChanged += this.Children_CollectionChanged;
            Transform = Transform.Identity;
        }

        public string Name { get; set; }        

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
                _transformableParent = value;
            }
        }

        public Frames Children { get { return _children; } }

        #region ITransformable implementation

        public Transform Transform { get; set; }

        public ITransformable TransformableParent
        {
            get { return this._transformableParent; }
            set
            {
                if (value is Frame)
                    this.Parent = (Frame)value;
                else
                {
                    this.Parent = null;
                    _transformableParent = value;
                }
            }
        }

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
