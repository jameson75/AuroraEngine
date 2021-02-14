using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.KillScript.Core;
using CipherPark.KillScript.Core.Module;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.UI.Controls;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Utils.Toolkit;
using CipherPark.KillScript.Core.World;
using CipherPark.KillScript.Core.Services;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Animation.Controllers;
using CipherPark.KillScript.Core.Effects;
//using CoreEffect = CipherPark.AngelJacket.Core.Effects.SurfaceEffect;
using CipherPark.KillScript.Core.Systems;

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
    public abstract class Model : IDisposable
    {
        private IGameApp _game = null;
    
        public Model(IGameApp game)
        {
            _game = game;            
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IGameApp Game { get { return _game; } }     
        
        /// <summary>
        /// 
        /// </summary>
        public SurfaceEffect Effect { get; set; }    
        
        /// <summary>
        /// 
        /// </summary>
        public abstract BoundingBox BoundingBox { get; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Effect?.Dispose();
            OnDispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Draw();           

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnApplyingEffect()
        { } 
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDispose()
        { }
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

        public override void Draw()
        {
            if (Effect != null)
            {                
                OnApplyingEffect();
                Effect.Apply();
                if (Mesh != null)
                    Mesh.Draw();
                Effect.Restore();
            }      
        }

        protected virtual void OnMeshChanged()
        { }

        protected override void OnDispose()
        {
            Mesh?.Dispose();
            base.OnDispose();
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

    /*
    public static class ModelExtension
    {       
        public static void UpdateInstanceData(this Model model, IEnumerable<Matrix> data)
        {
            //TODO: Add support for Rigged Model

            if (model is BasicModel == false)
                throw new NotSupportedException("This method is not supported for the model type. Only BasicModel is supported");

            Mesh mesh = GetMesh(model);

            if (mesh == null)
                throw new InvalidOperationException("Model does not contain a mesh");                

            if (mesh.IsInstanced == false || mesh.IsDynamic == false)
                throw new InvalidOperationException("Cannot set instance data to a mesh that is not both dynamic and instanced.");

            if (data.Count() > 0)
                mesh.UpdateVertexStream<InstanceVertexData>(data.Select(m => new InstanceVertexData() { Matrix = Matrix.Transpose(m) }).ToArray());
        }
        
        public static bool IsDynamicAndInstanced(this Model model)
        {
            Mesh mesh = GetMesh(model);
            if (mesh == null)
                return false;
            else
                return mesh.IsDynamic && mesh.IsInstanced;
        }

        private static Mesh GetMesh(Model model)
        {
            if (model is BasicModel)
            {
                return ((BasicModel)model).Mesh;
            }

            else
                return null;
        }
    }
    */
}
