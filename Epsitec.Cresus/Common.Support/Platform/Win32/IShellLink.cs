//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	[ComImport]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid ("000214F9-0000-0000-C000-000000000046")]
	
	internal interface IShellLink
	{
		void GetPath([Out, MarshalAs (UnmanagedType.LPWStr)] System.Text.StringBuilder pszFile, int cchMaxPath, out ShellApi.WIN32_FIND_DATAW pfd, ShellApi.SLGP_FLAGS fFlags);

		void GetIDList(out System.IntPtr ppidl);

		void SetIDList(System.IntPtr pidl);

		void GetDescription([Out, MarshalAs (UnmanagedType.LPWStr)] System.Text.StringBuilder pszName, int cchMaxName);

		void SetDescription([MarshalAs (UnmanagedType.LPWStr)] string pszName);

		void GetWorkingDirectory([Out, MarshalAs (UnmanagedType.LPWStr)] System.Text.StringBuilder pszDir, int cchMaxPath);

		void SetWorkingDirectory([MarshalAs (UnmanagedType.LPWStr)] string pszDir);

		void GetArguments([Out, MarshalAs (UnmanagedType.LPWStr)] System.Text.StringBuilder pszArgs, int cchMaxPath);

		void SetArguments([MarshalAs (UnmanagedType.LPWStr)] string pszArgs);

		void GetHotkey(out ushort pwHotkey);

		void SetHotkey(ushort wHotkey);

		void GetShowCmd(out int piShowCmd);

		void SetShowCmd(int iShowCmd);

		void GetIconLocation([Out, MarshalAs (UnmanagedType.LPWStr)] System.Text.StringBuilder pszIconPath, int cchIconPath, out int piIcon);

		void SetIconLocation([MarshalAs (UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

		void SetRelativePath([MarshalAs (UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

		void Resolve(System.IntPtr hwnd, ShellApi.SLR_FLAGS fFlags);

		void SetPath([MarshalAs (UnmanagedType.LPWStr)] string pszFile);
	}
}
