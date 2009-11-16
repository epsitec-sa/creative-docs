//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

#include "stdafx.h"
#include "GraphConnector.Win32.h"


extern "C" DLL
int
__cdecl GraphConnectorSendData(void* window, const char* documentPath, const char* meta, const char* data)
{
	//	Convertit le texte natif ANSI en une instance de la classe .NET 'string' :
	System::String ^ managedPath = Marshal::PtrToStringAnsi (System::IntPtr ((void*) documentPath));
	System::String ^ managedMeta = Marshal::PtrToStringAnsi (System::IntPtr ((void*) meta));
	System::String ^ managedData = Marshal::PtrToStringAnsi (System::IntPtr ((void*) data));

	Epsitec::Cresus::Graph::Connector::SendData (System::IntPtr (window), managedPath, managedMeta, managedData);

	return 0;
}

extern "C" DLL
void
__cdecl GraphConnectorFreeMemory(const void* ptr)
{
	Marshal::FreeHGlobal (System::IntPtr ((void*) ptr));
}
