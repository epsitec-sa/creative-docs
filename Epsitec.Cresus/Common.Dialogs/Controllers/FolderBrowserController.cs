//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX & Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs.Controllers
{
	public class FolderBrowserController
	{
		public FolderBrowserController(FileNavigationController navigationController)
		{
			this.navigationController = navigationController;
			this.navigationController.ActiveDirectoryChanged += this.HandleNavigationControllerActiveDirectoryChanged;
		}


		public TextFieldCombo				BrowserWidget
		{
			get
			{
				if (this.browser == null)
				{
					this.CreateUserInterface ();
				}

				return this.browser;
			}
		}

		private void CreateUserInterface()
		{
			this.browser = new TextFieldCombo ();
			this.browser.IsReadOnly = true;
			this.browser.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleBrowserComboOpening);
			this.browser.ComboClosed += new EventHandler (this.HandleBrowserComboClosed);
			this.browser.AddEventHandler (Widget.TextProperty, this.HandleFieldPathTextChanged);

			this.SyncBrowserText ();
		}

		private void HandleNavigationControllerActiveDirectoryChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.browser != null)
			{
				this.SyncBrowserText ();
			}
		}

		private void SyncBrowserText()
		{
			FolderItemIcon folderIcon = this.navigationController.ActiveSmallIcon;
			string folderName = this.navigationController.ActiveDirectoryDisplayName;

			this.browser.Text = FolderBrowserController.CreateIconAndLabel (folderIcon, folderName);
		}

		private void HandleBrowserComboOpening(object sender, CancelEventArgs e)
		{
			this.comboFolders = new List<FileListItem> ();

			//	Add all desktop and computer nodes
			
			FolderItem desktop  = FileManager.GetFolderItem (FolderId.VirtualDesktop, FolderQueryMode.SmallIcons);
			FolderItem computer = FileManager.GetFolderItem (FolderId.VirtualMyComputer, FolderQueryMode.SmallIcons);
			
			bool includeHidden = FolderItem.ShowHiddenFiles;

			this.comboFolders.Add (FolderBrowserController.CreateFileListItem (desktop, null));

			System.Diagnostics.Debug.Assert (this.comboFolders.Count == 1);
			
			FileListItem root = this.comboFolders[0];

			foreach (FolderItem item in FileManager.GetFolderItems (desktop, FolderQueryMode.SmallIcons))
			{
				if ((!item.IsFileSystemNode) ||
					(!item.IsFolder))
				{
					continue;
				}

				if ((!includeHidden) &&
					(item.IsHidden))
				{
					continue;
				}

				FileListItem parent = FolderBrowserController.CreateFileListItem (item, root);

				this.comboFolders.Add (parent);
				
				if (item.DisplayName == computer.DisplayName)
				{
					foreach (FolderItem subItem in FileManager.GetFolderItems (item, FolderQueryMode.SmallIcons))
					{
						if ((!subItem.IsFileSystemNode) ||
							(!subItem.IsFolder))
						{
							continue;
						}

						if ((!includeHidden) &&
							(subItem.IsHidden))
						{
							continue;
						}

						this.comboFolders.Add (FolderBrowserController.CreateFileListItem (subItem, parent));
					}
				}
			}

			//	Recursively add all parents.
			
			FolderItem currentFolder = this.navigationController.ActiveDirectory;
			int depth = 0;
			
			while (!currentFolder.IsEmpty)
			{
				this.comboFolders.Add (FolderBrowserController.CreateFileListItem (currentFolder, null));
				currentFolder = FileManager.GetParentFolderItem (currentFolder, FolderQueryMode.SmallIcons);
				depth++;
			}

			//	Link the parents together :
			
			int count = this.comboFolders.Count;
			
			for (int i = count-depth; i < count-1; i++)
			{
				this.comboFolders[i].Parent = this.comboFolders[i+1];
			}

			this.comboFolders.Sort ();

			Types.Collection.RemoveDuplicatesInSortedList (this.comboFolders);
			
			this.browser.Items.Clear ();
			
			foreach (FileListItem folder in this.comboFolders)
			{
				FolderItemIcon folderImage = folder.GetSmallIcon ();
				string         folderName  = folder.ShortFileName;
				
				string text = FolderBrowserController.CreateIconAndLabel (folderImage, folderName);
				
				text = FolderBrowserController.AddStringIndent (text, folder.Depth);

				this.browser.Items.Add (text);
			}

			this.comboSelected = -1;
		}

		private static string CreateIconAndLabel(FolderItemIcon folderIcon, string folderName)
		{
			if ((folderIcon != null) &&
				(!string.IsNullOrEmpty (folderIcon.ImageName)))
			{
				return string.Concat (@"<img src=""", folderIcon.ImageName, @"""/> ", TextLayout.ConvertToTaggedText (folderName));
			}
			else
			{
				return TextLayout.ConvertToTaggedText (folderName);
			}
		}

		private static FileListItem CreateFileListItem(FolderItem folderItem, FileListItem parent)
		{
			FileListItem item = new FileListItem (folderItem);

			item.Parent = parent;
			item.SortAccordingToLevel = true;

			return item;
		}

		private void HandleBrowserComboClosed(object sender)
		{
			//	Le menu pour le chemin d'accès a été fermé.
			if (this.comboSelected != -1)
			{
				this.navigationController.ActiveDirectory = this.comboFolders[this.comboSelected].FolderItem;
			}
		}

		private void HandleFieldPathTextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Le texte pour le chemin d'accès a changé.
			if (this.browser == null)
			{
				return;
			}

			string oldValue = (e.OldValue as string) ?? "";
			string newValue = (e.NewValue as string) ?? "";

			oldValue = oldValue.Trim ();
			newValue = newValue.Trim ();

			if (oldValue == newValue)
			{
				return;
			}

			this.comboSelected = this.browser.Items.FindIndex (
				delegate (object obj)
				{
					string path = (string) obj;
					if (path.Trim () == newValue)
					{
						return true;
					}
					else
					{
						return false;
					}
				});

			this.browser.Text = FolderBrowserController.RemoveStartingIndent (this.browser.Text);
		}

		private static string AddStringIndent(string text, int level)
		{
			//	Ajoute des niveaux d'indentation au début d'un texte.
			while (level > 0)
			{
				text = "   "+text;
				level--;
			}
			return text;
		}

		private static string RemoveStartingIndent(string text)
		{
			//	Supprime tous les niveaux d'indentation au début d'un texte.
			while (text.StartsWith ("   "))
			{
				text = text.Substring (3);
			}

			return text;
		}


		private FileNavigationController navigationController;

		private TextFieldCombo				browser;
		
		private List<FileListItem>			comboFolders;
		private int							comboSelected;
	}
}
