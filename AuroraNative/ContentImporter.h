#define RIFF_CHUNK_ID	'RIFF'
#define WAVE_FMT_CHUNK_ID	'fmt ';
#define WAVE_DATA_CHUNK_ID	'data';

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

typedef union _HEADERID
{
	char data[4];
	DWORD dwData;
}HEADERID;

typedef struct _CHUNKINFO
{
	HEADERID id;
	DWORD dataSize;
}CHUNKINFO;

typedef struct _VOICE_DATA
{
	WAVEFORMATEX format;
	UINT32 audioBytes;
	BYTE* pAudioData;
}VOICE_DATA;

typedef struct _SIZEF
{
	FLOAT X;
	FLOAT Y;
}SIZEF;

void ZeroChunk(CHUNKINFO *pChunk)
{
	::RtlZeroMemory(pChunk, sizeof(CHUNKINFO));
}
