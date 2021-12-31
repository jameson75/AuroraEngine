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

inline void ConstructXVector4(float c1, float c2, float c3, float c4, XVECTOR4& v)
{
	v.C1 = c1;
	v.C2 = c2;
	v.C3 = c3;
	v.C4 = c4;
 }

 inline void ConstructXVector4(FXMVECTOR& v, XVECTOR4& x)
{
	x.C1 = (float)XMVectorGetX(v);
	x.C2 = (float)XMVectorGetY(v);
	x.C3 = (float)XMVectorGetZ(v);
	x.C4 = (float)XMVectorGetW(v);
}

ANGELJACKETNATIVE_API SpriteFont* STDCALL SpriteFont_New(ID3D11Device* lpDevice, wchar_t const* lpFileName)
{
	return new SpriteFont(lpDevice, lpFileName);	
}

ANGELJACKETNATIVE_API SpriteFont* STDCALL SpriteFont_New_2(ID3D11Device* lpDevice, byte* pBlob, size_t dataSize )
{	
	return new SpriteFont(lpDevice, pBlob, dataSize);
}

ANGELJACKETNATIVE_API SpriteFont* STDCALL SpriteFont_New_3(ID3D11ShaderResourceView *lpTexture, SpriteFont::Glyph* pGlyph, size_t glyphCount, float lineSpacing)
{
	return new SpriteFont(lpTexture, pGlyph, glyphCount, lineSpacing);
}

ANGELJACKETNATIVE_API void STDCALL SpriteFont_DrawString( SpriteFont* spriteFont, SpriteBatch* spriteBatch, const wchar_t* text, XMFLOAT2 position, XVECTOR4 color, float rotation, XMFLOAT2 origin, XMFLOAT2 scale, SpriteEffects effects, float layerDepth)
{
	FXMVECTOR _color = XMVectorSet(color.C1, color.C2, color.C3, color.C4);
	spriteFont->DrawString( spriteBatch, text, position, _color, rotation, origin, scale, effects, layerDepth);
}

ANGELJACKETNATIVE_API XVECTOR4 STDCALL SpriteFont_MeasureString(SpriteFont* spriteFont, wchar_t const* text)
{
	XMVECTOR stringDimensions = spriteFont->MeasureString( text );
	XVECTOR4 result;
	ConstructXVector4(stringDimensions, result);
	return result;
}

ANGELJACKETNATIVE_API bool STDCALL SpriteFont_ContainsCharacter(SpriteFont* spriteFont, wchar_t character)
{
	return spriteFont->ContainsCharacter(character);
}

ANGELJACKETNATIVE_API void STDCALL SpriteFont_Delete(SpriteFont* spriteFont)
{
	delete spriteFont;
}