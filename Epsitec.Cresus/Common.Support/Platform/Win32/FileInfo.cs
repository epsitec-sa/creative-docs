using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support.Platform.Win32
{
	public class FileInfo
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
		
		public static System.Drawing.Icon GetIcon(SystemFileId file, FileInfoSelection selection, FileInfoIconSize iconSize)
		{
			System.Drawing.Icon icon;
			string displayName;
			string typeName;
			ShellApi.CSIDL csidl = FileInfo.GetCsidl (file);

			FileInfo.GetIconAndDescription (csidl, selection, iconSize, out icon, out displayName, out typeName);
			
			return icon;
		}

		public static System.Drawing.Icon GetIcon(string path, FileInfoSelection selection, FileInfoIconSize iconSize)
		{
			System.Drawing.Icon icon;
			string displayName;
			string typeName;
			
			FileInfo.GetIconAndDescription (path, selection, iconSize, out icon, out displayName, out typeName);

			return icon;
		}

		public static void GetDisplayAndTypeNames(SystemFileId file, out string displayName, out string typeName)
		{
			System.Drawing.Icon icon;
			ShellApi.CSIDL csidl = FileInfo.GetCsidl (file);

			FileInfo.GetIconAndDescription (csidl, FileInfoSelection.Normal, FileInfoIconSize.None, out icon, out displayName, out typeName);
		}

		public static void GetDisplayAndTypeNames(string path, out string displayName, out string typeName)
		{
			System.Drawing.Icon icon;
			
			FileInfo.GetIconAndDescription (path, FileInfoSelection.Normal, FileInfoIconSize.None, out icon, out displayName, out typeName);
		}
		
		public static string GetPath(SystemFileId file)
		{
			ShellApi.CSIDL csidl = FileInfo.GetCsidl (file);
			System.IntPtr  pidl = System.IntPtr.Zero;

			try
			{
				ShellApi.SHGetFolderLocation (System.IntPtr.Zero, (short) csidl, System.IntPtr.Zero, 0, out pidl);

				if (pidl == System.IntPtr.Zero)
				{
					return null;
				}

				System.Text.StringBuilder path = new System.Text.StringBuilder (260);
				ShellApi.SHGetPathFromIDList (pidl, path);
				return path.ToString ();
			}
			finally
			{
				FileInfo.instance.allocator.Free (pidl);
			}
		}

		public static IEnumerable<FolderItem> GetFolderItems(string path)
		{
			System.IntPtr pidlPath;
			System.IntPtr pidlElement;

			System.IntPtr ptrPath;
			System.IntPtr ptrList;
			
			uint attributes = 0;
			uint eaten = 0;
			uint count;

			IShellFolder folder;
			IEnumIDList  list;

			FileInfo.instance.root.ParseDisplayName (System.IntPtr.Zero, System.IntPtr.Zero, path, ref eaten, out pidlPath, ref attributes);
			FileInfo.instance.root.BindToObject (pidlPath, System.IntPtr.Zero, ref ShellGuids.IID_IShellFolder, out ptrPath);
			
			folder = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrPath, typeof (IShellFolder)) as IShellFolder;

			ShellApi.SHCONT flags = ShellApi.SHCONT.SHCONTF_FOLDERS | ShellApi.SHCONT.SHCONTF_NONFOLDERS;

			folder.EnumObjects (System.IntPtr.Zero, (int) flags, out ptrList);

			list = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrList, typeof (IEnumIDList)) as IEnumIDList;

			System.Text.StringBuilder buffer = new System.Text.StringBuilder (260);
			System.Drawing.Icon icon;

			while (list.Next (1, out pidlElement, out count) == 0)
			{
				System.Diagnostics.Debug.Assert (count == 1);
				
				string displayName;
				string typeName;

				System.IntPtr pidlTemp = ShellApi.ILCombine (pidlPath, pidlElement);

				if (pidlTemp != System.IntPtr.Zero)
				{
					FileInfo.GetIconAndDescription (FileInfoSelection.Normal, FileInfoIconSize.None, pidlElement, out icon, out displayName, out typeName);
					ShellApi.SHGetPathFromIDList (pidlTemp, buffer);
					FileInfo.instance.allocator.Free (pidlTemp);
					Drawing.Image image = null;

					if (icon != null)
					{
						image = Drawing.Bitmap.FromNativeIcon (icon);
					}

					FolderItem item = new FolderItem (image, displayName, typeName, buffer.ToString (), System.IntPtr.Zero);
				
					yield return item;
				}

				FileInfo.instance.allocator.Free (pidlElement);
			}

			FileInfo.instance.allocator.Free (pidlPath);

			System.Runtime.InteropServices.Marshal.ReleaseComObject (list);
		}
		
		
		private static bool GetIconAndDescription(ShellApi.CSIDL csidl, FileInfoSelection selection, FileInfoIconSize iconSize, out System.Drawing.Icon icon, out string displayName, out string typeName)
		{
			System.IntPtr pidl = System.IntPtr.Zero;
			ShellApi.SHGetFolderLocation (System.IntPtr.Zero, (short) csidl, System.IntPtr.Zero, 0, out pidl);

			try
			{
				return FileInfo.GetIconAndDescription (selection, iconSize, pidl, out icon, out displayName, out typeName);
			}
			finally
			{
				if (pidl != System.IntPtr.Zero)
				{
					FileInfo.instance.allocator.Free (pidl);
				}
			}
		}

		private static bool GetIconAndDescription(string path, FileInfoSelection selection, FileInfoIconSize iconSize, out System.Drawing.Icon icon, out string displayName, out string typeName)
		{
			System.IntPtr pidl = System.IntPtr.Zero;
			uint attribute;
			ShellApi.SHParseDisplayName (path, System.IntPtr.Zero, out pidl, 0, out attribute);

			try
			{
				return FileInfo.GetIconAndDescription (selection, iconSize, pidl, out icon, out displayName, out typeName);
			}
			finally
			{
				if (pidl != System.IntPtr.Zero)
				{
					FileInfo.instance.allocator.Free (pidl);
				}
			}
		}

		private static bool GetIconAndDescription(FileInfoSelection selection, FileInfoIconSize iconSize, System.IntPtr pidl, out System.Drawing.Icon icon, out string displayName, out string typeName)
		{
			if (pidl == System.IntPtr.Zero)
			{
				icon        = null;
				displayName = null;
				typeName    = null;

				return false;
			}

			ShellApi.SHFILEINFO info = new ShellApi.SHFILEINFO ();
			
			FileInfo.GetShellFileInfo (pidl, selection, iconSize, ref info);

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

		private static void GetShellFileInfo(System.IntPtr pidl, FileInfoSelection selection, FileInfoIconSize iconSize, ref ShellApi.SHFILEINFO info)
		{
			ShellApi.SHGFI flags = ShellApi.SHGFI.SHGFI_DISPLAYNAME |
					/**/			   ShellApi.SHGFI.SHGFI_TYPENAME |
					/**/			   ShellApi.SHGFI.SHGFI_PIDL;

			switch (selection)
			{
				case FileInfoSelection.Active:
					flags |= ShellApi.SHGFI.SHGFI_SELECTED;
					break;
			}

			switch (iconSize)
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

		private static ShellApi.CSIDL GetCsidl(SystemFileId id)
		{
			switch (id)
			{
				case SystemFileId.AdminTools:
					return ShellApi.CSIDL.CSIDL_ADMINTOOLS;
				case SystemFileId.AltStartup:
					return ShellApi.CSIDL.CSIDL_ALTSTARTUP;
				case SystemFileId.ApplicatioData:
					return ShellApi.CSIDL.CSIDL_APPDATA;
				case SystemFileId.CdBurnArea:
					return ShellApi.CSIDL.CSIDL_CDBURN_AREA;
				case SystemFileId.CommonAdminTools:
					return ShellApi.CSIDL.CSIDL_COMMON_ADMINTOOLS;
				case SystemFileId.CommonAltStartup:
					return ShellApi.CSIDL.CSIDL_COMMON_ALTSTARTUP;
				case SystemFileId.CommonAppData:
					return ShellApi.CSIDL.CSIDL_COMMON_APPDATA;
				case SystemFileId.CommonDesktopDirectory:
					return ShellApi.CSIDL.CSIDL_COMMON_DESKTOPDIRECTORY;
				case SystemFileId.CommonDocuments:
					return ShellApi.CSIDL.CSIDL_COMMON_DOCUMENTS;
				case SystemFileId.CommonFavorites:
					return ShellApi.CSIDL.CSIDL_COMMON_FAVORITES;
				case SystemFileId.CommonMusic:
					return ShellApi.CSIDL.CSIDL_COMMON_MUSIC;
				case SystemFileId.CommonPictures:
					return ShellApi.CSIDL.CSIDL_COMMON_PICTURES;
				case SystemFileId.CommonStartMenu:
					return ShellApi.CSIDL.CSIDL_COMMON_STARTMENU;
				case SystemFileId.CommonStartMenuPrograms:
					return ShellApi.CSIDL.CSIDL_COMMON_PROGRAMS;
				case SystemFileId.CommonStartMenuProgramsStartup:
					return ShellApi.CSIDL.CSIDL_COMMON_STARTUP;
				case SystemFileId.CommonTemplates:
					return ShellApi.CSIDL.CSIDL_COMMON_TEMPLATES;
				case SystemFileId.CommonVideo:
					return ShellApi.CSIDL.CSIDL_COMMON_VIDEO;
				case SystemFileId.Cookies:
					return ShellApi.CSIDL.CSIDL_COOKIES;
				case SystemFileId.DesktopDirectory:
					return ShellApi.CSIDL.CSIDL_DESKTOPDIRECTORY;
				case SystemFileId.Favorites:
					return ShellApi.CSIDL.CSIDL_FAVORITES;
				case SystemFileId.Fonts:
					return ShellApi.CSIDL.CSIDL_FONTS;
				case SystemFileId.History:
					return ShellApi.CSIDL.CSIDL_HISTORY;
				case SystemFileId.Internet:
					return ShellApi.CSIDL.CSIDL_INTERNET;
				case SystemFileId.InternetCache:
					return ShellApi.CSIDL.CSIDL_INTERNET_CACHE;
				case SystemFileId.LocalAppData:
					return ShellApi.CSIDL.CSIDL_LOCAL_APPDATA;
				case SystemFileId.MyDocuments:
					return ShellApi.CSIDL.CSIDL_PERSONAL;
				case SystemFileId.MyMusic:
					return ShellApi.CSIDL.CSIDL_MYMUSIC;
				case SystemFileId.MyPictures:
					return ShellApi.CSIDL.CSIDL_MYPICTURES;
				case SystemFileId.MyVideo:
					return ShellApi.CSIDL.CSIDL_MYVIDEO;
				case SystemFileId.NetHood:
					return ShellApi.CSIDL.CSIDL_NETHOOD;
				case SystemFileId.PrintHood:
					return ShellApi.CSIDL.CSIDL_PRINTHOOD;
				case SystemFileId.Profile:
					return ShellApi.CSIDL.CSIDL_PROFILE;
				case SystemFileId.Profiles:
					return ShellApi.CSIDL.CSIDL_PROFILES;
				case SystemFileId.ProgramFiles:
					return ShellApi.CSIDL.CSIDL_PROGRAM_FILES;
				case SystemFileId.ProgramFilesCommon:
					return ShellApi.CSIDL.CSIDL_PROGRAM_FILES_COMMON;
				case SystemFileId.Recent:
					return ShellApi.CSIDL.CSIDL_RECENT;
				case SystemFileId.SendTo:
					return ShellApi.CSIDL.CSIDL_SENDTO;
				case SystemFileId.StartMenu:
					return ShellApi.CSIDL.CSIDL_STARTMENU;
				case SystemFileId.StartMenuPrograms:
					return ShellApi.CSIDL.CSIDL_PROGRAMS;
				case SystemFileId.StartMenuProgramsStartup:
					return ShellApi.CSIDL.CSIDL_STARTUP;
				case SystemFileId.Templates:
					return ShellApi.CSIDL.CSIDL_TEMPLATES;
				case SystemFileId.VirtualControlPanel:
					return ShellApi.CSIDL.CSIDL_CONTROLS;
				case SystemFileId.VirtualDesktop:
					return ShellApi.CSIDL.CSIDL_DESKTOP;
				case SystemFileId.VirtualMyComputer:
					return ShellApi.CSIDL.CSIDL_DRIVES;
				case SystemFileId.VirtualMyDocuments:
					return ShellApi.CSIDL.CSIDL_MYDOCUMENTS;
				case SystemFileId.VirtualNetwork:
					return ShellApi.CSIDL.CSIDL_NETWORK;
				case SystemFileId.VirtualPrinters:
					return ShellApi.CSIDL.CSIDL_PRINTERS;
				case SystemFileId.VirtualRecycleBin:
					return ShellApi.CSIDL.CSIDL_BITBUCKET;
				case SystemFileId.Windows:
					return ShellApi.CSIDL.CSIDL_WINDOWS;
				case SystemFileId.WindowsSystem:
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
