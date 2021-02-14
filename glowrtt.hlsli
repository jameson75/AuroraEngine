/*********************************************************************NVMH3****
*******************************************************************************
$Revision: #4 $

Copyright NVIDIA Corporation 2008
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

To learn more about shading, shaders, and to bounce ideas off other shader
    authors and users, visit the NVIDIA Shader Library Forums at:

    http://developer.nvidia.com/forums/

*******************************************************************************
******************************************************************************/


// pre-multiply and un-pre-mutliply functions. The precision
//	of thse operations is often limited to 8-bit so don't
//	always count on them!
// The macro value of NV_ALPHA_EPSILON, if defined, is used to
//	avoid IEEE "NaN" values that may occur when erroneously
//	dividing by a zero alpha (thanks to Pete Warden @ Apple
//	Computer for the suggestion in GPU GEMS II)

// multiply color by alpha to turn an un-premultipied
//	pixel value into a premultiplied one
float4 premultiply(QUAD_REAL4 C)
{
    return float4((C.w*C.xyz),C.w);
}

#define NV_ALPHA_EPSILON 0.0001

// given a premultiplied pixel color, try to undo the premultiplication.
// beware of precision errors
float4 unpremultiply(QUAD_REAL4 C)
{
#ifdef NV_ALPHA_EPSILON
    float a = C.w + NV_ALPHA_EPSILON;
    return float4((C.xyz / a),C.w);
#else /* ! NV_ALPHA_EPSILON */
    return float4((C.xyz / C.w),C.w);
#endif /* ! NV_ALPHA_EPSILON */
}

/////////////////////////////////////////////////////////////////////////
// Structure Declaration ////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////

struct QuadVertexOutput {
    float4 Position	: POSITION;
    float2 UV	: TEXCOORD0;
};

/////////////////////////////////////////////////////////////////////////
// Hidden tweakables declared by this .fxh file /////////////////////////
/////////////////////////////////////////////////////////////////////////

float2 QuadScreenSize;

////////////////////////////////////////////////////////////
////////////////////////////////// vertex shaders //////////
////////////////////////////////////////////////////////////

QuadVertexOutput ScreenQuadVS(
    QUAD_REAL3 Position : POSITION, 
    QUAD_REAL3 TexCoord : TEXCOORD0
) {
    QuadVertexOutput OUT;
    OUT.Position = QUAD_REAL4(Position, 1);
#ifdef NO_TEXEL_OFFSET
    OUT.UV = TexCoord.xy;
#else /* NO_TEXEL_OFFSET */
    OUT.UV = QUAD_REAL2(TexCoord.xy+QuadTexelOffsets); 
#endif /* NO_TEXEL_OFFSET */
    return OUT;
}

QuadVertexOutput ScreenQuadVS2(
    QUAD_REAL3 Position : POSITION, 
    QUAD_REAL3 TexCoord : TEXCOORD0,
    uniform QUAD_REAL2 TexelOffsets
) {
    QuadVertexOutput OUT;
    OUT.Position = QUAD_REAL4(Position, 1);
    OUT.UV = QUAD_REAL2(TexCoord.xy+TexelOffsets); 
    return OUT;
}

//////////////////////////////////////////////////////
////////////////////////////////// pixel shaders /////
//////////////////////////////////////////////////////

//
// Draw textures into screen-aligned quad
//
QUAD_REAL4 TexQuadPS(QuadVertexOutput IN,
    uniform sampler2D InputSampler) : COLOR
{   
    QUAD_REAL4 texCol = tex2D(InputSampler, IN.UV);
    return texCol;
}  

QUAD_REAL4 TexQuadBiasPS(QuadVertexOutput IN,
    uniform sampler2D InputSampler,QUAD_REAL TBias) : COLOR
{   
    QUAD_REAL4 texCol = tex2Dbias(InputSampler, QUAD_REAL4(IN.UV,0,TBias));
    return texCol;
}  

#endif /* _QUAD_FXH */

////////////// eof ///



/*********************************************************************NVMH3****
*******************************************************************************
$Revision: #4 $

Copyright NVIDIA Corporation 2008
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

Comments:
    Utility declarations for doing 2D blur effects in FXComposer.

    To use:
    * Choose a render target size. The default is 256x256. You can
	change this by defining RTT_SIZE before including this header.
    * Choose filter size and filter weights. 9x9 and 5x5 filter routines
	are supplied in this file. They use filter weights defined
	as WT9_0 through WT9_4 and WT5_0 through WT5_2. Again, if you
	want something other than the default, specify these weights
	via #define before #including this header file.
    * Declare render targets. Use the SQUARE_TARGET macro to declare both the
	texturetarget and a sampler to read it. For two-pass
	convolutions you'll need at least two such targets.

    For an example of use, see the "post_glow_screenSize" shader



To learn more about shading, shaders, and to bounce ideas off other shader
    authors and users, visit the NVIDIA Shader Library Forums at:

    http://developer.nvidia.com/forums/

*******************************************************************************
******************************************************************************/



#ifndef _H_BLUR59_
#define _H_BLUR59_

#include <include\\Quad.fxh>

// Default rendertargets are this size.
//   If you want a different size, define RTT_SIZE before
// 	including this file.
#ifndef RTT_SIZE
#define RTT_SIZE 256
#endif /* RTT_SIZE */

#define RTT_TEXEL_SIZE (1.0f/RTT_SIZE)

#ifdef ONLY_FIXED_SIZE_SQUARE_TEXTURES
QUAD_REAL gRTTTexelIncrement <
    string UIName =  "RTT Texel Size";
> = RTT_TEXEL_SIZE;
#endif /* ONLY_FIXED_SIZE_SQUARE_TEXTURES */

//
// By default, samples are one texel apart. You can redefine this value
//	(or assign it to a global parameter)
//
#ifndef BLUR_STRIDE
#define BLUR_STRIDE 1.0
#endif /* !BLUR_STRIDE */



// weights for 5x5 filtering

#ifndef WT5_0
// Relative filter weights indexed by distance (in texels) from "home" texel
//	(WT5_0 is the "home" or center of the filter, WT5_2 is two texels away)
#define WT5_0 1.0
#define WT5_1 0.8
#define WT5_2 0.2
#endif /* WT5_0 */


// these values are based on WT5_0 through WT5_2
#define WT5_NORMALIZE (WT5_0+2.0*(WT5_1+WT5_2))
#define K5_0 (WT5_0/WT5_NORMALIZE)
#define K5_1 (WT5_1/WT5_NORMALIZE)
#define K5_2 (WT5_2/WT5_NORMALIZE)

// RTT Textures

// call SQUARE_TARGET(tex,sampler) to create the declarations for a rendertarget
//	texture and its associated sampler. You will get a square 8-bit texture
//	of RTT_SIZE texels on each side

#define SQUARE_TARGET(texName,samplerName) texture texName : RENDERCOLORTARGET < \
    int width = RTT_SIZE; \
    int height = RTT_SIZE; \
    int MIPLEVELS = 1; \
    string format = "X8R8G8B8"; \
    string UIWidget = "None"; \
>; \
sampler2D samplerName = sampler_state { \
    texture = <texName>; \
    AddressU = Clamp; \
    AddressV = Clamp; \
    Filter = MIN_MAG_LINEAR_MIP_POINT; \
};

/************* DATA STRUCTS **************/

// nine texcoords, to sample (usually) nine in-line texels
struct ScreenAligned9TexelVOut
{
    QUAD_REAL4 Position : POSITION;
    QUAD_REAL2 UV  : TEXCOORD0;
    QUAD_REAL4 UV1 : TEXCOORD1; // these contain xy and zw pairs
    QUAD_REAL4 UV2 : TEXCOORD2;
    QUAD_REAL4 UV3 : TEXCOORD3;
    QUAD_REAL4 UV4 : TEXCOORD4;
};

// five texcoords, to sample (usually) five in-line texels
struct ScreenAligned5TexelVOut
{
    QUAD_REAL4 Position : POSITION;
    QUAD_REAL2 UV  : TEXCOORD0;
    QUAD_REAL4 UV1 : TEXCOORD1; // these contain xy and zw pairs
    QUAD_REAL4 UV2 : TEXCOORD2;
};


///////////////////////////// eof ////
