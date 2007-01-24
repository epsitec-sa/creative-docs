//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Platform
{
	/// <summary>
	/// The <c>Shortcut</c> class wraps the shortcut creation and resolution
	/// functionality.
	/// </summary>
	internal static class Shortcut
	{
		/// <summary>
		/// Resolves the shortcut defined by the specified path and returns its
		/// target <see cref="FolderItem"/>.
		/// </summary>
		/// <param name="path">The path to the shortcut file.</param>
		/// <param name="mode">The query mode.</param>
		/// <returns>The <c>FolderItem</c> if the shortcut could be resolved; otherwise, <c>null</c>.</returns>
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
