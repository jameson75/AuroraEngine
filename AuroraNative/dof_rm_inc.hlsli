cbuffer Constants
{
	float fViewportWidth;
	float fViewportHeight;
}

struct VS_OUTPUT {
   float4 Pos: SV_POSITION;
   float2 texCoord: TEXCOORD;
};

typedef VS_OUTPUT PS_INPUT;