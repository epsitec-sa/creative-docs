using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class FileManagerTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Widget.Initialize ();
		}
		
		[Test]
		public void _AutomaticRun()
		{
			//	If run individually, automaticRun will be false when the CheckDelete test
			//	is run interactively; otherwise, the CheckDelete test will be run silently,
			//	as it would block with the confirmation request from the Windows UI.

			this.automaticRun = true;
		}

		[Test]
		public void CheckDelete()
		{
			System.IO.File.WriteAllText (@"S:\test 1.txt", "Fichier 1\r\n");
			System.IO.File.WriteAllText (@"S:\test 2.txt", "Fichier 2\r\n");

			FileOperationMode mode = new FileOperationMode ();

			mode.AutoConfirmation = this.automaticRun;

			FileManager.DeleteFiles (mode, @"S:\test 1.txt", @"S:\test 2.txt");

			Assert.IsFalse (System.IO.File.Exists (@"S:\test 1.txt"));
			Assert.IsFalse (System.IO.File.Exists (@"S:\test 2.txt"));
		}
		
		[Test]
		public void CheckCopyMoveRename()
		{
			System.IO.File.WriteAllText (@"S:\test 1.txt", "Fichier 1\r\n");
			System.IO.File.WriteAllText (@"S:\test 2.txt", "Fichier 2\r\n");

			FileOperationMode mode = new FileOperationMode ();

			FileManager.CopyFiles (mode, new string[] { @"S:\test 1.txt", @"S:\test 2.txt" }, new string[] { @"S:\test 1 bis.txt", @"S:\Epsitec.Cresus\test 2 bis.txt" });
			FileManager.RenameFile (mode, @"S:\test 1.txt", @"S:\test A.txt");
			FileManager.RenameFile (mode, @"S:\test 2.txt", @"S:\test B.txt");
			FileManager.MoveFiles (mode, new string[] { @"S:\test A.txt", @"S:\test B.txt" }, new string[] { @"S:\Epsitec.Cresus\test 1 move.txt", @"S:\test 2 move.txt" });

			Assert.IsTrue (System.IO.File.Exists (@"S:\test 1 bis.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\Epsitec.Cresus\test 2 bis.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\Epsitec.Cresus\test 1 move.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\test 2 move.txt"));

			System.IO.File.Delete (@"S:\test 1 bis.txt");
			System.IO.File.Delete (@"S:\Epsitec.Cresus\test 2 bis.txt");
			System.IO.File.Delete (@"S:\Epsitec.Cresus\test 1 move.txt");
			System.IO.File.Delete (@"S:\test 2 move.txt");
		}

		[Test]
		public void CheckCopyMoveToFolderDeleteFolder()
		{
			System.IO.File.WriteAllText (@"S:\test 1.txt", "Fichier 1\r\n");
			System.IO.File.WriteAllText (@"S:\test 2.txt", "Fichier 2\r\n");

			System.IO.Directory.CreateDirectory (@"S:\file manager test 1");
//			System.IO.Directory.CreateDirectory (@"S:\file manager test 2");

			FileOperationMode mode = new FileOperationMode ();

			mode.AutoCreateDirectory = true;

			FileManager.CopyFilesToFolder (mode, new string[] { @"S:\test 1.txt", @"S:\test 2.txt" }, @"S:\file manager test 1");
			FileManager.MoveFilesToFolder (mode, new string[] { @"S:\test 1.txt", @"S:\test 2.txt" }, @"S:\file manager test 2");

			Assert.IsFalse (System.IO.File.Exists (@"S:\test 1.txt"));
			Assert.IsFalse (System.IO.File.Exists (@"S:\test 2.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 1\test 1.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 1\test 2.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 2\test 1.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 2\test 2.txt"));

			mode.AutoConfirmation = true;

			FileManager.DeleteFiles (mode, @"S:\file manager test 1", @"S:\file manager test 2");
		}

		[Test]
		public void CheckCopyMoveWildcardToFolderDeleteFolder()
		{
			System.IO.File.WriteAllText (@"S:\test 1.txt", "Fichier 1\r\n");
			System.IO.File.WriteAllText (@"S:\test 2.txt", "Fichier 2\r\n");

			System.IO.Directory.CreateDirectory (@"S:\file manager test 1");
			System.IO.Directory.CreateDirectory (@"S:\file manager test 2");

			FileOperationMode mode = new FileOperationMode ();

			FileManager.CopyFilesToFolder (mode, new string[] { @"S:\*.txt" }, @"S:\file manager test 1");
			FileManager.MoveFilesToFolder (mode, new string[] { @"S:\*.txt" }, @"S:\file manager test 2");

			Assert.IsFalse (System.IO.File.Exists (@"S:\test 1.txt"));
			Assert.IsFalse (System.IO.File.Exists (@"S:\test 2.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 1\test 1.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 1\test 2.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 2\test 1.txt"));
			Assert.IsTrue (System.IO.File.Exists (@"S:\file manager test 2\test 2.txt"));

			mode.AutoConfirmation = true;
			
			FileManager.DeleteFiles (mode, @"S:\file manager test 1", @"S:\file manager test 2");
		}

		[Test]
		public void CheckRecentDocuments()
		{
			string path = @"S:\Epsitec.Cresus\Example Document.txt";

			if (!System.IO.File.Exists (path))
			{
				System.IO.File.CreateText (path).Close ();
			}
			
			FileManager.AddToRecentDocuments (path);

			bool ok = false;

			FolderItem folderItem = FileManager.GetFolderItem (FolderId.Recent, FolderQueryMode.NoIcons);
			
			foreach (FolderItem item in FileManager.GetFolderItems (folderItem, FolderQueryMode.NoIcons))
			{
				System.Console.Out.WriteLine ("{0}", item);

				if (item.IsShortcut)
				{
					string resolvesTo = FileManager.ResolveShortcut (item, FolderQueryMode.NoIcons).FullPath;
					System.Console.Out.WriteLine ("  --> {0}", resolvesTo);
					if (path == resolvesTo)
					{
						ok = true;
					}
				}
				else
				{
					System.Console.Out.Flush ();
					Assert.Fail (string.Format ("IsShortcut={0} for item {1}", item.IsShortcut, item));
				}
			}

			if (System.IO.File.Exists (path))
			{
				System.IO.File.Delete (path);
			}

			Assert.IsTrue (ok, "Not found in recent folder");
		}

		private bool automaticRun;
	}
}
