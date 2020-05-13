#include "sh-inc.hlsli"

Texture2D rgb_map;

SamplerState rgb_map_sampler
{
   Filter = MIN_MAG_LINEAR_MIP_POINT;
   AddressU = Wrap;
   AddressV = Wrap;
};

//------------------------------------------------------------------//
// Spherical harmonic lighting, per-vertex illumination effect      //
//                                                                  //
// (c) Nathaniel Hoffman 2003                                       //
//                                                                  //
// Based on 'An Efficient Representation for Irradiance             //
// Environment Maps', SIGGRAPH 2001, by Ravi Ramamoorthi and Pat    //
// Hanrahan from Stanford University                                //
//------------------------------------------------------------------//

float4 main( PS_INPUT psInput ) : SV_TARGET
{
    float4 c = psInput.Diff * rgb_map.Sample(rgb_map_sampler, psInput.Tex.xy) * 2.0f; 
    return c;
}
