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
    public class BasicEffect
    {
        private IntPtr _nativeObject = IntPtr.Zero;
        private Device _device = null;
        private Matrix _world = Matrix.Zero;
        private Matrix _view = Matrix.Zero;
        private Matrix _projection = Matrix.Zero;

        public IntPtr NativeObject
        {
            get { return _nativeObject; }
        }

        public BasicEffect(Device device)
        {
            _device = device;
            _nativeObject = UnsafeNativeMethods.New(device.NativePointer);           
        }
       
        public  void Delete()
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
            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_New")]
            public static extern IntPtr New(IntPtr deviceContext);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_Delete")]
            public static extern void Delete(IntPtr basicEffect);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SelectShaderByteCode")]
            public static extern IntPtr SelectShaderByteCode(IntPtr basicEffect, out uint bufferSize);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_Apply")]
            public static extern void Apply(IntPtr basicEffect, IntPtr deviceContext);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetWorld")]
             public static extern void SetWorld(IntPtr basicEffect, float[] m);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetView")]
             public static extern void SetView(IntPtr basicEffect, float[] m);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetProjection")]
             public static extern void SetProjection(IntPtr basicEffect, float[] m);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetDiffuseColor")]
             public static extern void SetDiffuseColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetEmissiveColor")]
             public static extern void SetEmissiveColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetSpecularColor")]
             public static extern void SetSpecularColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetSpecularPower")]
             public static extern void SetSpecularPower(IntPtr basicEffect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetAlpha")]
             public static extern void SetAlpha(IntPtr basicEffect, float value);      

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetLightingEnabled")]
             public static extern void SetLightingEnabled(IntPtr basicEffect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetPerPixelLighting")]
             public static extern void SetPerPixelLighting(IntPtr basicEffect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetAmbientLightColor")]
             public static extern void SetAmbientLightColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetLightEnabled")]
             public static extern void SetLightEnabled(IntPtr basicEffect, int whichLight, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetLightDirection")]
             public static extern void SetLightDirection(IntPtr basicEffect, int whichLight, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetLightDiffuseColor")]
             public static extern void SetLightDiffuseColor(IntPtr basicEffect, int whichLight, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetLightSpecularColor")]
             public static extern void SetLightSpecularColor(IntPtr basicEffect, int whichLight, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_EnableDefaultLighting")]
             public static extern void EnableDefaultLighting(IntPtr basicEffect);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetFogEnabled")]
             public static extern void SetFogEnabled(IntPtr basicEffect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetFogStart")]
             public static extern void SetFogStart(IntPtr basicEffect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetFogEnd")]
             public static extern void SetFogEnd(IntPtr basicEffect, float value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetFogColor")]
             public static extern void SetFogColor(IntPtr basicEffect, XVECTOR4 value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetVertexColorEnabled")]
             public static extern void SetVertexColorEnabled(IntPtr basicEffect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetTextureEnabled")]
             public static extern void SetTextureEnabled(IntPtr basicEffect, bool value);

            [DllImport("AngelJacketNative.dll", EntryPoint="BasicEffect_SetTexture")]
             public static extern void SetTexture(IntPtr basicEffect, IntPtr value);
        }
    }

    public class BasicEffectEx
    {
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private BasicEffect _effect = null;
        private bool _enableVertexColor = false;

        public BasicEffectEx(Device device) 
        {
            _effect = new BasicEffect(device);
        }

        public Matrix World
        {
            get { return _world; }
            set
            {
                _world = value;
                _effect.SetWorld(_world);
            }
        }

        public Matrix View
        {
            get { return _view; }
            set
            {
                _view = value;
                _effect.SetView(_view);
            }
        }

        public Matrix Projection
        {
            get { return _projection; }
            set
            {
                _projection = value;
                _effect.SetProjection(_projection);
            }
        }       

        public bool EnableVertexColor
        {
            get { return _enableVertexColor; }
            set
            {
                _enableVertexColor = value;
                _effect.SetVertexColorEnabled(_enableVertexColor);
            }
        }

        public void Apply()
        {
            _effect.Apply();
        }

        public byte[] SelectShaderByteCode()
        {
            return _effect.SelectShaderByteCode();
        }
    }

    public abstract class PostEffect
    {
        public Device GraphicsDevice { get; private set; }

        public PostEffect(Device graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
        public ShaderResourceView Texture { get; set; }
        public abstract void BeginDraw();
        public abstract void EndDraw();
    }

    public class PassThruPostEffect : PostEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;

        public PassThruPostEffect(Device graphicsDevice)
            : base(graphicsDevice)
        {
            string psFileName = "Content\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Content\\Shaders\\postpassthru-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
        }

        public override void BeginDraw()
        {
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);
        }

        public override void EndDraw()
        {
            //Un-bind Texture from pixel shader input so it can possibly be used later as a render target.
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
        }

        public byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }
    }


    public class PostEffectChain : System.Collections.ObjectModel.ObservableCollection<PostEffect>
    {
        private Texture2D _texture = null;
        private RenderTargetView _textureRenderTarget = null;
        private ShaderResourceView _textureShaderResource = null;
        private Texture2D _auxTexture = null;
        private RenderTargetView _auxTextureRenderTarget = null;
        private ShaderResourceView _auxTextureShaderResource = null;
        private IGameApp _game = null;
        private Matrix _quadTransform = Matrix.Zero;
        private Matrix _viewMatrix = Matrix.Zero;
        private Matrix _projectionMatrix = Matrix.Zero;
        private Mesh _quad = null;
        private RenderTargetView _originalRenderView = null;
        //private DepthStencilView _cachedDepthStencilTarget = null;
        private bool _isEffectInProgress = false;
        private PassThruPostEffect passThruEffect = null;

        private PostEffectChain(IGameApp game)
        {
            _game = game;
        }

        public static PostEffectChain Create(IGameApp game)
        {
            PostEffectChain effectChain = new PostEffectChain(game);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width = game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;
            effectChain._texture = new Texture2D(game.GraphicsDevice, textureDesc);
            effectChain._auxTexture = new Texture2D(game.GraphicsDevice, textureDesc);

            RenderTargetViewDescription targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            effectChain._textureRenderTarget = new RenderTargetView(game.GraphicsDevice, effectChain._texture, targetDesc);
            effectChain._auxTextureRenderTarget = new RenderTargetView(game.GraphicsDevice, effectChain._auxTexture, targetDesc);

            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            effectChain._textureShaderResource = new ShaderResourceView(game.GraphicsDevice, effectChain._texture, resourceDesc);
            effectChain._auxTextureShaderResource = new ShaderResourceView(game.GraphicsDevice, effectChain._auxTexture, resourceDesc);

            effectChain.passThruEffect = new PassThruPostEffect(game.GraphicsDevice);           
            byte[] targetShaderByteCode = effectChain.passThruEffect.SelectShaderByteCode();

            effectChain._quad = ContentBuilder.BuildViewportQuad(game, targetShaderByteCode);
            
            return effectChain;
            //TODO: Validate this method call.
        }

        public void Begin(long gameTime)
        {
            _originalRenderView = _game.GraphicsDeviceContext.OutputMerger.GetRenderTargets(1)[0];
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_textureRenderTarget);
            _game.GraphicsDeviceContext.ClearRenderTargetView(_textureRenderTarget, Color.Black);
            _isEffectInProgress = true;
        }

        public void End(long gameTime)
        {
            //TODO: Validate this method call.   
            
            foreach (PostEffect postEffect in this)
            {
                _game.GraphicsDeviceContext.OutputMerger.SetTargets(_auxTextureRenderTarget);
                _game.GraphicsDeviceContext.ClearRenderTargetView(_auxTextureRenderTarget, Color.Black);
                _game.GraphicsDeviceContext.ClearDepthStencilView(_game.DepthStencil, DepthStencilClearFlags.Depth, 0.0f, 0);
                
                postEffect.Texture = _textureShaderResource;
                postEffect.BeginDraw();
                _quad.Draw(gameTime);
                postEffect.EndDraw();
                               
                Swap<ShaderResourceView>(ref _textureShaderResource, ref _auxTextureShaderResource);
                Swap<RenderTargetView>(ref _textureRenderTarget, ref _auxTextureRenderTarget);
            }
                
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalRenderView);            
            _game.GraphicsDeviceContext.ClearDepthStencilView(_game.DepthStencil, DepthStencilClearFlags.Depth, 0.0f, 0);
            passThruEffect.Texture = _textureShaderResource;
            passThruEffect.BeginDraw();
            _quad.Draw(gameTime);
            passThruEffect.EndDraw();           

            this._isEffectInProgress = false;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }

    public class PlexicNodeEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private Device _graphicsDevice = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;
        
        public Device GraphicsDevice { get { return _graphicsDevice; } }

        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public PlexicNodeEffect(Device graphicsDevice)           
        {
            _graphicsDevice = graphicsDevice;
            ForegroundColor = Color.Transparent;
            BackgroundColor = Color.Transparent;
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            string psFileName = "Content\\Shaders\\plexicnode-ps.cso";
            string vsFileName = "Content\\Shaders\\plexicnode-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16 + sizeof(float) * 4 * 2; //size of WorldViewProj + ForegroundColor + BackgroundColor
            _constantBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public void Apply()
        {
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
        }

        public byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = _graphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix worldViewProjection = this.World * this.View * this.Projection;
            worldViewProjection.Transpose();
            Utilities.Write<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            Vector4 vForegroundColor = ForegroundColor.ToVector4();
            Utilities.Write<Vector4>(dataBox.DataPointer, ref vForegroundColor);
            Vector4 vBackgroundColor = BackgroundColor.ToVector4();
            Utilities.Write<Vector4>(dataBox.DataPointer, ref vBackgroundColor);
            _graphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
