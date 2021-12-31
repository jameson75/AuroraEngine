struct PS_IN
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

float distance(float2 p1, float2 p2)
{
	float dx = p2.x - p1.x;
	float dy = p2.y - p1.y;
	return sqrt((dx * dx) + (dy * dy));
}

float4 main(PS_IN input) : SV_TARGET
{	
	float2 texCenter = float2(0.5, 0.5);
	//return abs(distance(texCenter, input.TexCoord)) <= 0.5 ? ForegroundColor : BackgroundColor;
	return float4(1.0f, 1.0f, 1.0f, 1.0f);
}