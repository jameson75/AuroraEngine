
cbuffer MatrixBuffer
{
	float4x4 worldViewProj;
	float4x4 view;
}

struct VS_IN 
{ 	
	float4 pos : SV_POSITION; 	
	float4 col : COLOR; 
};  

struct PS_IN 
{ 	
	float4 pos : SV_POSITION; 	
	float4 col : COLOR; 
};  

PS_IN main( VS_IN input ) 
{ 	
	PS_IN output = (PS_IN)0; 	 	
	output.pos = mul(input.pos, worldViewProj); 	
	output.col = input.col; 	 	
	return output; 
}  

