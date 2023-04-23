cbuffer Constants
{
	float Step;
	float2 WipeTextureSize;
	float4 WipeColor;
};

Texture2D InputTexture;
SamplerState InputTextureSamplerState;

Texture2D WipeTexture;
SamplerState WipeTextureSamplerState;

struct PSIN
{
    float4 Position	: SV_POSITION;
    float2 UV	: TEXCOORD0;
};

bool IsWipePixel(float2 uv)
{
	return uv.x <= Step;
}

float4 main(PSIN input) : SV_TARGET
{	
	if (IsWipePixel(input.UV))
	{
		if (WipeColor.w == 0)
		{
			return WipeTexture.Sample(WipeTextureSamplerState, input.UV);
		}
		else
		{
			return WipeColor;
		}
	}

	return InputTexture.Sample(InputTextureSamplerState, input.UV);
}


