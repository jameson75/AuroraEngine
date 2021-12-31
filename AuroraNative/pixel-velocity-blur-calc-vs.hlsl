//-----------------------------------------------------------------------------
// Vertex shader output structure
//-----------------------------------------------------------------------------
struct VS_OUTPUT
{
    float4 Position : SV_POSITION;   // position of the vertex
    float4 Diffuse  : COLOR0;     // diffuse color of the vertex
    float2 TextureUV : TEXCOORD0;  // typical texture coords stored here
    float2 VelocityUV : TEXCOORD1;  // per-vertex velocity stored here
};

cbuffer Constants
{
	float4x4 mWorld;
	float4x4 mWorldViewProjection;
	float4x4 mWorldViewProjectionLast;
	float4 MaterialAmbientColor;
	float4 MaterialDiffuseColor;	
	float4 LightAmbient = { 1.0f, 1.0f, 1.0f, 1.0f };    // ambient
	float4 LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };    // diffuse
	float3 LightDir = normalize(float3(1.0f, 1.0f, 1.0f));
};

//-----------------------------------------------------------------------------
// Name: WorldVertexShader     
// Type: Vertex shader                                      
// Desc: In addition to standard transform and lighting, it calculates the velocity 
//       of the vertex and outputs this as a texture coord.
//-----------------------------------------------------------------------------
VS_OUTPUT WorldVertexShader( float4 vPos : SV_POSITION, 
                             float3 vNormal : NORMAL,
                             float2 vTexCoord0 : TEXCOORD0 )
{
    VS_OUTPUT Output;
    float3 vNormalWorldSpace;
    float4 vPosProjSpaceCurrent; 
    float4 vPosProjSpaceLast; 
  
    vNormalWorldSpace = normalize(mul(vNormal, (float3x3)mWorld)); // normal (world space)
    
    // Transform from object space to homogeneous projection space
    vPosProjSpaceCurrent = mul(vPos, mWorldViewProjection);
    vPosProjSpaceLast = mul(vPos, mWorldViewProjectionLast);
    
    // Output the vetrex position in projection space
    Output.Position = vPosProjSpaceCurrent;

    // Convert to non-homogeneous points [-1,1] by dividing by w 
    vPosProjSpaceCurrent /= vPosProjSpaceCurrent.w;
    vPosProjSpaceLast /= vPosProjSpaceLast.w;
    
    // Vertex's velocity (in non-homogeneous projection space) is the position this frame minus 
    // its position last frame.  This information is stored in a texture coord.  The pixel shader 
    // will read the texture coordinate with a sampler and use it to output each pixel's velocity.
    float2 velocity = (vPosProjSpaceCurrent - vPosProjSpaceLast).xy;    
    
    // The velocity is now between (-2,2) so divide by 2 to get it to (-1,1)
    velocity /= 2.0f;   

    // Store the velocity in a texture coord
    Output.VelocityUV = velocity;
        
    // Compute simple lighting equation
    Output.Diffuse = MaterialDiffuseColor * LightDiffuse * max(0,dot(vNormalWorldSpace, LightDir)) + 
                     MaterialAmbientColor * LightAmbient;   
    Output.Diffuse.a = 1.0f; 
    
    // Just copy the texture coordinate through
    Output.TextureUV = vTexCoord0; 
    
    return Output;    
}