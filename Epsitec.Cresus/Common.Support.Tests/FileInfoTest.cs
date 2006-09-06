using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class FileInfoTest
	{
		[Test]
		public void CheckGetIcon1()
		{
			System.Array ids = System.Enum.GetValues (typeof (SystemFileId));

			FolderDetailsMode modeNormal = FolderDetailsMode.LargeIcons;
			FolderDetailsMode modeActive = FolderDetailsMode.LargeIcons;
			
			modeActive.IconSelection = FileInfoSelection.Active;

			foreach (int id in ids)
			{
				System.Drawing.Icon iconNormal = Platform.Win32.FileInfo.GetIcon ((SystemFileId) id, modeNormal);
				System.Drawing.Icon iconActive = Platform.Win32.FileInfo.GetIcon ((SystemFileId) id, modeActive);

				string displayName;
				string typeName;

				Platform.Win32.FileInfo.GetDisplayAndTypeNames ((SystemFileId) id, out displayName, out typeName);
				
				string path = Platform.Win32.FileInfo.GetPath ((SystemFileId) id);

				System.Console.Out.WriteLine ("{0} : {1} ({2}){3}{4}", (SystemFileId) id, displayName, typeName, (iconNormal == null) ? " no normal icon" : "", (iconActive == null) ? " no normal icon" : "");
				System.Console.Out.WriteLine ("----> path = {0}", path);

				if (iconNormal != null)
				{
					Drawing.Bitmap image = Drawing.Bitmap.FromNativeIcon (iconNormal) as Drawing.Bitmap;

					System.Console.Out.WriteLine ("Image is {0} x {1}", image.Width, image.Height);

					byte[] data = image.Save (Epsitec.Common.Drawing.ImageFormat.Png);
					System.IO.File.WriteAllBytes (string.Concat ((SystemFileId) id, ".png"), data);
				}
			}
		}
		
		[Test]
		public void CheckGetIcon2()
		{
			string[] paths = new string[]
			{
				@"s:\Epsitec.Cresus\External\nunit-gui.exe",
				@"s:\Epsitec.Cresus\Visual.bat",
				@"s:\Epsitec.Cresus\Tralala.pdf",
				@"C:\",
				@"C:\WINDOWS",
				@"C:\Documents and Settings\Arnaud\Desktop"
			};

			foreach (string path in paths)
			{
				System.Drawing.Icon icon = Platform.Win32.FileInfo.GetIcon (path, FolderDetailsMode.LargeIcons);
				
				string displayName;
				string typeName;

				Platform.Win32.FileInfo.GetDisplayAndTypeNames (path, out displayName, out typeName);

				string file = System.IO.Path.GetFileName (path);

				System.Console.Out.WriteLine ("{0} : {1} ({2}){3}", file, displayName, typeName, (icon == null) ? " no normal icon" : "");
				System.Console.Out.WriteLine ("----> path = {0}", path);
				
				if (icon != null)
				{
					Drawing.Bitmap image = Drawing.Bitmap.FromNativeIcon (icon) as Drawing.Bitmap;

					System.Console.Out.WriteLine ("Image is {0} x {1}", image.Width, image.Height);

					byte[] data = image.Save (Epsitec.Common.Drawing.ImageFormat.Png);
					System.IO.File.WriteAllBytes (string.Concat (file, ".png"), data);
				}
			}
		}

		[Test]
		public void CheckGetFolderItem()
		{
			System.Array ids = System.Enum.GetValues (typeof (SystemFileId));

			FolderDetailsMode modeNormal = FolderDetailsMode.LargeIcons;

			foreach (int id in ids)
			{
				FolderItem item = Platform.Win32.FileInfo.GetFolderItem ((SystemFileId) id, modeNormal);

				System.Console.Out.WriteLine ("{0} : {1} ({2}) virtual={3}", (SystemFileId) id, item.DisplayName, item.TypeName, item.IsVirtual);
				System.Console.Out.WriteLine ("----> path = {0}", item.FullPath);

				if (item.Icon != null)
				{
					byte[] data = item.Icon.BitmapImage.Save (Epsitec.Common.Drawing.ImageFormat.Png);
					System.IO.File.WriteAllBytes (string.Concat ("Item ", (SystemFileId) id, ".png"), data);
				}
			}
		}

		[Test]
		public void CheckGetFolderItems()
		{
			string path = @"S:\Epsitec.Cresus\External";

			foreach (FolderItem item in Platform.Win32.FileInfo.GetFolderItems (path, FolderDetailsMode.LargeIcons))
			{
				System.Console.Out.WriteLine ("{0} ({1}), {2}, Virtual={3}", item.DisplayName, item.TypeName, item.FullPath, item.IsVirtual);
			}
		}
	}
}
