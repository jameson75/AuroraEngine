struct VS_INPUT
{
	float2 Pos : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

cbuffer Constants
{
	float     fViewportWidth;
	float     fViewportHeight;
};

struct VS_OUTPUT 
{
   float4 Pos: SV_POSITION;
   float2 texCoord: TEXCOORD0;
   float2 pixelSizeHigh: TEXCOORD1;
};

VS_OUTPUT main(VS_INPUT input)
{
   VS_OUTPUT Out;

   // this is half a pixel with and height because
   // the screen-aligned quad has a width and height of 2
   float2 halfPixelSize = 1.0 / float2( fViewportWidth, fViewportHeight );
   
   // Clean up inaccuracies
   input.Pos.xy = sign(input.Pos.xy);
   Out.Pos = float4(input.Pos.xy, 0, 1);
   
   // offset to properly align pixels with texels   
   Out.Pos.xy += float2(-1, 1) * halfPixelSize;
   
   Out.texCoord = 0.5 * input.Pos.xy + 0.5;
   Out.texCoord.y = 1.0 - Out.texCoord.y;

   // pixel size (1/image resolution) of full resolution image
   // this is computed here and passed down so that the pixel
   // shader doesn't blow the 2.0 instruction limit
   Out.pixelSizeHigh = 1.0/fViewportHeight;   

   return Out;
}