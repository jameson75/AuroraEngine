
//http://www.youtube.com/watch?v=6TFNDPLSiZY

struct PSINPUT {
    float4 Position	: SV_POSITION;
    float2 UV		: TEXCOORD0;
};

float4 main(PSINPUT input) : SV_TARGET
{

	return float4(1.0f, 1.0f, 1.0f, 1.0f);
}