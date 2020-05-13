#include "stdafx.h"
#include "ContentImporter.h"
#include <WICTextureLoader.h>
#include "MarshableDXTypes.h"
#include "FBXWrapper.h"
#include "XAudio2StreamManager.h"

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

//eg. 1234h = 3412h
WORD SwapBytes(WORD data)
{
	return ((data & 0x00FF) << 8) | ((data & 0xFF00) >> 8);
}

//eg. 12345678h = 78563412h
DWORD SwapBytes(DWORD data)
{
	return ((data & 0x000000FF) << 24) | ((data & 0x0000FF00) << 8) | ((data & 0x00FF0000) >> 8) | ((data & 0xFF000000) >> 24);
}

WORD ReadWORD(HANDLE hFile, BOOL swapBytes)
{
	WORD data = 0;
	DWORD dwBytesActuallyRead = 0;
	::ReadFile(hFile, &data, sizeof(WORD), &dwBytesActuallyRead, NULL);		
	return (swapBytes) ? ::SwapBytes(data) : data;
}

DWORD ReadDWORD(HANDLE hFile, BOOL swapBytes)
{
	DWORD data = 0;
	DWORD dwBytesActuallyRead = 0;
	::ReadFile(hFile, &data, sizeof(DWORD), &dwBytesActuallyRead, NULL);	
	return (swapBytes) ? ::SwapBytes(data) : data;
}

DWORD ReadBytes(HANDLE hFile, UINT32 bufferSize, BYTE* pBuffer)
{
	DWORD dwBytesActuallyRead = 0;
	::ReadFile(hFile, pBuffer, bufferSize, &dwBytesActuallyRead, NULL);	
	return dwBytesActuallyRead;
}

VOID ReadChunkInfo(HANDLE hFile, CHUNKINFO *pChunkInfo)
{
	pChunkInfo->id.dwData = ::ReadDWORD(hFile, false);
	pChunkInfo->dataSize = ::ReadDWORD(hFile, false);
}

VOID SkipChunkData(HANDLE hFile, const CHUNKINFO *pChunkInfo)
{
	::SetFilePointer(hFile, pChunkInfo->dataSize, 0, FILE_CURRENT);
}

ANGELJACKETNATIVE_API void STDCALL MeasureString( HWND hWnd, LPCWSTR text, SIZE *pSize)
{
	HDC hDC = ::GetDC(hWnd);
	::ZeroMemory(pSize, sizeof(SIZE));
	::GetTextExtentPoint32W(hDC, text, ::lstrlenW(text), pSize);
	::ReleaseDC(hWnd, hDC);
}

ANGELJACKETNATIVE_API void STDCALL ContentImporter_LoadVoiceDataFromWav( LPCWSTR fileName, VOICE_DATA *pVoiceData )
{
	HANDLE hFile = ::CreateFile(fileName, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
	::SetFilePointer(hFile, 0, 0, FILE_CURRENT);

	CHUNKINFO fileHeaderChunk;
	::ZeroChunk(&fileHeaderChunk);
	::ReadChunkInfo(hFile, &fileHeaderChunk);	
	//NOTE: fileHeaderChunk.DataSize should == 4;
	//TODO: Validate correct data size.

	HEADERID formatID = {0};
	formatID.dwData = ::ReadDWORD(hFile, false);	
	//NOTE formatID.data should == 'WAVE';
	//TODO: Validate correct format ID.

	CHUNKINFO waveChunkInfo;
	::ZeroChunk(&waveChunkInfo);
	::ReadChunkInfo(hFile, &waveChunkInfo);

	::ZeroMemory(pVoiceData, sizeof(VOICE_DATA));	
	::ReadBytes(hFile, sizeof(WAVEFORMATEX) - 2, (BYTE*)&pVoiceData->format);

	CHUNKINFO dataChunkInfo;
	::ZeroChunk(&dataChunkInfo);
	::ReadChunkInfo(hFile, &dataChunkInfo);

	BYTE *pBuffer = (BYTE*)::LocalAlloc(0, dataChunkInfo.dataSize);
	::ReadBytes(hFile, dataChunkInfo.dataSize, pBuffer);
	
	::CloseHandle(hFile);
	
	pVoiceData->audioBytes = dataChunkInfo.dataSize;	
	pVoiceData->pAudioData = pBuffer;	
}

ANGELJACKETNATIVE_API LPVOID STDCALL ContentImporter_LoadStreamingAudio(LPCWSTR fileName)
{	
	MFStartup(MF_VERSION);
	return (LPVOID)XAudio2StreamingManager::Create(fileName);
}
 
ANGELJACKETNATIVE_API ID3D11Resource* STDCALL ContentImporter_CreateTextureFromFile( ID3D11DeviceContext *pDeviceContext, const wchar_t* fileName)
{
	ID3D11Device *pDevice = nullptr;
	ID3D11Resource *pTexture = nullptr;
	pDeviceContext->GetDevice(&pDevice);
	DirectX::CreateWICTextureFromFile(pDevice, pDeviceContext, fileName, &pTexture, nullptr);
	pDevice->Release();
	return pTexture;
}

//VOID SetupExportMesh(XFbxMesh *pMesh, FBX_MESH* pFbxMesh)
//{	
//	for(int i = 0; i < 4; i++)
//		for(int j = 0; j < 4; j++)
//			pFbxMesh->m[i * 4 + j] = DirectX::XMVectorGetByIndex(pMesh->Transform.r[i], j);
//	pFbxMesh->Vertices = pMesh->VertexBuffer;
//	pFbxMesh->VertexCount = pMesh->VertexCount;
//	pFbxMesh->Indices = pMesh->IndexBuffer;
//	pFbxMesh->IndexCount = pMesh->IndexCount;
//
//	if( pMesh->Children->size() != 0 )
//	{
//		SIZE_T childrenMemSize = sizeof(FBX_MESH) * pMesh->Children->size();
//		pFbxMesh->Children = (FBX_MESH*)::LocalAlloc(0, childrenMemSize);
//		::ZeroMemory(pFbxMesh->Children, childrenMemSize);
//		pFbxMesh->ChildCount = pMesh->Children->size();
//		std::list<XFbxMesh*>::iterator iterator = pMesh->Children->begin();		
//		while(iterator != pMesh->Children->end())
//		{
//			int i = std::distance(pMesh->Children->begin(), iterator);
//			FBX_MESH* pFbxChildMesh = (pFbxMesh->Children + i);
//			XFbxMesh* pChildMesh = (*iterator);
//			SetupExportMesh(pChildMesh, pFbxChildMesh);		
//			iterator++;
//		}	
//	}	
//}

ANGELJACKETNATIVE_API XFBX_SCENE* STDCALL ContentImporter_LoadFBX(LPCWSTR fileName)
{
	XFbxScene *pXScene = XFbxScene::CreateFromFile(fileName);
	XFBX_SCENE *pXMScene = pXScene->ConvertToMarshableType(true);		
	delete pXScene;
	pXScene = nullptr;
	return pXMScene;
}

