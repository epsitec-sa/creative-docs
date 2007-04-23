//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs.Controllers
{
	public class FileNavigationController
	{
		public FileNavigationController(CommandContext context)
		{
			this.context = context;
			this.directoriesVisited = new List<FolderItem> ();
			this.directoriesVisitedIndex = -1;
		}

		
		public FolderItem ActiveDirectory
		{
			get
			{
				return this.activeDirectory;
			}
			set
			{
				this.SetActiveDirectory (value, RecordInHistory.Yes);
			}
		}

		public FolderItemIcon ActiveSmallIcon
		{
			get
			{
				if (this.activeSmallIcon == null)
				{
					this.activeSmallIcon = FileManager.GetFolderItemIcon (this.ActiveDirectory, FolderQueryMode.SmallIcons);
				}

				return this.activeSmallIcon;
			}
		}

		public string ActiveDirectoryPath
		{
			get
			{
				return this.ActiveDirectory.FullPath;
			}
		}

		public string ActiveDirectoryDisplayName
		{
			get
			{
				return this.ActiveDirectory.DisplayName;
			}
		}



		public void NavigatePrev()
		{
			if (this.directoriesVisitedIndex > 0)
			{
				this.SetActiveDirectory (this.directoriesVisited[--this.directoriesVisitedIndex], RecordInHistory.No);
			}
		}

		public void NavigateNext()
		{
			if (this.directoriesVisitedIndex < this.directoriesVisited.Count-1)
			{
				this.SetActiveDirectory (this.directoriesVisited[++this.directoriesVisitedIndex], RecordInHistory.No);
			}
		}

		public void NavigateParent()
		{
			FolderItem parent = FileManager.GetParentFolderItem (this.ActiveDirectory, FolderQueryMode.NoIcons);
			
			if (parent.IsEmpty)
			{
				return;
			}

			this.ActiveDirectory = parent;
		}


		public VMenu CreateVisitedMenu()
		{
			//	Crée le menu pour choisir un dossier visité.
			VMenu menu = new VMenu ();

			int max = 10;  // +/-, donc 20 lignes au maximum
			int end   = System.Math.Min (this.directoriesVisitedIndex+max, this.directoriesVisited.Count-1);
			int start = System.Math.Max (end-max*2, 0);

			if (start > 0)  // commence après le début ?
			{
				menu.Items.Add (this.CreateVisitedMenuItem (0));  // met "1: dossier"

				if (start > 1)
				{
					menu.Items.Add (new MenuSeparator ());  // met séparateur "------"
				}
			}

			for (int i=start; i<=end; i++)
			{
				if (i-1 == this.directoriesVisitedIndex)
				{
					menu.Items.Add (new MenuSeparator ());  // met séparateur "------"
				}

				menu.Items.Add (this.CreateVisitedMenuItem (i));  // met "n: dossier"
			}

			if (end < this.directoriesVisited.Count-1)  // fini avant la fin ?
			{
				if (end < this.directoriesVisited.Count-2)
				{
					menu.Items.Add (new MenuSeparator ());  // met séparateur "------"
				}

				menu.Items.Add (this.CreateVisitedMenuItem (this.directoriesVisited.Count-1));  // met "n: dossier"
			}

			menu.AdjustSize ();
			return menu;
		}

		private MenuItem CreateVisitedMenuItem(int index)
		{
			//	Crée une case du menu pour choisir un dossier visité.
			if (index == -1)
			{
				return new MenuItem ("ChangeVisitedDirectory", "", "...", null);
			}
			else
			{
				FolderItem folder = this.directoriesVisited[index];

				bool isCurrent = (index == this.directoriesVisitedIndex);
				bool isNext    = (index >  this.directoriesVisitedIndex);

				string icon = "";
				if (!isNext)
				{
					icon = isCurrent ? "manifest:Epsitec.Common.Widgets.Images.ActiveCurrent.icon" : "manifest:Epsitec.Common.Widgets.Images.ActiveNo.icon";
				}

				string text = TextLayout.ConvertToTaggedText (folder.DisplayName);
				if (isNext)
				{
					text = string.Concat ("<i>", text, "</i>");
				}
				text = string.Format ("{0}: {1}", (index+1).ToString (), text);

				string tooltip = TextLayout.ConvertToTaggedText (folder.FullPath);

				string name = index.ToString (System.Globalization.CultureInfo.InvariantCulture);

				MenuItem item = new MenuItem ("ChangeVisitedDirectory", icon, text, null, name);
				item.Pressed += new MessageEventHandler (this.HandleVisitedMenuPressed);
				ToolTip.Default.SetToolTip (item, tooltip);

				return item;
			}
		}

		private void HandleVisitedMenuPressed(object sender, MessageEventArgs e)
		{
			//	Une case du menu pour choisir un dossier visité a été actionnée.
			MenuItem item = sender as MenuItem;
			this.directoriesVisitedIndex = System.Int32.Parse (item.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetActiveDirectory (this.directoriesVisited[this.directoriesVisitedIndex], RecordInHistory.No);
		}

		
		protected virtual void OnActiveDirectoryChanged(string oldPath, string newPath)
		{
			if (this.ActiveDirectoryChanged != null)
			{
				this.ActiveDirectoryChanged (this, new DependencyPropertyChangedEventArgs ("ActiveDirectory", oldPath, newPath));
			}
		}

		protected void AddToVisited(FolderItem folder)
		{
			if (folder.IsEmpty)
			{
				return;
			}

			if (this.directoriesVisited.Count > 0)
			{
				FolderItem current = this.directoriesVisited[this.directoriesVisitedIndex];

				if (current == folder)
				{
					return;
				}

				while (this.directoriesVisitedIndex < this.directoriesVisited.Count-1)
				{
					this.directoriesVisited.RemoveAt (this.directoriesVisited.Count-1);
				}
			}

			this.directoriesVisited.Add (folder);
			this.directoriesVisitedIndex = this.directoriesVisited.Count-1;

			this.UpdateCommandStates ();
		}

		private void UpdateCommandStates()
		{
			FolderItem parent = FileManager.GetParentFolderItem (this.ActiveDirectory, FolderQueryMode.NoIcons);
			
			this.context.GetCommandState (Res.Commands.Dialog.File.NavigatePrev).Enable = (this.directoriesVisitedIndex > 0);
			this.context.GetCommandState (Res.Commands.Dialog.File.NavigateNext).Enable = (this.directoriesVisitedIndex < this.directoriesVisited.Count-1);
			this.context.GetCommandState (Res.Commands.Dialog.File.ParentFolder).Enable = parent.IsValid;
		}

		private void SetActiveDirectory(FolderItem folder, RecordInHistory recordMode)
		{
			string oldPath = this.activeDirectory.FullPath;

			if (this.activeDirectory != folder)
			{
				this.activeDirectory = folder;
				this.activeSmallIcon = null;
			}

			string newPath = this.activeDirectory.FullPath;

			if (oldPath != newPath)
			{
				if (recordMode == RecordInHistory.Yes)
				{
					this.AddToVisited (this.activeDirectory);
				}

				this.OnActiveDirectoryChanged (oldPath, newPath);
				this.UpdateCommandStates ();
			}
		}

		private enum RecordInHistory
		{
			No,
			Yes,
		}

		public event EventHandler<DependencyPropertyChangedEventArgs> ActiveDirectoryChanged;

		private CommandContext context;

		private FolderItem activeDirectory;
		private FolderItemIcon activeSmallIcon;

		private int disableHistoryCount;
		
		private List<FolderItem> directoriesVisited;
		private int directoriesVisitedIndex;
	}
}
