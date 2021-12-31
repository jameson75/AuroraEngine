Texture2D MeshTexture;
SamplerState MeshTextureSampler
{    
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};

float4 WorldPixelShaderColor( float4 Diffuse : COLOR0,
                              float2 TextureUV : TEXCOORD0 ) : SV_TARGET
{ 
    // Lookup mesh texture and modulate it with diffuse
    return MeshTexture.Sample( MeshTextureSampler, TextureUV ) * Diffuse;
}

