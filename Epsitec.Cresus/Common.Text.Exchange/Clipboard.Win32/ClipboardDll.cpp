//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ
//  DLL pour lire le contenu du presse-papier en format "HTML format".
//  Depuis .net il semble impossible d'obtenir le texte html sans que certains
//	caractères encodé avec UTF-8 ne soient mutilés
//
//	Attention: extern "C" est nécessaire, sinon on se retrouve avec le nom "mangled" dans la DLL

#include "stdafx.h"

#ifdef _MANAGED
#pragma managed(push, off)
#endif

#define EXPORT extern "C" __declspec( dllexport )

#
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
    return TRUE;
}


static HANDLE globalLockHandle ;
static int clipboardSize ;


// retourne un pointeur sur les octets du presse-papiers
// retourne NULL si OpenClipboard ne fonctionne pas.

EXPORT BYTE *ReadHtmlFromClipboard()
{
  if (OpenClipboard(NULL))
  {
	  UINT format = RegisterClipboardFormat(L"HTML Format") ;
	  globalLockHandle = GetClipboardData(format) ;

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


// retourne la taille du contenu du presse-papiers. ReadHtmlFromClipboard() doit
// avoir été appllé auparavant

EXPORT int GetClipboardSize()
{
	return clipboardSize ;
}

// libère la mémoire allouée lors du ReadHtmlFromClipboard()

EXPORT void FreeClipboard()
{
	GlobalUnlock(globalLockHandle) ;
	GlobalFree(globalLockHandle) ;
}


#ifdef _MANAGED
#pragma managed(pop)
#endif

