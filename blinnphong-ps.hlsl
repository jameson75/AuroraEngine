cbuffer Constants
{
	float3 LampColor;	
	float3 AmbiColor;
	float Ks;
	float Eccentricity;
};		

Texture2D DiffuseTexture;

SamplerState DiffuseSampler;
	
struct PSINPUT
{
	float4 HPosition :		SV_POSITION;
	float3 LightVec :		TEXCOORD0;
	float3 WorldView :		TEXCOORD1;
	float3 WorldNormal :	TEXCOORD2;
	float4 WorldTangent :	TEXCOORD3;
	float3 WorldBinormal :	TEXCOORD4;
	float2 UV :				TEXCOORD5;	
};

float4 blinn_PS(PSINPUT IN) : SV_TARGET 
{
    // "standard" normalized vectors....
    float3 Ln = normalize(IN.LightVec);	
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
	//---- BEGIN MULTI-LIGHT LOOP HERE...
    float eSq = Eccentricity*Eccentricity;
    float distrib = eSq / (rdv * rdv * (eSq - 1.0) + 1.0);
    distrib = distrib * distrib;
    float Gb = 2.0 * hdn * ndv / hdv;
    float Gc = 2.0 * hdn * ldn / hdv;
    float Ga = min(1.0,min(Gb,Gc));
    float fresnelHack = 1.0 - pow(ndv,5.0);
    hdn = distrib * Ga * fresnelHack / ndv;
    float3 diffContrib = ldn * LampColor;
    float3 specContrib = hdn * Ks * LampColor;
	//--- END MULTI-LIGHT LOOP HERE...
	//--- RESTORE GETTING DIFFUSE FROM TEXTURE
    float3 diffuseColor = float4(1, 0, 0, 0); // DiffuseTexture.Sample(DiffuseSampler,IN.UV).rgb;
    float3 result = specContrib+(diffuseColor*(diffContrib+AmbiColor));
    // return as float4
    return float4(result,1);
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