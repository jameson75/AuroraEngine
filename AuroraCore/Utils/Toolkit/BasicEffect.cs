using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine
// This source code is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils.Toolkit
{
    public class BasicEffect
    {
        private IntPtr _nativeObject = IntPtr.Zero;
        private Device _device = null;
      
        public IntPtr NativeObject
        {
            get { return _nativeObject; }
        }

        public BasicEffect(Device device)
        {
            _device = device;
            _nativeObject = UnsafeNativeMethods.New(device.NativePointer);           
        }

        ~BasicEffect()
        {
            Delete();
        }

        private void Delete()
        {
            if (_nativeObject != IntPtr.Zero)
            {
                UnsafeNativeMethods.Delete(_nativeObject);
                _nativeObject = IntPtr.Zero;
            }
        }

        public void Apply()
        {
            UnsafeNativeMethods.Apply(_nativeObject, _device.ImmediateContext.NativePointer);
        }       
       
        public byte[] SelectShaderByteCode()
        {
            IntPtr bytePtr = IntPtr.Zero;
            byte[] bytes = null;
            uint sizeRef = 0;
            bytePtr = UnsafeNativeMethods.SelectShaderByteCode(_nativeObject, out sizeRef);
            if( bytePtr != IntPtr.Zero )
            {
                bytes = new byte[sizeRef];
                Marshal.Copy(bytePtr, bytes, 0, (int)sizeRef);
            }
            return bytes;
        }       

        public void SetWorld(Matrix world)
        {
            UnsafeNativeMethods.SetWorld(_nativeObject, world.ToArray());
        }
       
        public void SetView(Matrix world)
        {
            UnsafeNativeMethods.SetView(_nativeObject, world.ToArray());
        }
        
        public void SetProjection(Matrix projection)
        {
            UnsafeNativeMethods.SetProjection(_nativeObject, projection.ToArray());
        }
        
        public void SetDiffuseColor(Color color)
        {
            UnsafeNativeMethods.SetDiffuseColor(_nativeObject, new XVECTOR4(color));
        }
        
        public void SetEmissiveColor(Color color)
        {
            UnsafeNativeMethods.SetEmissiveColor(_nativeObject, new XVECTOR4(color));
        }
       
        public void SetSpecularColor(Color color)
        {
            UnsafeNativeMethods.SetSpecularColor(_nativeObject, new XVECTOR4(color));
        }
        
        public void SetSpecularPower(float value)
        {
            UnsafeNativeMethods.SetSpecularPower(_nativeObject, value);
        }

        public void SetAlpha(float value)
        {
            UnsafeNativeMethods.SetAlpha(_nativeObject, value);
        }
        
        public void SetLightingEnabled(bool value)
        {
            UnsafeNativeMethods.SetLightingEnabled(_nativeObject, value);
        }
        
        public void SetPerPixelLighting(bool value)
        {
            UnsafeNativeMethods.SetPerPixelLighting(_nativeObject, value);
        }
        
        public void SetAmbientLightColor(Color color)
        {
            UnsafeNativeMethods.SetAmbientLightColor(_nativeObject, new XVECTOR4(color));
        }
       
        public void SetLightEnabled(int whichLight, bool value)
        {
            UnsafeNativeMethods.SetLightEnabled(_nativeObject, whichLight, value);
        }
       
        public void SetLightDirection(int whichLight, Vector3 direction)
        {
            UnsafeNativeMethods.SetLightDirection(_nativeObject, whichLight, new XVECTOR4(direction.X, direction.Y, direction.Z, 0.0f));
        }
        
        public void SetLightDiffuseColor(int whichLight, Color color)
        {
            UnsafeNativeMethods.SetLightDiffuseColor(_nativeObject, whichLight, new XVECTOR4(color));
        }
        
        public void SetLightSpecularColor(int whichLight, Color color)
        {
            UnsafeNativeMethods.SetLightSpecularColor(_nativeObject, whichLight, new XVECTOR4(color));
        }
        
        public void EnableDefaultLighting()
        {
            UnsafeNativeMethods.EnableDefaultLighting(_nativeObject);
        }
      
        public  void SetFogEnabled(bool value)
        {
            UnsafeNativeMethods.SetFogEnabled(_nativeObject, value);
        }
        
        public  void SetFogStart(float value)
        {
            UnsafeNativeMethods.SetFogStart(_nativeObject, value);
        }

        public void SetFogEnd(float value)
        {
            UnsafeNativeMethods.SetFogEnd(_nativeObject, value);
        }
        
        public void SetFogColor(Color color)
        {
            UnsafeNativeMethods.SetFogColor(_nativeObject, new XVECTOR4(color));
        }
        
        public void SetVertexColorEnabled(bool value)
        {
            UnsafeNativeMethods.SetVertexColorEnabled(_nativeObject, value);
        }

        public void SetTextureEnabled(bool value)
        {
            UnsafeNativeMethods.SetTextureEnabled(_nativeObject, value);
        }
        
        public void SetTexture( ShaderResourceView resourceView)
        {
            UnsafeNativeMethods.SetTexture(_nativeObject, resourceView.NativePointer);
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_New")]
            public static extern IntPtr New(IntPtr deviceContext);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_Delete")]
            public static extern void Delete(IntPtr basicEffect);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SelectShaderByteCode")]
            public static extern IntPtr SelectShaderByteCode(IntPtr basicEffect, out uint bufferSize);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_Apply")]
            public static extern void Apply(IntPtr basicEffect, IntPtr deviceContext);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetWorld")]
             public static extern void SetWorld(IntPtr basicEffect, float[] m);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetView")]
             public static extern void SetView(IntPtr basicEffect, float[] m);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetProjection")]
             public static extern void SetProjection(IntPtr basicEffect, float[] m);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetDiffuseColor")]
             public static extern void SetDiffuseColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetEmissiveColor")]
             public static extern void SetEmissiveColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetSpecularColor")]
             public static extern void SetSpecularColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetSpecularPower")]
             public static extern void SetSpecularPower(IntPtr basicEffect, float value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetAlpha")]
             public static extern void SetAlpha(IntPtr basicEffect, float value);      

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetLightingEnabled")]
             public static extern void SetLightingEnabled(IntPtr basicEffect, bool value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetPerPixelLighting")]
             public static extern void SetPerPixelLighting(IntPtr basicEffect, bool value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetAmbientLightColor")]
             public static extern void SetAmbientLightColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetLightEnabled")]
             public static extern void SetLightEnabled(IntPtr basicEffect, int whichLight, bool value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetLightDirection")]
             public static extern void SetLightDirection(IntPtr basicEffect, int whichLight, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetLightDiffuseColor")]
             public static extern void SetLightDiffuseColor(IntPtr basicEffect, int whichLight, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetLightSpecularColor")]
             public static extern void SetLightSpecularColor(IntPtr basicEffect, int whichLight, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_EnableDefaultLighting")]
             public static extern void EnableDefaultLighting(IntPtr basicEffect);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetFogEnabled")]
             public static extern void SetFogEnabled(IntPtr basicEffect, bool value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetFogStart")]
             public static extern void SetFogStart(IntPtr basicEffect, float value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetFogEnd")]
             public static extern void SetFogEnd(IntPtr basicEffect, float value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetFogColor")]
             public static extern void SetFogColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetVertexColorEnabled")]
             public static extern void SetVertexColorEnabled(IntPtr basicEffect, bool value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetTextureEnabled")]
             public static extern void SetTextureEnabled(IntPtr basicEffect, bool value);

            [DllImport("AuroraNative.dll", EntryPoint="BasicEffect_SetTexture")]
             public static extern void SetTexture(IntPtr basicEffect, IntPtr value);
        }
    }      
}
