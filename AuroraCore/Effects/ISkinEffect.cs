using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public interface ISkinEffect
    {
        Matrix[] BoneTransforms { get; set; }
    }
}

