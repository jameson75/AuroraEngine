// Shaped Bokeh Depth of Field
// CoC Calculations based off Knu's DoF shader for MGE
// Blurring Algorithm Created by Tomerk


#include "bokeh-knu-inc.hlsli"

//HorizontalBlur
float4 main( PSIN input ) : SV_TARGET
{
	float2 tex = input.UVCoord;
	float s = focus*depthRange;
    float z = readDepth(tex)*depthRange;
 
    float fpf = clamp(1 / s + fr, fp, fp + fpa);
    float c = base_blur_radius * dmax * (fr - fpf + 1 / z) / fr / k;
    c = min(abs(c), R) * step(eps, z);
 
	if (distantblur)
	{
		float s_depth = smoothstep(distantblur_startrange, distantblur_endrange, z);
		c = pow( s_depth, distantblur_pow ) * R;
	}
 
	if (NoWeaponBlur)
	{
		c *= smoothstep(weaponblur_cutoff-0.05, weaponblur_cutoff, z);
	}
 
	clip(c-0.001);
 
	float scale = c / 8;
 
	float4 color = lastpass.Sample(passSampler, tex) * 70;
 
	color += lastpass.Sample(passSampler, float2(tex.x-(rcpres.x)*scale, tex.y)) * 56;
	color += lastpass.Sample(passSampler, float2(tex.x+(rcpres.x)*scale, tex.y)) * 56;
 
	color += lastpass.Sample(passSampler, float2(tex.x-(rcpres.x*2)*scale, tex.y)) * 28;
	color += lastpass.Sample(passSampler, float2(tex.x+(rcpres.x*2)*scale, tex.y)) * 28;
 
	color += lastpass.Sample(passSampler, float2(tex.x-(rcpres.x*3)*scale, tex.y)) * 8;
	color += lastpass.Sample(passSampler, float2(tex.x+(rcpres.x*3)*scale, tex.y)) * 8;
 
	color += lastpass.Sample(passSampler, float2(tex.x-(rcpres.x*4)*scale, tex.y)) * 1;
	color += lastpass.Sample(passSampler, float2(tex.x+(rcpres.x*4)*scale, tex.y)) * 1;
 
	return color / 256;

}

