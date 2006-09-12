namespace Epsitec.Common.Support.Platform
{
	internal static class Shortcut
	{
		public static FolderItem Resolve(string path, FolderQueryMode mode)
		{
			using (Win32.ShellShortcut shortcut = new Win32.ShellShortcut (path))
			{
				return Win32.FileInfo.CreateFolderItem (shortcut.TargetPidl, mode);
			}
		}
	}
}
