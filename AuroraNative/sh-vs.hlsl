#include "sh-inc.hlsli"

//------------------------------------------------------------------//
// Spherical harmonic lighting, per-vertex illumination effect      //
//                                                                  //
// (c) Nathaniel Hoffman 2003                                       //
//                                                                  //
// Based on 'An Efficient Representation for Irradiance             //
// Environment Maps', SIGGRAPH 2001, by Ravi Ramamoorthi and Pat    //
// Hanrahan from Stanford University                                //
//------------------------------------------------------------------//

typedef PS_INPUT VS_OUTPUT;

VS_OUTPUT main(VS_INPUT vsInput)
{
   VS_OUTPUT Out = (VS_OUTPUT) 0; 
   Out.Pos = mul(view_proj_matrix, vsInput.vPosition);

   Out.Tex = vsInput.vTex;

   // Rotate normal from object/world space to light (view space)
   // (in RenderMonkey, lights are defined in view space).
   float4 normal4 = float4(vsInput.vNormal.x, vsInput.vNormal.y, vsInput.vNormal.z, 0.0);
   normal4 = mul(view_matrix, normal4);
   normal4.w = 1.0;

   // Evaluate spherical harmonic
   Out.Diff.r = dot(mul(r_sh_matrix, normal4), normal4);
   Out.Diff.g = dot(mul(g_sh_matrix, normal4), normal4);
   Out.Diff.b = dot(mul(b_sh_matrix, normal4), normal4);
   Out.Diff.a = 1.0;

   return Out;
}