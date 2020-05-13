// Shaped Bokeh Depth of Field
// CoC Calculations based off Knu's DoF shader for MGE
// Blurring Algorithm Created by Tomerk

#include "bokeh-knu-inc.hlsli"

//SmartBlur
float4 main( PSIN input ) : SV_TARGET
{
	float2 tex = input.UVCoord;
    float4 color = thisframe.Sample(frameSampler, tex);
    float c = base_blur_radius * 2 * R * (lastpass.Sample(passSampler, tex).r - 0.5);
 
    float amount = 1;
 
    for (int i = 0; i < M; i++)
    {
 
        float2 dir = taps[i];
 
        float2 s_tex = tex + rcpres * dir * c;
        float4 s_color = thisframe.Sample(frameSampler, s_tex);
        float s_c = (lastpass.Sample(passSampler, s_tex).r - 0.5) * 2 * R;
        float weight = min(exp2(-(c-s_c) / blur_falloff), 2);
 
        color += s_color * weight;
        amount += weight;
    }
 
    return float4(color.rgb / amount, 1);

}

