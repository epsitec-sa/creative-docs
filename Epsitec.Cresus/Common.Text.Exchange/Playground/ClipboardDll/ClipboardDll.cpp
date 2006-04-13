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
#if 0
	TCHAR buffer[100] ;
	wsprintf (buffer, L"reason = %d, reserved = %d", ul_reason_for_call, lpReserved) ;
	MessageBox(0, buffer, L"2", 0);
#endif
    return TRUE;
}



// extern "C" est nécessaire, sinon on se retrouve avec le nom "mangled" dans la DLL

extern "C" __declspec( dllexport ) int TestEntry(int value)
{
	return 2*value ;
}


static HANDLE globalLockHandle ;
static int clipboardSize ;

extern "C" __declspec( dllexport ) BYTE *ReadHtmlFromClipboard()
{
  if (OpenClipboard(NULL))
  {
	  globalLockHandle = GetClipboardData(CF_TEXT) ;

	  if (globalLockHandle != NULL)
	  {
		BYTE* pData = (BYTE*)GlobalLock(globalLockHandle);
		clipboardSize = GlobalSize(globalLockHandle);
		CloseClipboard() ;
		return pData ;
	  }
	  
	  return (BYTE*)1 ;
  }

  return NULL ;
}


extern "C" __declspec( dllexport ) int GetClipboardSize()
{
	return clipboardSize ;
}


extern "C" __declspec( dllexport ) void FreeClipboard()
{
	GlobalUnlock(globalLockHandle) ;
	GlobalFree(globalLockHandle) ;
}



#ifdef _MANAGED
#pragma managed(pop)
#endif

