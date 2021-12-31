// Shaped Bokeh Depth of Field
// CoC Calculations based off Knu's DoF shader for MGE
// Blurring Algorithm Created by Tomerk 
// **
// ** ADJUSTABLE VARIABLES 

cbuffer AppData
{	
	float1 fr;// = 60.0; // retina focus point, dpt// set slightly lower than fp to simulate myopia or as an alternative to MGE's distant blur/ set slightly higher than fp+fpa to simulate hyperopia 
	float1 fp; // = 60.0; // eye relaxed focus power, dpt
	float1 fpa; // = 10.0; // accomodation, dpt	// set lower to simulate presbyopia 
	float1 dmax; // = 0.009;  // pupil diameter range, m 
	float1 base_blur_radius; // = 1.0; // base blur radius;	// higher values mean more blur when out of DoF and shorter DoF. 
	float1 blur_falloff; // = 2.0; // More means more blur and less respect for edges 
	float1 R; // = 8.0; // maximum blur radius in pixels; 	
	float1 distantblur_startrange; // = 250; //Start range for distant blur blurring
	float1 distantblur_endrange; // = 1000; //Range at which distant blur is fully blurred
	float1 distantblur_pow; // = 1; exponent of the power function that defines the distance blur's blurring. e.g. 1 blurs linearly with distance, 2 blurs quadratically.
	float1 weaponblur_cutoff; // = 0.8; //Cutoff distance for what is considered a weapon
	float1 nearZ;
	float1 farZ;	
	bool distantblur; //Set to true for distance blur instead of lens blur
	bool NoWeaponBlur; //Set to true to keep weapons from blurring
	float2 rcpres;
};
// ** END OF
// **
 
#define SMOOTHBLUR // when on, smooths the blur, may have a performance hit
 
Texture2D thisframe : register(t0);
Texture2D Depth : register(t1);
Texture2D lastpass : register(t2);
 
SamplerState frameSampler : register(s0);
SamplerState depthSampler : register(s1);
SamplerState passSampler : register(s2);
 
static float k = 0.00001; 
static float eps = 0.000001; 
//static const float nearZ = m44proj._43 / m44proj._33;
//static const float farZ = (m44proj._33 * nearZ) / (m44proj._33 - 1.0f);
static const float depthRange = (nearZ-farZ)*0.01;

struct VSOUT
{
	float4 vertPos : SV_POSITION;
	float2 UVCoord : TEXCOORD0;
};
 
struct VSIN
{
	float2 vertPos : SV_POSITION;
	float2 UVCoord : TEXCOORD0;
};
 
typedef VSOUT PSIN;

float readDepth(float2 coord)
{
	float posZ = Depth.Sample(depthSampler, coord).x;
	return (2.0f * nearZ) / (nearZ + farZ - posZ * (farZ - nearZ));
}
 
static float focus = readDepth(float2(0.5,0.5)); 

#define M 12
static const float2 taps[M] =
{
	float2(-0.326212, -0.40581 ),
	float2(-0.840144, -0.07358 ),
	float2(-0.695914,  0.457137),
	float2(-0.203345,  0.620716),
	float2( 0.96234 , -0.194983),
	float2( 0.473434, -0.480026),
	float2( 0.519456,  0.767022),
	float2( 0.185461, -0.893124),
	float2( 0.507431,  0.064425),
	float2( 0.89642 ,  0.412458),
	float2(-0.32194 , -0.932615),
	float2(-0.791559, -0.59771 )
};