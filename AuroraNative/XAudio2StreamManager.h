#include <initguid.h>
#include <mfapi.h>
#include <mfidl.h>
#include <mfreadwrite.h>

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

class XAudio2StreamingManager
{
private:
	bool EndOfSource;

public:
	IMFSourceReader* SourceReader;
	WAVEFORMATEX* WaveFormat;

private:
	XAudio2StreamingManager() : WaveFormat(nullptr), SourceReader(nullptr), EndOfSource(false)
	{ }	

public:
	bool IsAtEndOfSource()
	{
		return this->EndOfSource;
	}

public:
	static XAudio2StreamingManager* STDCALL Create(LPCWSTR fileName)
	{
		//*****************************************************************************************
		//This code was inspired by the Charles Petzold's MSDN article, 
		//"Streaming and Manipulating Audio Files in Windows 8" - which should work in Windows 7
		//and Vista as well. I don't believe Windows XP is supported.
		//Reference http://msdn.microsoft.com/en-us/magazine/dn166936.aspx
		//*****************************************************************************************
		
		XAudio2StreamingManager *pXAudioStreamingManager = new XAudio2StreamingManager();
		
		HRESULT hr = 0;
	
		//I. Create the byte stream.

		IMFByteStream *pByteStream = nullptr;
		hr = MFCreateFile(MF_ACCESSMODE_READ, MF_OPENMODE_FAIL_IF_NOT_EXIST, MF_FILEFLAGS_NONE, fileName, &pByteStream);

		//II. "Create an attribute for low latency operation" - what ever that means.
	
		IMFAttributes *pAttributes = nullptr;
		hr = MFCreateAttributes(&pAttributes, 1);

		//III. Use the byte stream and attributes to create stream reader.
	
		hr = MFCreateSourceReaderFromByteStream(pByteStream, pAttributes, &pXAudioStreamingManager->SourceReader);

		//IV. Create a media type of Audio with a format of FLOAT and use it to set the mediatype of the 
		//source reader.

		IMFMediaType *pMediaType = nullptr;
		hr = MFCreateMediaType(&pMediaType);
		hr = pMediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
		hr = pMediaType->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_Float);
		pXAudioStreamingManager->SourceReader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, pMediaType);

		//V. Generate wave format info that will be used to, later, create an XAudio2 Source Voice.		
	
		UINT waveFormatLength = 0;
		IMFMediaType* pCurMediaType = nullptr;
		pXAudioStreamingManager->SourceReader->GetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, &pCurMediaType);
		hr = MFCreateWaveFormatExFromMFMediaType(pCurMediaType, &pXAudioStreamingManager->WaveFormat, &waveFormatLength);
			
		//VI. Clean up - free COM interfaces and allocations...	

		pByteStream->Release();
		pByteStream = nullptr;
		pAttributes->Release();
		pAttributes = nullptr;	
		pMediaType->Release();
		pMediaType = nullptr;
		pCurMediaType->Release();
		pCurMediaType = nullptr;		

		return pXAudioStreamingManager;
	}

	void STDCALL Dispose()
	{		
		this->SourceReader->Release();
		this->SourceReader = nullptr;
		::CoTaskMemFree(this->WaveFormat);
		this->WaveFormat = nullptr;
	}

public:

	byte* GetNextBlock(int * pAudioBufferLength)
	{		
		IMFSample* pSample = nullptr;
		DWORD flags = 0;
		HRESULT hresult = 0;
		
		// Get an IMFSample object
		
		hresult = SourceReader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM,
										   0, nullptr, &flags, nullptr,
										   &pSample);		

		//Loop. (Temp Code)...
		//{{
		if (flags & MF_SOURCE_READERF_ENDOFSTREAM)
		{
			flags = 0;
			PROPVARIANT variantPosition;
			::ZeroMemory(&variantPosition, sizeof(variantPosition)); 
			variantPosition.vt = VT_I8;
			variantPosition.intVal = 0;
			SourceReader->SetCurrentPosition(GUID_NULL, variantPosition);
			hresult = SourceReader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM,
										   0, nullptr, &flags, nullptr,
										   &pSample);	
		}
		//}}

		// Check if we’re at the end of the file
		if (flags & MF_SOURCE_READERF_ENDOFSTREAM)
		{		
			EndOfSource = true;
			*pAudioBufferLength = 0;
			return nullptr;
		}
		// If not, convert the data to a contiguous buffer
		IMFMediaBuffer* pMediaBuffer = nullptr;
		hresult = pSample->ConvertToContiguousBuffer(&pMediaBuffer);
		// Lock the audio buffer and copy the samples to local memory
		byte* pAudioData = nullptr;
		DWORD audioDataLength = 0;
		hresult = pMediaBuffer->Lock(&pAudioData, nullptr, &audioDataLength);
		byte * pAudioBuffer = new byte[audioDataLength];
		CopyMemory(pAudioBuffer, pAudioData, audioDataLength);
		hresult = pMediaBuffer->Unlock();		
		*pAudioBufferLength = audioDataLength;	
		pMediaBuffer->Release();
		pMediaBuffer = nullptr;
		pSample->Release();
		pSample = nullptr;
		return pAudioBuffer;
	}
};