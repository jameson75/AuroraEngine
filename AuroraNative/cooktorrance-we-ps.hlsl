///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// CREDITS												   
// This code is based from the book						   	
// "Programming Vertex and Pixel Shaders" by Wolfgang Engel
///////////////////////////////////////////////////////////////////////////////


float4 main(  float3 Normal : TEXCOORD0,
			  float4 Light : TEXCOORD1,
			  float3 View : TEXCOORD2,
			  float3 Half : TEXCOORD3) : SV_TARGET
{
	const float PI = 3.1415926535;
	const float m = 0.2f;										// roughness
	const float RI = 0.01f;										// Fresnel reflection index	
	
	const float Si = 0.3f;                                      // specular intensity
	const float Di = 0.7f;                                      // diffuse intenstiy
	const float Ai = 0.1f;										// ambient intensity
	const float dw = 3.0f;									    // unit solid angle
	
    // copper-like color RGB(184, 115, 51)
	const float4 C = {1/256.0f * 184.0f, 1/256.0f * 115.0f, 1/256 * 51.0f, 1.0f}; 	
	
	float3 N = normalize(Normal);
	float3 L = normalize(Light.xyz);
	float3 V = normalize(View);
	float3 H = normalize(Half);
	
	float NH = saturate(dot(N, H));
	float VH = saturate(dot(V, H));
	float NV = saturate(dot(N, V));
	float NL = saturate(dot(L, N));
		
	//------------------------------
	// Compute Beckmann's distribution function
	//					  -[(1 - N.H^2) / m^2 * (N.H)^2]
	// D =     1       e^		
	//      m^2 N.H^4        
	// ß is the angle between N and H
	// m is the root-mean-square slope of the microfacets
	//------------------------------
	float NH2 = NH * NH;
	float m2 = m * m;
	float D = (1 / m2 * NH2 * NH2) * (exp(-((1 - NH2) / (m2 * NH2))));
	
	
	//------------------------------
	// Compute Fresnel term (Schlick's approximation) 
	// F = IR + (1-IR)*(1 - (N.V))^5
	// // IR + (1 - N.V)^5 * (1 - IR)
	//------------------------------
	float F = RI  +  (1 - RI) * pow((1 - NV), 5.0f);
	
	//------------------------------
	// Compute self shadowing term 
	// G = min(1, 2 * N.H * N.L, 2 * N.H * N.V)
	//                 V.H           V.H
	//------------------------------
//	float G = min(1.0f, min((2 * NH * NL) / VH, (2 * NH * NV) / VH));

	// optimized:
	float G = (2 * NH * min(NL, NV))/ VH;

	//------------------------------
	// Compute final Cook-Torrance specular term
	// Rs = (D * F * G) * 1 / PI * N.V * N.L
	//------------------------------
	float S = (F * D * G) / (PI * NL * NV);
	
	//------------------------------
	// Original: Ir = Ai * Ac + Ii(N.L)dw * (Si * Sc + Di * Dc)
	// My formula: Ir = Ia * C + N.L * sat(lidw * (Di * Dc * C + Si * Sc))
	//------------------------------
	return Ai * C + (NL * saturate(dw * ((Di * NL * C) + (Si * S)))) * Light.w;
}