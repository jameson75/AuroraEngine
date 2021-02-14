////////////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Credits: Source derived from MSDN "How to: Create a SkySphere"
// Link: http://msdn.microsoft.com/en-us/library/bb464016(v=xnagamestudio.10).aspx
////////////////////////////////////////////////////////////////////////////////////

TextureCube tSkybox
{
	Filter = MIG_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

SamplerState tSkyboxSampler
{
	Filter = MIG_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

struct PS_IN
{
	float4 Position : SV_POSITION;
	float3 TexCoord : TEXCOORD0;
};

float4 main(PS_IN input) : SV_TARGET
{
	return tSkybox.Sample(tSkyboxSampler, input.TexCoord);	
}