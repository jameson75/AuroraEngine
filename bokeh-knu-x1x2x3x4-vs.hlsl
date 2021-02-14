// Shaped Bokeh Depth of Field
// CoC Calculations based off Knu's DoF shader for MGE
// Blurring Algorithm Created by Tomerk
#include "bokeh-knu-inc.hlsli"

//FrameVS
VSOUT main(VSIN IN)
{
	VSOUT OUT = (VSOUT)0.0f; 
	OUT.vertPos = float4(IN.vertPos, 0, 1.0f);
	OUT.UVCoord = IN.UVCoord;
	return OUT;
}
 
