#include "stdafx.h"
#include "ContentImporter.h"
#include <WICTextureLoader.h>
#include "MarshableDXTypes.h"
#include "XAudio2StreamManager.h"

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
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

AURORA_NATIVE_API void STDCALL MeasureString( HWND hWnd, LPCWSTR text, SIZE *pSize)
{
	HDC hDC = ::GetDC(hWnd);
	::ZeroMemory(pSize, sizeof(SIZE));
	::GetTextExtentPoint32W(hDC, text, ::lstrlenW(text), pSize);
	::ReleaseDC(hWnd, hDC);
}

AURORA_NATIVE_API void STDCALL ContentImporter_LoadVoiceDataFromWav( LPCWSTR fileName, VOICE_DATA *pVoiceData )
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

AURORA_NATIVE_API LPVOID STDCALL ContentImporter_LoadStreamingAudio(LPCWSTR fileName)
{	
	MFStartup(MF_VERSION);
	return (LPVOID)XAudio2StreamingManager::Create(fileName);
}
 
AURORA_NATIVE_API ID3D11Resource* STDCALL ContentImporter_CreateTextureFromFile( ID3D11DeviceContext *pDeviceContext, const wchar_t* fileName)
{
	ID3D11Device *pDevice = nullptr;
	ID3D11Resource *pTexture = nullptr;
	pDeviceContext->GetDevice(&pDevice);
	DirectX::CreateWICTextureFromFile(pDevice, pDeviceContext, fileName, &pTexture, nullptr);	
	pDevice->Release();
	return pTexture;
}

AURORA_NATIVE_API ID3D11Resource* STDCALL ContentImporter_CreateTextureFromMemory(ID3D11DeviceContext *pDeviceContext, const uint8_t *pBlob, size_t dataSize)
{
	ID3D11Device *pDevice = nullptr;
	ID3D11Resource *pTexture = nullptr;
	pDeviceContext->GetDevice(&pDevice);
	DirectX::CreateWICTextureFromMemory(pDevice, pDeviceContext, pBlob, dataSize, &pTexture, nullptr);
	pDevice->Release();
	return pTexture;
}
