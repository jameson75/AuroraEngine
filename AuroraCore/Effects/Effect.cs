using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using System.IO;
using CipherPark.Aurora.Core.Content;
using System.Linq;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public abstract class Effect : IDisposable
    {   
        protected const int SizeOfMatrix =     64;
        protected const int SizeOfVector4 =    16;
        protected const int SizeOfFloat =      4;
        
        private IGameApp _game = null;
        private List<EffectPass> _passes = new List<EffectPass>();        

        protected Effect(IGameApp game)
        {
            _game = game;
        }       

        public IGameApp Game { get { return _game; } }

        public Device GraphicsDevice { get { return _game.GraphicsDevice; } }

        public List<EffectPass> Passes { get { return _passes; } }

        public bool IsDisposed { get; private set; }

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

        protected virtual void OnDispose()
        { }

        protected byte[] LoadVertexShader(string resourcePath, out VertexShader shader)
        {
            byte[] shaderByteCode = ReadByteStream(resourcePath);
            shader = new VertexShader(Game.GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }

        protected byte[] LoadPixelShader(string resourcePath, out PixelShader shader)
        {
            byte[] shaderByteCode = ReadByteStream(resourcePath);
            shader = new PixelShader(Game.GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }

        protected byte[] LoadGeometryShader(string resourcePath, out GeometryShader shader)
        {
            byte[] shaderByteCode = ReadByteStream(resourcePath);
            shader = new GeometryShader(Game.GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }

        protected byte[] ReadByteStream(string resourcePath)
        {
            return ContentImporter.ReadEmbeddedResource(resourcePath) ??
                   File.ReadAllBytes(resourcePath);
        }

        public void Dispose()
        {
            OnDispose();
            IsDisposed = true;
        }
    }

    public abstract class SurfaceEffect : Effect
    {
        protected SurfaceEffect(IGameApp game)
            : base(game)
        { }

        public virtual Matrix World { get; set; }

        public virtual Matrix View { get; set; }

        public virtual Matrix Projection { get; set; }

        public virtual byte[] GetVertexShaderByteCode()
        {
            return null;
        }

        public virtual void Restore()
        {
            for (int i = 0; i < Passes.Count; i++)
                ((SurfaceEffectPass)Passes[i]).Reset();
        }

        public virtual EffectDataChannels GetDataChannels()
        {
            return EffectDataChannels.None;
        }

        public virtual SurfaceVertexType GetSurfaceVertexType()
        {
            return SurfaceVertexType.None;
        }

        public static EffectDataChannels GetDataChannelsForSurfaceVertexType(SurfaceVertexType surfaceVertexType)
        {
            var effectDataChannels = EffectDataChannels.Position;
            switch (surfaceVertexType)
            {
                case SurfaceVertexType.PositionColor:
                case SurfaceVertexType.InstancePositionColor:
                    effectDataChannels |= EffectDataChannels.Color;
                    break;
                case SurfaceVertexType.PositionNormalColor:
                case SurfaceVertexType.InstancePositionNormalColor:
                    effectDataChannels |= EffectDataChannels.Color | EffectDataChannels.Normal;
                    break;
                case SurfaceVertexType.PositionNormalTexture:
                case SurfaceVertexType.InstancePositionNormalTexture:
                    effectDataChannels |= EffectDataChannels.Normal | EffectDataChannels.Texture;
                    break;
                case SurfaceVertexType.PositionTexture:
                case SurfaceVertexType.InstancePositionTexture:
                    effectDataChannels |= EffectDataChannels.Texture;
                    break;
                case SurfaceVertexType.SkinColor:
                    effectDataChannels |= EffectDataChannels.Skinning | EffectDataChannels.Color;
                    break;
                case SurfaceVertexType.SkinNormalColor:
                    effectDataChannels |= EffectDataChannels.Skinning | EffectDataChannels.Normal;
                    break;
                case SurfaceVertexType.SkinNormalTexture:
                    effectDataChannels |= EffectDataChannels.Skinning | EffectDataChannels.Normal | EffectDataChannels.Texture;
                    break;
                case SurfaceVertexType.SkinTexture:
                    effectDataChannels |= EffectDataChannels.Skinning | EffectDataChannels.Texture;
                    break;
            }

            if (new[] {
                SurfaceVertexType.InstancePositionColor,
                SurfaceVertexType.InstancePositionNormalColor,
                SurfaceVertexType.InstancePositionNormalTexture,
                SurfaceVertexType.InstancePositionTexture }.Contains(surfaceVertexType))
            {
                effectDataChannels |= EffectDataChannels.Instancing;
            }

            return effectDataChannels;
        }
    }
}
