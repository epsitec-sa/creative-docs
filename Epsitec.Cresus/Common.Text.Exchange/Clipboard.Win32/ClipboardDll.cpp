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


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
    return TRUE;
}


static void*  clipboardData = NULL;
static SIZE_T clipboardSize = 0;
static int    clipboardHtmlFormat = RegisterClipboardFormat(L"HTML Format");


// retourne la taille des données contenues dans le clipboard, ou 0 si
// aucune donnée compatible HTML n'a été trouvée.

EXPORT int ReadHtmlFromClipboard(HWND mainWindow)
{
	if (clipboardData != NULL)
	{
		free (clipboardData);
		clipboardData = NULL;
	}
	
	clipboardData = NULL;
	clipboardSize = 0;
	
	if (!IsClipboardFormatAvailable (clipboardHtmlFormat))
	{
		return 0;
	}
	if (OpenClipboard(mainWindow))
	{
		HGLOBAL globalLockHandle = GetClipboardData(clipboardHtmlFormat);
		
		if (globalLockHandle != NULL)
		{
			BYTE* pData = (BYTE*)GlobalLock(globalLockHandle);
			
			if (pData != NULL)
			{
				clipboardSize = GlobalSize(globalLockHandle);
				clipboardData = malloc(clipboardSize);
				
				if (clipboardData != NULL)
				{
					memcpy(clipboardData, pData, clipboardSize);
				}
				else
				{
					clipboardSize = 0;
				}
				
				GlobalUnlock(globalLockHandle);
			}
		}
		
		CloseClipboard();
	}
	
	return (int) clipboardSize;
}


// copie les données du clipboard dans le buffer passé en entrée; le buffer
// doit avoir une taille suffisante pour recevoir les données et il faut
// avoir appelé ReadHtmlFromClipboard avant...

EXPORT void ReadClipboardData(BYTE* buffer, int size)
{
	if (size > (int) clipboardSize)
	{
		size = (int) clipboardSize;
	}
	
	if (clipboardData != NULL)
	{
		memcpy(buffer, clipboardData, size);
	}
}


// libère la mémoire allouée lors du ReadHtmlFromClipboard()

EXPORT void FreeClipboardData()
{
	if (clipboardData != NULL)
	{
		free (clipboardData);
		
		clipboardData = NULL;
		clipboardSize = 0;
	}
}


#ifdef _MANAGED
#pragma managed(pop)
#endif

