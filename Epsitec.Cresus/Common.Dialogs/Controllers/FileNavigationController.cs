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
			this.visitedFolderItems = new List<FolderItem> ();
			this.visitedIndex = -1;
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
			if (this.visitedIndex > 0)
			{
				this.SetActiveDirectory (this.visitedFolderItems[--this.visitedIndex], RecordInHistory.No);
			}
		}

		public void NavigateNext()
		{
			if (this.visitedIndex < this.visitedFolderItems.Count-1)
			{
				this.SetActiveDirectory (this.visitedFolderItems[++this.visitedIndex], RecordInHistory.No);
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


		public VMenu CreateHistoryMenu()
		{
			//	Crée le menu pour choisir un dossier visité.
			VMenu menu = new VMenu ();

			int max   = 10;  // +/-, donc 20 lignes au maximum
			int end   = System.Math.Min (this.visitedIndex+max, this.visitedFolderItems.Count-1);
			int start = System.Math.Max (end-max*2, 0);

			if (start > 0)  // commence après le début ?
			{
				menu.Items.Add (this.CreateHistoryMenuItem (0));  // met "1: dossier"

				if (start > 1)
				{
					menu.Items.Add (new MenuSeparator ());  // met séparateur "------"
				}
			}

			for (int i = start; i <= end; i++)
			{
				if (i-1 == this.visitedIndex)
				{
					menu.Items.Add (new MenuSeparator ());  // met séparateur "------"
				}

				menu.Items.Add (this.CreateHistoryMenuItem (i));  // met "n: dossier"
			}

			if (end < this.visitedFolderItems.Count-1)  // fini avant la fin ?
			{
				if (end < this.visitedFolderItems.Count-2)
				{
					menu.Items.Add (new MenuSeparator ());  // met séparateur "------"
				}

				menu.Items.Add (this.CreateHistoryMenuItem (this.visitedFolderItems.Count-1));  // met "n: dossier"
			}

			menu.AdjustSize ();
			
			return menu;
		}

		#region History Menu related methods

		private MenuItem CreateHistoryMenuItem(int index)
		{
			//	Crée une case du menu pour choisir un dossier visité.
			if (index == -1)
			{
				return new MenuItem ("ChangeVisitedDirectory", "", "...", null);
			}
			else
			{
				FolderItem folder = this.visitedFolderItems[index];

				bool isCurrent = (index == this.visitedIndex);
				bool isNext    = (index >  this.visitedIndex);

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
				item.Pressed += new MessageEventHandler (this.HandleHistoryMenuItemPressed);
				ToolTip.Default.SetToolTip (item, tooltip);

				return item;
			}
		}

		private void HandleHistoryMenuItemPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;

			int index;

			if (int.TryParse (item.Name, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out index))
			{
				this.visitedIndex = index;
				this.SetActiveDirectory (this.visitedFolderItems[this.visitedIndex], RecordInHistory.No);
			}
		}

		#endregion

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

			if (this.visitedFolderItems.Count > 0)
			{
				FolderItem current = this.visitedFolderItems[this.visitedIndex];

				if (current == folder)
				{
					return;
				}

				while (this.visitedIndex < this.visitedFolderItems.Count-1)
				{
					this.visitedFolderItems.RemoveAt (this.visitedFolderItems.Count-1);
				}
			}

			this.visitedFolderItems.Add (folder);
			this.visitedIndex = this.visitedFolderItems.Count-1;

			this.UpdateCommandStates ();
		}

		private void UpdateCommandStates()
		{
			FolderItem parent = FileManager.GetParentFolderItem (this.ActiveDirectory, FolderQueryMode.NoIcons);
			
			this.context.GetCommandState (Res.Commands.Dialog.File.NavigatePrev).Enable = (this.visitedIndex > 0);
			this.context.GetCommandState (Res.Commands.Dialog.File.NavigateNext).Enable = (this.visitedIndex < this.visitedFolderItems.Count-1);
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

		private CommandContext					context;

		private FolderItem						activeDirectory;
		private FolderItemIcon					activeSmallIcon;

		private List<FolderItem>				visitedFolderItems;
		private int								visitedIndex;
	}
}
