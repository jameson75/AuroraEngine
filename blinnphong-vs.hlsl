cbuffer Constants
{
	float4x4 WorldITXf;
	float4x4 WorldXf;
	float4x4 ViewIXf;
	float4x4 WvpXf;
	float4 LampDirPos;
};

struct VSINPUT 
{
	float4 Position : SV_POSITION;
	/*float4 Normal : TEXCOORD0;
	float4 Tangent : TEXCOORD1;
	float4 Binormal : TEXCOORD2;
	float2 UV : TEXCOORD3;*/
	float4 Normal : NORMAL;
	float4 Color: COLOR;
};

struct VSOUTPUT
{
	float4 HPosition :		SV_POSITION;
	float3 LightVec :		TEXCOORD0;
	float3 WorldView :		TEXCOORD1;
	float3 WorldNormal :	TEXCOORD2;
	float4 WorldTangent :	TEXCOORD3;
	float3 WorldBinormal :	TEXCOORD4;
	float2 UV :				TEXCOORD5;	
};

VSOUTPUT blinn_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
    OUT.WorldNormal = mul(IN.Normal,WorldITXf).xyz;
    //OUT.WorldTangent = mul(IN.Tangent,WorldITXf).xyz;
    //OUT.WorldBinormal = mul(IN.Binormal,WorldITXf).xyz;
    float4 Po = IN.Position;
    float4 Pw = mul(Po,WorldXf);	// convert to "world" space
//#ifdef OBJECT_SPACE_LIGHTS
//    float4 Lw = mul(LampDirPos,WorldXf);	// convert to "world" space
//#else /* !OBJECT_SPACE_LIGHTS -- standard world-space lights */
    float4 Lw = LampDirPos;
//#endif /* !OBJECT_SPACE_LIGHTS */
    //(Eugene's comment...) 
	//It seems when Lw.w == 0, the light is a directional/parallel light.
	//Otherwise it is a point light.
    if (Lw.w == 0) {
	OUT.LightVec = -normalize(Lw.xyz);
    } else {
    /*Eugene - Commenting out passing an non-normalized vector 
	so I could pass in a normalized one.
	// we are still passing a (non-normalized) vector 
	OUT.LightVec = Lw.xyz - Pw.xyz;
	*/
	OUT.LightVec = normalize(Lw.xyz - Pw.xyz);
    }
//#ifdef FLIP_TEXTURE_Y
//    OUT.UV = float2(IN.UV.x,(1.0-IN.UV.y));
//#else /* !FLIP_TEXTURE_Y */
    //OUT.UV = IN.UV.xy;
//#endif /* !FLIP_TEXTURE_Y */
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po,WvpXf);	
    return OUT;
}