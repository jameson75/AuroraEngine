using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public interface ISkinEffect
    {
        Matrix[] BoneTransforms { get; set; }
    }
    public class SkinnedEffectEx : Effect, ISkinEffect
    {
        private SkinnedEffect _effect = null;
        private int _weightsPerVertex = 0;

        public SkinnedEffectEx(Device graphicsDevice)
            : base(graphicsDevice)
        {
            _effect = new SkinnedEffect(graphicsDevice);
        }

        public override Matrix World
        {
            get
            {
                return base.World;
            }
            set
            {
                base.World = value;
                _effect.SetWorld(value);
            }
        }

        public override Matrix View
        {
            get
            {
                return base.View;
            }
            set
            {
                base.View = value;
                _effect.SetView(value);
            }
        }

        public override Matrix Projection
        {
            get
            {
                return base.Projection;
            }
            set
            {
                base.Projection = value;
                _effect.SetProjection(value);
            }
        }

        public Matrix[] BoneTransforms { get; set; }

        public int WeightsPerVertex
        {
            get { return _weightsPerVertex; }
            set
            {
                //******************************************************************************
                //NOTE: We set this parameter to the effect immediately 
                //(instead of doing lazily in the Apply() method) because
                //the parameter determines the bytecode returned from SelectShaderByteCode()
                //******************************************************************************
                _effect.SetWeightsPerVertex(value);
                _weightsPerVertex = value;
            }
        }

        public void EnableDefaultLighting()
        {
            _effect.EnableDefaultLighting();
        }

        public void SetTexture(ShaderResourceView resourceView)
        {
            _effect.SetTexture(resourceView);
        }

        public override byte[] SelectShaderByteCode()
        {
            return _effect.SelectShaderByteCode();
        }

        public override void Apply()
        {
            _effect.SetWorld(World);
            _effect.SetView(View);
            _effect.SetProjection(Projection);  
            if(BoneTransforms != null)
                _effect.SetBoneTransforms(BoneTransforms);
            _effect.Apply();
        }
    }
}
