// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include <mfapi.h>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		//Initialize Microsoft Media Foundation services when this
		//dll is, initially, loaded into the process.
		//MFStartup(MF_VERSION);		
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		//Uninitialize Microsoft Media Foundation services when this
		//dll is unloaded from the process.
		//MFShutdown();
		break;
	}
	return TRUE;
}

