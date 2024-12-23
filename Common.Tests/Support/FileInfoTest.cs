/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Epsitec.Common.Support;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class FileInfoTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Document.Engine.Initialize();
            Epsitec.Common.Widgets.Widget.Initialize();
        }

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

            FolderQueryMode mode = FolderQueryMode.LargeIcons;

            mode.AsOpenFolder = true;

            foreach (string path in paths)
            {
                FolderItem item = FileManager.GetFolderItem(path, mode);
                string file = System.IO.Path.GetFileName(path);

                System.Console.Out.WriteLine(
                    "{0} : {1} ({2}){3}",
                    file,
                    item.DisplayName,
                    item.TypeName,
                    (item.Icon == null) ? " no normal icon" : ""
                );
                System.Console.Out.WriteLine("----> path = {0}", item.FullPath);

                if (item.Icon != null)
                {
                    byte[] data = item.Icon.Image.BitmapImage.Save(
                        Epsitec.Common.Drawing.ImageFormat.Png
                    );
                    System.IO.File.WriteAllBytes(string.Concat("Files ", file, ".png"), data);
                }
            }
        }

        [Test]
        [Ignore("has no assert -> should probably be deleted")]
        public void CheckCreateFolderItem2()
        {
            /*
            System.Array ids = System.Enum.GetValues(typeof(FolderId));

            FolderQueryMode modeNormal = FolderQueryMode.LargeIcons;
            List<FolderId> failedList = new List<FolderId>();
            List<FolderId> virtualList = new List<FolderId>();

            foreach (int id in ids)
            {
                FolderItem item = FileManager.GetFolderItem((FolderId)id, modeNormal);

                if (item.IsEmpty)
                {
                    failedList.Add(((FolderId)id));
                }
                else if (item.IsVirtual)
                {
                    virtualList.Add((FolderId)id);
                }
                else
                {
                    System.Console.Out.WriteLine(
                        "{0} : {1} ({2})",
                        (FolderId)id,
                        item.DisplayName,
                        item.TypeName
                    );
                    System.Console.Out.WriteLine("----> path = {0}", item.FullPath);

                    if (item.Icon != null)
                    {
                        byte[] data = item.Icon.Image.BitmapImage.Save(
                            Epsitec.Common.Drawing.ImageFormat.Png
                        );
                        System.IO.File.WriteAllBytes(
                            string.Concat("Item ", (FolderId)id, ".png"),
                            data
                        );
                    }
                }
            }

            System.Console.Out.WriteLine();
            System.Console.Out.WriteLine("Pure virtual folders");
            System.Console.Out.WriteLine("--------------------");

            foreach (FolderId id in virtualList)
            {
                FolderItem item = FileManager.GetFolderItem(id, modeNormal);

                System.Console.Out.WriteLine(
                    "{0} : {1} ({2})",
                    id,
                    item.DisplayName,
                    item.TypeName
                );

                if (item.Icon != null)
                {
                    byte[] data = item.Icon.Image.BitmapImage.Save(
                        Epsitec.Common.Drawing.ImageFormat.Png
                    );
                    System.IO.File.WriteAllBytes(string.Concat("Item ", id, ".png"), data);
                }
            }

            if (failedList.Count > 0)
            {
                System.Console.Out.WriteLine();
                System.Console.Out.WriteLine("Failed to resolve");
                System.Console.Out.WriteLine("-----------------");

                foreach (FolderId id in failedList)
                {
                    System.Console.Out.WriteLine("{0}", id);
                }
            }
            */
            throw new System.NotImplementedException();
        }

        [Test]
        [Ignore("Checks for absolute path + has no assert -> should probably be deleted")]
        public void CheckGetFolderItemsFromFolder()
        {
            string path = @"S:\Epsitec.Cresus\External";
            FolderItem root = FileManager.GetFolderItem(path, FolderQueryMode.NoIcons);

            foreach (
                FolderItem item in FileManager.GetFolderItems(root, FolderQueryMode.LargeIcons)
            )
            {
                System.Console.Out.WriteLine(
                    "{0} ({1}), {2}, Virtual={3}",
                    item.DisplayName,
                    item.TypeName,
                    item.FullPath,
                    item.IsVirtual
                );
                System.Console.Out.WriteLine("  {0}", item);
            }
        }

        [Test]
        public void CheckGetFolderItemsFromDesktop()
        {
            FolderItem root = FileManager.GetFolderItem(
                Environment.SpecialFolder.Desktop,
                FolderQueryMode.NoIcons
            );

            foreach (
                FolderItem item in FileManager.GetFolderItems(root, FolderQueryMode.LargeIcons)
            )
            {
                System.Console.Out.WriteLine(
                    "{0} ({1}), {2}, Virtual={3}",
                    item.DisplayName,
                    item.TypeName,
                    item.FullPath,
                    item.IsVirtual
                );
                System.Console.Out.WriteLine("  {0}", item);
            }
        }

        [Test]
        public void CheckGetFolderItemsFromMyComputer()
        {
            FolderItem root = FileManager.GetFolderItem(
                Environment.SpecialFolder.MyComputer,
                FolderQueryMode.NoIcons
            );

            foreach (
                FolderItem item in FileManager.GetFolderItems(root, FolderQueryMode.LargeIcons)
            )
            {
                System.Console.Out.WriteLine(
                    "{0} ({1}), {2}, Virtual={3}",
                    item.DisplayName,
                    item.TypeName,
                    item.FullPath,
                    item.IsVirtual
                );
                System.Console.Out.WriteLine("  {0}", item);

                System.IO.DriveInfo drive = item.DriveInfo;

                if (drive != null)
                {
                    System.Console.Out.WriteLine("  DriveType={0}", drive.DriveType);
                }
            }
        }

        [Test]
        public void CheckEqualsFolderItem()
        {
            FolderItem desktop = FileManager.GetFolderItem(
                Environment.SpecialFolder.Desktop,
                FolderQueryMode.NoIcons
            );
            FolderItem computer = FileManager.GetFolderItem(
                Environment.SpecialFolder.MyComputer,
                FolderQueryMode.NoIcons
            );
            FolderItem documents = FileManager.GetFolderItem(
                Environment.SpecialFolder.MyDocuments,
                FolderQueryMode.NoIcons
            );

            bool okComputer = false;
            bool okDocuments = false;

            FolderItem parent1 = FolderItem.Empty;
            FolderItem parent2 = FolderItem.Empty;

            Assert.AreEqual(parent1, parent2);

            foreach (
                FolderItem item in FileManager.GetFolderItems(desktop, FolderQueryMode.NoIcons)
            )
            {
                if (item == computer)
                {
                    okComputer = true;
                    parent1 = FileManager.GetParentFolderItem(item, FolderQueryMode.NoIcons);
                }
                if (item == documents)
                {
                    okDocuments = true;
                    parent2 = FileManager.GetParentFolderItem(item, FolderQueryMode.NoIcons);
                }
            }

            //	There is no guarantee that there is a computer or a document folder on
            //	the desktop of the test machine...

            if (okDocuments && okComputer)
            {
                Assert.AreEqual(parent1, parent2);
                Assert.AreEqual(desktop, parent1);
                Assert.AreEqual(desktop, parent2);
            }
        }

        [Test]
        public void CheckEqualsFolderIcons()
        {
            FolderItem documents1 = FileManager.GetFolderItem(
                Environment.SpecialFolder.MyDocuments,
                FolderQueryMode.LargeIcons
            );
            FolderItem documents2 = FileManager.GetFolderItem(
                Environment.SpecialFolder.MyDocuments,
                FolderQueryMode.LargeIcons
            );

            byte[] image1 = documents1.Icon.Image.BitmapImage.GetRawBitmapBytes();
            byte[] image2 = documents1.Icon.Image.BitmapImage.GetRawBitmapBytes();

            Assert.AreEqual(
                IO.Checksum.ComputeCrc32(
                    delegate(IO.IChecksum checksum)
                    {
                        checksum.Update(image1);
                    }
                ),
                IO.Checksum.ComputeCrc32(
                    delegate(IO.IChecksum checksum)
                    {
                        checksum.Update(image2);
                    }
                )
            );

            Assert.AreEqual(documents1.Icon.ImageName, documents2.Icon.ImageName);

            System.Console.Out.WriteLine(
                "Large icon name for Desktop: {0}",
                FileManager
                    .GetFolderItem(Environment.SpecialFolder.Desktop, FolderQueryMode.LargeIcons)
                    .Icon.ImageName
            );
            System.Console.Out.WriteLine(
                "Small icon name for Desktop: {0}",
                FileManager
                    .GetFolderItem(Environment.SpecialFolder.Desktop, FolderQueryMode.SmallIcons)
                    .Icon.ImageName
            );
        }

        [Test]
        public void CheckShowHiddenFiles()
        {
            System.Console.Out.WriteLine("Show hidden files : {0}", FolderItem.ShowHiddenFiles);
        }

        [Test]
        public void CheckGetParentFolderItem1()
        {
            FolderItem item = FileManager.GetFolderItem(
                Environment.SpecialFolder.MyDocuments,
                FolderQueryMode.NoIcons
            );

            while (!item.IsEmpty)
            {
                System.Console.Out.WriteLine(
                    "{0} ({1}), {2}",
                    item.DisplayName,
                    item.TypeName,
                    item.FullPath
                );

                item = FileManager.GetParentFolderItem(item, FolderQueryMode.NoIcons);
            }
        }

        [Test]
        public void CheckGetParentFolderItem2()
        {
            FolderItem item = FileManager.GetFolderItem(
                @"S:\Epsitec.Cresus\External",
                FolderQueryMode.NoIcons
            );

            while (!item.IsEmpty)
            {
                System.Console.Out.WriteLine(
                    "{0} ({1}), {2}",
                    item.DisplayName,
                    item.TypeName,
                    item.FullPath
                );

                item = FileManager.GetParentFolderItem(item, FolderQueryMode.NoIcons);
            }
        }

        [Test]
        public void CheckGetParentFolderItem3()
        {
            FolderItem item = FileManager.GetFolderItem(
                @"\\bigdell\s$\Arnaud\Desktop\5000.txt",
                FolderQueryMode.NoIcons
            );

            while (!item.IsEmpty)
            {
                System.Console.Out.WriteLine(
                    "{0} ({1}), {2}",
                    item.DisplayName,
                    item.TypeName,
                    item.FullPath
                );

                item = FileManager.GetParentFolderItem(item, FolderQueryMode.NoIcons);
            }
        }
    }
}
