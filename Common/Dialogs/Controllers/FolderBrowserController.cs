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


using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs.Controllers
{
    public class FolderBrowserController
    {
        public FolderBrowserController(FileNavigationController navigationController)
        {
            this.navigationController = navigationController;
            this.navigationController.ActiveDirectoryChanged +=
                this.HandleNavigationControllerActiveDirectoryChanged;
        }

        public TextFieldCombo BrowserWidget
        {
            get
            {
                if (this.browser == null)
                {
                    this.CreateUserInterface();
                }

                return this.browser;
            }
        }

        private void CreateUserInterface()
        {
            this.browser = new TextFieldCombo();
            this.browser.IsReadOnly = true;

            this.browser.ComboOpening += new EventHandler<CancelEventArgs>(
                this.HandleBrowserComboOpening
            );
            this.browser.ComboClosed += this.HandleBrowserComboClosed;

            this.browser.ItemTextConverter = delegate(string itemText)
            {
                return itemText.Trim();
            };

            this.SyncBrowserText();
        }

        private void HandleNavigationControllerActiveDirectoryChanged(
            object sender,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (this.browser != null)
            {
                this.SyncBrowserText();
            }
        }

        private void SyncBrowserText()
        {
            FolderItemIcon folderIcon = this.navigationController.ActiveSmallIcon;
            string folderName = this.navigationController.ActiveDirectoryDisplayName;

            this.browser.Text = FolderBrowserController.CreateIconAndLabel(folderIcon, folderName);
        }

        private void HandleBrowserComboOpening(object sender, CancelEventArgs e)
        {
            this.comboFolders = new List<FileListItem>();

            //	Add all desktop and computer nodes

            FolderItem desktop = FileManager.GetFolderItem(
                System.Environment.SpecialFolder.Desktop,
                FolderQueryMode.SmallIcons
            );
            FolderItem computer = FileManager.GetFolderItem(
                System.Environment.SpecialFolder.MyDocuments,
                FolderQueryMode.SmallIcons
            );

            bool includeHidden = FolderItem.ShowHiddenFiles;

            this.comboFolders.Add(FolderBrowserController.CreateFileListItem(desktop, null));

            System.Diagnostics.Debug.Assert(this.comboFolders.Count == 1);

            FileListItem root = this.comboFolders[0];

            foreach (
                FolderItem item in FileManager.GetFolderItems(desktop, FolderQueryMode.SmallIcons)
            )
            {
                if ((!item.IsFileSystemNode) || (!item.IsFolder))
                {
                    continue;
                }

                if ((!includeHidden) && (item.IsDrive == false) && (item.IsHidden))
                {
                    continue;
                }

                FileListItem parent = FolderBrowserController.CreateFileListItem(item, root);

                this.comboFolders.Add(parent);

                if (item.DisplayName == computer.DisplayName)
                {
                    foreach (
                        FolderItem subItem in FileManager.GetFolderItems(
                            item,
                            FolderQueryMode.SmallIcons
                        )
                    )
                    {
                        if ((!subItem.IsFileSystemNode) || (!subItem.IsFolder))
                        {
                            continue;
                        }

                        if ((!includeHidden) && (subItem.IsDrive == false) && (subItem.IsHidden))
                        {
                            continue;
                        }

                        this.comboFolders.Add(
                            FolderBrowserController.CreateFileListItem(subItem, parent)
                        );
                    }
                }
            }

            //	Recursively add all parents.

            FolderItem currentFolder = this.navigationController.ActiveDirectory;
            int depth = 0;

            while (!currentFolder.IsEmpty)
            {
                this.comboFolders.Add(
                    FolderBrowserController.CreateFileListItem(currentFolder, null)
                );
                currentFolder = FileManager.GetParentFolderItem(
                    currentFolder,
                    FolderQueryMode.SmallIcons
                );
                depth++;
            }

            //	Link the parents together :

            int count = this.comboFolders.Count;

            for (int i = count - depth; i < count - 1; i++)
            {
                this.comboFolders[i].Parent = this.comboFolders[i + 1];
            }

            this.comboFolders.Sort();

            Types.Collection.RemoveDuplicatesInSortedList(this.comboFolders);

            this.browser.Items.Clear();

            foreach (FileListItem folder in this.comboFolders)
            {
                FolderItemIcon folderImage = folder.GetSmallIcon();
                string folderName = folder.ShortFileName;

                string text;

                text = FolderBrowserController.CreateIconAndLabel(folderImage, folderName);
                text = string.Concat(new string(' ', 3 * folder.Depth), text);

                this.browser.Items.Add(text);
            }
        }

        private static string CreateIconAndLabel(FolderItemIcon folderIcon, string folderName)
        {
            if ((folderIcon != null) && (!string.IsNullOrEmpty(folderIcon.ImageName)))
            {
                return string.Concat(
                    @"<img src=""",
                    folderIcon.ImageName,
                    @"""/> ",
                    TextLayout.ConvertToTaggedText(folderName)
                );
            }
            else
            {
                return TextLayout.ConvertToTaggedText(folderName);
            }
        }

        private static FileListItem CreateFileListItem(FolderItem folderItem, FileListItem parent)
        {
            FileListItem item = new FileListItem(folderItem);

            item.Parent = parent;
            item.SortAccordingToLevel = true;

            return item;
        }

        private void HandleBrowserComboClosed(object sender)
        {
            if (this.browser.SelectedItemIndex != -1)
            {
                this.navigationController.ActiveDirectory = this.comboFolders[
                    this.browser.SelectedItemIndex
                ].FolderItem;
            }
        }

        private FileNavigationController navigationController;
        private TextFieldCombo browser;
        private List<FileListItem> comboFolders;
    }
}
