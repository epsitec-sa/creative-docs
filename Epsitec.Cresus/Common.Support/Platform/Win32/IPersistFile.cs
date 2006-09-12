using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	[
	  ComImport (),
	  InterfaceType (ComInterfaceType.InterfaceIsIUnknown),
	  Guid ("0000010B-0000-0000-C000-000000000046")
	]
	public interface IPersistFile
	{
		#region Methods inherited from IPersist

		void GetClassID(
		  out Guid pClassID);

		#endregion

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
		  out IntPtr ppszFileName);

	}
}
