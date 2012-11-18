﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ColorContent : UIContent
    {
        Color4 _color = SharpDX.Color.Transparent;
        ShaderResourceView _backgroundTextureView = null;
        Texture2D _backgroundTexture = null;
        IntPtr _nativeColorData = IntPtr.Zero;
        bool _textureResourcesCreated = false;

        public ColorContent()
        { }

        public ColorContent(Color4 color)
        {
            _color = color;            
        }

        public Color4 Color 
        { 
            get { return _color; } 
            set { 
                _color = value;
                OnColorChanged(); 
            }         
        }

        public override void Draw(long gameTime)
        {            
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");

            if (_color != SharpDX.Color.Transparent)
            {
                if (!_textureResourcesCreated)
                    CreateTextureResources();
                RectangleF screenRectangle = Container.BoundsToSurface(Container.Bounds);

                if (!HasDrawParameters)
                    Container.ControlSpriteBatch.Begin();
                else
                    Container.ControlSpriteBatch.Begin(SpriteSortMode == null ? CipherPark.AngelJacket.Core.Utils.Interop.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, CustomShaderCallback, TransformationMatrix);
                Container.ControlSpriteBatch.Draw(_backgroundTextureView, screenRectangle.Position(), SharpDX.Color.White);
                Container.ControlSpriteBatch.End();
            }
        }

        protected virtual void OnColorChanged()
        {
            DestroyTextureResources();
        }

        public override Rectangle CalculateSmallestBoundingRect()
        {
            return Rectangle.Empty;
        }

        private void CreateTextureResources()
        {     
            const int SizeOfColorInBytes = sizeof(int);       
            int pixelCount = (int)Container.Size.Width * (int)Container.Size.Height;           
            int colorR8G8B8A8 = _color.ToRgba();            
            int[] textureData = new int[pixelCount];
            int sizeOfTextureInBytes = textureData.Length * SizeOfColorInBytes;
            for (int i = 0; i < pixelCount; i++)
                textureData[i] = colorR8G8B8A8;
            _nativeColorData = Marshal.AllocHGlobal(sizeOfTextureInBytes);
            Marshal.Copy(textureData, 0, _nativeColorData, textureData.Length);
            DataRectangle textureDataRect = new DataRectangle(_nativeColorData, (int)Container.Size.Width * SizeOfColorInBytes);
            textureDataRect.DataPointer = _nativeColorData;
            Texture2DDescription desc = new Texture2DDescription();
            desc.Width = (int)Container.Size.Width;
            desc.Height = (int)Container.Size.Height;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.SampleDescription.Count = 1;
            desc.Usage = ResourceUsage.Dynamic;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.Write;
            _backgroundTexture = new Texture2D(Container.Game.GraphicsDevice, desc, textureDataRect);
            _backgroundTextureView = new ShaderResourceView(Container.Game.GraphicsDevice, _backgroundTexture);
            _textureResourcesCreated = true;
        }

        private void DestroyTextureResources()
        {
            if (_textureResourcesCreated)
            {
                Marshal.FreeHGlobal(_nativeColorData);
                _nativeColorData = IntPtr.Zero;

                _backgroundTextureView.Dispose();
                _backgroundTextureView = null;

                _backgroundTexture.Dispose();
                _backgroundTexture = null;

                _textureResourcesCreated = false;
            }
        }
    }
}
