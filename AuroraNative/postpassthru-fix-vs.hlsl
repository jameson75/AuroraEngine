
cbuffer buffer
{
	float viewport_inv_width;
	float viewport_inv_height;
}

struct VS_OUTPUT {
   float4 Pos: SV_POSITION;
   float2 texCoord: TEXCOORD0;
};

VS_OUTPUT main(float4 Pos: SV_POSITION)
{
   VS_OUTPUT Out;
   // Clean up inaccuracies
   Pos.xy = sign(Pos.xy);
   Out.Pos = float4(Pos.xy, 0, 1);
   // Image-space
   Out.texCoord.x = 0.5 * (1 + Pos.x + viewport_inv_width);
   Out.texCoord.y = 0.5 * (1 - Pos.y + viewport_inv_height);
   return Out;
}

