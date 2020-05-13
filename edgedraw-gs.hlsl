cbuffer Constants
{
	float4x4 matView;
};

struct GSOutput
{
	float4 pos : SV_POSITION;
};

struct GSInput
{
	float4 pos : SV_POSITION;
	float3 normal1 : TEXCOORD0;
	float3 normal2 : TEXCOORD1;
	float3 normal3 : TEXCOORD2;
};

[maxvertexcount(12)]
void main(
	line GSInput input[2], 
	inout TriangleStream< GSOutput > output
)
{
	float3 camZ = float3(matView._13, matView._23, matView._33);	
	float d01 = dot(camZ, input[0].normal1);
	float d02 = dot(camZ, input[0].normal2);
	float d03 = dot(camZ, input[0].normal3);	

	//First we'll define the extruded edge as a quad with an area of zero (all points are colinear).

	float3 p0 = input[1].pos.xyz;
	float3 p1 = input[0].pos.xyz;
	float3 p2 = p1;
	float3 p3 = p0;
	
	//TODO: if input[1] or input[2] have exactly 2 back facing normals,
	//p0 and p1 need to be extended along edge vector.
	
	//if((d01 < 0 || d02 < 0 || d03 < 0) && (d01 >= 0 || d02 >=0 || d03 >= 0))
	{			
		//if( d01 < 0)
			p3 += input[0].normal1 * 5.0f;		
		//if( d02 < 0)
			p3 += input[0].normal2 * 5.0f;
		//if( d03 < 0)
			p3 += input[0].normal3 * 5.0f;
	}	

	float d11 = dot(camZ, input[1].normal1);
	float d12 = dot(camZ, input[1].normal2);
	float d13 = dot(camZ, input[1].normal3);	
	//if((d11 < 0 || d12 < 0 || d13 < 0) && (d11 >= 0 || d12 >=0 || d13 >= 0))
	{			
		//if( d11 < 0)
			p2 += input[1].normal1 * 5.0f;		
		//if( d12 < 0)
			p2 += input[1].normal2 * 5.0f;
		//if( d13 < 0)
			p2 += input[1].normal3 * 5.0f;
	}

	float4 positions[6] = {
		float4(p0, 1.0f),
		float4(p1, 1.0f),
		float4(p2, 1.0f),
		float4(p2, 1.0f),
		float4(p3, 1.0f),
		float4(p0, 1.0f),	
	};

	for(int i = 0; i < 6; i++)
	{
		GSOutput element;
		element.pos = positions[i];
		output.Append(element);
	}

	for(int i = 0; i < 6; i++)
	{
		GSOutput element;
		element.pos = positions[5-i];
		output.Append(element);
	}
}