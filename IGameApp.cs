using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.XAudio2;
using SharpDX.DirectInput;
using SharpDX.DXGI;
using CipherPark.AngelJacket.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core
{
    public interface IGameApp
    {       
        ServiceTable Services { get; }

        DeviceContext GraphicsDeviceContext { get; }

        SharpDX.Direct3D11.Device GraphicsDevice { get; }

        MasteringVoice MasteringVoice { get; }

        XAudio2 AudioDevice { get; }      

        RenderTargetView RenderTargetView { get; }

        ShaderResourceView RenderTargetShaderResource { get; }

        DepthStencilView DepthStencil { get; }       

        Keyboard Keyboard { get; }

        Mouse Mouse { get; }

        IntPtr DeviceHwnd { get; }

        event Action BuffersResizing;

        event Action BuffersResized;

        event Action ViewportSizeChanged;
    }
}
