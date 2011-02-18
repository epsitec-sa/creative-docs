//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	[ComImport]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid ("0000010B-0000-0000-C000-000000000046")]
	
	public interface IPersistFile
	{
		void GetClassID(out System.Guid pClassID);

		[PreserveSig]
		int IsDirty();

		void Load(
		  [MarshalAs (UnmanagedType.LPWStr)] string pszFileName,
		  int dwMode);

		void Save(
		  [MarshalAs (UnmanagedType.LPWStr)] string pszFileName,
		  [MarshalAs (UnmanagedType.Bool)] bool fRemember);

		void SaveCompleted(
		  [MarshalAs (UnmanagedType.LPWStr)] string pszFileName);

		void GetCurFile(
		  out System.IntPtr ppszFileName);
	}
}
