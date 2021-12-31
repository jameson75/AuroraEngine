// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//
#pragma once
#define _STDAFX_H
#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>

// TODO: reference additional headers your program requires here
#include <xaudio2.h>
#include <d3d11.h>
#include <DirectXMath.h>

#define NATIVE_API extern "C" __declspec(dllexport)
#define AURORA_NATIVE_API NATIVE_API
#define STDCALL __stdcall
