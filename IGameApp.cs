using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.XAudio2;
using SharpDX.DirectInput;

using CipherPark.AngelJacket.Core.Services;

namespace CipherPark.AngelJacket.Core
{
    public interface IGameApp
    {
        ServiceTable Services { get; }

        DeviceContext GraphicsDeviceContext { get; }

        SharpDX.Direct3D11.Device GraphicsDevice { get; }

        MasteringVoice MasteringVoice { get; }       

        Keyboard Keyboard { get; }

        Mouse Mouse { get; }

        IntPtr DeviceHwnd { get; }
    }
}
