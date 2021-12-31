///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Credits: Jose Maria Mendez
// Link: http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/a-simple-and-practical-approach-to-ssao-r2753
// License: MIT
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture2D g_tBuffer_norm;
Texture2D g_tBuffer_pos;
Texture2D g_tBuffer_posb;
Texture2D g_tRandom;

SamplerState g_buffer_norm;
SamplerState g_buffer_pos;
SamplerState g_buffer_posb;
SamplerState g_random;

cbuffer Constants
{
	float random_size;
	float g_sample_rad;
	float g_intensity;
	float g_scale;
	float g_bias;
	float2 g_screen_size;
	bool g_enable_back_occlusion;
};

struct PS_INPUT
{
	float2 uv : TEXCOORD0;
};

struct PS_OUTPUT
{
	float4 color : SV_TARGET;
};

float3 getPosition(in float2 uv)
{
	return g_tBuffer_pos.Sample(g_buffer_pos,uv).xyz;
}

float3 getNormal(in float2 uv)
{
	return normalize(g_tBuffer_norm.Sample(g_buffer_norm, uv).xyz * 2.0f - 1.0f);
}

float2 getRandom(in float2 uv)
{
	return normalize(g_tRandom.Sample(g_random, g_screen_size * uv / random_size).xy * 2.0f - 1.0f);
}

float3 getPositionBack(in float2 uv)
{
	return g_tBuffer_posb.Sample(g_buffer_posb,uv).xyz;
}

float doAmbientOcclusion(in float2 tcoord,in float2 uv, in float3 p, in float3 cnorm)
{
	float3 diff = getPosition(tcoord + uv) - p;
	const float3 v = normalize(diff);
	const float d = length(diff)*g_scale;
	return max(0.0,dot(cnorm,v)-g_bias)*(1.0/(1.0+d))*g_intensity;
}

float doAmbientOcclusionBack(in float2 tcoord,in float2 uv, in float3 p, in float3 cnorm)
{
	float3 diff = getPositionBack(tcoord + uv) - p;
	const float3 v = normalize(diff);
	const float d = length(diff)*g_scale;
	return max(0.0,dot(cnorm,v)-g_bias)*(1.0/(1.0+d));
}

PS_OUTPUT main(PS_INPUT i)
{
	PS_OUTPUT o = (PS_OUTPUT)0;

	o.color.rgb = 1.0f;
	const float2 vec[4] = {float2(1,0),float2(-1,0),
				float2(0,1),float2(0,-1)};

	float3 p = getPosition(i.uv);
	float3 n = getNormal(i.uv);
	float2 rand = getRandom(i.uv);

	float ao = 0.0f;
	float rad = g_sample_rad/p.z;

	//**SSAO Calculation**//
	int iterations = 4;
	for (int j = 0; j < iterations; ++j)
	{
		  float2 coord1 = reflect(vec[j],rand)*rad;
		  float2 coord2 = float2(coord1.x*0.707 - coord1.y*0.707,
					  coord1.x*0.707 + coord1.y*0.707);
  
		  ao += doAmbientOcclusion(i.uv,coord1*0.25, p, n);
		  ao += doAmbientOcclusion(i.uv,coord2*0.5, p, n);
		  ao += doAmbientOcclusion(i.uv,coord1*0.75, p, n);
		  ao += doAmbientOcclusion(i.uv,coord2, p, n);

		  if(g_enable_back_occlusion)
		  {
			  ao += doAmbientOcclusionBack(i.uv,coord1*(0.25+0.125), p, n);
			  ao += doAmbientOcclusionBack(i.uv,coord2*(0.5+0.125), p, n);
			  ao += doAmbientOcclusionBack(i.uv,coord1*(0.75+0.125), p, n);
			  ao += doAmbientOcclusionBack(i.uv,coord2*1.125, p, n);
		  }
	}

	ao/=(float)iterations*4.0;
	//**END**//

	//Do stuff here with your occlusion value âaoâ: modulate ambient lighting, write it to a buffer for later //use, etc.
	return o;
}