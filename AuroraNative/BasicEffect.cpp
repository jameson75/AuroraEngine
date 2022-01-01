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

AURORA_NATIVE_API BasicEffect* STDCALL BasicEffect_New(ID3D11Device* device)
{
	return new BasicEffect(device);
}

AURORA_NATIVE_API void STDCALL BasicEffect_Delete(BasicEffect* basicEffect)
{
	delete basicEffect;
}

AURORA_NATIVE_API byte* STDCALL BasicEffect_SelectShaderByteCode(BasicEffect* basicEffect, UINT *pSize)
{
	byte* pShaderByteCode = nullptr;
	size_t size = 0;
	basicEffect->GetVertexShaderBytecode((const void**)&pShaderByteCode, &size);
	*pSize = (UINT)size;
	return pShaderByteCode;
}

AURORA_NATIVE_API void STDCALL BasicEffect_Apply(BasicEffect* basicEffect, ID3D11DeviceContext* deviceContext)
{
	basicEffect->Apply(deviceContext);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetWorld(BasicEffect* basicEffect, float m[16])
{
	basicEffect->SetWorld(XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetView(BasicEffect* basicEffect, float m[16])
{
	basicEffect->SetView(XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetProjection(BasicEffect* basicEffect, float m[16])
{
	basicEffect->SetProjection(XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetDiffuseColor(BasicEffect* basicEffect, XVECTOR4 value)
{
	basicEffect->SetDiffuseColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetEmissiveColor(BasicEffect* basicEffect, XVECTOR4 value)
{
	basicEffect->SetEmissiveColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetSpecularColor(BasicEffect* basicEffect, XVECTOR4 value)
{
	basicEffect->SetSpecularColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetSpecularPower(BasicEffect* basicEffect, float value)
{
	basicEffect->SetSpecularPower(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetAlpha(BasicEffect* basicEffect, float value)
{
	basicEffect->SetAlpha(value);
}
        
// Light settings.
AURORA_NATIVE_API void STDCALL BasicEffect_SetLightingEnabled(BasicEffect* basicEffect, bool value)
{
	basicEffect->SetLightingEnabled(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetPerPixelLighting(BasicEffect* basicEffect, bool value)
{
	basicEffect->SetPerPixelLighting(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetAmbientLightColor(BasicEffect* basicEffect, XVECTOR4 value)
{
	basicEffect->SetAmbientLightColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetLightEnabled(BasicEffect* basicEffect, int whichLight, bool value)
{
	basicEffect->SetLightEnabled(whichLight, value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetLightDirection(BasicEffect* basicEffect, int whichLight, XVECTOR4 value)
{
	basicEffect->SetLightDirection(whichLight, XMVectorSet(value.C1, value.C2, value.C3, value.C4) );
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetLightDiffuseColor(BasicEffect* basicEffect, int whichLight, XVECTOR4 value)
{
	basicEffect->SetLightDiffuseColor(whichLight, XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetLightSpecularColor(BasicEffect* basicEffect, int whichLight, XVECTOR4 value)
{
	basicEffect->SetLightSpecularColor(whichLight, XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

AURORA_NATIVE_API void STDCALL BasicEffect_EnableDefaultLighting(BasicEffect* basicEffect)
{
	basicEffect->EnableDefaultLighting();
}

// Fog settings.
AURORA_NATIVE_API void STDCALL BasicEffect_SetFogEnabled(BasicEffect* basicEffect, bool value)
{
	basicEffect->SetFogEnabled(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetFogStart(BasicEffect* basicEffect, float value)
{
	basicEffect->SetFogStart(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetFogEnd(BasicEffect* basicEffect, float value)
{
	basicEffect->SetFogEnd(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetFogColor(BasicEffect* basicEffect, XVECTOR4 value)
{
	basicEffect->SetFogColor(XMVectorSet(value.C1, value.C2, value.C3, value.C4));
}

// Vertex color setting.
AURORA_NATIVE_API void STDCALL BasicEffect_SetVertexColorEnabled(BasicEffect* basicEffect, bool value)
{
	basicEffect->SetVertexColorEnabled(value);
}

// Texture setting.
AURORA_NATIVE_API void STDCALL BasicEffect_SetTextureEnabled(BasicEffect* basicEffect, bool value)
{
	basicEffect->SetTextureEnabled(value);
}

AURORA_NATIVE_API void STDCALL BasicEffect_SetTexture(BasicEffect* basicEffect, ID3D11ShaderResourceView* value)
{
	basicEffect->SetTexture(value);
}