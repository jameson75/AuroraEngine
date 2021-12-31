cbuffer buffer
{
	float blur;
}

Texture2D RTTexture;
Texture2D SumTexture;
SamplerState RT;
SamplerState Sum;

float4 main(float2 texCoord: TEXCOORD0) : SV_TARGET 
{
   float4 render = RTTexture.Sample(RT, texCoord);
   float4 sum = SumTexture.Sample(Sum, texCoord);
   return lerp(render, sum, blur);
}
