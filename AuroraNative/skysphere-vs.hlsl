////////////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Credits: Source derived from MSDN "How to: Create a SkySphere"
// Link: http://msdn.microsoft.com/en-us/library/bb464016(v=xnagamestudio.10).aspx
////////////////////////////////////////////////////////////////////////////////////

cbuffer Parameters
{
	float4x4 View;
	float4x4 Projection;
};

struct VS_OUT
{
	float4 Position : SV_POSITION;
	float4 TexCoord : TEXCOORD0;
};

VS_OUT main( float4 pos : SV_POSITION )
{
	VS_OUT output;
	// Calculate rotation. Using a float3 result, so translation is ignored
    float3 rotatedPosition = mul(pos.xyz, View);    
	// Calculate projection, moving all vertices to the far clip plane 
    // (w and z both 1.0)
    output.Position = mul(float4(rotatedPosition, 1), Projection).xyww;    
	output.Position.z = output.Position.z - 0.0001;
	output.TexCoord = pos;
	return output;
}