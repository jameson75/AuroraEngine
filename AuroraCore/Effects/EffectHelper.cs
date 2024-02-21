using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams// 
// Copyright © 2010-2024
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public static class EffectHelper
    {
        public static BlendState CreateBlendForTransparency(Device device)
        {
            BlendStateDescription blendDesc = BlendStateDescription.Default();
            blendDesc.RenderTarget[0].IsBlendEnabled = true;
            blendDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
            blendDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
            return new BlendState(device, blendDesc);
        }

        public static DepthStencilState CreateDepthStencilStateForTransparency(Device device)
        {
            var currentDepthStencilState = device.ImmediateContext.OutputMerger.DepthStencilState;
            var newDepthStencilStateDescription = currentDepthStencilState.Description;
            newDepthStencilStateDescription.IsDepthEnabled = false;
            return new DepthStencilState(device, newDepthStencilStateDescription);
        }
    }
}

