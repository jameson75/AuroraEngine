using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX.Direct3D9;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ColorContent : UIContent
    {
        Color4 _color = SharpDX.Color.Transparent;
        ShaderResourceView _backgroundTextureView = null;
        Texture2D _backgroundTexture = null;
        IntPtr _nativeColorData = IntPtr.Zero;
        bool _textureResourcesCreated = false;
        readonly Size2F DefaultTextureSize = new Size2F(100.0f, 100.0f);

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

        public override void Draw(GameTime gameTime)
        {            
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");

            if (_color != SharpDX.Color.Transparent)
            {
                if (!_textureResourcesCreated)
                    CreateTextureResources();
                Vector2 contentSurfacePosition = Container.PositionToSurface(Container.Position);
                float scaleX = Container.Bounds.Width / DefaultTextureSize.Width;
                float scaleY = Container.Bounds.Height / DefaultTextureSize.Height;
                TransformationMatrix = Matrix.Transformation2D(Vector2.Zero,
                                                               0, new Vector2(scaleX, scaleY),
                                                               Vector2.Zero,
                                                               0, contentSurfacePosition);
                
                this.BeginDraw();
                //******************************************************************************************************************************
                //NOTE: We use the transformation in ControlSpriteBatch.Begin() to specify the translation of the drawn texture, so
                //we specify Vector.Zero as the position parameter in the ControlSpriteBatch.Draw() call to avoid "doubling" the translation
                //******************************************************************************************************************************             
                Container.ControlSpriteBatch.Draw(_backgroundTextureView, Vector2.Zero, Color);                                
                this.EndDraw();
            }
        }

        public override RectangleF CalculateSmallestBoundingRect()
        {
            return RectangleF.Empty;
        }

        public override void ApplyStyle(Components.UIStyle style)
        {
            Components.ColorStyle colorStyle = style as Components.ColorStyle;
            if (colorStyle == null)
                throw new ArgumentException("Template was not of type ColorContentTemplate", "template");

            if (colorStyle.Color != SharpDX.Color.Transparent)
                this.Color = colorStyle.Color.Value;

            base.ApplyStyle(style);
        }

        protected virtual void OnColorChanged()
        {
            //TODO: Refactor this content class so that changing the color 
            //doesn't destroy the texture... (ie: tint a texture that's always white, insted).
            //DestroyTextureResources();
        }

        private void CreateTextureResources()
        {     
            const int SizeOfColorInBytes = sizeof(int);       
            //int pixelCount = (int)Container.Size.Width * (int)Container.Size.Height;  
            int pixelCount = (int)DefaultTextureSize.Width * (int)DefaultTextureSize.Height;
            //int colorR8G8B8A8 = _color.ToRgba();            
            int colorR8G8B8A8 = SharpDX.Color.White.ToRgba();
            int[] textureData = new int[pixelCount];
            int sizeOfTextureInBytes = textureData.Length * SizeOfColorInBytes;
            for (int i = 0; i < pixelCount; i++)
                textureData[i] = colorR8G8B8A8;
            _nativeColorData = Marshal.AllocHGlobal(sizeOfTextureInBytes);
            Marshal.Copy(textureData, 0, _nativeColorData, textureData.Length);
            //DataRectangle textureDataRect = new DataRectangle(_nativeColorData, (int)Container.Size.Width * SizeOfColorInBytes);
            DataRectangle textureDataRect = new DataRectangle(_nativeColorData, (int)DefaultTextureSize.Width * SizeOfColorInBytes);
            textureDataRect.DataPointer = _nativeColorData;
            Texture2DDescription desc = new Texture2DDescription();
            //desc.Width = (int)Container.Size.Width;
            //desc.Height = (int)Container.Size.Height;
            desc.Width = (int)DefaultTextureSize.Width;
            desc.Height = (int)DefaultTextureSize.Height;
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
        
        //private void DestroyTextureResources()
        //{
        //    if (_textureResourcesCreated)
        //    {
        //        Marshal.FreeHGlobal(_nativeColorData);
        //        _nativeColorData = IntPtr.Zero;

        //        _backgroundTextureView.Dispose();
        //        _backgroundTextureView = null;

        //        _backgroundTexture.Dispose();
        //        _backgroundTexture = null;

        //        _textureResourcesCreated = false;
        //    }
        //}      
    }
}
