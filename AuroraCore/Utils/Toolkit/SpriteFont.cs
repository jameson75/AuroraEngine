using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine
// This source code is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils.Toolkit
{
    public class SpriteFont
    {
        private IntPtr _nativeObject = IntPtr.Zero;
           
        public SpriteFont(Device graphicsDevice, string fileName)
        {
            _nativeObject = UnsafeNativeMethods.New(graphicsDevice.NativePointer, fileName);
        }

        public SpriteFont(Device graphicsDevice, byte[] blob)
        {
            _nativeObject = UnsafeNativeMethods.New(graphicsDevice.NativePointer, blob, blob.Length);
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

        public Size2F MeasureString(string text)
        {
            XVECTOR4 metrics = UnsafeNativeMethods.MeasureString(this._nativeObject, text);
            //NOTE: The DirectXTK SpriteFont::MeasureString() doesn't account for trailings
            //spaces. We call our helper method for a work around.
            float trailingSpacesLength = MeasureTrailingSpaces(text);
            return new Size2F(metrics.C1 + trailingSpacesLength, metrics.C2);
        }

        public bool ContainsCharacter(char character)
        {
            return UnsafeNativeMethods.ContainsCharacter(this._nativeObject, character);
        }

        private float MeasureTrailingSpaces(string text)
        {
            float trailingSpacesLength = 0;
            if (text.EndsWith(" "))
            {
                StringBuilder trailingSpaces = new StringBuilder();
                if (text.Trim().Length != 0)
                {
                    int lastNonSpaceIndex = text.LastIndexOf(text.LastOrDefault((c => c != ' ')));
                    trailingSpaces.Append(text.Substring(lastNonSpaceIndex + 1));
                }
                else
                    trailingSpaces.Append(text);
                string arbString = "W";
                StringBuilder trailSpacesArb = new StringBuilder().Append(trailingSpaces).Append(arbString);
                XVECTOR4 arbMetrics = UnsafeNativeMethods.MeasureString(this._nativeObject, arbString);
                XVECTOR4 trailingSpacesArbMetrics = UnsafeNativeMethods.MeasureString(this._nativeObject, trailSpacesArb.ToString());
                trailingSpacesLength = trailingSpacesArbMetrics.C1 - arbMetrics.C1;
            }
            return trailingSpacesLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Glyph
        {
            uint Character;
            long Subrect_Left;
            long Subrect_Top;
            long Subrect_Right;
            long Subrect_Bottom;
            float XOffset;
            float YOffset;
            float XAdvance;
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_New")]
            public static extern IntPtr New(IntPtr NativeDevicePointer, [MarshalAs(UnmanagedType.LPTStr)] string fileName);

            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_New_2")]
            public static extern IntPtr New(IntPtr nativeDevicePointer, [MarshalAs(UnmanagedType.LPArray)] byte[] blob, int dataSize);

            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_New_3")]
            public static extern IntPtr New(IntPtr nativeTextureShaderResourcePointer, [MarshalAs(UnmanagedType.LPArray)] Glyph[] glyphs, int glyphCount, float lineSpacing);

            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_DrawString")]
            public static extern void DrawString(IntPtr nativeSpriteFontPointer, IntPtr nativeSpriteBatchPointer, [MarshalAs(UnmanagedType.LPTStr)] string text, XMFLOAT2 position, XVECTOR4 color, float rotation, XMFLOAT2 origin, XMFLOAT2 scale, SpriteEffects effects, float layerDepth);

            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_MeasureString")]
            public static extern XVECTOR4 MeasureString(IntPtr nativeSpriteFontPointer, [MarshalAs(UnmanagedType.LPTStr)] string text);

            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_ContainsCharacter")]
            public static extern bool ContainsCharacter(IntPtr nativeSpriteFontPointer, char character);

            [DllImport("AuroraNative.dll", EntryPoint = "SpriteFont_Delete")]
            public static extern void Delete(IntPtr nativeSpriteFontPointer);
        }
    }
}
