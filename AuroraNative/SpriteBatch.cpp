#include "stdafx.h"
#include <SpriteFont.h>
#include <SpriteBatch.h>
#include <WICTextureLoader.h>
#include "MarshableDXTypes.h"

using namespace DirectX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

AURORA_NATIVE_API SpriteBatch* STDCALL SpriteBatch_New(ID3D11DeviceContext* deviceContext)
{
	return new SpriteBatch(deviceContext);
}

AURORA_NATIVE_API void STDCALL SpriteBatch_Begin(SpriteBatch* spriteBatch, SpriteSortMode sortMode, ID3D11BlendState* pBlendState, ID3D11SamplerState* pSamplerState, ID3D11DepthStencilState* pDepthStencilState, ID3D11RasterizerState* pRasterizerState, void (_cdecl* customShaderFunc)(void), float m[16])
{	
	 XMMATRIX transformMatrix = (m != NULL) ? XMMatrixSet(m[0], m[1], m[2], m[3], 
											m[4], m[5], m[6], m[7], 
											m[8], m[9], m[10], m[11],
											m[12], m[13], m[14], m[15]) : XMMatrixIdentity();
	 std::function<void()> setCustomShader = nullptr;
	 if(customShaderFunc != nullptr) 
		setCustomShader = customShaderFunc;
	 spriteBatch->Begin(sortMode, pBlendState, pSamplerState, pDepthStencilState, pRasterizerState, setCustomShader, transformMatrix);
}

AURORA_NATIVE_API void STDCALL SpriteBatch_Begin_2(SpriteBatch* spriteBatch)
{
	spriteBatch->Begin();
}

AURORA_NATIVE_API void STDCALL SpriteBatch_End(SpriteBatch* spriteBatch)
{
	spriteBatch->End();
}

AURORA_NATIVE_API void STDCALL SpriteBatch_Draw(SpriteBatch* spriteBatch, ID3D11ShaderResourceView* texture, XMFLOAT2 position, RECT* sourceRectangle, XVECTOR4 color, float rotation, XMFLOAT2 _origin, XMFLOAT2 _scale, SpriteEffects effects, float layerDepth)
{
	XMVECTOR _color = XMVectorSet(color.C1, color.C2, color.C3, color.C4);
	spriteBatch->Draw(texture, position, sourceRectangle, _color, rotation, _origin, _scale, (SpriteEffects)effects, layerDepth);
}

AURORA_NATIVE_API void STDCALL SpriteBatch_Draw_2(SpriteBatch* spriteBatch, ID3D11ShaderResourceView* texture, RECT destinationRectangle, RECT* sourceRectangle, XVECTOR4 color, float rotation, XMFLOAT2 origin, SpriteEffects effects, float layerDepth)
{
	XMVECTOR _color = XMVectorSet(color.C1, color.C2, color.C3, color.C4);
	spriteBatch->Draw(texture, destinationRectangle, sourceRectangle, _color, rotation, origin, effects, layerDepth);
}

AURORA_NATIVE_API void STDCALL SpriteBatch_Delete(SpriteBatch* spriteBatch)
{
	delete spriteBatch;
}
