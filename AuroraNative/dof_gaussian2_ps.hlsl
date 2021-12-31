Texture2D tLowResImage;
SamplerState tLowResImageSampler
{
	Filter = MIG_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

cbuffer Constants
{
	float     fViewportWidth;
	float     fViewportHeight;
};

struct PS_INPUT
{
   float4 Pos: SV_POSITION;
   float2 texCoord: TEXCOORD0;
};

float4 main( PS_INPUT input ) : SV_TARGET
{
   float2 texCoordSample = 0;
   float4 cOut;
   
   float2 pixelSize = 4.0 / float2( fViewportWidth, fViewportHeight );
    
   cOut = 0.5 * tLowResImage.Sample(tLowResImageSampler, input.texCoord);   
  
   texCoordSample.x = input.texCoord.x;
   texCoordSample.y = input.texCoord.y + pixelSize.y;
   cOut += 0.25 * tLowResImage.Sample(tLowResImageSampler, texCoordSample);
   
   texCoordSample.y = input.texCoord.y - pixelSize.y;
   cOut += 0.25 * tLowResImage.Sample(tLowResImageSampler, texCoordSample);
      
   return cOut;
}



