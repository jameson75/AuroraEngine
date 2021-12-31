struct VSIN
{
	float2 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

struct VSOUT
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

VSOUT main( VSIN input )
{
	VSOUT output;
	output.position = float4(input.position.x, input.position.y, 0, 1);
	output.texCoord = input.texCoord;
	return output;
}