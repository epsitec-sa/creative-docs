//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Platform
{
	/// <summary>
	/// The <c>Shortcut</c> class wraps the shortcut creation and resolution
	/// functionality.
	/// </summary>
	internal static class Shortcut
	{
		public static FolderItem Resolve(string path, FolderQueryMode mode)
		{
			using (Win32.ShellShortcut shortcut = new Win32.ShellShortcut (path))
			{
				FolderItemHandle handle = shortcut.TargetPidl;
				
				return handle == null ? FolderItem.Empty : Win32.FileInfo.CreateFolderItem (handle, mode);
			}
		}
	}
}
