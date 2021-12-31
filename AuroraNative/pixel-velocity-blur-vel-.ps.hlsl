float4 WorldPixelShaderVelocity( float2 VelocityUV : TEXCOORD1 ) : SV_TARGET
{ 
    // Velocity of this pixel simply equals the linear interpolation of the 
    // velocity of this triangle's vertices.  The interpolation is done
    // automatically since the velocity is stored in a texture coord.
    // We're storing the velocity in .R & .G channels and the app creates 
    // a D3DFMT_G16R16F texture to store this info in high precison.
    return float4( VelocityUV, 1.0f, 1.0f );
}