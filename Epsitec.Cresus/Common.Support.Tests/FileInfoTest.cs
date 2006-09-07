using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class FileInfoTest
	{
		[Test]
		public void CheckCreateFolderItem1()
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
				FolderItem item = FileManager.CreateFolderItem (path, FolderDetailsMode.LargeIcons);
				string file = System.IO.Path.GetFileName (path);

				System.Console.Out.WriteLine ("{0} : {1} ({2}){3}", file, item.DisplayName, item.TypeName, (item.Icon == null) ? " no normal icon" : "");
				System.Console.Out.WriteLine ("----> path = {0}", item.FullPath);
				
				if (item.Icon != null)
				{
					byte[] data = item.Icon.BitmapImage.Save (Epsitec.Common.Drawing.ImageFormat.Png);
					System.IO.File.WriteAllBytes (string.Concat ("Files ", file, ".png"), data);
				}
			}
		}

		[Test]
		public void CheckCreateFolderItem2()
		{
			System.Array ids = System.Enum.GetValues (typeof (FolderId));

			FolderDetailsMode modeNormal = FolderDetailsMode.LargeIcons;

			foreach (int id in ids)
			{
				FolderItem item = FileManager.CreateFolderItem ((FolderId) id, modeNormal);

				System.Console.Out.WriteLine ("{0} : {1} ({2}) virtual={3}", (FolderId) id, item.DisplayName, item.TypeName, item.IsVirtual);
				System.Console.Out.WriteLine ("----> path = {0}", item.FullPath);

				if (item.Icon != null)
				{
					byte[] data = item.Icon.BitmapImage.Save (Epsitec.Common.Drawing.ImageFormat.Png);
					System.IO.File.WriteAllBytes (string.Concat ("Item ", (FolderId) id, ".png"), data);
				}
			}
		}

		[Test]
		public void CheckGetFolderItemsFromFolder()
		{
			string path = @"S:\Epsitec.Cresus\External";
			FolderItem root = FileManager.CreateFolderItem (path, FolderDetailsMode.NoIcons);

			foreach (FolderItem item in FileManager.GetFolderItems (root, FolderDetailsMode.LargeIcons))
			{
				System.Console.Out.WriteLine ("{0} ({1}), {2}, Virtual={3}", item.DisplayName, item.TypeName, item.FullPath, item.IsVirtual);
			}
		}

		[Test]
		public void CheckGetFolderItemsFromDesktop()
		{
			FolderItem root = FileManager.CreateFolderItem (FolderId.VirtualDesktop, FolderDetailsMode.NoIcons);

			foreach (FolderItem item in FileManager.GetFolderItems (root, FolderDetailsMode.LargeIcons))
			{
				System.Console.Out.WriteLine ("{0} ({1}), {2}, Virtual={3}", item.DisplayName, item.TypeName, item.FullPath, item.IsVirtual);
			}
		}

		[Test]
		public void CheckGetFolderItemsFromMyComputer()
		{
			FolderItem root = FileManager.CreateFolderItem (FolderId.VirtualMyComputer, FolderDetailsMode.NoIcons);

			foreach (FolderItem item in FileManager.GetFolderItems (root, FolderDetailsMode.LargeIcons))
			{
				System.Console.Out.WriteLine ("{0} ({1}), {2}, Virtual={3}", item.DisplayName, item.TypeName, item.FullPath, item.IsVirtual);
			}
		}
	}
}
