#include "stdafx.h"
#include <Effects.h>
#include <WICTextureLoader.h>
#include "MarshableDXTypes.h"

using namespace DirectX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

AURORA_NATIVE_API SkinnedEffect* STDCALL SkinnedEffect_New(ID3D11Device* device)
{
	return new SkinnedEffect(device);

}

AURORA_NATIVE_API void STDCALL SkinnedEffect_Delete(SkinnedEffect* skinnedEffect)
{
	delete skinnedEffect;
}

AURORA_NATIVE_API byte* STDCALL SkinnedEffect_SelectShaderByteCode(SkinnedEffect* skinnedEffect, UINT *pSize)
{
	byte* pShaderByteCode = nullptr;
	size_t size = 0;
	skinnedEffect->GetVertexShaderBytecode((const void**)&pShaderByteCode, &size);
	*pSize = (UINT)size;
	return pShaderByteCode;
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_Apply(SkinnedEffect* skinnedEffect, ID3D11DeviceContext* deviceContext)
{
	skinnedEffect->Apply(deviceContext);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetWorld(SkinnedEffect* skinnedEffect, float m[16])
{
	skinnedEffect->SetWorld(XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetView(SkinnedEffect* skinnedEffect, float m[16])
{
	skinnedEffect->SetView(XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetProjection(SkinnedEffect* skinnedEffect, float m[16])
{
	skinnedEffect->SetProjection(XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetDiffuseColor(SkinnedEffect* skinnedEffect, XVECTOR4 value)
{
	skinnedEffect->SetDiffuseColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetEmissiveColor(SkinnedEffect* skinnedEffect, XVECTOR4 value)
{
	skinnedEffect->SetEmissiveColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetSpecularColor(SkinnedEffect* skinnedEffect, XVECTOR4 value)
{
	skinnedEffect->SetSpecularColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetSpecularPower(SkinnedEffect* skinnedEffect, float value)
{
	skinnedEffect->SetSpecularPower(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetAlpha(SkinnedEffect* skinnedEffect, float value)
{
	skinnedEffect->SetAlpha(value);
}        

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetPerPixelLighting(SkinnedEffect* skinnedEffect, bool value)
{
	skinnedEffect->SetPerPixelLighting(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetAmbientLightColor(SkinnedEffect* skinnedEffect, XVECTOR4 value)
{
	skinnedEffect->SetAmbientLightColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetLightEnabled(SkinnedEffect* skinnedEffect, int whichLight, bool value)
{
	skinnedEffect->SetLightEnabled(whichLight, value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetLightDirection(SkinnedEffect* skinnedEffect, int whichLight, XVECTOR4 value)
{
	skinnedEffect->SetLightDirection(whichLight, XMVectorSet(value.C1, value.C2, value.C3, value.C4) );
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetLightDiffuseColor(SkinnedEffect* skinnedEffect, int whichLight, XVECTOR4 value)
{
	skinnedEffect->SetLightDiffuseColor(whichLight, XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetLightSpecularColor(SkinnedEffect* skinnedEffect, int whichLight, XVECTOR4 value)
{
	skinnedEffect->SetLightSpecularColor(whichLight, XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_EnableDefaultLighting(SkinnedEffect* skinnedEffect)
{
	skinnedEffect->EnableDefaultLighting();
}

// Fog settings.
AURORA_NATIVE_API void STDCALL SkinnedEffect_SetFogEnabled(SkinnedEffect* skinnedEffect, bool value)
{
	skinnedEffect->SetFogEnabled(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetFogStart(SkinnedEffect* skinnedEffect, float value)
{
	skinnedEffect->SetFogStart(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetFogEnd(SkinnedEffect* skinnedEffect, float value)
{
	skinnedEffect->SetFogEnd(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetFogColor(SkinnedEffect* skinnedEffect, XVECTOR4 value)
{
	skinnedEffect->SetFogColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

//// Vertex color setting.
//AURORA_NATIVE_API void STDCALL SkinnedEffect_SetVertexColorEnabled(SkinnedEffect* skinnedEffect, bool value)
//{
//	skinnedEffect->SetVertexColorEnabled(value);
//}
//
//// Texture setting.
//AURORA_NATIVE_API void STDCALL SkinnedEffect_SetTextureEnabled(SkinnedEffect* skinnedEffect, bool value)
//{
//	skinnedEffect->SetTextureEnabled(value);
//}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetTexture(SkinnedEffect* skinnedEffect, ID3D11ShaderResourceView* value)
{
	skinnedEffect->SetTexture(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetWeightsPerVertex(SkinnedEffect* skinnedEffect, int value)
{
	skinnedEffect->SetWeightsPerVertex(value);
}

AURORA_NATIVE_API void STDCALL SkinnedEffect_SetBoneTransforms(SkinnedEffect* skinnedEffect, float* bones, int count)
{
	assert(count <= SkinnedEffect::MaxBones);
	XMMATRIX _bones[SkinnedEffect::MaxBones];	
	for(int i = 0; i < SkinnedEffect::MaxBones; i++)
		_bones[i] = XMMatrixIdentity();		
	for(int i = 0; i < count; i++)
	{
		float *m = (bones + i * 16);
		_bones[i] = XMMatrixSet(m[0], m[1], m[2], m[3], 
								  m[4], m[5], m[6], m[7], 
								  m[8], m[9], m[10], m[11],
								  m[12], m[13], m[14], m[15]);
	}
	skinnedEffect->SetBoneTransforms(_bones, count);	
}
