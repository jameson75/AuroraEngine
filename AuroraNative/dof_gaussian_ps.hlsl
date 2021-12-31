struct PS_INPUT
{
   float4 Pos: SV_POSITION;
   float2 texCoord: TEXCOORD0;
};

cbuffer Constants
{
	float     fViewportWidth;
	float     fViewportHeight;
};

Texture2D tLowResImage;
SamplerState tLowResImageSampler
{
	Filter = MIG_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

float4 main( PS_INPUT input ) : SV_TARGET
{
   float2 texCoordSample = 0;
   float4 cOut;
   
   float2 pixelSize = 4.0 / float2( fViewportWidth, fViewportHeight );

   // using bilinear texture lookups, this could be implemented
   // with just 2 texture fetches
   
   cOut = 0.5 * tLowResImage.Sample(tLowResImageSampler, input.texCoord);
   
   // ideally the vertex shader would compute the texture
   // coords and pass them down   
   texCoordSample.x = input.texCoord.x;
   texCoordSample.y = input.texCoord.y + pixelSize.y;
   cOut += 0.25 * tLowResImage.Sample(tLowResImageSampler, texCoordSample);
   
   texCoordSample.y = input.texCoord.y - pixelSize.y;
   cOut += 0.25 * tLowResImage.Sample(tLowResImageSampler, texCoordSample);
      
   return cOut;
}


