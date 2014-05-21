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
using CoreEffect = CipherPark.AngelJacket.Core.Effects.ForwardEffect;
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
          

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }        
    
        public CoreEffect Effect { get; set; }    

        public abstract void Draw(GameTime gameTime);      

        protected virtual void OnApplyingEffect()
        { }

        public abstract BoundingBox BoundingBox { get; }

        public void Move(Vector3 offset)
        {
            this.Transform = new Transform(this.Transform.Rotation, this.Transform.Translation + offset);
        }
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

        public override void Draw(GameTime gameTime)
        {
            if (Effect != null)
            {
                Effect.World = ((ITransformable)this).ParentToWorld(this.Transform).ToMatrix();
                OnApplyingEffect();
                Effect.Apply();
                if (Mesh != null)
                    Mesh.Draw(gameTime);
                Effect.Restore();
            }      
        }

        protected virtual void OnMeshChanged()
        { } 
    }   

    /// <summary>
    /// 
    /// </summary>
    public class ComplexModel : Model, IAnimatedModel
    {
        private List<Mesh> _meshes = new List<Mesh>();      

        public List<Mesh> Meshes { get { return _meshes; } }        
        
        #region IAnimatedModel
        public Frame FrameTree { get; set; }        
        public List<KeyframeAnimationController> AnimationRig { get; set; }
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
        
        public override void Draw(GameTime gameTime)
        {
            List<Frame> frameList = null;
                if(FrameTree != null)
                    frameList = FrameTree.FlattenToList();
             
            //TODO: Finish implementing this draw method my applying the frame transformation to the correct mesh.
            //How??

            if (Effect != null)
            {
                Effect.World = ((ITransformable)this).ParentToWorld(this.Transform).ToMatrix();
                OnApplyingEffect();
                Effect.Apply();
                foreach (Mesh mesh in Meshes)
                    mesh.Draw(gameTime);
                Effect.Restore();
            }           
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
}
