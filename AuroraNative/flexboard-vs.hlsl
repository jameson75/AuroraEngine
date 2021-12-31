
cbuffer Parameters
{       
	float4x4 WorldView;
	float4x4 View;
	float4x4 Projection;
	float4x4 camParentXf;
	float4 camLocalPosition;
};

struct VS_IN
{
	float4 Position : SV_POSITION;
	float4 TexCoord : TEXCOORD0;
	float4 SlopeDir : TEXCOORD1;
};

struct VS_OUT
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

/*
VS_OUT main(VS_IN input)
{
	VS_OUT output;
	
	//get the position of strip's origin in view space.
	float4 vPos = mul(input.Position, WorldView);	
	//get the strip's slope direction in view space.
	float4 vSlopeDir = normalize(mul(input.SlopeDir, WorldView));
	//get the direction towards the camera.
	float3 camDir = normalize(-float3(View._13, View._23, View._33));		
	//get the co planar vector of the strip.
	float3 vWidthDir = normalize(cross(vSlopeDir, camDir));	
	//get the vertice offset from the strip origin.
	float2 offset = input.TexCoord.zw;
	//calculate the final position of the strip's vertice by offsetting it in the coplanar directions.
	float4 finalPos = vPos + (offset.x * float4(vWidthDir, 1)) + (offset.y * vSlopeDir);
	//output projected vertice.
	output.Position =  mul(finalPos, Projection);
	//pass through texture coords.
	output.TexCoord = input.TexCoord.xy;

	return output;
}
*/

VS_OUT main(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;

	/*
	//output projected vertice.
	output.Position =  mul(finalPos, Projection);
	//pass through texture coords.
	output.TexCoord = input.TexCoord.xy;
	*/
	return output;
}