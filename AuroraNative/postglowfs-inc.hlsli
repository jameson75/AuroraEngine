///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Credits: NVIDIA
// Comments: Source was derived from NVIDIA Shader Library for FX Composer
//			 and coverted to shader 4.x syntax.
// License <see below>
///////////////////////////////////////////////////////////////////////////////

/******************************************************************************************************************
NVIDIA LICENSE
Copyright 2008 NVIDIA Corporation
BY DOWNLOADING THE SOFTWARE AND OTHER AVAILABLE MATERIALS, YOU  ("DEVELOPER") AGREE TO BE BOUND BY THE FOLLOWING TERMS AND CONDITIONS
The materials available for download to Developers may include software in both sample source ("Source Code") and object code ("Object Code") versions, documentation ("Documentation"), certain art work ("Art Assets") and other materials (collectively, these materials referred to herein as "Materials").  Except as expressly indicated herein, all terms and conditions of this Agreement apply to all of the Materials.
Except as expressly set forth herein, NVIDIA owns all of the Materials and makes them available to Developer only under the terms and conditions set forth in this Agreement.
License:  Subject to the terms of this Agreement, NVIDIA hereby grants to Developer a royalty-free, non-exclusive license to possess and to use the Materials.  The following terms apply to the specified type of Material:
Source Code:  Developer shall have the right to modify and create derivative works with the Source Code.  Developer shall own any derivative works ("Derivatives") it creates to the Source Code, provided that Developer uses the Materials in accordance with the terms of this Agreement.  Developer may distribute the Derivatives, provided that all NVIDIA copyright notices and trademarks are used properly and the Derivatives include the following statement: "This software contains source code provided by NVIDIA Corporation."  
Object Code:  Developer agrees not to disassemble, decompile or reverse engineer the Object Code versions of any of the Materials.  Developer acknowledges that certain of the Materials provided in Object Code version may contain third party components that may be subject to restrictions, and expressly agrees not to attempt to modify or distribute such Materials without first receiving consent from NVIDIA.
Art Assets:  Developer shall have the right to modify and create Derivatives of the Art Assets, but may not distribute any of the Art Assets or Derivatives created therefrom without NVIDIA’s prior written consent.
Government End Users: If you are acquiring the Software on behalf of any unit or agency of the United States Government, the following provisions apply. The Government agrees the Software and documentation were developed at private expense and are provided with “RESTRICTED RIGHTS”. Use, duplication, or disclosure by the Government is subject to restrictions as set forth in DFARS 227.7202-1(a) and 227.7202-3(a) (1995), DFARS 252.227-7013(c)(1)(ii) (Oct 1988), FAR 12.212(a)(1995), FAR 52.227-19, (June 1987) or FAR 52.227-14(ALT III) (June 1987),as amended from time to time. In the event that this License, or any part thereof, is deemed inconsistent with the minimum rights identified in the Restricted Rights provisions, the minimum rights shall prevail.
No Other License. No rights or licenses are granted by NVIDIA under this License, expressly or by implication, with respect to any proprietary information or patent, copyright, trade secret or other intellectual property right owned or controlled by NVIDIA, except as expressly provided in this License.
Term:  This License is effective until terminated.  NVIDIA may terminate this Agreement (and with it, all of Developer’s right to the Materials) immediately upon written notice (which may include email) to Developer, with or without cause.
Support:  NVIDIA has no obligation to support or to continue providing or updating any of the Materials.
No Warranty:  THE SOFTWARE AND ANY OTHER MATERIALS PROVIDED BY NVIDIA TO DEVELOPER HEREUNDER ARE PROVIDED "AS IS."  NVIDIA DISCLAIMS ALL WARRANTIES, EXPRESS, IMPLIED OR STATUTORY, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF TITLE, MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
LIMITATION OF LIABILITY:  NVIDIA SHALL NOT BE LIABLE TO DEVELOPER, DEVELOPER’S CUSTOMERS, OR ANY OTHER PERSON OR ENTITY CLAIMING THROUGH OR UNDER DEVELOPER FOR ANY LOSS OF PROFITS, INCOME, SAVINGS, OR ANY OTHER CONSEQUENTIAL, INCIDENTAL, SPECIAL, PUNITIVE, DIRECT OR INDIRECT DAMAGES (WHETHER IN AN ACTION IN CONTRACT, TORT OR BASED ON A WARRANTY), EVEN IF NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.  THESE LIMITATIONS SHALL APPLY NOTWITHSTANDING ANY FAILURE OF THE ESSENTIAL PURPOSE OF ANY LIMITED REMEDY.  IN NO EVENT SHALL NVIDIA’S AGGREGATE LIABILITY TO DEVELOPER OR ANY OTHER PERSON OR ENTITY CLAIMING THROUGH OR UNDER DEVELOPER EXCEED THE AMOUNT OF MONEY ACTUALLY PAID BY DEVELOPER TO NVIDIA FOR THE SOFTWARE OR ANY OTHER MATERIALS.
******************************************************************************************************************************************************/

// Relative filter weights indexed by distance (in texels) from "home" texel
//   (WT_0 is the "home" or center of the filter, WT_4 is four texels away)
// Try changing these around for different filter patterns....
#define WT_0 1.0
#define WT_1 0.9
#define WT_2 0.55
#define WT_3 0.18
#define WT_4 0.1
// these ones are based on the above....
#define WT_NORMALIZE (WT_0+2.0*(WT_1+WT_2+WT_3+WT_4))
#define KW_0 (WT_0/WT_NORMALIZE)
#define KW_1 (WT_1/WT_NORMALIZE)
#define KW_2 (WT_2/WT_NORMALIZE)
#define KW_3 (WT_3/WT_NORMALIZE)
#define KW_4 (WT_4/WT_NORMALIZE)

#define FLIP_TEXTURE_Y	/* Different in OpenGL & DirectX */

cbuffer ApplicationParameters
{
	//[min=0.2f, max=8.0f, step=0.05f]
	float GlowSpan = 2.5f; //Span of texels to sample.
	//[min=0.0f, max=2.0f, step=0.05f]
	float Glowness = 0.7f; //Glow Strength.
	//[min=0.0f, max=2.0f, step=0.05f]
	float Sceneness = 0.3f; //Scene Strength.
	float ClearDepth = 1.0;
	float4 ClearColor = {0,0,0,0};	
	float2 ViewportSize;
};

// nine texcoords, to sample nine in-line texels
struct NineTexelVertex
{
    float4 Position   : SV_POSITION;
    float2 UV    : TEXCOORD0;
    float4 UV1   : TEXCOORD1; // xy AND zw used as UV coords
    float4 UV2   : TEXCOORD2; // xy AND zw used as UV coords
    float4 UV3   : TEXCOORD3; // xy AND zw used as UV coords
    float4 UV4   : TEXCOORD4; // xy AND zw used as UV coords
};

struct OneTexelVertex {
    float4 Position	: SV_POSITION;
    float2 UV		: TEXCOORD0;
};