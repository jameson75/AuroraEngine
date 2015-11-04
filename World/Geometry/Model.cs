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
        public abstract void Draw(GameTime gameTime);           

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

        public override void Draw(GameTime gameTime)
        {
            if (Effect != null)
            {                
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
}
