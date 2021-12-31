cbuffer buffer
{
	int BandCount;
	int LevelCount;
	float BandMarginSize;
	float LevelMarginSize;	
	float4 LitLEDColor;
	float4 UnlitLEDColor;
	float4 BackColor;
	float4 Power[24];
}

struct SprectrumAnalyzerTexelPSIN
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
};

float4 GetBackgroundTexel(float2 uv)
{
	return BackColor;
}

float4 GetLedUnlitTexel(float2 uv)
{
	return UnlitLEDColor;
}

float4 GetLedLitTexel(float2 uv, float intesity)
{
	return LitLEDColor;
}

float GetBandPower(int band)
{
	int i = band;
	return ((float[4])(Power[i/4]))[i%4];
}

float4 main(SprectrumAnalyzerTexelPSIN IN) : SV_TARGET
{
	float4 color = 0;
	float bandLength = 1.0f / (float)BandCount;
	float ledLength = bandLength - (bandLength * BandMarginSize); //The margin size is a percentage of the band length.
	float bandU = floor(IN.UV.x / bandLength);
	if( IN.UV.x - (bandU * bandLength) <= ledLength )
	{
		float levelHeight = 1 / (float)LevelCount;
		float ledHeight = levelHeight - (levelHeight * LevelMarginSize); //The margin size if the percentage of the level height.		
		float levelV = floor((1 - IN.UV.y) / levelHeight);
		if( (1 - IN.UV.y) - (levelV * levelHeight) <= ledHeight )
		{
			if( GetBandPower((int)bandU) - 1.0f >= levelV )
				color = GetLedLitTexel(IN.UV, 1.0f);
			else
				color = GetLedUnlitTexel(IN.UV);
		}
		else 
			color = GetBackgroundTexel(IN.UV);
	}	
	else 
		color = GetBackgroundTexel(IN.UV);

	return color;
}

