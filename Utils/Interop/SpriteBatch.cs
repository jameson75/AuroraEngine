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
    public class SpriteBatch
    {
        private IntPtr _nativeObject = IntPtr.Zero;
        
        public IntPtr NativeObject
        {
            get { return _nativeObject; }
        }

        public SpriteBatch(DeviceContext deviceContext)
        {
            _nativeObject = UnsafeNativeMethods.New(deviceContext.NativePointer);
        }

        public void Begin(SpriteSortMode mode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Action customShaderCallback, Matrix? transformationMatrix)
        {            
            float[] _transformationMatrix = (transformationMatrix.HasValue) ? transformationMatrix.Value.ToArray() : Matrix.Identity.ToArray();
            IntPtr customShaderFunc = (customShaderCallback != null) ? Marshal.GetFunctionPointerForDelegate(customShaderCallback) : IntPtr.Zero;
            UnsafeNativeMethods.Begin(this._nativeObject, mode, blendState.NativePointer, samplerState.NativePointer, depthStencilState.NativePointer, rasterizerState.NativePointer, customShaderFunc, _transformationMatrix);
        }

        public void Draw(ShaderResourceView texture, Vector2 position, Rectangle? sourceRectangle, Color4 color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            IntPtr rectPtr = IntPtr.Zero;
            if(sourceRectangle.HasValue)
            {
                WIN32_RECT rect = new WIN32_RECT(sourceRectangle.Value);
                rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                Marshal.StructureToPtr(rect, rectPtr, false);
            }
            UnsafeNativeMethods.Draw(this._nativeObject, texture.NativePointer, new XMFLOAT2(position), rectPtr, new XVECTOR4(color), rotation, new XMFLOAT2(origin), new XMFLOAT2(scale), effects, layerDepth);
            if( rectPtr != IntPtr.Zero )
            {
                Marshal.FreeHGlobal(rectPtr);
                rectPtr = IntPtr.Zero;
            }
        }

        public void Draw(ShaderResourceView texture, Vector2 position, Color4 color)
        {          
            Draw(texture, position, null, color, 0, XMFLOAT2.Zero, XMFLOAT2.Unit, SpriteEffects.None, 0);
        }

        public void Draw(ShaderResourceView texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color4 color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {  
            IntPtr rectPtr = IntPtr.Zero;
            if (sourceRectangle.HasValue)
            {
                WIN32_RECT rect = new WIN32_RECT(sourceRectangle.Value);
                rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                Marshal.StructureToPtr(rect, rectPtr, false);
            }
            UnsafeNativeMethods.Draw(this._nativeObject, texture.NativePointer, new WIN32_RECT(destinationRectangle), rectPtr, new XVECTOR4(color), rotation, new XMFLOAT2(origin), effects, layerDepth);
            if (rectPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(rectPtr);
                rectPtr = IntPtr.Zero;
            }
        }

        public void Draw(ShaderResourceView texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color4 color)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawText(SpriteFont font, string text, Vector2 position, Color4 color)
        {
            font.DrawString(this, text, position, color);
        }

        public void DrawText(SpriteFont font, string text, Vector2 position, Color4 color, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0)
        {
            font.DrawString(this, text, position, color, rotation, origin, effects, layerDepth);
        }

        public void End()
        {
            UnsafeNativeMethods.End(this._nativeObject);
        }

        ~SpriteBatch()
        {
            Delete();
        }

        private void Delete()
        {
            if (this._nativeObject != IntPtr.Zero)
            {
                UnsafeNativeMethods.Delete(this._nativeObject);
                this._nativeObject = IntPtr.Zero;
            }
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint="SpriteBatch_New")]
            public static extern IntPtr New(IntPtr deviceContext);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteBatch_Begin")]
            public static extern void Begin(IntPtr nativeSpriteBatch, SpriteSortMode sortMode, IntPtr blendState, IntPtr samplerState, IntPtr depthStencilState, IntPtr rasterizerState, IntPtr customShaderFunction, [MarshalAs(UnmanagedType.LPArray, SizeConst=16)] float[] transformationMatrix);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteBatch_Draw")]
            public static extern void Draw(IntPtr nativeSpriteBatch, IntPtr texture, XMFLOAT2 position, IntPtr sourceRectangle, XVECTOR4 color, float rotation, XMFLOAT2 origin, XMFLOAT2 scale, SpriteEffects effects, float layerDepth);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SpriteBatch_Draw")]
            public static extern void Draw(IntPtr nativeSpriteBatch, IntPtr texture, WIN32_RECT destinationRectangle, IntPtr sourceRectangle, XVECTOR4 color, float rotation, XMFLOAT2 origin, SpriteEffects effects, float layerDepth); 

            [DllImport("AngelJacketNative.dll", EntryPoint = "End")]
            public static extern void End(IntPtr nativeSpriteBatch);

            [DllImport("AngelJacketNative.dll", EntryPoint = "Delete")]
            public static extern void Delete(IntPtr nativeSpriteBatch);
        }
    }

    public enum SpriteSortMode
    {
        Deferred,
        Immediate,
        Texture,
        BackToFront,
        FrontToBack,
    };

    [Flags]
    public enum SpriteEffects
    {
        None = 0,
        FlipHorizontally = 1,
        FlipVertically = 2,
        FlipBoth = FlipHorizontally | FlipVertically,
    };
}
