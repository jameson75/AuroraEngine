using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World.Geometry;
using SharpDX;
using SharpDX.Direct3D11;

namespace Aurora.Sample.ContentBuilder
{
    public class ContentBuilderGameApp : GameAppWPF
    {
        private SurfaceEffect contentEffect;
        private Mesh content;

        public UITree UI { get; private set; }

        public ContentBuilderGameApp()
        {
            ViewportSizeChanged += ContentBuilderGameApp_ViewportSizeChanged;
        }        

        protected override void OnInitializing()
        {
            //Register game app services.                            
            Services.RegisterService(new InputService(this)); //Required For Graphics UI
        }

        protected override void OnInitialized()
        {
            UI = new UITree(this);
            UI.Theme = new SampleGameAppTheme(this);

            DisplayFlatTriangle();           
        }

        protected override void OnUpdate()
        {
            //Update the input service (poll directx input devices).
            Services.GetService<IInputService>().UpdateState();           
        }

        protected override void OnRender(bool isNewSurface)
        {
            GraphicsDevice.ImmediateContext.ClearRenderTargetView(this.RenderTargetView, Color.Black);
            GraphicsDevice.ImmediateContext.ClearDepthStencilView(this.DepthStencil, DepthStencilClearFlags.Depth, 1, 0);
            Render();
            GraphicsDevice.ImmediateContext.Flush();
        }

        private void Render()
        {
            if (contentEffect != null && content != null)
            {
                contentEffect.Apply();
                content.Draw();
            }
        }

        private void DisplayFlatTriangle()
        {
            DisposeCurrentContent();

            contentEffect = new FlatEffect(this, SurfaceVertexType.PositionColor);
            contentEffect.World = Matrix.Identity;
            contentEffect.View = Matrix.LookAtLH(new Vector3(0, 500, 0), Vector3.Zero, Vector3.UnitZ);
            contentEffect.Projection = CalculateProjection();

            content = CipherPark.Aurora.Core.Content.ContentBuilder.BuildColoredTriangle(
                this.GraphicsDevice,
                contentEffect.GetVertexShaderByteCode(),
                RectangleDimensions.FromCenter(100, 100),
                Color.Aquamarine);
        }

        private void DisposeCurrentContent()
        {
            contentEffect?.Dispose();
            contentEffect = null;

            content?.Dispose();
            content = null;
        }

        private Matrix CalculateProjection()
        {
            var viewport = GraphicsDevice.ImmediateContext.Rasterizer.GetViewports<ViewportF>()[0];
            return Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(60), viewport.Width / viewport.Height, 0.1f, 10000.0f);
        }

        private void ContentBuilderGameApp_ViewportSizeChanged()
        {
            if (contentEffect != null)
                contentEffect.Projection = CalculateProjection();
        }
    }
}