//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Michael WALZ, Maintainer: Pierre ARNAUD

/*
 *	This DLL is used to interface .NET with the clipboard API, in order
 *	to be able to properly manipulate data in the "HTML format" without
 *	having .NET interfer with the UTF-8 encoding (problem which exists
 *	in .NET version 2.0).
 */

#include "stdafx.h"

/*****************************************************************************/

BOOL
APIENTRY
DllMain(HMODULE hModule, DWORD ulReasonForCall, LPVOID lpReserved)
{
	return TRUE;
}

struct ClipboardData
{
	void*  data;
	int    size;
};


/*****************************************************************************/

static int clipboardHtmlFormat = ::RegisterClipboardFormat(L"HTML Format");

/*****************************************************************************/

/*
 *	Reads the "HTML Format" data found in the clipboard (if any) and
 *	stores the data in the provided structure. Returns the size of the
 *	data, zero if no compatible data was found or -1 if no structure
 *	was provided or if the clipboard could not be opened.
 */

EXPORT
int
__stdcall
ReadHtmlFromClipboard(ClipboardData* data)
{
	if (data == NULL)
	{
		return -1;
	}
	if (data->data != NULL)
	{
		free (data->data);
	}
	
	data->data = NULL;
	data->size = 0;
	
	if (!::IsClipboardFormatAvailable (clipboardHtmlFormat))
	{
		return 0;
	}

	if (::OpenClipboard (NULL))
	{
		HGLOBAL globalLockHandle = ::GetClipboardData (clipboardHtmlFormat);
		
		if (globalLockHandle != NULL)
		{
			BYTE* pData = (BYTE*)::GlobalLock (globalLockHandle);
			
			if (pData != NULL)
			{
				data->size = ::GlobalSize (globalLockHandle) & 0x7fffffff;
				data->data = ::malloc (data->size);
				
				if (data->data != NULL)
				{
					memcpy (data->data, pData, data->size);
				}
				else
				{
					data->size = 0;
				}
				
				::GlobalUnlock (globalLockHandle);
			}
		}
		
		::CloseClipboard ();
		
		return data->size;
	}
	else
	{
		return -1;
	}
}

/*
 *	Copies the data from the structure to the provided buffer.
 *	The ReadHtmlFromClipboard function must have been called
 *	previously.
 */

EXPORT
void
__stdcall
CopyClipboardData(const ClipboardData* data, BYTE* buffer, int size)
{
	if (data != NULL)
	{
		if (size > (int) data->size)
		{
			size = (int) data->size;
		}
		
		if ((data->data != NULL) &&
			(size > 0))
		{
			memcpy (buffer, data->data, size);
		}
	}
}


/*
 *	Frees the memory allocated by ReadHtmlFromClipboard.
 */

EXPORT
void
__stdcall
FreeClipboardData(ClipboardData* data)
{
	if (data != NULL)
	{
		if (data->data != NULL)
		{
			free (data->data);
			
			data->data = NULL;
			data->size = 0;
		}
	}
}

/*****************************************************************************/
