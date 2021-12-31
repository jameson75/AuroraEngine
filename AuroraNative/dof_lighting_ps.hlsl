cbuffer Constants
{
	float far_clamp;
	float d_far;
	float d_focus;
	float d_near;
	float4 ambient;
	float4 diffuse;
	float4 specular;
	float n_specular;
	float Ka;
	float Ks;
	float Kd;
};

//Texture2D tBase;
//SamplerState tBaseSampler;

float ComputeDepthBlur (float depth)
{
   float f;

   if (depth < d_focus)
   {
      // scale depth value between near blur distance and focal distance to [-1, 0] range
      f = (depth - d_focus)/(d_focus - d_near);
   }
   else
   {
      // scale depth value between focal distance and far blur
      // distance to [0, 1] range
      f = (depth - d_focus)/(d_far - d_focus);

      // clamp the far blur to a maximum blurriness
      f = clamp (f, 0, far_clamp);
   }

   // scale and bias into [0, 1] range
   return f * 0.5f + 0.5f;
}

struct PS_INPUT 
{
	//float4 Diff   : COLOR0;
	float4 Position : SV_POSITION;
    float3 Normal : TEXCOORD0;
    float3 View   : TEXCOORD1;
    float3 Light  : TEXCOORD2;
	float  depth  : TEXCOORD3;
    float2 Tex    : TEXCOORD4;    
};

float4 main( PS_INPUT input ) : SV_TARGET
{
   // Compute the reflection vector:
   float3 vReflect = normalize( 2 * dot( input.Normal, input.Light) * input.Normal - input.Light );       

   // Compute ambient term:
   float4 AmbientColor = ambient * Ka;

   // Compute diffuse term:
   float4 DiffuseColor = diffuse * Kd * max( 0, dot( input.Normal, input.Light ));

   // Compute specular term:
   float4 SpecularColor = specular * Ks * pow( max( 0, dot(vReflect, input.View)), n_specular );
   
   float4 FinalColor = (AmbientColor + DiffuseColor) * /*tBase.Sample(tBaseSampler, input.Tex)*/ + SpecularColor;

   float depthBlur = ComputeDepthBlur(input.depth);
   //FinalColor.a = depthBlur; //ComputeDepthBlur (input.depth);    
   
   return float4(1.0f, 1.0f, 1.0f, depthBlur);
}

