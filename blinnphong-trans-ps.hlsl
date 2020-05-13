#define MAX_LIGHTS 8

cbuffer Constants
{
	float3 LampColor[MAX_LIGHTS];	
	float3 AmbiColor;
	float Ks;
	float Eccentricity;
	int NLights;	
	bool TransparencyEnabled;
	float4 LightPosDir[MAX_LIGHTS];
};		

Texture2D DiffuseTexture;
SamplerState DiffuseSampler;

Texture2D TransparencyMap;
SamplerState TransparencySampler;
	
struct PSINPUT
{
	float4 HPosition :		SV_POSITION;	
	float3 WorldView :		TEXCOORD0;
	float3 WorldNormal :	TEXCOORD1;
	float3 UVW :			TEXCOORD2;	
	float4 WorldPosition :	TEXCOORD4;
};

float4 blinn_PS(PSINPUT IN) : SV_TARGET 
{
    // "standard" normalized vectors...
	
	float3 diffContrib = float3(0,0,0);
	float3 specContrib = float3(0,0,0);    
	
	for(int i = 0; i < MAX_LIGHTS; i++)
	{
		if( i < NLights)
		{
			float4 Lw = LightPosDir[i];
			float3 Ln = float3(0,0,0);
			//If w == 0, then we treat the light as a directional light.
			if( Lw.w == 0 )
				Ln = -normalize(Lw.xyz);
			//Otherwise, we treat the light as a point light.
			else
				Ln = normalize(Lw.xyz - IN.WorldPosition.xyz);	
			float3 Vn = normalize(IN.WorldView);
			float3 Nn = normalize(IN.WorldNormal);		
			float3 Hn = normalize(Vn + Ln);
			float hdn = dot(Hn,Nn);
			float3 Rv = reflect(-Ln,Nn);
			float rdv = dot(Rv,Vn);
			rdv = max(rdv,0.001);
			float ldn=dot(Ln,Nn);
			ldn = max(ldn,0.0);		
			float ndv = dot(Nn,Vn);	
			float hdv = dot(Hn,Vn);
			float eSq = Eccentricity*Eccentricity;
			float distrib = eSq / (rdv * rdv * (eSq - 1.0) + 1.0);
			distrib = distrib * distrib;
			float Gb = 2.0 * hdn * ndv / hdv;
			float Gc = 2.0 * hdn * ldn / hdv;
			float Ga = min(1.0,min(Gb,Gc));
			float fresnelHack = 1.0 - pow(ndv,5.0);
			hdn = distrib * Ga * fresnelHack / ndv;
			diffContrib+= (ldn * LampColor[i]);
			specContrib+= (hdn * Ks * LampColor[i]);
		}
	}		

    float3 diffuseColor = DiffuseTexture.Sample(DiffuseSampler,IN.UV).rgb;
    float3 result = specContrib+(diffuseColor*(diffContrib+AmbiColor));
	float alpha = 1;	
	if(TransparencyEnabled)
		alpha = TransparencyMap.Sample(TransparencySampler, IN.UV2).r;

    return float4(result, alpha);
}

/* ATTENUATION EQUATIONS

//Taken from blinn_phong_sm30
-----------------------------
 l = (lights[i].pos - IN.worldPos) / lights[i].radius;
        atten = saturate(1.0f - dot(l, l))
		
		... 
	  
        color += (material.ambient * (globalAmbient + (atten * lights[i].ambient))) +
                 (material.diffuse * lights[i].diffuse * nDotL * atten) +
                 (material.specular * lights[i].specular * power * atten);

//Taken from WIKI
-----------------
lightDir = lightDir / distance; // = normalize( lightDir );
		distance = distance * distance; //This line may be optimised using Inverse square root
 
		//Intensity of the diffuse light. Saturate to keep within the 0-1 range.
		float NdotL = dot( normal, lightDir );
		float intensity = saturate( NdotL );
 
		// Calculate the diffuse light factoring in light color, power and the attenuation
		OUT.Diffuse = intensity * light.diffuseColor * light.diffusePower / distance; 
 
                //Calculate the half vector between the light vector and the view vector.
                //This is faster than calculating the actual reflective vector.
                float3 H = normalize( lightDir + viewDir );
 
	        //Intensity of the specular light
		float NdotH = dot( normal, H );
		intensity = pow( saturate( NdotH ), specularHardness );
 
		//Sum up the specular light factoring
		OUT.Specular = intensity * light.specularColor * light.specularPower / distance; 
	}
*/