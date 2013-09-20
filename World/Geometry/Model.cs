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
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Renderers;

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
    public abstract class Model : ITransformable
    {
        private IGameApp _game = null;
        // private Mesh _mesh = null;        

        public Model(IGameApp game)
        {
            _game = game;
            //Transform = Matrix.Identity;
        }

        public string Name { get; set; }

        public IGameApp Game { get { return _game; } }

       // public Mesh Mesh { get { return _mesh; } set { _mesh = value; OnMeshChanged(); } }       

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }
        
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

        protected virtual void OnApplyingEffect()
        { }

        public abstract BoundingBox BoundingBox { get; }
    }

    public class BasicModel : Model
    {
        public Mesh Mesh { get; set; }

        public override BoundingBox BoundingBox
        {
            get 
            { 
                return (Mesh != null) ? Mesh.BoundingBox : BoundingBoxExtension.Empty; 
            }
        }

        public BasicModel(IGameApp game) : base(game)
        {
           
        }   

        public override void Draw(long gameTime)
        {
            if (Effect != null)
            {
                Effect.World = ((ITransformable)this).ParentToWorld(this.Transform).ToMatrix();
                OnApplyingEffect();
                Effect.Apply();
                if (Mesh != null)
                    Mesh.Draw(gameTime);
                Effect.RestoreGraphicsState();
            }      
        }

        protected virtual void OnMeshChanged()
        { } 
    }

    //public class CompositeModel : Model
    //{
    //    private List<CompositeModel> _childModels = new List<CompositeModel>();

    //    public List<CompositeModel> ChildModels { get { return _childModels; } }

    //    public CompositeModel Parent { get; set; }

    //    public CompositeModel(IGameApp app)
    //        : base(app)
    //    { }      

    //    public override void Draw(long gameTime)
    //    {
    //        if (Effect != null)
    //            Effect.Apply();

    //        if (Mesh != null)
    //            Mesh.Draw(gameTime);

    //        foreach (CompositeModel childModel in _childModels)
    //        {
    //            if (Effect != null)
    //            {
    //                childModel.Effect.World = childModel.LocalToWorld(childModel.Transform.ToMatrix());
    //                childModel.Effect.View = Effect.View;
    //                childModel.Effect.Projection = Effect.Projection;
    //            }
    //            childModel.Draw(gameTime);
    //        }
    //    }

    //    //public Matrix LocalToWorld(Matrix localTransform)
    //    //{
    //    //    MatrixStack stack = new MatrixStack();
    //    //    stack.Push(localTransform);
    //    //    CompositeModel model = this.Parent;
    //    //    while (model != null)
    //    //    {
    //    //        stack.Push(model.Transform.ToMatrix());
    //    //        model = model.Parent;
    //    //    }
    //    //    return stack.Transform;
    //    //}

    //    //public Transform LocalToWorld(Transform localTransform)
    //    //{
    //    //    TransformStack stack = new TransformStack();
    //    //    stack.Push(localTransform);
    //    //    CompositeModel model = this.Parent;
    //    //    while (model != null)
    //    //    {
    //    //        stack.Push(model.Transform);
    //    //        model = model.Parent;
    //    //    }
    //    //    return stack.Transform;
    //    //} 
    //}

    /// <summary>
    /// 
    /// </summary>
    public class ComplexModel : Model, IAnimatedModel
    {
        private List<Mesh> _meshes = new List<Mesh>();
        
        private List<Emitter> _emitters = new List<Emitter>();

        public List<Mesh> Meshes { get { return _meshes; } }        
        
        public List<Emitter> Emitters { get { return _emitters; } }

        public ParticleRenderer ParticleRenderer { get; set; }
        
        #region IAnimatedModel
        public Frame FrameTree { get; set; }        
        public List<KeyframeAnimationController> Animation { get; set; }
        #endregion        
        
        public List<MeshTextures> MeshTextures { get; set; }

        public override BoundingBox BoundingBox
        {
            get 
            {
                Vector3 min = Meshes.Min(m => m.BoundingBox.Minimum);
                Vector3 max = Meshes.Max(m => m.BoundingBox.Maximum);
                return new BoundingBox(min, max);
            }
        }

        public ComplexModel(IGameApp game)
            : base(game)
        { }
        
        public override void Draw(long gameTime)
        {
            List<Frame> frameList = null;
                if(FrameTree != null)
                    frameList = FrameTree.FlattenToList();
             
            if (Effect != null)
            {
                Effect.World = ((ITransformable)this).ParentToWorld(this.Transform).ToMatrix();
                OnApplyingEffect();
                Effect.Apply();
                foreach (Mesh mesh in Meshes)
                    mesh.Draw(gameTime);
                Effect.RestoreGraphicsState();
            }

            if (Emitters != null && ParticleRenderer != null)
                ParticleRenderer.Draw(gameTime, Emitters.SelectMany(e => e.Particles));              
        }

        protected override void OnApplyingEffect()
        {
            base.OnApplyingEffect();
        }
    }  
    
    /// <summary>
    /// 
    /// </summary>
    public class MeshTextures
    {            
        public string MeshName { get; set; }
        public Texture2D Texture { get; set; }
        public ModelTextureChannel Channel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ModelTextureChannel
    {
        Diffuse = 0,
        Normal,     
        Alpha
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAnimatedModel
    {
        Frame FrameTree { get; set; }
        List<KeyframeAnimationController> Animation { get; }
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
                    Transform endPoseTransform = (Transform)model.Animation.Find(a => a.Target == frame).Animation.GetActiveKeyFrameAtT(poseKeyTime).Value;
                    AnimationKeyFrame endPoseKeyFrame = new AnimationKeyFrame(transitionTimeSpan, endPoseTransform);
                    transitionAnimation.SetKeyFrame(endPoseKeyFrame);
                    KeyframeAnimationController frameController = new KeyframeAnimationController(transitionAnimation, frame);
                    result.Add(frameController);
                }
            }
            return new CompositeAnimationController(result);
        }
    }
}
