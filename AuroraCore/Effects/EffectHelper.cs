using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams// 
// Copyright © 2010-2013
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
    }
}

