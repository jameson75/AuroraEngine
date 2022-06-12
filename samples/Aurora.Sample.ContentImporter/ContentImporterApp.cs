using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
///////////////////////////////////////////////////////////////////////////////

namespace Aurora.Sample.ContentImporter
{
    public class ContentImporterApp : GameAppWinforms
    {
        private IInputService _inputService;
        private XAudio2StreamingManager _backgroundMusic; 
        private Model msMarvelModel;
       
        public ContentImporterApp()
        {           
            //Services
            //--------
            _inputService = new InputService(this);

            ViewportSizeChanged += ContentImporterApp_ViewportSizeChanged;
        }        

        protected override void Initialize()
        {            
            //Register services
            //-----------------
            Services.RegisterService(_inputService);            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadMsMarvelContent();

            LoadBackgroundMusicContent();

            base.LoadContent();
        }

        protected override void Update()
        {                      
            //Update game time.
            GameTime.Update(); //TODO: Lift this action up to the parent class.
            
            _inputService.UpdateState();            

            base.Update();
        }

        protected override void Draw()
        {           
            GraphicsDeviceContext.ClearRenderTargetView(RenderTargetView, SharpDX.Color.Black);
            GraphicsDeviceContext.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth, 1.0f, 0);

            msMarvelModel?.Draw();

            SwapChain.Present(0, SharpDX.DXGI.PresentFlags.None);                
            base.Draw();
        }

        protected override void UnloadContent()
        {
            msMarvelModel.Dispose();
            msMarvelModel = null;

            _backgroundMusic?.Stop();
            _backgroundMusic?.Dispose();
            _backgroundMusic = null;
          
            base.UnloadContent();
        }

        private Matrix CalculateProjection()
        {
            var viewport = GraphicsDevice.ImmediateContext.Rasterizer.GetViewports<ViewportF>()[0];
            return Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(60), viewport.Width / viewport.Height, 0.1f, 10000.0f);
        }

        private void ContentImporterApp_ViewportSizeChanged()
        {
            if (msMarvelModel != null)
                msMarvelModel.Effect.Projection = CalculateProjection();
        }        

        private void LoadMsMarvelContent()
        {
            BlinnPhongEffect2 modelEffect = new BlinnPhongEffect2(this, SurfaceVertexType.SkinNormalTexture);
            modelEffect.LampColorArray[0] = new Color(192.0f / 255, 191.0f / 255, 173.0f / 255, 1.0f);
            modelEffect.AmbientColor = Color.White;
            modelEffect.LampDirPosArray[0] = Vector3.UnitY * 150.0f;
            modelEffect.SpecularPower = 0.4f;
            modelEffect.Eccentricity = 0.3f;
            string modelTexturePath = @"Assets\Textures\MsMarvel.png";
            var modelTexture = CipherPark.Aurora.Core.Content.ContentImporter
                .LoadTexture(GraphicsDeviceContext, modelTexturePath)
                .ToShaderResourceView(GraphicsDevice);
            modelEffect.Texture = modelTexture;
            var modelFilePath = @"Assets\Models\MsMarvel.X";
            var model = CipherPark.Aurora.Core.Content.ContentImporter.ImportX(
                this,
                modelFilePath,
                modelEffect.GetVertexShaderByteCode(),
                XFileChannels.CommonAlinRiggedLitTexture1);
            model.Effect = modelEffect;
            model.Effect.World = Matrix.Identity;
            var eyeLevel = 100;
            model.Effect.View = Matrix.LookAtLH(new Vector3(0, eyeLevel, -200), new Vector3(0, eyeLevel, 0), Vector3.UnitY);
            model.Effect.Projection = CalculateProjection();
            msMarvelModel = model;
        }

        private void LoadBackgroundMusicContent()
        {
            _backgroundMusic = CipherPark.Aurora.Core.Content.ContentImporter.LoadStreamingVoice(
                AudioDevice,
                @"Assets\Music\LetsMakeIt.wma");

            _backgroundMusic.Start();
        }
    }
}
