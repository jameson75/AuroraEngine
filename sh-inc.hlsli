cbuffer Constants
{
	float4x4 view_proj_matrix;
	float4x4 r_sh_matrix;
	float4x4 g_sh_matrix;
	float4x4 b_sh_matrix;
	float4x4 view_matrix;
};

struct VS_INPUT
{
   float4 vPosition: SV_POSITION;
   float4 vNormal: NORMAL;
   float4 vTex: TEXCOORD;
};

struct PS_INPUT
{
   float4 Pos:    SV_POSITION;
   float4 Diff:   COLOR;
   float4 Tex:    TEXCOORD;
};