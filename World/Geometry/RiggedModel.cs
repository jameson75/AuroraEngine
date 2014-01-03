using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Effects;
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
    public class RiggedModel : BasicModel, IAnimatedModel
    {
        private SkinOffsets _bones = new SkinOffsets();
       
        private List<KeyframeAnimationController> _animationControllers = new List<KeyframeAnimationController>();

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
                if (FrameTree != null)
                {                    
                    Matrix[] finalBoneMatrices = new Matrix[SkinOffsets.Count];
                    for (int i = 0; i < SkinOffsets.Count; i++ )
                    {
                        Frame bone = SkinOffsets[i].BoneReference;                       
                        finalBoneMatrices[i] = SkinOffsets[i].Transform.ToMatrix() * bone.ParentToWorld(bone.Transform.ToMatrix());
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
