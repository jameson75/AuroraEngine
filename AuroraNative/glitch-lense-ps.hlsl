//http://www.youtube.com/watch?v=sYSuuHebBCQ

cbuffer Constants
{
	float4 Channel;
	float2 Offset;
}

Texture2D Texture;
SamplerState TextureSampler : register(s0);

struct PSINPUT {
    float4 Position	: SV_POSITION;
    float2 UV		: TEXCOORD0;
};

float4 main(PSINPUT input) : SV_TARGET
{
	float4 c = 0;

	if(Channel.r != 0)
		c.x = Texture.Sample(TextureSampler, input.UV + Offset).x;
	else if(Channel.g != 0)
		c.y = Texture.Sample(TextureSampler, input.UV + Offset).y;
	else if(Channel.b != 0)
		c.z = Texture.Sample(TextureSampler, input.UV + Offset).z;

	return float4(c.xyz, 1.0f);
}