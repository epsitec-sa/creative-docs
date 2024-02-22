using Epsitec.Common.Support;
using NUnit.Framework;
using System.IO;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class FileManagerTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Document.Engine.Initialize();
            Epsitec.Common.Widgets.Widget.Initialize();
        }

        [Test]
        public void CheckDelete()
        {
            string testFile1 = Path.GetTempFileName();
            string testFile2 = Path.GetTempFileName();
            System.IO.File.WriteAllText(testFile1, "Fichier 1\r\n");
            System.IO.File.WriteAllText(testFile2, "Fichier 2\r\n");

            FileOperationMode mode = new FileOperationMode();

            mode.AutoConfirmation = true;

            FileManager.DeleteFiles(mode, testFile1, testFile2);

            Assert.IsFalse(System.IO.File.Exists(testFile1));
            Assert.IsFalse(System.IO.File.Exists(testFile2));
        }

        [Test]
        [Ignore("Broken behavior. Seems unused.")]
        public void CheckCopy()
        {
            string testFile = Path.GetTempFileName();
            System.IO.File.WriteAllText(testFile, "Fichier 1\r\n");

            FileOperationMode mode = new FileOperationMode();

            string firstCopy = GetRandomTempPath();
            string secondCopy = GetRandomTempPath();
            FileManager.CopyFiles(
                mode,
                new string[] { testFile },
                new string[] { firstCopy, secondCopy }
            );

            Assert.IsTrue(System.IO.File.Exists(testFile));
            Assert.IsTrue(System.IO.File.Exists(firstCopy));
            Assert.IsTrue(System.IO.File.Exists(secondCopy));

            System.IO.File.Delete(testFile);
            System.IO.File.Delete(firstCopy);
            System.IO.File.Delete(secondCopy);
        }

        [Test]
        public void CheckMove()
        {
            string testFile1 = Path.GetTempFileName();
            string testFile2 = Path.GetTempFileName();
            System.IO.File.WriteAllText(testFile1, "Fichier 1\r\n");
            System.IO.File.WriteAllText(testFile2, "Fichier 2\r\n");

            FileOperationMode mode = new FileOperationMode();

            string movedFile1 = GetRandomTempPath();
            string movedFile2 = GetRandomTempPath();
            FileManager.MoveFiles(
                mode,
                new string[] { testFile1, testFile2 },
                new string[] { movedFile1, movedFile2 }
            );

            Assert.IsFalse(System.IO.File.Exists(testFile1));
            Assert.IsFalse(System.IO.File.Exists(testFile2));
            Assert.IsTrue(System.IO.File.Exists(movedFile1));
            Assert.IsTrue(System.IO.File.Exists(movedFile2));

            System.IO.File.Delete(movedFile1);
            System.IO.File.Delete(movedFile2);
        }
        
        [Test]
        public void CheckRename()
        {
            string testFile = Path.GetTempFileName();
            System.IO.File.WriteAllText(testFile, "Fichier 1\r\n");

            FileOperationMode mode = new FileOperationMode();

            string renamedFile = GetRandomTempPath();
            FileManager.RenameFile(mode, testFile, renamedFile);

            Assert.IsFalse(System.IO.File.Exists(testFile));
            Assert.IsTrue(System.IO.File.Exists(renamedFile));

            System.IO.File.Delete(renamedFile);
        }

        [Test]
        [Ignore("Broken behavior. Seems unused.")]
        public void CheckCopyToFolderDeleteFolder()
        {
            string testFile = Path.GetTempFileName();
            System.IO.File.WriteAllText(testFile, "Fichier 1\r\n");

            string folder = GetRandomTempPath();
            System.IO.Directory.CreateDirectory(folder);

            FileOperationMode mode = new FileOperationMode() { AutoCreateDirectory = true };

            string newFolder = GetRandomTempPath();
            string newFilePath = Path.Join(newFolder, Path.GetFileName(testFile));
            FileManager.CopyFilesToFolder(
                mode,
                new string[] { testFile },
                newFolder
            );

            Assert.IsTrue(System.IO.File.Exists(testFile));
            Assert.IsTrue(System.IO.File.Exists(newFilePath));

            mode.AutoConfirmation = true;

            FileManager.DeleteFiles(mode, newFolder);
        }
        [Test]
        [Ignore("Broken behavior. Seems unused.")]
        public void CheckMoveToFolderDeleteFolder()
        {
            string testFile = Path.GetTempFileName();
            System.IO.File.WriteAllText(testFile, "Fichier 1\r\n");

            string folder = GetRandomTempPath();
            System.IO.Directory.CreateDirectory(folder);

            FileOperationMode mode = new FileOperationMode() { AutoCreateDirectory = true };

            string newFolder = GetRandomTempPath();
            string newFilePath = Path.Join(newFolder, Path.GetFileName(testFile));
            FileManager.MoveFilesToFolder(
                mode,
                new string[] { testFile },
                newFolder
            );

            Assert.IsFalse(System.IO.File.Exists(testFile));
            Assert.IsTrue(System.IO.File.Exists(newFilePath));

            mode.AutoConfirmation = true;

            FileManager.DeleteFiles(mode, newFolder);
        }

        [Test]
        public void CheckMoveWildcardToFolderDeleteFolder()
        {
            string sourceFolder = GetRandomTempPath();
            System.IO.Directory.CreateDirectory(sourceFolder);
            
            string testFile1 = Path.Join(sourceFolder, Path.GetRandomFileName());
            string testFile2 = Path.Join(sourceFolder, Path.GetRandomFileName());
            System.IO.File.WriteAllText(testFile1, "Fichier 1\r\n");
            System.IO.File.WriteAllText(testFile2, "Fichier 2\r\n");

            FileOperationMode mode = new FileOperationMode() { AutoCreateDirectory = true };

            string newFolder = GetRandomTempPath();
            string newFilePath1 = Path.Join(newFolder, Path.GetFileName(testFile1));
            string newFilePath2 = Path.Join(newFolder, Path.GetFileName(testFile2));

            string wildcardPattern = Path.Join(sourceFolder, "*.*");
            FileManager.MoveFilesToFolder(
                mode,
                new string[] { wildcardPattern },
                newFolder
            );

            Assert.IsFalse(System.IO.File.Exists(testFile1));
            Assert.IsFalse(System.IO.File.Exists(testFile2));
            Assert.IsTrue(System.IO.File.Exists(newFilePath1));
            Assert.IsTrue(System.IO.File.Exists(newFilePath2));

            mode.AutoConfirmation = true;

            FileManager.DeleteFiles(mode, sourceFolder, newFolder);
        }

        [Test]
        public void CheckCopyWildcardToFolderDeleteFolder()
        {
            string sourceFolder = GetRandomTempPath();
            System.IO.Directory.CreateDirectory(sourceFolder);
            
            string testFile1 = Path.Join(sourceFolder, Path.GetRandomFileName());
            string testFile2 = Path.Join(sourceFolder, Path.GetRandomFileName());
            System.IO.File.WriteAllText(testFile1, "Fichier 1\r\n");
            System.IO.File.WriteAllText(testFile2, "Fichier 2\r\n");

            FileOperationMode mode = new FileOperationMode() { AutoCreateDirectory = true };

            string newFolder = GetRandomTempPath();
            string newFilePath1 = Path.Join(newFolder, Path.GetFileName(testFile1));
            string newFilePath2 = Path.Join(newFolder, Path.GetFileName(testFile2));

            string wildcardPattern = Path.Join(sourceFolder, "*.*");
            FileManager.CopyFilesToFolder(
                mode,
                new string[] { wildcardPattern },
                newFolder
            );

            Assert.IsTrue(System.IO.File.Exists(testFile1));
            Assert.IsTrue(System.IO.File.Exists(testFile2));
            Assert.IsTrue(System.IO.File.Exists(newFilePath1));
            Assert.IsTrue(System.IO.File.Exists(newFilePath2));

            mode.AutoConfirmation = true;

            FileManager.DeleteFiles(mode, sourceFolder, newFolder);
        }

        [Test]
        [Ignore("The 'Recent' folder seems to be handled differently now.")]
        public void CheckRecentDocuments()
        {
            string path = Path.GetTempFileName();
            System.IO.File.WriteAllText(path, "");

            FileManager.AddToRecentDocuments(path);

            bool ok = false;

            FolderItem folderItem = FileManager.GetFolderItem(
                FolderId.Recent,
                FolderQueryMode.NoIcons
            );

            foreach (
                FolderItem item in FileManager.GetFolderItems(folderItem, FolderQueryMode.NoIcons)
            )
            {

                if (item.IsShortcut)
                {
                    string resolvesTo = FileManager
                        .ResolveShortcut(item, FolderQueryMode.NoIcons)
                        .FullPath;
                    if (path == resolvesTo)
                    {
                        ok = true;
                        break;
                    }
                }
                else
                {
                    Assert.Fail(
                        string.Format("IsShortcut={0} for item {1}", item.IsShortcut, item)
                    );
                }
            }

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            Assert.IsTrue(ok, "Not found in recent folder");
        }

        private string GetRandomTempPath()
        {
            return Path.Join(Path.GetTempPath(), Path.GetRandomFileName());
        }
    }
}
