//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs.Controllers
{
	public class FileNavigationController
	{
		public FileNavigationController(CommandContext context, CommandDispatcher dispatcher)
		{
			this.context = context;
			this.dispatcher = dispatcher;
			this.visitedFolderItems = new List<FolderItem> ();
			this.visitedIndex = -1;

			this.RegisterCommands ();
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


		public Widget NavigationWidget
		{
			get
			{
				if (this.navigator == null)
				{
					this.CreateUserInterface ();
				}

				return this.navigator;
			}
		}

		private void CreateUserInterface()
		{
			this.navigator = new Widget ();
			this.navigator.PreferredWidth = 22+22+12;
			this.navigator.TabNavigationMode = TabNavigationMode.None;

			this.navigateCombo = new GlyphButton (this.navigator);
			this.navigateCombo.ButtonStyle = ButtonStyle.ComboItem;
			this.navigateCombo.GlyphShape = GlyphShape.ArrowDown;
			this.navigateCombo.GlyphSize = new Drawing.Size (12, 20);
			this.navigateCombo.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
			this.navigateCombo.AutoFocus = false;
			this.navigateCombo.TabNavigationMode = TabNavigationMode.None;
			this.navigateCombo.Anchor = AnchorStyles.All;
			this.navigateCombo.Clicked += this.HandleNavigatorComboClicked;
			
			ToolTip.Default.SetToolTip (this.navigateCombo, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.VisitedMenu);

			IconButton buttonPrev = new IconButton (this.navigator);
			buttonPrev.AutoFocus = false;
			buttonPrev.TabNavigationMode = TabNavigationMode.None;
			buttonPrev.CommandObject = Res.Commands.Dialog.File.NavigatePrev;
			buttonPrev.Dock = DockStyle.Left;

			IconButton buttonNext = new IconButton (this.navigator);
			buttonNext.AutoFocus = false;
			buttonNext.TabNavigationMode = TabNavigationMode.None;
			buttonNext.CommandObject = Res.Commands.Dialog.File.NavigateNext;
			buttonNext.Dock = DockStyle.Left;
			
		}

		private void HandleNavigatorComboClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;

			if (button == null)
			{
				return;
			}

			VMenu menu = this.CreateHistoryMenu ();
			
			menu.Host = button.Window;
			TextFieldCombo.AdjustComboSize (button, menu, false);
			menu.ShowAsComboList (button, Drawing.Point.Zero, button);
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
				item.Pressed += this.HandleHistoryMenuItemPressed;
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

		private void RegisterCommands()
		{
			this.dispatcher.Register (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NavigatePrev, this.NavigatePrev);
			this.dispatcher.Register (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NavigateNext, this.NavigateNext);
			this.dispatcher.Register (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.ParentFolder, this.NavigateParent);
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
			FolderItem oldItem = this.activeDirectory;
			string     oldPath = oldItem.FullPath;
			
			if (this.activeDirectory != folder)
			{
				this.activeDirectory = folder;
				this.activeSmallIcon = null;
			}

			FolderItem newItem = this.activeDirectory;
			string     newPath = newItem.FullPath;

			if (oldItem != newItem)
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
		private CommandDispatcher				dispatcher;

		private FolderItem						activeDirectory;
		private FolderItemIcon					activeSmallIcon;

		private List<FolderItem>				visitedFolderItems;
		private int								visitedIndex;

		private Widget							navigator;
		private GlyphButton						navigateCombo;
	}
}
