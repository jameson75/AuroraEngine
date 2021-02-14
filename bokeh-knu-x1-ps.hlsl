// Shaped Bokeh Depth of Field
// CoC Calculations based off Knu's DoF shader for MGE
// Blurring Algorithm Created by Tomerk

#include "bokeh-knu-inc.hlsli"

//DepthOfField
float4 main( PSIN input) : SV_TARGET
{
	float2 tex = input.UVCoord;
	float s = focus*depthRange;
	float z = readDepth(tex)*depthRange;
 
	float fpf = clamp(1 / s + fr, fp, fp + fpa);
	float c = dmax * (fr - fpf + 1 / z) / fr / k;
	c = sign(z-s) * min(abs(c), R) / (2 * R) * step(eps, z);
 
	if (distantblur)
	{
		float s_depth = smoothstep(distantblur_startrange, distantblur_endrange, z);
		c = 0.5 * pow( s_depth, distantblur_pow );
	}
 
	if (NoWeaponBlur)
	{
		c *= smoothstep(weaponblur_cutoff-0.05, weaponblur_cutoff, z);
	}
 
	c += 0.5;
 
	return float4(c, c, c, c);
}
