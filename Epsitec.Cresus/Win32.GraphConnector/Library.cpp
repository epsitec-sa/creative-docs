//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

#include "stdafx.h"
#include "Win32.GraphConnector.h"

/*****************************************************************************/

class DllLoadNotifier
{
public:
	DllLoadNotifier ()
	{
		System::Diagnostics::Debug::WriteLine ("Graph Connector DLL loaded");
	}
};

static DllLoadNotifier notifier = DllLoadNotifier ();

/*****************************************************************************/

extern "C" DLL
int
__cdecl GraphConnectorSendData(void* window, const char* documentPath, const char* meta, const char* data)
{
	//	Convert native ANSI text to .NET 'string' instances
	System::String ^ managedPath = Marshal::PtrToStringAnsi (System::IntPtr ((void*) documentPath));
	System::String ^ managedMeta = Marshal::PtrToStringAnsi (System::IntPtr ((void*) meta));
	System::String ^ managedData = Marshal::PtrToStringAnsi (System::IntPtr ((void*) data));

	System::Diagnostics::Debug::WriteLine ("GraphConnectorSendData");

	//	...and jump into C# world
	return Epsitec::Cresus::Graph::Connector::SendData (System::IntPtr (window), managedPath, managedMeta, managedData);
}

/*****************************************************************************/

extern "C" DLL
void
__cdecl GraphConnectorFreeMemory(const void* ptr)
{
	Marshal::FreeHGlobal (System::IntPtr ((void*) ptr));
}

/*****************************************************************************/

