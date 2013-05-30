using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;

namespace CipherPark.AngelJacket.Core.Utils.Toolkit
{
    public class BasicSkinnedEffect
    {
        private IntPtr _nativeObject = IntPtr.Zero;
        private Device _device = null;

        public IntPtr NativeObject
        {
            get { return _nativeObject; }
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
        
        public void SetTexture( ShaderResourceView resourceView)
        {
            UnsafeNativeMethods.SetTexture(_nativeObject, resourceView.NativePointer);
        }

        public void SetWeightsPerVertex(int nWeights)
        {
            UnsafeNativeMethods.SetWeightsPerVertex(_nativeObject, nWeights);
        }

        public void SetBoneTransforms(Matrix[] bones)
        {
            IntPtr _bonesPtr = Marshal.AllocHGlobal(bones.Length * sizeof(float) * 16);
            for (int i = 0; i < bones.Length; )
            {
                float[] boneValues = bones[i].ToArray();
                for (int j = 0; j < boneValues.Length; j++)
                {
                    byte[] valueBytes = BitConverter.GetBytes(boneValues[i]);
                    for (int k = 0; k < valueBytes.Length; k++)
                    {
                        //**********************************************************************************
                        //We're assuming boneValues.Length is always 16 and valueBytes.Length is always 4.
                        //**********************************************************************************
                        Marshal.WriteByte(_bonesPtr, i * 16 + j * 4 + k, valueBytes[k]);
                    }
                }
            }
            UnsafeNativeMethods.SetBoneTransforms(_nativeObject, _bonesPtr, bones.Length);
            Marshal.FreeHGlobal(_bonesPtr);
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_New")]
            public static extern IntPtr New(IntPtr deviceContext);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_Delete")]
            public static extern void Delete(IntPtr skinnedEffect);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SelectShaderByteCode")]
            public static extern IntPtr SelectShaderByteCode(IntPtr skinnedEfect, out uint bufferSize);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_Apply")]
            public static extern void Apply(IntPtr skinnedEfect, IntPtr deviceContext);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetWorld")]
            public static extern void SetWorld(IntPtr skinnedEfect, float[] m);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetView")]
            public static extern void SetView(IntPtr skinnedEfect, float[] m);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetProjection")]
            public static extern void SetProjection(IntPtr skinnedEfect, float[] m);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetDiffuseColor")]
            public static extern void SetDiffuseColor(IntPtr skinnedEfect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetEmissiveColor")]
            public static extern void SetEmissiveColor(IntPtr skinnedEfect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetSpecularColor")]
            public static extern void SetSpecularColor(IntPtr skinnedEfect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetSpecularPower")]
            public static extern void SetSpecularPower(IntPtr skinnedEfect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetAlpha")]
            public static extern void SetAlpha(IntPtr skinnedEfect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetPerPixelLighting")]
            public static extern void SetPerPixelLighting(IntPtr skinnedEfect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetAmbientLightColor")]
            public static extern void SetAmbientLightColor(IntPtr skinnedEfect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetLightEnabled")]
            public static extern void SetLightEnabled(IntPtr skinnedEfect, int whichLight, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetLightDirection")]
            public static extern void SetLightDirection(IntPtr skinnedEfect, int whichLight, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetLightDiffuseColor")]
            public static extern void SetLightDiffuseColor(IntPtr skinnedEfect, int whichLight, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetLightSpecularColor")]
            public static extern void SetLightSpecularColor(IntPtr skinnedEfect, int whichLight, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_EnableDefaultLighting")]
            public static extern void EnableDefaultLighting(IntPtr skinnedEfect);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetFogEnabled")]
            public static extern void SetFogEnabled(IntPtr skinnedEfect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetFogStart")]
            public static extern void SetFogStart(IntPtr skinnedEfect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetFogEnd")]
            public static extern void SetFogEnd(IntPtr skinnedEfect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetFogColor")]
            public static extern void SetFogColor(IntPtr skinnedEfect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetTexture")]
            public static extern void SetTexture(IntPtr skinnedEfect, IntPtr value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetWeightsPerVertexTexture")]
            public static extern void SetWeightsPerVertex(IntPtr skinnedEfect, int value);

            [DllImport("AngelJacketNative.dll", EntryPoint = "SkinnedEffect_SetBoneTransforms")]
            public static extern void SetBoneTransforms(IntPtr skinnedEfect, IntPtr bones, int count);
        }
    }
}
