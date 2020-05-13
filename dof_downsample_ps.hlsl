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

Texture2D tScreenImage;
SamplerState tScreenImageSampler
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
   float2 pixelSize = 1.0 / float2( fViewportWidth, fViewportHeight );

   // it would be more efficient if the texture coordinates
   // were computed in the vertex shader and passed down
   texCoordSample.x = input.texCoord.x - pixelSize.x;
   texCoordSample.y = input.texCoord.y + pixelSize.y;
   cOut = tScreenImage.Sample(tScreenImageSampler, texCoordSample);
   
   texCoordSample.x = input.texCoord.x + pixelSize.x;
   texCoordSample.y = input.texCoord.y + pixelSize.y;
   cOut += tScreenImage.Sample(tScreenImageSampler, texCoordSample);
   
   texCoordSample.x = input.texCoord.x + pixelSize.x;
   texCoordSample.y = input.texCoord.y - pixelSize.y;
   cOut += tScreenImage.Sample(tScreenImageSampler, texCoordSample);
   
   texCoordSample.x = input.texCoord.x - pixelSize.x;
   texCoordSample.y = input.texCoord.y - pixelSize.y;
   cOut += tScreenImage.Sample(tScreenImageSampler, texCoordSample);
      
   return cOut * 0.25;
}
