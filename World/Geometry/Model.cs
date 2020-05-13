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
using CipherPark.AngelJacket.Core.Animation.Controllers;
using CipherPark.AngelJacket.Core.Effects;
//using CoreEffect = CipherPark.AngelJacket.Core.Effects.SurfaceEffect;
using CipherPark.AngelJacket.Core.Systems;

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
    public abstract class Model
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
        /// <param name="gameTime"></param>
        public abstract void Draw();           

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnApplyingEffect()
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
