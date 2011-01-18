//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform.Win32
{
	internal sealed class FileInfo
	{
		private FileInfo()
		{
			System.IntPtr ptrRoot;
			
			ShellApi.SHGetDesktopFolder (out ptrRoot);
			
			this.root = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrRoot, typeof (IShellFolder)) as IShellFolder;

			FileInfo.CreateFolderItem (FolderId.VirtualDesktop, FolderQueryMode.NoIcons);

			System.Diagnostics.Debug.Assert (PidlHandle.VirtualDesktopHandle.Pidl != System.IntPtr.Zero);
		}

		~FileInfo()
		{
			System.Runtime.InteropServices.Marshal.ReleaseComObject (this.root);

			this.root      = null;
		}

		public static void InitializeWellKnownFolderItems()
		{
			lock (FileInfo.wellKnownFolders)
			{
				if (FileInfo.wellKnownFoldersReady == false)
				{
					FileInfo.InitializeWellKnownFolderItemsLocked ();
				}
			}
		}

		private static void InitializeWellKnownFolderItemsLocked()
		{
			if (FileInfo.wellKnownFolders.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine ("Filling folder item cache: started");
				FileInfo.InitializeWellKnownFolderItems (FolderQueryMode.LargeIcons, "Large-Closed");
				FileInfo.InitializeWellKnownFolderItems (FolderQueryMode.SmallIcons, "Small-Closed");
				FileInfo.InitializeWellKnownFolderItems (FolderQueryMode.LargeIcons.Open (), "Large-Open");
				FileInfo.InitializeWellKnownFolderItems (FolderQueryMode.SmallIcons.Open (), "Small-Open");
				System.Diagnostics.Debug.WriteLine ("Filling folder item cache: done");

				FileInfo.wellKnownFoldersReady = true;
			}
		}

		private static bool FindWellKnownFolderItem(FolderItemHandle handle, string fullPath, FolderQueryMode mode, out FolderItem item)
		{
			if (FileInfo.wellKnownFoldersReady)
			{
				string prefix;

				if (mode.IconSize == FolderQueryMode.LargeIcons.IconSize)
				{
					prefix = "Large-";
				}
				else
				{
					prefix = "Small-";
				}

				FolderItem test = new FolderItem (handle);
				FolderKey key;

				if (mode.AsOpenFolder)
				{
					key = new FolderKey (test, fullPath, string.Concat (prefix, "Open"));
				}
				else
				{
					key = new FolderKey (test, fullPath, string.Concat (prefix, "Closed"));
				}

				lock (FileInfo.wellKnownFolders)
				{
					FileInfo.InitializeWellKnownFolderItemsLocked ();
					return FileInfo.wellKnownFolders.TryGetValue (key, out item);
				}
			}
			else
			{
				item = FolderItem.Empty;
				return false;
			}
		}

		private static FolderQueryMode InitializeWellKnownFolderItems(FolderQueryMode mode, string prefix)
		{
			foreach (FolderId id in Types.EnumType.GetAllEnumValues<FolderId> ())
			{
				FolderItem item = FileInfo.CreateFolderItem (id, mode);
				
				if (item.Handle != null)
				{
					FolderKey key = new FolderKey (item, item.FullPath, prefix);
					FileInfo.wellKnownFolders[key] = item;
				}
			}
			return mode;
		}

		public static FolderItem CreateFolderItem(FolderId file, FolderQueryMode mode)
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
					//System.Diagnostics.Debug.WriteLine (string.Format ("Cannot map {0}, error code {1}", csidl, result.ToString ("X")));
				}
			}
			
			if (pidl == System.IntPtr.Zero)
			{
				return FolderItem.Empty;
			}

			FolderItem item = FileInfo.CreateFolderItemAndInheritPidl (mode, pidl);

			if (file == FolderId.VirtualDesktop)
			{
				if (PidlHandle.VirtualDesktopHandle.Pidl == System.IntPtr.Zero)
				{
					PidlHandle.VirtualDesktopHandle.SetPidlCopy (pidl);
				}

				item.Handle = PidlHandle.VirtualDesktopHandle;
			}
			
			return item;
		}

		public static FolderItem CreateFolderItem(string path, FolderQueryMode mode)
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

		public static FolderItem CreateFolderItem(FolderItemHandle handle, FolderQueryMode mode)
		{
			PidlHandle pidlHandle = handle as PidlHandle;
			System.IntPtr pidl = pidlHandle == null ? System.IntPtr.Zero : pidlHandle.Pidl;

			if (pidl == System.IntPtr.Zero)
			{
				return FolderItem.Empty;
			}

			pidl = ShellApi.ILCombine (pidl, System.IntPtr.Zero);

			return FileInfo.CreateFolderItemAndInheritPidl (mode, pidl);
		}

		public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
		{
			PidlHandle handle = path.Handle as PidlHandle;
			
			if ((handle != PidlHandle.VirtualDesktopHandle) &&
				(handle != null))
			{
				System.IntPtr pidl = ShellApi.ILCombine (handle.Pidl, System.IntPtr.Zero);
				
				if (ShellApi.ILRemoveLastID (pidl))
				{
					return FileInfo.CreateFolderItemAndInheritPidl (mode, pidl);
				}
				else
				{
					PidlHandle.FreePidl (pidl);
				}
			}
			
			return FolderItem.Empty;
		}

		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderQueryMode mode, System.Predicate<FileFilterInfo> filter)
		{
			//	Hack: on Vista-64, IShellFolder.EnumObjects does not return any result when
			//	applied to CSIDL_RECENT. This is a dirty hack to get things working nevertheless.

			if (path.FullPath == FileInfo.recentFolderItem.FullPath)
			{
				return FileInfo.GetRecentFolderItemsHack (mode, filter);
			}
			else
			{
				return FileInfo.GetFolderItems (path.Handle as PidlHandle, mode, false, filter);
			}
		}

		private static IEnumerable<FolderItem> GetRecentFolderItemsHack(FolderQueryMode mode, System.Predicate<FileFilterInfo> filter)
		{
			string[] paths = System.IO.Directory.GetFiles (FileInfo.recentFolderItem.FullPath);
			
			foreach (string path in paths)
			{
				//	Keep only the .LNK files which are of interest here; the other files do not
				//	belong to the list (i.e. desktop.ini has nothing to do in the recent document
				//	list returned to the caller)

				if (path.EndsWith (".lnk"))
				{
					if (filter == null)
					{
						yield return FileInfo.CreateFolderItem (path, mode);
					}
					else
					{
						FileFilterInfo filterInfo;

						try
						{
							filterInfo = new FileFilterInfo (path, 0);
						}
						catch
						{
							filterInfo = FileFilterInfo.Empty;
						}

						if ((!filterInfo.IsEmpty) && 
							(filter (filterInfo)))
						{
							yield return FileInfo.CreateFolderItem (path, mode);
						}
					}
				}
			}
		}

		private static IEnumerable<FolderItem> GetFolderItems(PidlHandle handle, FolderQueryMode mode, bool disposeHandleWhenFinished, System.Predicate<FileFilterInfo> filter)
		{
			if (handle == null)
			{
				throw new System.IO.FileNotFoundException ();
			}

			System.IntPtr pidlPath = handle.Pidl;
			System.IntPtr pidlElement;

			System.IntPtr ptrPath;
			System.IntPtr ptrList;
			
			uint count;

			IShellFolder folder;

			try
			{
				FileInfo.instance.root.BindToObject (pidlPath, System.IntPtr.Zero, ref ShellGuids.IID_IShellFolder, out ptrPath);
			}
			catch
			{
				yield break;
			}

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

			ShellApi.SHCONT flags = ShellApi.SHCONT.SHCONTF_FOLDERS | ShellApi.SHCONT.SHCONTF_NONFOLDERS | ShellApi.SHCONT.SHCONTF_INCLUDEHIDDEN;

			folder.EnumObjects (System.IntPtr.Zero, (int) flags, out ptrList);

			if (ptrList != System.IntPtr.Zero)
			{
				IEnumIDList list;
				
				list = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown (ptrList, typeof (IEnumIDList)) as IEnumIDList;

				while (list.Next (1, out pidlElement, out count) == 0)
				{
					System.Diagnostics.Debug.Assert (count == 1);
					
					System.IntPtr pidlTemp = ShellApi.ILCombine (pidlPath, pidlElement);

					if (pidlTemp != System.IntPtr.Zero)
					{
						string fullPath = FileInfo.GetFullPathFromPidl (pidlTemp);
						System.IO.FileAttributes fileAttributes;

						if (fullPath.EndsWith (".lnk"))
						{
							fileAttributes = 0;
						}
						else
						{
							fileAttributes = FileInfo.GetAttributes (fullPath);
						}

						if (filter == null)
						{
							yield return FileInfo.CreateFolderItemAndInheritPidl (mode, pidlTemp, fullPath, fileAttributes);
						}
						else
						{
							FileFilterInfo filterInfo;

							try
							{
								filterInfo = new FileFilterInfo (fullPath, fileAttributes);
							}
							catch
							{
								filterInfo = FileFilterInfo.Empty;
							}
							
							if ((!filterInfo.IsEmpty) && filter (filterInfo))
							{
								yield return FileInfo.CreateFolderItemAndInheritPidl (mode, pidlTemp, fullPath, fileAttributes);
							}
						}
					}

					PidlHandle.FreePidl (pidlElement);
				}

				System.Runtime.InteropServices.Marshal.ReleaseComObject (list);
			}

			if (folder != FileInfo.instance.root)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject (folder);
			}
			
			if (disposeHandleWhenFinished)
			{
				handle.Dispose ();
			}
		}

		private static System.IO.FileAttributes GetAttributes(string fullPath)
		{
			try
			{
				if ((!string.IsNullOrEmpty (fullPath)) &&
					(!((fullPath.Length == 3) && (fullPath[1] == ':'))))
				{
					if (!fullPath.StartsWith ("::"))
					{
						return System.IO.File.GetAttributes (fullPath);
					}
				}
			}
			catch
			{
			}

			return (System.IO.FileAttributes) 0;
		}

		private static string GetFullPathFromPidl(System.IntPtr pidl)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.MAX_PATH);
			ShellApi.SHGetPathFromIDList (pidl, buffer);
			return buffer.ToString ();
		}

		private static FolderItem CreateFolderItemAndInheritPidl(FolderQueryMode mode, System.IntPtr pidl)
		{
			return FileInfo.CreateFolderItemAndInheritPidl (mode, pidl, FileInfo.GetFullPathFromPidl (pidl));
		}

		private static FolderItem CreateFolderItemAndInheritPidl(FolderQueryMode mode, System.IntPtr pidl, string fullPath)
		{
			return FileInfo.CreateFolderItemAndInheritPidl (mode, pidl, fullPath, (System.IO.FileAttributes) 0);
		}

		private static FolderItem CreateFolderItemAndInheritPidl(FolderQueryMode mode, System.IntPtr pidl, string fullPath, System.IO.FileAttributes fileAttributes)
		{
			//	Do not free pidl, as it is transmitted to the PidlHandle. This
			//	avoids a useless copy operation.

			FolderItem item;

			if (FileInfo.wellKnownFoldersReady)
			{
				if (FileInfo.FindWellKnownFolderItem (new PidlHandle (pidl), fullPath, mode, out item))
				{
					return item;
				}
			}
			
			System.Drawing.Icon icon;

			string displayName;
			string typeName;
			FolderItemAttributes attributes;
			uint fileAttr = (uint) fileAttributes;

			if (string.IsNullOrEmpty (fullPath))
			{
				fileAttr = 0;
			}
			else
			{
				if (!FileInfo.wellKnownFoldersReady)
				{
					fileAttr = 0;
				}
			}
			
			FileInfo.GetIconAndDescription (pidl, fullPath, mode, fileAttr, out icon, out displayName, out typeName, out attributes);

			Drawing.Image image = null;

			if (icon != null)
			{
				image = Drawing.Bitmap.FromNativeIcon (icon);
			}

			PidlHandle handle = PidlHandle.Inherit (pidl);

			if ((PidlHandle.VirtualDesktopHandle.Pidl != System.IntPtr.Zero) &&
				(handle.Equals (PidlHandle.VirtualDesktopHandle)))
			{
				handle.Dispose ();
				handle = PidlHandle.VirtualDesktopHandle;
			}
			
			return new FolderItem (image, mode, displayName, typeName, fullPath, handle, attributes);
		}


		private static bool GetIconAndDescription(System.IntPtr pidl, string fullPath, FolderQueryMode mode, uint attr, out System.Drawing.Icon icon, out string displayName, out string typeName, out FolderItemAttributes attributes)
		{
			if (pidl == System.IntPtr.Zero)
			{
				icon        = null;
				displayName = null;
				typeName    = null;
				attributes  = FolderItemAttributes.None;

				return false;
			}

			ShellApi.SHFILEINFO info = new ShellApi.SHFILEINFO ();

			bool requestAttributes;
			
			attributes = FolderItemAttributes.None;

			if ((!string.IsNullOrEmpty (fullPath)) &&
				(fullPath.Length == 3) &&
				(fullPath[1] == ':'))
			{
				requestAttributes = false;
				
				attributes |= FolderItemAttributes.Folder;
				attributes |= FolderItemAttributes.FileSystemNode;
			}
			else
			{
				requestAttributes = true;
			}

			FileInfo.GetShellFileInfo (pidl, fullPath, mode, requestAttributes, attr, ref info);
			
			if (info.hIcon == System.IntPtr.Zero)
			{
				icon = null;
			}
			else
			{
				icon = System.Drawing.Icon.FromHandle (info.hIcon);
				icon = icon == null ? null : icon.Clone () as System.Drawing.Icon;
				ShellApi.DestroyIcon (info.hIcon);
			}

			displayName = info.szDisplayName;
			typeName    = info.szTypeName;

#if false
			System.Diagnostics.Debug.WriteLine (string.Format ("{0} {1} : {2}", displayName, typeName, info.dwAttributes.ToString ("X")));

			foreach (uint bit in System.Enum.GetValues (typeof (ShellApi.SFGAO)))
			{
				if ((info.dwAttributes & bit) == bit)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("- {0}", (ShellApi.SFGAO) bit));
				}
			}
#endif

			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_BROWSABLE) != 0)
			{
				attributes |= FolderItemAttributes.Browsable;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_FOLDER) != 0)
			{
				attributes |= FolderItemAttributes.Folder;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_CANCOPY) != 0)
			{
				attributes |= FolderItemAttributes.CanCopy;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_CANDELETE) != 0)
			{
				attributes |= FolderItemAttributes.CanDelete;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_CANMOVE) != 0)
			{
				attributes |= FolderItemAttributes.CanMove;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_CANRENAME) != 0)
			{
				attributes |= FolderItemAttributes.CanRename;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_COMPRESSED) != 0)
			{
				attributes |= FolderItemAttributes.Compressed;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_ENCRYPTED) != 0)
			{
				attributes |= FolderItemAttributes.Encrypted;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_HIDDEN) != 0)
			{
				attributes |= FolderItemAttributes.Hidden;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_READONLY) != 0)
			{
				attributes |= FolderItemAttributes.ReadOnly;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_SHARE) != 0)
			{
				attributes |= FolderItemAttributes.Shared;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_LINK) != 0)
			{
				if (string.Equals (System.IO.Path.GetExtension (fullPath), ".lnk", System.StringComparison.OrdinalIgnoreCase))
				{
					attributes |= FolderItemAttributes.Shortcut;
				}
				else if (string.Equals (System.IO.Path.GetExtension (fullPath), ".url", System.StringComparison.OrdinalIgnoreCase))
				{
					attributes |= FolderItemAttributes.WebLink;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("Unsupported SGFAO_LINK on file: " + fullPath);
				}
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_FILESYSANCESTOR) != 0)
			{
				attributes |= FolderItemAttributes.FileSystemNode;
			}
			if ((info.dwAttributes & (uint) ShellApi.SFGAO.SFGAO_FILESYSTEM) != 0)
			{
				attributes |= FolderItemAttributes.FileSystemNode;
			}

			return true;
		}

		private static void GetShellFileInfo(System.IntPtr pidl, string fullPath, FolderQueryMode mode, bool requestAttributes, uint fileAttr, ref ShellApi.SHFILEINFO info)
		{
			ShellApi.SHGFI flags = ShellApi.SHGFI.SHGFI_DISPLAYNAME | ShellApi.SHGFI.SHGFI_TYPENAME;

			if (fileAttr == 0)
			{
				flags |= ShellApi.SHGFI.SHGFI_PIDL;

				if (requestAttributes)
				{
					flags |= ShellApi.SHGFI.SHGFI_ATTRIBUTES;
				}
			}
			else
			{
				flags |= ShellApi.SHGFI.SHGFI_USEFILEATTRIBUTES;
			}

			if (mode.AsOpenFolder)
			{
				flags |= ShellApi.SHGFI.SHGFI_OPENICON;
			}

			switch (mode.IconSelection)
			{
				case FileInfoIconSelection.Active:
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

			if (fileAttr == 0)
			{
				ShellApi.SHGetFileInfo (pidl, 0, out info, System.Runtime.InteropServices.Marshal.SizeOf (info), flags);
			}
			else
			{
				ShellApi.SHGetFileInfo (fullPath, fileAttr, out info, System.Runtime.InteropServices.Marshal.SizeOf (info), flags);
			}
		}

		private static ShellApi.CSIDL GetCsidl(FolderId id)
		{
			switch (id)
			{
				case FolderId.AdminTools:
					return ShellApi.CSIDL.CSIDL_ADMINTOOLS;
				case FolderId.AltStartup:
					return ShellApi.CSIDL.CSIDL_ALTSTARTUP;
				case FolderId.ApplicationData:
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

		private struct FolderKey : System.IEquatable<FolderKey>
		{
			public FolderKey(FolderItem item, string path, string mode)
			{
				this.item = item;
				this.path = path ?? "";
				this.mode = mode;
			}

			#region IEquatable<FolderKey> Members

			public bool Equals(FolderKey other)
			{
				return (this.mode == other.mode) && this.item.Handle.Equals (other.item.Handle);
			}

			#endregion

			public override bool  Equals(object obj)
			{
				return this.Equals ((FolderKey) obj);
			}

			public override int  GetHashCode()
			{
 				return this.path.GetHashCode () ^ this.mode.GetHashCode ();
			}

			private FolderItem item;
			private string mode;
			private string path;
		}

		private static FolderItem recentFolderItem = FileInfo.CreateFolderItem (FolderId.Recent, FolderQueryMode.NoIcons);
		private static Dictionary<FolderKey, FolderItem> wellKnownFolders = new Dictionary<FolderKey, FolderItem> ();
		private static bool wellKnownFoldersReady;
		private static FileInfo instance = new FileInfo ();
		
		private IShellFolder root;
	}
}
