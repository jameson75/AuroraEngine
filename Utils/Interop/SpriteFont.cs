using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.Utils.Interop
{
    public class SpriteFont
    {
        private IntPtr _nativeObject = IntPtr.Zero;
           
        public SpriteFont(Device graphicsDevice, string textureFileName)
        {
            _nativeObject = UnsafeNativeMethods.New(graphicsDevice.NativePointer, textureFileName);
        }

        ~SpriteFont()
        {
            Delete();
        }

        public IntPtr NativeObject
        {
            get { return _nativeObject; }
        }

        private void Delete()
        {
            if (_nativeObject != IntPtr.Zero)
            {
                UnsafeNativeMethods.Delete(_nativeObject);
                _nativeObject = IntPtr.Zero;
            }
        }

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color4 color)
        {
            DrawString(spriteBatch, text, position, color, 0, Vector2.Zero, Vector2.One);
        }
    
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color4 color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0)
        {
            UnsafeNativeMethods.DrawString(this.NativeObject, spriteBatch.NativeObject, text, new XMFLOAT2(position), new XVECTOR4(color), rotation, new XMFLOAT2(origin), new XMFLOAT2(scale), effects, layerDepth);
        }

        public DrawingSizeF MeasureString(string text)
        {
            XVECTOR4 metrics = UnsafeNativeMethods.MeasureString(this._nativeObject, text);
            return new DrawingSizeF(metrics.C1, metrics.C2);
        }

        public bool ContainsCharacter(char character)
        {
            return UnsafeNativeMethods.ContainsCharacter(this._nativeObject, character);
        }    

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_New")]
            public static extern IntPtr New(IntPtr NativeDevicePointer, [MarshalAs(UnmanagedType.LPTStr)] string fileName);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_New_2")]
            public static extern IntPtr New(IntPtr nativeDevicePointer, IntPtr blob, int dataSize);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_New_3")]
            public static extern IntPtr New(IntPtr nativeTextureShaderResourcePointer, IntPtr glyphCount, float lineSpacing);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_DrawString")]
            public static extern void DrawString(IntPtr nativeSpriteFontPointer, IntPtr nativeSpriteBatchPointer, [MarshalAs(UnmanagedType.LPTStr)] string text, XMFLOAT2 position, XVECTOR4 color, float rotation, XMFLOAT2 origin, XMFLOAT2 scale, SpriteEffects effects, float layerDepth);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_MeasureString")]
            public static extern XVECTOR4 MeasureString(IntPtr nativeSpriteFontPointer, [MarshalAs(UnmanagedType.LPTStr)] string text);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_ContainsCharacter")]
            public static extern bool ContainsCharacter(IntPtr nativeSpriteFontPointer, char character);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteFont_Delete")]
            public static extern void Delete(IntPtr nativeSpriteFontPointer);
        }
    }
}
