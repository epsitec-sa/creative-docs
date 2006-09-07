//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform.Win32
{
	internal sealed class FileInfo
	{
		private FileInfo()
		{
			this.allocator = ShellFunctions.GetMalloc ();

			System.IntPtr ptrRoot;
			
			ShellApi.SHGetDesktopFolder (out ptrRoot);
			
			this.root = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrRoot, typeof (IShellFolder)) as IShellFolder;
		}

		~FileInfo()
		{
			System.Runtime.InteropServices.Marshal.ReleaseComObject (this.allocator);
			System.Runtime.InteropServices.Marshal.ReleaseComObject (this.root);

			this.allocator = null;
			this.root      = null;
		}

		public static FolderItem CreateFolderItem(FolderId file, FolderDetailsMode mode)
		{
			ShellApi.CSIDL csidl = FileInfo.GetCsidl (file);
			System.IntPtr  pidl  = System.IntPtr.Zero;
			
			uint result = ShellApi.SHGetFolderLocation (System.IntPtr.Zero, (short) csidl, System.IntPtr.Zero, 0, out pidl);

			//	For some weird reason, I cannot get the shell folder location for "My Documents"
			//	on my 2003 development machine. Here is a hack :
			
			if ((result != 0) &&
				(result != 1))
			{
				if (csidl == ShellApi.CSIDL.CSIDL_MYDOCUMENTS)
				{
					return FileInfo.CreateFolderItem ("::{450d8fba-ad25-11d0-98a8-0800361b1103}", mode);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Cannot map {0}, error code {1}", csidl, result.ToString ("X")));
				}
			}
			
			if (pidl == System.IntPtr.Zero)
			{
				return FolderItem.Empty;
			}

			FolderItem item = FileInfo.CreateFolderItemAndInheritPidl (mode, pidl);

			if (file == FolderId.VirtualDesktop)
			{
				item.Handle = PidlHandle.VirtualDesktopHandle;
			}
			
			return item;
		}
		
		public static FolderItem CreateFolderItem(string path, FolderDetailsMode mode)
		{
			System.IntPtr pidl = System.IntPtr.Zero;
			
			uint attributes = 0;
			uint eaten = 0;

			FileInfo.instance.root.ParseDisplayName (System.IntPtr.Zero, System.IntPtr.Zero, path, ref eaten, out pidl, ref attributes);
			
			if (pidl == System.IntPtr.Zero)
			{
				return FolderItem.Empty;
			}

			return FileInfo.CreateFolderItemAndInheritPidl (mode, pidl);
		}

		public static FolderItem GetParentFolderItem(FolderItem path, FolderDetailsMode mode)
		{
			PidlHandle handle = path.Handle as PidlHandle;
			
			if ((handle != PidlHandle.VirtualDesktopHandle) &&
				(handle != null))
			{
#if false
				System.IntPtr pidlPath = handle.Pidl;
				System.IntPtr pidlLast = System.IntPtr.Zero;
				System.IntPtr ptrParent;
				
				IShellFolder folder;
				
				ShellApi.SHBindToParent (pidlPath, ref ShellGuids.IID_IShellFolder, out ptrParent, ref pidlLast);
				
				//	There is no need to free the relative pidl (pidlLast), as documented on MSDN.

				pidlLast = System.IntPtr.Zero;

				if (ptrParent != System.IntPtr.Zero)
				{
					folder = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrParent, typeof (IShellFolder)) as IShellFolder;

					
					
					//	TODO: ...

					folder.
					
					System.Runtime.InteropServices.Marshal.ReleaseComObject (folder);
				}
#else
				System.IntPtr pidl = ShellApi.ILCombine (handle.Pidl, System.IntPtr.Zero);
				
				if (ShellApi.ILRemoveLastID (pidl))
				{
					return FileInfo.CreateFolderItemAndInheritPidl (mode, pidl);
				}
				else
				{
					PidlHandle.FreePidl (pidl);
				}
#endif
			}
			
			return FolderItem.Empty;
		}
		
		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderDetailsMode mode)
		{
			return FileInfo.GetFolderItems (path.Handle as PidlHandle, mode, false);
		}

		private static IEnumerable<FolderItem> GetFolderItems(PidlHandle handle, FolderDetailsMode mode, bool disposeHandleWhenFinished)
		{
			System.IntPtr pidlPath = handle.Pidl;
			System.IntPtr pidlElement;

			System.IntPtr ptrPath;
			System.IntPtr ptrList;
			
			uint count;

			IShellFolder folder;
			IEnumIDList list;

			FileInfo.instance.root.BindToObject (pidlPath, System.IntPtr.Zero, ref ShellGuids.IID_IShellFolder, out ptrPath);

			if (ptrPath == System.IntPtr.Zero)
			{
				if (handle != PidlHandle.VirtualDesktopHandle)
				{
					yield break;
				}

				folder = FileInfo.instance.root;
			}
			else
			{
				folder = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrPath, typeof (IShellFolder)) as IShellFolder;
			}

			ShellApi.SHCONT flags = ShellApi.SHCONT.SHCONTF_FOLDERS | ShellApi.SHCONT.SHCONTF_NONFOLDERS;

			folder.EnumObjects (System.IntPtr.Zero, (int) flags, out ptrList);

			list = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrList, typeof (IEnumIDList)) as IEnumIDList;

			while (list.Next (1, out pidlElement, out count) == 0)
			{
				System.Diagnostics.Debug.Assert (count == 1);
				
				System.IntPtr pidlTemp = ShellApi.ILCombine (pidlPath, pidlElement);

				if (pidlTemp != System.IntPtr.Zero)
				{
					FolderItem item = FileInfo.CreateFolderItemAndInheritPidl (mode, pidlTemp);

					yield return item;
				}

				PidlHandle.FreePidl (pidlElement);
			}

			System.Runtime.InteropServices.Marshal.ReleaseComObject (list);

			if (folder != FileInfo.instance.root)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject (folder);
			}
			
			if (disposeHandleWhenFinished)
			{
				handle.Dispose ();
			}
		}

		private static FolderItem CreateFolderItemAndInheritPidl(FolderDetailsMode mode, System.IntPtr pidl)
		{
			//	Do not free pidl, as it is transmitted to the PidlHandle. This
			//	avoids a useless copy operation.
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder (260);
			System.Drawing.Icon icon;

			string displayName;
			string typeName;

			FileInfo.GetIconAndDescription (mode, pidl, out icon, out displayName, out typeName);
			ShellApi.SHGetPathFromIDList (pidl, buffer);
			Drawing.Image image = null;

			if (icon != null)
			{
				image = Drawing.Bitmap.FromNativeIcon (icon);
			}

			return new FolderItem (image, displayName, typeName, buffer.ToString (), PidlHandle.Inherit (pidl));
		}


		private static bool GetIconAndDescription(ShellApi.CSIDL csidl, FolderDetailsMode mode, out System.Drawing.Icon icon, out string displayName, out string typeName)
		{
			System.IntPtr pidl = System.IntPtr.Zero;
			ShellApi.SHGetFolderLocation (System.IntPtr.Zero, (short) csidl, System.IntPtr.Zero, 0, out pidl);

			try
			{
				return FileInfo.GetIconAndDescription (mode, pidl, out icon, out displayName, out typeName);
			}
			finally
			{
				PidlHandle.FreePidl (pidl);
			}
		}

		private static bool GetIconAndDescription(string path, FolderDetailsMode mode, out System.Drawing.Icon icon, out string displayName, out string typeName)
		{
			System.IntPtr pidl = System.IntPtr.Zero;
			uint attribute;
			ShellApi.SHParseDisplayName (path, System.IntPtr.Zero, out pidl, 0, out attribute);

			try
			{
				return FileInfo.GetIconAndDescription (mode, pidl, out icon, out displayName, out typeName);
			}
			finally
			{
				PidlHandle.FreePidl (pidl);
			}
		}

		private static bool GetIconAndDescription(FolderDetailsMode mode, System.IntPtr pidl, out System.Drawing.Icon icon, out string displayName, out string typeName)
		{
			if (pidl == System.IntPtr.Zero)
			{
				icon        = null;
				displayName = null;
				typeName    = null;

				return false;
			}

			ShellApi.SHFILEINFO info = new ShellApi.SHFILEINFO ();
			
			FileInfo.GetShellFileInfo (pidl, mode, ref info);

			if (info.hIcon == System.IntPtr.Zero)
			{
				icon = null;
			}
			else
			{
				icon = System.Drawing.Icon.FromHandle (info.hIcon);
				icon = icon == null ? null : icon.Clone () as System.Drawing.Icon;
				FileInfo.DestroyIcon (info.hIcon);
			}

			displayName = info.szDisplayName;
			typeName    = info.szTypeName;

			return true;
		}

		private static void GetShellFileInfo(System.IntPtr pidl, FolderDetailsMode mode, ref ShellApi.SHFILEINFO info)
		{
			ShellApi.SHGFI flags = ShellApi.SHGFI.SHGFI_DISPLAYNAME |
					/**/			   ShellApi.SHGFI.SHGFI_TYPENAME |
					/**/			   ShellApi.SHGFI.SHGFI_PIDL;

			switch (mode.IconSelection)
			{
				case FileInfoSelection.Active:
					flags |= ShellApi.SHGFI.SHGFI_SELECTED;
					break;
			}

			switch (mode.IconSize)
			{
				case FileInfoIconSize.Small:
					flags |= ShellApi.SHGFI.SHGFI_SMALLICON;
					flags |= ShellApi.SHGFI.SHGFI_ICON;
					break;
				case FileInfoIconSize.Large:
					flags |= ShellApi.SHGFI.SHGFI_LARGEICON;
					flags |= ShellApi.SHGFI.SHGFI_ICON;
					break;
			}

			ShellApi.SHGetFileInfo (pidl, 0, out info, System.Runtime.InteropServices.Marshal.SizeOf (info), flags);
		}

		private static ShellApi.CSIDL GetCsidl(FolderId id)
		{
			switch (id)
			{
				case FolderId.AdminTools:
					return ShellApi.CSIDL.CSIDL_ADMINTOOLS;
				case FolderId.AltStartup:
					return ShellApi.CSIDL.CSIDL_ALTSTARTUP;
				case FolderId.ApplicatioData:
					return ShellApi.CSIDL.CSIDL_APPDATA;
				case FolderId.CdBurnArea:
					return ShellApi.CSIDL.CSIDL_CDBURN_AREA;
				case FolderId.CommonAdminTools:
					return ShellApi.CSIDL.CSIDL_COMMON_ADMINTOOLS;
				case FolderId.CommonAltStartup:
					return ShellApi.CSIDL.CSIDL_COMMON_ALTSTARTUP;
				case FolderId.CommonAppData:
					return ShellApi.CSIDL.CSIDL_COMMON_APPDATA;
				case FolderId.CommonDesktopDirectory:
					return ShellApi.CSIDL.CSIDL_COMMON_DESKTOPDIRECTORY;
				case FolderId.CommonDocuments:
					return ShellApi.CSIDL.CSIDL_COMMON_DOCUMENTS;
				case FolderId.CommonFavorites:
					return ShellApi.CSIDL.CSIDL_COMMON_FAVORITES;
				case FolderId.CommonMusic:
					return ShellApi.CSIDL.CSIDL_COMMON_MUSIC;
				case FolderId.CommonPictures:
					return ShellApi.CSIDL.CSIDL_COMMON_PICTURES;
				case FolderId.CommonStartMenu:
					return ShellApi.CSIDL.CSIDL_COMMON_STARTMENU;
				case FolderId.CommonStartMenuPrograms:
					return ShellApi.CSIDL.CSIDL_COMMON_PROGRAMS;
				case FolderId.CommonStartMenuProgramsStartup:
					return ShellApi.CSIDL.CSIDL_COMMON_STARTUP;
				case FolderId.CommonTemplates:
					return ShellApi.CSIDL.CSIDL_COMMON_TEMPLATES;
				case FolderId.CommonVideo:
					return ShellApi.CSIDL.CSIDL_COMMON_VIDEO;
				case FolderId.Cookies:
					return ShellApi.CSIDL.CSIDL_COOKIES;
				case FolderId.DesktopDirectory:
					return ShellApi.CSIDL.CSIDL_DESKTOPDIRECTORY;
				case FolderId.Favorites:
					return ShellApi.CSIDL.CSIDL_FAVORITES;
				case FolderId.Fonts:
					return ShellApi.CSIDL.CSIDL_FONTS;
				case FolderId.History:
					return ShellApi.CSIDL.CSIDL_HISTORY;
				case FolderId.Internet:
					return ShellApi.CSIDL.CSIDL_INTERNET;
				case FolderId.InternetCache:
					return ShellApi.CSIDL.CSIDL_INTERNET_CACHE;
				case FolderId.LocalAppData:
					return ShellApi.CSIDL.CSIDL_LOCAL_APPDATA;
				case FolderId.MyDocuments:
					return ShellApi.CSIDL.CSIDL_PERSONAL;
				case FolderId.MyMusic:
					return ShellApi.CSIDL.CSIDL_MYMUSIC;
				case FolderId.MyPictures:
					return ShellApi.CSIDL.CSIDL_MYPICTURES;
				case FolderId.MyVideo:
					return ShellApi.CSIDL.CSIDL_MYVIDEO;
				case FolderId.NetHood:
					return ShellApi.CSIDL.CSIDL_NETHOOD;
				case FolderId.PrintHood:
					return ShellApi.CSIDL.CSIDL_PRINTHOOD;
				case FolderId.Profile:
					return ShellApi.CSIDL.CSIDL_PROFILE;
				case FolderId.Profiles:
					return ShellApi.CSIDL.CSIDL_PROFILES;
				case FolderId.ProgramFiles:
					return ShellApi.CSIDL.CSIDL_PROGRAM_FILES;
				case FolderId.ProgramFilesCommon:
					return ShellApi.CSIDL.CSIDL_PROGRAM_FILES_COMMON;
				case FolderId.Recent:
					return ShellApi.CSIDL.CSIDL_RECENT;
				case FolderId.SendTo:
					return ShellApi.CSIDL.CSIDL_SENDTO;
				case FolderId.StartMenu:
					return ShellApi.CSIDL.CSIDL_STARTMENU;
				case FolderId.StartMenuPrograms:
					return ShellApi.CSIDL.CSIDL_PROGRAMS;
				case FolderId.StartMenuProgramsStartup:
					return ShellApi.CSIDL.CSIDL_STARTUP;
				case FolderId.Templates:
					return ShellApi.CSIDL.CSIDL_TEMPLATES;
				case FolderId.VirtualControlPanel:
					return ShellApi.CSIDL.CSIDL_CONTROLS;
				case FolderId.VirtualDesktop:
					return ShellApi.CSIDL.CSIDL_DESKTOP;
				case FolderId.VirtualMyComputer:
					return ShellApi.CSIDL.CSIDL_DRIVES;
				case FolderId.VirtualMyDocuments:
					return ShellApi.CSIDL.CSIDL_MYDOCUMENTS;
				case FolderId.VirtualNetwork:
					return ShellApi.CSIDL.CSIDL_NETWORK;
				case FolderId.VirtualPrinters:
					return ShellApi.CSIDL.CSIDL_PRINTERS;
				case FolderId.VirtualRecycleBin:
					return ShellApi.CSIDL.CSIDL_BITBUCKET;
				case FolderId.Windows:
					return ShellApi.CSIDL.CSIDL_WINDOWS;
				case FolderId.WindowsSystem:
					return ShellApi.CSIDL.CSIDL_SYSTEM;
			}

			throw new System.ArgumentException (string.Format ("{0} cannot be mapped to a CSIDL", id));
		}

		[System.Runtime.InteropServices.DllImport ("user32.dll")]
		private static extern int DestroyIcon(System.IntPtr hIcon);

		private static FileInfo instance = new FileInfo ();
		
		private IMalloc allocator;
		private IShellFolder root;
	}
}
