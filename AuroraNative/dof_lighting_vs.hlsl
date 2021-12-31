cbuffer Constants
{
	//float3 teapotPos;
	float4x4 world_view_matrix;
	float4x4 world_view_proj_matrix;
	float4 lightDir;
};

struct VS_OUTPUT
{
   float4 Pos    : SV_POSITION;
   float3 Norm   : TEXCOORD0;
   float3 View   : TEXCOORD1;
   float3 Light  : TEXCOORD2;
   float  depth  : TEXCOORD3;
   float2 Tex    : TEXCOORD4;
};

struct VS_INPUT
{
   float4 inPos  : SV_POSITION;
   float3 inNorm : NORMAL;
   float2 inTex  : TEXCOORD0;
};

VS_OUTPUT main( VS_INPUT input )
{
   VS_OUTPUT Out = (VS_OUTPUT) 0; 
       
   Out.Pos = mul(input.inPos, world_view_proj_matrix); 

   // Output light vector:
   Out.Light = -lightDir.xyz;

   // Compute position in world-view space:
   float3 Pview = mul(input.inPos, world_view_matrix).xyz; 

   // Transform the input normal to world-view space:
   Out.Norm = normalize(mul(float4(input.inNorm, 1.0f), world_view_matrix)).xyz;   
 
   // Compute the view direction in world-view space:
   Out.View = - normalize( Pview );

   // Propagate texture coordinate for the object:
   Out.Tex = input.inTex;

   Out.depth = Pview.z;

   return Out;
}
