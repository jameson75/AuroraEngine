#include "stdafx.h"
#include "XAudio2StreamManager.h"

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

NATIVE_API LPVOID STDCALL XAudio2StreamingManager_Create(LPCWSTR fileName)
{	
	MFStartup(MF_VERSION);
	return (LPVOID)XAudio2StreamingManager::Create(fileName);
}

NATIVE_API VOID STDCALL XAudio2StreamingManager_Dispose(LPVOID pXAudioStreamingManager)
{
	((XAudio2StreamingManager*)pXAudioStreamingManager)->Dispose();
	delete pXAudioStreamingManager;
	pXAudioStreamingManager = nullptr;
}

NATIVE_API BOOL STDCALL XAudio2StreamingManager_IsAtEndOfStream(LPVOID pXAudioStreamingManager)
{
	return ((XAudio2StreamingManager*)pXAudioStreamingManager)->IsAtEndOfSource();	
}

NATIVE_API LPVOID STDCALL XAudio2StreamingManager_GetNextBlock(LPVOID pXAudioStreamingManager, int *blockLength)
{
	return ((XAudio2StreamingManager*)pXAudioStreamingManager)->GetNextBlock(blockLength);
}

NATIVE_API VOID STDCALL XAudio2StreamingManager_GetWaveFormat(LPVOID pXAudioStreamingManager, WAVEFORMATEX* pWaveFormat)
{
	::CopyMemory(pWaveFormat, ((XAudio2StreamingManager*)pXAudioStreamingManager)->WaveFormat, sizeof(WAVEFORMATEX));
}

NATIVE_API VOID STDCALL XAudio2StreamingManager_DestroyBlock(LPVOID block)
{
	//NOTE: It is expected that "block" was allocated using "new[]". See XAudioStreamingManager::GetNextBlock().
	delete[] block;
}