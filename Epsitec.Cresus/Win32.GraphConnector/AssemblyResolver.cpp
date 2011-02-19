//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

#include "stdafx.h"

/*****************************************************************************/

/*
 *	The AssemblyResolver class is called by the AppDomain when a DLL cannot
 *	be found by the standard .NET probing mechanism. See Registrar below.
 */

ref class AssemblyResolver
{
private:
	System::String^ const path;

public:
	explicit AssemblyResolver(System::String^ path)
        : path(path)
    {
	}

	System::Reflection::Assembly^ ResolveHandler(System::Object^ sender, System::ResolveEventArgs^ args)
    {
        //	We receive the name of the assembly, which contains several details; only
		//	the first element is the real name of the DLL which .NET could not locate.

		System::String^ name = args->Name->Substring (0, args->Name->IndexOf (',')) + ".dll";
        System::String^ path = System::IO::Path::Combine (this->path, name);

		if (System::IO::File::Exists (path))
		{
			return System::Reflection::Assembly::LoadFile (path);
		}
		else
		{
			return nullptr;
		}
    }
};

/*****************************************************************************/

/*
 *	The Registrar class is used to set up the AssemblyResolver so that unknown
 *	DLLs (which are not found in the caller application folder) are searched in
 *	Graph's own directory.
 */

class Registrar
{
public:
	Registrar()
	{
		System::String^ graphDirPath = (System::String^) Microsoft::Win32::Registry::GetValue ("HKEY_LOCAL_MACHINE\\SOFTWARE\\Epsitec\\Cresus Graphe\\Setup", "InstallDir", nullptr);
		
		if (System::Diagnostics::Debugger::IsAttached)
		{
			System::String^ debugDirPath = "S:\\Epsitec.Cresus\\Cresus.Graph\\bin\\Debug";

			if (System::IO::Directory::Exists (debugDirPath))
			{
				graphDirPath = debugDirPath;
			}
		}
		
		AssemblyResolver^ resolver = gcnew AssemblyResolver (graphDirPath);
		System::AppDomain::CurrentDomain->AssemblyResolve += gcnew System::ResolveEventHandler (resolver, &AssemblyResolver::ResolveHandler);
	}
};

/*****************************************************************************/

/*
 *	This singleton instance of the registrar is used to automatically set
 *	up the AssemblyResolver on DLL load (trick: constructors get run when
 *	the DLL gets first loaded by Windows, so we don't need to bother with
 *	the low level attach/detach mechanisms).
 */

static const Registrar registrar;

/*****************************************************************************/