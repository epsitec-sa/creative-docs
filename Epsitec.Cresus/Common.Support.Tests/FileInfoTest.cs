using NUnit.Framework;
using System.Collections.Generic;

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
			List<FolderId> failedList = new List<FolderId> ();
			List<FolderId> virtualList = new List<FolderId> ();

			foreach (int id in ids)
			{
				FolderItem item = FileManager.CreateFolderItem ((FolderId) id, modeNormal);

				if (item.IsEmpty)
				{
					failedList.Add (((FolderId) id));
				}
				else if (item.IsVirtual)
				{
					virtualList.Add ((FolderId) id);
				}
				else
				{
					System.Console.Out.WriteLine ("{0} : {1} ({2})", (FolderId) id, item.DisplayName, item.TypeName);
					System.Console.Out.WriteLine ("----> path = {0}", item.FullPath);

					if (item.Icon != null)
					{
						byte[] data = item.Icon.BitmapImage.Save (Epsitec.Common.Drawing.ImageFormat.Png);
						System.IO.File.WriteAllBytes (string.Concat ("Item ", (FolderId) id, ".png"), data);
					}
				}
			}
			
			System.Console.Out.WriteLine ();
			System.Console.Out.WriteLine ("Pure virtual folders");
			System.Console.Out.WriteLine ("--------------------");
			
			foreach (FolderId id in virtualList)
			{
				FolderItem item = FileManager.CreateFolderItem (id, modeNormal);

				System.Console.Out.WriteLine ("{0} : {1} ({2})", id, item.DisplayName, item.TypeName);

				if (item.Icon != null)
				{
					byte[] data = item.Icon.BitmapImage.Save (Epsitec.Common.Drawing.ImageFormat.Png);
					System.IO.File.WriteAllBytes (string.Concat ("Item ", id, ".png"), data);
				}
			}

			if (failedList.Count > 0)
			{
				System.Console.Out.WriteLine ();
				System.Console.Out.WriteLine ("Failed to resolve");
				System.Console.Out.WriteLine ("-----------------");

				foreach (FolderId id in failedList)
				{
					System.Console.Out.WriteLine ("{0}", id);
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

		[Test]
		public void CheckGetParentFolderItem1()
		{
			FolderItem item = FileManager.CreateFolderItem (FolderId.VirtualMyDocuments, FolderDetailsMode.NoIcons);

			while (!item.IsEmpty)
			{
				System.Console.Out.WriteLine ("{0} ({1}), {2}", item.DisplayName, item.TypeName, item.FullPath);

				item = FileManager.GetParentFolderItem (item, FolderDetailsMode.NoIcons);
			}
		}

		[Test]
		public void CheckGetParentFolderItem2()
		{
			FolderItem item = FileManager.CreateFolderItem (@"S:\Epsitec.Cresus\External", FolderDetailsMode.NoIcons);

			while (!item.IsEmpty)
			{
				System.Console.Out.WriteLine ("{0} ({1}), {2}", item.DisplayName, item.TypeName, item.FullPath);

				item = FileManager.GetParentFolderItem (item, FolderDetailsMode.NoIcons);
			}
		}
	}
}
