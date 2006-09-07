using System;
using System.Runtime.InteropServices;


namespace Epsitec.Common.Support.Platform.Win32
{

	internal static class ShellFunctions
	{
		public static IMalloc GetMalloc()
		{
			IntPtr ptrRet;
			ShellApi.SHGetMalloc (out ptrRet);

			Object obj = Marshal.GetTypedObjectForIUnknown (ptrRet, GetMallocType ());
			IMalloc imalloc = (IMalloc) obj;

			return imalloc;
		}

		public static IShellFolder GetDesktopFolder()
		{
			IntPtr ptrRet;
			ShellApi.SHGetDesktopFolder (out ptrRet);

			System.Type shellFolderType = typeof (IShellFolder);
			Object obj = Marshal.GetTypedObjectForIUnknown (ptrRet, shellFolderType);
			IShellFolder ishellFolder = (IShellFolder) obj;

			return ishellFolder;
		}

		public static Type GetShellFolderType()
		{
			System.Type shellFolderType = typeof (IShellFolder);
			return shellFolderType;
		}

		public static Type GetMallocType()
		{
			System.Type mallocType = typeof (IMalloc);
			return mallocType;
		}

		public static Type GetFolderFilterType()
		{
//-			System.Type folderFilterType = typeof (IFolderFilter);
			throw new System.NotImplementedException ();
//-			return folderFilterType;
		}

		public static Type GetFolderFilterSiteType()
		{
//-			System.Type folderFilterSiteType = typeof (IFolderFilterSite);
			throw new System.NotImplementedException ();
//-			return folderFilterSiteType;
		}

		public static IShellFolder GetShellFolder(IntPtr ptrShellFolder)
		{
			System.Type shellFolderType = GetShellFolderType ();
			Object obj = Marshal.GetTypedObjectForIUnknown (ptrShellFolder, shellFolderType);
			IShellFolder RetVal = (IShellFolder) obj;
			return RetVal;
		}

	}

}