///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// CREDITS												   
// This code is based from the book						   	
// "Programming Vertex and Pixel Shaders" by Wolfgang Engel
///////////////////////////////////////////////////////////////////////////////

cbuffer Constants
{
	float4x4 matWorldViewProj;	
	float4x4 matWorld;	
	float4 vecLightDir;
	float4 vecEye;
};

struct VS_OUTPUT
{
    float4 Pos  : SV_POSITION;
    float3 Normal : TEXCOORD0;
    float4 Light : TEXCOORD1;
    float3 View : TEXCOORD2;
    float3 Half : TEXCOORD3;
};

VS_OUTPUT main(float4 Pos : SV_POSITION, float3 Normal : NORMAL)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;      
    Out.Pos = mul(Pos, matWorldViewProj);	   
    Out.Normal = mul(float4(Normal, 0), matWorld).xyz;
    float3 PosWorld = normalize(mul(Pos, matWorld)).xyz;    
    float3 Light = normalize(vecLightDir.xyz - PosWorld).xyz;
    float LightRange = 0.1;    
    Out.Light.xyz = Light;
    Out.Light.w = saturate(1 - dot(Light * LightRange, Light * LightRange)); // 1 - Attenuation        
    Out.View = normalize(vecEye.xyz - PosWorld).xyz;
    Out.Half = normalize(Out.Light.xyz + Out.View).xyz;    
	return Out;
}