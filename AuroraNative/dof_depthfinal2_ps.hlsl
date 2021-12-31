struct PS_INPUT
{
   float4 Pos: SV_POSITION;
   float2 texCoord: TEXCOORD0;
   float2 pixelSizeHigh : TEXCOORD1;
};

cbuffer Constants
{
	float2 fInverseViewportDimensions;
	// maximum CoC radius and diameter in pixels
	float2 vMaxCoC;
	// scale factor for maximum CoC size on low res. image
	float radiusScale;
};

Texture2D tSource;        // full resolution image
Texture2D tSourceLow     // downsampled and filtered image
{
	Filter = MIG_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

SamplerState tSourceSampler;
SamplerState tSourceLowSampler
{
	Filter = MIG_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

#define NUM_TAPS 4

// contains poisson-distributed positions on the unit circle
static float2 poisson[8] = 
{  
  float2( 0.0,      0.0),
  float2( 0.527837,-0.085868),
  float2(-0.040088, 0.536087),
  float2(-0.670445,-0.179949),
  float2(-0.419418,-0.616039),
  float2( 0.440453,-0.639399),
  float2(-0.757088, 0.349334),
  float2( 0.574619, 0.685879)
};

float4 main(PS_INPUT input) : SV_TARGET
{
  float4 cOut;
  float discRadius, discRadiusLow, centerDepth;

  // pixel size of low resolution image
  float2 pixelSizeLow = 4.0 * input.pixelSizeHigh;

  cOut = tSource.Sample(tSourceSampler, input.texCoord);   // fetch center tap
  centerDepth = cOut.a;              // save its depth

  // convert depth into blur radius in pixels
  discRadius = abs(cOut.a * vMaxCoC.y - vMaxCoC.x);

  // compute disc radius on low-res image
  discRadiusLow = discRadius * radiusScale;
  
  // reuse cOut as an accumulator
  cOut = 0;

  for(int t = 4; t < 4+NUM_TAPS; t++)
  {
    // fetch low-res tap
    float2 coordLow = input.texCoord + (pixelSizeLow * poisson[t] *
                      discRadiusLow);
    float4 tapLow = tSourceLow.Sample(tSourceLowSampler, coordLow);

    // fetch high-res tap
    float2 coordHigh = input.texCoord + (input.pixelSizeHigh * poisson[t] *
                       discRadius);
    float4 tapHigh = tSource.Sample(tSourceSampler, coordHigh);

    // put tap blurriness into [0, 1] range
    float tapBlur = abs(tapHigh.a * 2.0 - 1.0);
    
    // mix low- and hi-res taps based on tap blurriness
    float4 tap = lerp(tapHigh, tapLow, tapBlur);

    // apply leaking reduction: lower weight for taps that are
    // closer than the center tap and in focus
    tap.a = (tap.a >= centerDepth) ? 1.0 : abs(tap.a * 2.0 - 1.0);

    // accumulate
    cOut.rgb += tap.rgb * tap.a;
    cOut.a += tap.a;
  }

  // normalize and return result
  cOut = cOut / cOut.a;
  
  // alpha of 0.5 so that alpha blending averages results with
  // previous DoF pass
  cOut.a = 0.5;
  
  return cOut;
}
