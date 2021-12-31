
cbuffer MatrixBuffer
{
	float4x4 worldViewProj;
	float4x4 view;
}

struct PS_IN 
{ 	
	float4 pos : SV_POSITION; 	
	float4 col : COLOR; 
};  

float4 main( PS_IN input ) : SV_Target 
{ 	
	return input.col; 
} 