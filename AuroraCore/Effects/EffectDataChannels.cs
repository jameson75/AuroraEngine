using System;

namespace CipherPark.Aurora.Core.Effects
{
    [Flags]
    public enum EffectDataChannels
    {
        None =          0x00,
        Position =      0x01,
        Color =         0x02,
        Normal =        0x04,
        Texture = 0x08,
        Skinning =      0x10,
        Instancing =    0x80,
    }
}
