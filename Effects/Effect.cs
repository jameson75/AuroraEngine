using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public abstract class Effect
    {      
        private Device _graphicsDevice = null;
        private List<EffectPass> _passes = new List<EffectPass>();        

        protected Effect(Device graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }       

        public Device GraphicsDevice { get { return _graphicsDevice; } }

        public List<EffectPass> Passes { get { return _passes; } }

        public virtual void Apply()
        {
            OnBeginApply();
            for (int i = 0; i < _passes.Count; i++)
                _passes[i].Execute();
            OnEndApply();
        }

        protected virtual void OnBeginApply()
        { }

        protected virtual void OnEndApply()
        { }

        protected byte[] LoadVertexShader(string fileName, out VertexShader shader)
        {
            byte[] shaderByteCode = System.IO.File.ReadAllBytes(fileName);
            shader = new VertexShader(GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }

        protected byte[] LoadPixelShader(string fileName, out PixelShader shader)
        {
            byte[] shaderByteCode = System.IO.File.ReadAllBytes(fileName);
            shader = new PixelShader(GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }     
    }

    public abstract class ForwardEffect : Effect
    {
        protected ForwardEffect(Device graphicsDevice)
            : base(graphicsDevice)
        { }

        public virtual Matrix World { get; set; }

        public virtual Matrix View { get; set; }

        public virtual Matrix Projection { get; set; }

        public virtual byte[] SelectShaderByteCode()
        {
            return null;
        }

        public virtual void Restore()
        {
            for (int i = 0; i < Passes.Count; i++)
                ((ForwardEffectPass)Passes[i]).CleanUp();
        }
    }
}
