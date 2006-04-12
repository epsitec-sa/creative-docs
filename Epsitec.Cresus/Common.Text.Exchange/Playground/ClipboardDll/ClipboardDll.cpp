// ClipboardDll.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"


#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	TCHAR buffer[100] ;
	wsprintf (buffer, L"reason = %d, reserved = %d", ul_reason_for_call, lpReserved) ;
	MessageBox(0, buffer, L"2", 0);

    return TRUE;
}



// extern "C" est nécessaire, sinon on se retrouve avec le nom "mangled" dans la DLL

extern "C" __declspec( dllexport ) int TestEntry(int value)
{
	return 2*value ;
}



#ifdef _MANAGED
#pragma managed(pop)
#endif

