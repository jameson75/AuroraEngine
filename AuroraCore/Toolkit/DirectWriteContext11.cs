using SharpDX;

namespace CipherPark.Aurora.Core.Toolkit
{
    // Reference: https://bobobobo.wordpress.com/2021/08/
    // Reference: https://www.braynzarsoft.net/viewtutorial/q16390-14-simple-font
    // Reference: https://learn.microsoft.com/en-us/windows/win32/direct3darticles/surface-sharing-between-windows-graphics-apis
    // Reference: https://stackoverflow.com/questions/71830460/directx11-with-a-multiple-video-adapter-gpu-pc        

    /// <summary>
    /// This class is implemented as a stop-gap to include support for DirectWrite until I can figure
    /// out how to properly integrate support into the Aurora Engine.
    /// </summary>
    /// 
    // TODO: Move this support to the Aurora engine.
    public class DirectWriteContext11
    {
        private readonly SharpDX.Direct3D11.Device d3d11Device;       

        public DirectWriteContext11(SharpDX.Direct3D11.Device d3d11Device)
        {
            this.d3d11Device = d3d11Device;
        }     

        public void Initialize()
        {            
            var dxgiDevice = ComObject.As<SharpDX.DXGI.Device>(d3d11Device.NativePointer);
            var dxgiAdapter = dxgiDevice.GetParent<SharpDX.DXGI.Adapter>();
            D3d10Device = new SharpDX.Direct3D10.Device1(
                dxgiAdapter, // Use the same adapter as the d3d11 device
                SharpDX.Direct3D10.DeviceCreationFlags.Debug | SharpDX.Direct3D10.DeviceCreationFlags.BgraSupport);

            D3d10Device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.PointList;
        }
        
        public SharpDX.Direct3D10.Device1 D3d10Device { get; private set; }
    }
}
