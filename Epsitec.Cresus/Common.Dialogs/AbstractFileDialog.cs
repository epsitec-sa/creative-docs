//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX & Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.IO;
using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>AbstractFileDialog</c> is used as a base class for every file
	/// dialog (open, save, etc.) which displays a list of files.
	/// </summary>
	public abstract class AbstractFileDialog
	{
		public AbstractFileDialog()
		{
			this.directoriesVisited = new List<FolderItem> ();
			this.directoriesVisitedIndex = -1;
		}


		public void ChangeTitle(string title)
		{
			//	Modifie le titre de la fenêtre du dialogue.
			if (this.window == null)
			{
				this.title = title;
			}
			else
			{
				this.window.Text = title;
			}
		}

		public DialogResult Result
		{
			//	Indique si le dialogue a été fermé avec 'ouvrir' ou 'annuler'.
			get
			{
				if (this.selectedFileName == null)
				{
					return DialogResult.Cancel;
				}
				else
				{
					return DialogResult.Accept;
				}
			}
		}

		public string InitialDirectory
		{
			//	Dossier initial.
			get
			{
				return this.initialDirectory.FullPath;
			}
			set
			{
				this.isRedirected = false;
				FolderItem folder;

				if (value == "")  // poste de travail ?
				{
					folder = FileManager.GetFolderItem (FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
				}
				else
				{
					if (this.FileDialogType == FileDialogType.Save)
					{
						string oldPath = value;
						string newPath = this.RedirectPath (oldPath);
						this.isRedirected = oldPath != newPath;
						value = newPath;
					}

					folder = FileManager.GetFolderItem (value, FolderQueryMode.NoIcons);

					if (folder.IsEmpty)
					{
						folder = FileManager.GetFolderItem (FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
					}
				}

				this.SetInitialFolder (folder, true);
			}
		}

		public string InitialFileName
		{
			//	Nom de fichier initial.
			get
			{
				return this.initialFileName;
			}
			set
			{
				if (this.FileDialogType == FileDialogType.Save)
				{
					value = this.RedirectPath (value);
				}

				if (this.initialFileName != value)
				{
					this.initialFileName = value;
					this.UpdateInitialFileName ();
				}
			}
		}

		public string FileExtension
		{
			//	Extension unique, par exemple ".crdoc".
			get
			{
				return this.fileExtension;
			}
			set
			{
				if (value.StartsWith ("."))
				{
					this.fileExtension = value.ToLowerInvariant ();
					this.fileFilterPattern = string.Concat ("*", value);

					if (this.fieldExtension != null)
					{
						this.fieldExtension.Text = this.fileExtension;
					}
				}
				else
				{
					throw new System.FormatException ("Incorrect file extension: " + value);
				}
			}
		}

		public string FileFilterPattern
		{
			//	Liste des extensions, par exemple "*.tif|*.jpg".
			//	Il faut mettre en premier les extensions qu'on souhaite voir.
			get
			{
				return this.fileFilterPattern;
			}
			set
			{
				this.fileFilterPattern = value;
			}
		}

		public bool IsDirectoryRedirected
		{
			//	Indique si le dossier passé avec InitialDirectory a dû être
			//	redirigé de 'Exemples originaux' vers 'Mes exemples'.
			get
			{
				return this.isRedirected;
			}
		}

		public bool EnableMultipleSelection
		{
			get
			{
				return this.enableMultipleSelection;
			}
			set
			{
				this.enableMultipleSelection = value;
			}
		}

		public string FileName
		{
			//	Retourne le nom du fichier à ouvrir, ou null si l'utilisateur a choisi
			//	le bouton 'annuler'.
			get
			{
				return this.selectedFileName;
			}
		}

		public string[] FileNames
		{
			//	Retourne les noms des fichiers à ouvrir, ou null si l'utilisateur a choisi
			//	le bouton 'annuler'.
			get
			{
				if (this.selectedFileName == null)  // annuler ?
				{
					return null;
				}

				if (this.selectedFileNames == null)
				{
					this.selectedFileNames = new string[1];
					this.selectedFileNames[0] = this.selectedFileName;
				}

				return this.selectedFileNames;
			}
		}


		public DialogResult ShowDialog()
		{
			if (this.window == null)
			{
				this.CreateWindow ();
			}

			this.UpdateAll (this.FileDialogType == FileDialogType.New ? 0 : -1, true);
			this.window.ShowDialog ();

			return this.Result;
		}


		public virtual void PersistWindowBounds()
		{
		}

		protected virtual string RedirectPath(string path)
		{
			return path;
		}

		protected abstract IFavoritesSettings FavoritesSettings
		{
			get;
		}

		protected abstract FileDialogType FileDialogType
		{
			get;
		}

		protected virtual string FileTypeLabel
		{
			get
			{
				return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Label;
			}
		}

		protected virtual string ActionButtonName
		{
			get
			{
				switch (this.FileDialogType)
				{
					case FileDialogType.New:
						return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.New;

					case FileDialogType.Save:
						return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.Save;

					default:
						return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.Open;
				}
			}
		}

		protected abstract void CreateWindow();


		private void SetInitialFolder(FolderItem folder, bool updateVisited)
		{
			//	Change le dossier courant.
			if (folder.IsEmpty)
			{
				this.initialDirectory = FileManager.GetFolderItem (FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
			}
			else
			{
				this.initialDirectory = folder;
			}

			this.initialSmallIcon = FileManager.GetFolderItemIcon (this.initialDirectory, FolderQueryMode.SmallIcons);

			if (updateVisited)
			{
				this.AddToVisitedDirectories (this.initialDirectory);
			}

			this.UpdateInitialDirectory ();
			this.UpdateTable (-1);

			if (this.table != null)
			{
				this.UpdateFileList ();
			}
		}

		private void AddToVisitedDirectories(FolderItem folder)
		{
			if (folder.IsEmpty)
			{
				return;  // on n'insère jamais un folder vide
			}

			if (this.directoriesVisited.Count != 0)
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
		}


		protected void CreateUserInterface(string name, Size windowSize, string title, double cellHeight, Window owner)
		{
			//	Crée la fenêtre et tous les widgets pour peupler le dialogue.
			this.window = new Window ();
			this.window.MakeSecondaryWindow ();
			this.window.PreventAutoClose = true;
			this.window.Name = name;
			this.window.Text = (title == null) ? this.title : title;
			this.window.Owner = owner;
			this.window.Icon = owner == null ? null : this.window.Owner.Icon;

			this.SetWindowGeometry (windowSize.Width, windowSize.Height, true);

			this.window.WindowCloseClicked += new EventHandler (this.HandleWindowCloseClicked);

			this.timer = new Timer ();
			this.timer.TimeElapsed += this.HandleTimerTimeElapsed;
			this.timer.Delay = AbstractFileDialog.TimerRefreshRate;
			this.timer.AutoRepeat = AbstractFileDialog.TimerRefreshRate;

			this.CreateCommandDispatcher ();
			this.CreateResizer ();
			this.CreateAccess ();
			this.CreateToolbar ();
			this.CreateTable (cellHeight);
			this.CreateRename ();

			//	Dans l'ordre de bas en haut :
			this.CreateFooter ();
			this.CreateOptionsUserInterface ();
		}

		protected virtual void CreateOptionsUserInterface()
		{
		}

		protected virtual void UpdateOptions()
		{
		}

		protected virtual Rectangle GetPersistedWindowBounds(string name)
		{
			return Rectangle.Empty;
		}


		protected abstract Rectangle GetOwnerBounds();

		private void UpdateAll(int initialSelection, bool focusInFileName)
		{
			//	Mise à jour lorsque les widgets sont déjà créés, avant de montrer le dialogue.
			this.selectedFileName = null;
			this.selectedFileNames = null;
			this.UpdateFavorites ();
			this.UpdateTable (initialSelection);
			this.UpdateInitialDirectory ();
			this.UpdateInitialFileName ();
			this.UpdateButtons ();
			this.UpdateOptions ();

			if (focusInFileName)
			{
				this.fieldFileName.SelectAll ();
				this.fieldFileName.Focus ();  // focus pour frapper le nom du fichier à ouvrir
			}
			else
			{
				this.table.Focus ();  // focus dans la liste des fichiers
			}
		}

		private void SetWindowGeometry(double dx, double dy, bool resizable)
		{
			this.window.ClientSize = new Size (dx, dy);

			dx = this.window.WindowSize.Width;
			dy = this.window.WindowSize.Height;  // taille avec le cadre

			Rectangle bounds = this.GetPersistedWindowBounds (this.window.Name);

			if (bounds.IsValid)
			{
				if (resizable)
				{
					this.window.ClientSize = bounds.Size;
					dx = this.window.WindowSize.Width;
					dy = this.window.WindowSize.Height;  // taille avec le cadre
				}

				bounds.Size = new Size (dx, dy);
				bounds = ScreenInfo.FitIntoWorkingArea (bounds);
				this.window.WindowBounds = bounds;
			}
			else
			{
				Rectangle cb = this.GetOwnerBounds ();
				this.window.WindowBounds = new Rectangle (cb.Center.X-dx/2, cb.Center.Y-dy/2, dx, dy);
			}

			this.window.Root.MinSize = new Size (400, 200);
			this.window.Root.Padding = new Margins (8, 8, 8, 8);
		}

		private void CreateCommandDispatcher()
		{
			this.dispatcher = new CommandDispatcher ();
			this.context = new CommandContext ();

			this.prevState            = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NavigatePrev, this.NavigatePrev);
			this.nextState            = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NavigateNext, this.NavigateNext);
			this.parentState          = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.ParentFolder, this.ParentDirectory);
			this.newState             = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NewFolder, this.NewDirectory);
			this.renameState          = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Rename, this.RenameStarting);
			this.deleteState          = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Delete, this.FileDelete);
			this.refreshState         = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Refresh, this.Refresh);
			this.viewDispositionState = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.ViewDisposition, this.ToggleViewDisposition);
			this.viewSizeState        = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.ViewSize, this.ToggleViewSize);
			this.favoritesAddState    = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Add, this.AddFavorite);
			this.favoritesRemoveState = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Remove, this.FavoritesRemove);
			this.favoritesUpState     = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Up, this.FavoritesMoveUp);
			this.favoritesDownState   = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Down, this.FavoritesMoveDown);
			this.favoritesBigState    = this.RegisterCommand (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.ToggleSize, this.FavoritesToggleSize);

			CommandDispatcher.SetDispatcher (this.window, this.dispatcher);
			CommandContext.SetContext (this.window, this.context);
		}

		private CommandState RegisterCommand(Command command, SimpleCallback handler)
		{
			this.dispatcher.Register (command, handler);
			return this.context.GetCommandState (command);
		}


		private void CreateResizer()
		{
			//	Crée l'icône en bas à droite pour signaler que la fenêtre est redimensionnable.
			ResizeKnob resize = new ResizeKnob (this.window.Root);
			resize.Anchor = AnchorStyles.BottomRight;
			resize.Margins = new Margins (0, -8, 0, -8);
			ToolTip.Default.SetToolTip (resize, Epsitec.Common.Dialogs.Res.Strings.Dialog.Tooltip.Resize);
		}

		private void CreateTable(double cellHeight)
		{
			//	Crée la table principale contenant la liste des fichiers et dossiers.
			Widget group = new Widget (this.window.Root);
			group.Dock = DockStyle.Fill;
			group.TabIndex = 3;
			group.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.favorites = new Scrollable (group);
			this.favorites.PreferredWidth = AbstractFileDialog.LeftColumnWidth-5;
			this.favorites.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.favorites.VerticalScrollerMode = ScrollableScrollerMode.Auto;
			this.favorites.Panel.IsAutoFitting = true;
			this.favorites.PaintForegroundFrame = true;
			this.favorites.Margins = new Margins (0, 5, 0, 0);
			this.favorites.Dock = DockStyle.Left;

			this.CreateCollectionView ();

			this.table = new Epsitec.Common.UI.ItemTable (group);
			this.table.Dock = DockStyle.Fill;
			this.table.TabIndex = 2;
			this.table.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.table.SourceType = FileListItem.GetStructuredType ();
			this.table.Items = this.filesCollectionView;
			this.table.AutoFocus = true;
			this.table.VerticalScrollMode = ItemTableScrollMode.ItemBased;
			this.table.ItemPanel.ItemViewDefaultExpanded = true;
			
			this.SetItemViewDisposition (cellHeight, ItemPanelLayout.RowsOfTiles);
			
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("icon", 72));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("name", 195));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("info", 120, FileListItem.GetDescriptionPropertyComparer ()));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("date", 96));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("size", 54));

			this.table.Columns[0].SortDirection = ListSortDirection.None;
//-			this.table.Columns[1].SortDirection = ListSortDirection.Ascending;
//-			this.table.Columns[2].SortDirection = ListSortDirection.Ascending;
			
			this.table.Columns[4].ContentAlignment = ContentAlignment.MiddleRight;

			this.table.ColumnHeader.SetColumnSort (1, Epsitec.Common.Types.ListSortDirection.Ascending);
			this.table.ColumnHeader.SetColumnSort (2, Epsitec.Common.Types.ListSortDirection.Ascending);
			this.table.ItemPanel.ItemSelectionMode = this.enableMultipleSelection ? ItemPanelSelectionMode.Multiple : ItemPanelSelectionMode.ZeroOrOne;
			this.table.ItemPanel.SelectionBehavior = ItemPanelSelectionBehavior.ManualOne;

			this.table.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
			this.table.ItemPanel.DoubleClicked += this.HandleTableDoubleClicked;
			this.table.ItemPanel.SelectionChanged += this.HandleFilesItemTableSelectionChanged;

			this.slider.Value = (decimal) Widget.DefaultFontHeight+4;

			this.UpdateFileList ();
		}

		private void SetItemViewDisposition(double size)
		{
			this.SetItemViewDisposition (size, this.GetItemTableLayout ());
		}
		
		private void SetItemViewDisposition(ItemPanelLayout layout)
		{
			this.SetItemViewDisposition (this.itemViewSize, layout);
		}
		
		private void SetItemViewDisposition(double size, ItemPanelLayout layout)
		{
			this.SetItemTableLayout (layout);

			this.itemViewSize = size;

			switch (layout)
			{
				case ItemPanelLayout.RowsOfTiles:
				case ItemPanelLayout.ColumnsOfTiles:
					size += 40;  // les tailles 20..100 passent à 60..140
					break;
			}

			this.table.ItemPanel.ItemViewDefaultSize = new Size (size, size);
		}

		private ItemPanelLayout GetItemTableLayout()
		{
			if (this.filesCollectionView.GroupDescriptions.Count > 0)
			{
				return this.table.ItemPanel.LayoutGroups;
			}
			else
			{
				return this.table.ItemPanel.Layout;
			}
		}

		private void SetItemTableLayout(ItemPanelLayout layout)
		{
			if (this.filesCollectionView.GroupDescriptions.Count > 0)
			{
				this.table.ItemPanel.LayoutGroups = layout;
			}
			else
			{
				this.table.ItemPanel.Layout = layout;
			}
		}

		private void CreateCollectionView()
		{
			if (this.filesCollectionView == null)
			{
				this.files = new ObservableList<FileListItem> ();
				this.ClearFileList ();
				this.filesCollectionView = new CollectionView (this.files);
				this.filesCollectionView.CurrentChanged += this.HandleFilesCollectionViewCurrentChanged;
#if false
				this.filesCollectionView.GroupDescriptions.Add (new PropertyGroupDescription ("info")); 
#endif

				this.filesCollectionView.InvalidationHandler =
					delegate (Epsitec.Common.Types.CollectionView cv)
					{
						lock (this.exclusion)
						{
							if (this.timer.State != TimerState.Running)
							{
								Epsitec.Common.Widgets.Application.QueueAsyncCallback (this.AsyncRefreshCollectionView);
							}

							this.refreshRequested = true;
							this.timer.Start ();
						}
					};
			}
		}

		private void HandleTimerTimeElapsed(object sender)
		{
			bool stopped = false;

			lock (this.exclusion)
			{
				if (this.fileListJobs.Count == 0)
				{
					this.timer.Stop ();
					stopped = true;
				}
			}

			if (this.refreshRequested)
			{
				Epsitec.Common.Widgets.Application.QueueAsyncCallback (this.AsyncRefreshCollectionView);
			}

			this.OnTimerTicked ();

			if (stopped)
			{
				this.OnTimerStopped ();
			}
		}

		
		private void OnTimerTicked()
		{
			this.NextProgressIndicator();
		}

		private void OnTimerStopped()
		{
			this.StopProgressIndicator();
		}

		
		private void HandleFilesItemTableSelectionChanged(object sender)
		{
			this.UpdateAfterSelectionChanged ();
		}

		private void HandleFilesCollectionViewCurrentChanged(object sender)
		{
			this.UpdateAfterSelectionChanged ();
		}

		private void UpdateAfterSelectionChanged()
		{
			if (this.table == null)
			{
				return;
			}

			FileListItem item = this.GetCurrentFileListItem ();

			if ((item != null) &&
				(item.IsDataFile))
			{
				string name = this.GetFilename(item);

				IList<ItemView> selectedItemViews = this.table.ItemPanel.GetSelectedItemViews ();
				if (selectedItemViews.Count > 1)
				{
					System.Text.StringBuilder builder = new System.Text.StringBuilder();

					foreach (ItemView view in selectedItemViews)
					{
						FileListItem oneItem = view.Item as FileListItem;
						if (oneItem.IsDataFile)
						{
							builder.Append("\"");
							builder.Append(this.GetFilename(oneItem));
							builder.Append("\" ");
						}
					}

					name = builder.ToString();
				}

				this.fieldFileName.Text = TextLayout.ConvertToTaggedText (name);
				this.fieldFileName.SelectAll ();
			}

			this.UpdateButtons ();
		}

		private string GetFilename(FileListItem item)
		{
			string name = item.ShortFileName;

			if (this.fileExtension != null && name.EndsWith (this.fileExtension))
			{
				name = name.Substring (0, name.Length - this.fileExtension.Length);
			}

			return name;
		}

		private void CreateRename()
		{
			//	Crée le widget permettant de renommer un fichier/dossier.
			//	Normalement, ce widget est caché.
			this.fieldRename = new TextFieldEx (this.window.Root);
			this.fieldRename.Visibility = false;
			this.fieldRename.ButtonShowCondition = ShowCondition.Always;
			this.fieldRename.EditionAccepted += new EventHandler (this.HandleRenameAccepted);
			this.fieldRename.EditionRejected += new EventHandler (this.HandleRenameRejected);
			this.fieldRename.SwallowEscape = true;
			this.fieldRename.SwallowReturn = true;
			this.fieldRename.IsModal = true;
		}

		private void CreateAccess()
		{
			//	Crée la partie controlant le chemin d'accès.
			Widget group = new Widget (this.window.Root);
			group.PreferredHeight = 20;
			group.Margins = new Margins (0, 0, 0, 8);
			group.Dock = DockStyle.Top;
			group.TabIndex = 1;
			group.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.toolbarExtend = new GlyphButton (group);
			this.toolbarExtend.PreferredWidth = 16;
			this.toolbarExtend.ButtonStyle = ButtonStyle.Slider;
			this.toolbarExtend.AutoFocus = false;
			this.toolbarExtend.TabNavigationMode = TabNavigationMode.None;
			this.toolbarExtend.Dock = DockStyle.Left;
			this.toolbarExtend.Margins = new Margins (0, 0, 2, 2);
			this.toolbarExtend.Clicked += new MessageEventHandler (this.HandleToolbarExtendClicked);
			ToolTip.Default.SetToolTip (this.toolbarExtend, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.ExtendToolbar);

			StaticText label = new StaticText (group);
			label.Text = this.FileDialogType == FileDialogType.Save ? Epsitec.Common.Dialogs.Res.Strings.Dialog.File.LabelPath.Save : Epsitec.Common.Dialogs.Res.Strings.Dialog.File.LabelPath.Open;
			label.PreferredWidth = AbstractFileDialog.LeftColumnWidth-16-10;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Dock = DockStyle.Left;
			label.Margins = new Margins (0, 10, 0, 0);

			this.fieldPath = new TextFieldCombo (group);
			this.fieldPath.IsReadOnly = true;
			this.fieldPath.Dock = DockStyle.Fill;
			this.fieldPath.Margins = new Margins (0, 5, 0, 0);
			this.fieldPath.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleFieldPathComboOpening);
			this.fieldPath.ComboClosed += new EventHandler (this.HandleFieldPathComboClosed);
			this.fieldPath.AddEventHandler (Widget.TextProperty, this.HandleFieldPathTextChanged);
			this.fieldPath.TabIndex = 1;
			this.fieldPath.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Il faut créer ces boutons dans l'ordre 'de droite à gauche'.
			IconButton buttonViewSize = new IconButton (group);
			buttonViewSize.AutoFocus = false;
			buttonViewSize.TabNavigationMode = TabNavigationMode.None;
			buttonViewSize.CommandObject = this.viewSizeState.Command;
			buttonViewSize.Dock = DockStyle.Right;

			IconButton buttonViewDisposition = new IconButton (group);
			buttonViewDisposition.AutoFocus = false;
			buttonViewDisposition.TabNavigationMode = TabNavigationMode.None;
			buttonViewDisposition.CommandObject = this.viewDispositionState.Command;
			buttonViewDisposition.Margins = new Margins(10, 0, 0, 0);
			buttonViewDisposition.Dock = DockStyle.Right;

			IconButton buttonParent = new IconButton (group);
			buttonParent.AutoFocus = false;
			buttonParent.TabNavigationMode = TabNavigationMode.None;
			buttonParent.CommandObject = this.parentState.Command;
			buttonParent.Dock = DockStyle.Right;

			//	Groupe-combo composé des boutons "prev/next/v".
			Widget combo = new Widget(group);
			combo.PreferredWidth = 22+22+12;
			combo.TabNavigationMode = TabNavigationMode.None;
			combo.Dock = DockStyle.Right;
			combo.Margins = new Margins(0, 10, 0, 0);

			this.navigateCombo = new GlyphButton(combo);
			this.navigateCombo.ButtonStyle = ButtonStyle.ComboItem;
			this.navigateCombo.GlyphShape = GlyphShape.ArrowDown;
			this.navigateCombo.GlyphSize = new Size(12, 20);
			this.navigateCombo.ContentAlignment = ContentAlignment.MiddleRight;
			this.navigateCombo.AutoFocus = false;
			this.navigateCombo.TabNavigationMode = TabNavigationMode.None;
			this.navigateCombo.Anchor = AnchorStyles.All;
			this.navigateCombo.Clicked += new MessageEventHandler(this.HandleNavigateComboClicked);
			ToolTip.Default.SetToolTip(this.navigateCombo, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.VisitedMenu);

			IconButton buttonPrev = new IconButton(combo);
			buttonPrev.PreferredHeight = group.PreferredHeight;
			buttonPrev.AutoFocus = false;
			buttonPrev.TabNavigationMode = TabNavigationMode.None;
			buttonPrev.CommandObject = this.prevState.Command;
			buttonPrev.Dock = DockStyle.Left;

			IconButton buttonNext = new IconButton(combo);
			buttonNext.PreferredHeight = group.PreferredHeight;
			buttonNext.AutoFocus = false;
			buttonNext.TabNavigationMode = TabNavigationMode.None;
			buttonNext.CommandObject = this.nextState.Command;
			buttonNext.Dock = DockStyle.Left;
		}

		private void CreateToolbar()
		{
			//	Crée la grande toolbar.
			this.toolbar = new HToolBar (this.window.Root);
			this.toolbar.Margins = new Margins (0, 0, 0, -1);
			this.toolbar.Dock = DockStyle.Top;
			this.toolbar.TabNavigationMode = TabNavigationMode.None;
			this.toolbar.Visibility = false;

			IconButton buttonFavoritesAdd = new IconButton ();
			buttonFavoritesAdd.AutoFocus = false;
			buttonFavoritesAdd.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesAdd.CommandObject = this.favoritesAddState.Command;
			this.toolbar.Items.Add (buttonFavoritesAdd);

			IconButton buttonFavoritesRemove = new IconButton ();
			buttonFavoritesRemove.AutoFocus = false;
			buttonFavoritesRemove.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesRemove.CommandObject = this.favoritesRemoveState.Command;
			this.toolbar.Items.Add (buttonFavoritesRemove);

			this.toolbar.Items.Add (new IconSeparator ());

			IconButton buttonFavoritesUp = new IconButton ();
			buttonFavoritesUp.AutoFocus = false;
			buttonFavoritesUp.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesUp.CommandObject = this.favoritesUpState.Command;
			this.toolbar.Items.Add (buttonFavoritesUp);

			IconButton buttonFavoritesDown = new IconButton ();
			buttonFavoritesDown.AutoFocus = false;
			buttonFavoritesDown.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesDown.CommandObject = this.favoritesDownState.Command;
			this.toolbar.Items.Add (buttonFavoritesDown);

			this.toolbar.Items.Add (new IconSeparator ());

			IconButton buttonFavoritesBig = new IconButton ();
			buttonFavoritesBig.AutoFocus = false;
			buttonFavoritesBig.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesBig.CommandObject = this.favoritesBigState.Command;
			this.toolbar.Items.Add (buttonFavoritesBig);


			this.slider = new HSlider ();
			this.slider.AutoFocus = false;
			this.slider.TabNavigationMode = TabNavigationMode.None;
			this.slider.PreferredWidth = 110;
			this.slider.ShowMinMaxButtons = true;
			this.slider.Dock = DockStyle.Left;
			this.slider.Margins = new Margins (50, 0, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 100.0M;
			this.slider.SmallChange = 1.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += new EventHandler (this.HandleSliderChanged);
			ToolTip.Default.SetToolTip (this.slider, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.PreviewSize);
			this.toolbar.Items.Add (this.slider);

			//	Dans l'ordre de droite à gauche:
			IconButton buttonDelete = new IconButton ();
			buttonDelete.AutoFocus = false;
			buttonDelete.TabNavigationMode = TabNavigationMode.None;
			buttonDelete.CommandObject = this.deleteState.Command;
			buttonDelete.Dock = DockStyle.Right;
			this.toolbar.Items.Add (buttonDelete);

			IconButton buttonRename = new IconButton ();
			buttonRename.AutoFocus = false;
			buttonRename.TabNavigationMode = TabNavigationMode.None;
			buttonRename.CommandObject = this.renameState.Command;
			buttonRename.Dock = DockStyle.Right;
			this.toolbar.Items.Add (buttonRename);

			IconButton buttonNew = new IconButton ();
			buttonNew.AutoFocus = false;
			buttonNew.TabNavigationMode = TabNavigationMode.None;
			buttonNew.CommandObject = this.newState.Command;
			buttonNew.Dock = DockStyle.Right;
			this.toolbar.Items.Add (buttonNew);

			IconButton buttonRefresh = new IconButton ();
			buttonRefresh.AutoFocus = false;
			buttonRefresh.TabNavigationMode = TabNavigationMode.None;
			buttonRefresh.CommandObject = this.refreshState.Command;
			buttonRefresh.Dock = DockStyle.Right;
			buttonRefresh.Margins = new Margins(0, 10, 0, 0);
			this.toolbar.Items.Add (buttonRefresh);
		}

		private void CreateFooter()
		{
			//	Crée le pied du dialogue, avec les boutons 'ouvrir/enregistrer' et 'annuler'.
			Widget footer = new Widget (this.window.Root);
			footer.PreferredHeight = 22;
			footer.Margins = new Margins (0, 0, 8, 0);
			footer.Dock = DockStyle.Bottom;
			footer.TabIndex = 6;
			footer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.CreateFooterOptions (footer);

			this.filenameLabel = new StaticText (footer);
			this.filenameLabel.Text = this.FileTypeLabel;
			this.filenameLabel.PreferredWidth = AbstractFileDialog.LeftColumnWidth-10;
			this.filenameLabel.ContentAlignment = ContentAlignment.MiddleRight;
			this.filenameLabel.Dock = DockStyle.Left;
			this.filenameLabel.Margins = new Margins (0, 10, 0, 0);

			this.progressIndicator = new ProgressIndicator(footer);
			this.progressIndicator.ProgressStyle = ProgressIndicatorStyle.UnknownDuration;
			this.progressIndicator.PreferredWidth = AbstractFileDialog.LeftColumnWidth-10;
			this.progressIndicator.Dock = DockStyle.Left;
			this.progressIndicator.Margins = new Margins (0, 10, 0, 0);
			this.progressIndicator.Visibility = false;

			this.fieldFileName = new TextField (footer);
			this.fieldFileName.Dock = DockStyle.Fill;
			this.fieldFileName.AutoFocus = true;
			this.fieldFileName.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
			this.fieldFileName.TextEdited += this.HandleFileNameTextEdited;
			this.fieldFileName.TabIndex = 1;
			this.fieldFileName.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Dans l'ordre de droite à gauche:
			this.buttonCancel = new Button (footer);
			this.buttonCancel.PreferredWidth = 75;
			this.buttonCancel.Text = Epsitec.Common.Dialogs.Res.Strings.Dialog.Generic.Button.Cancel;
			this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
			this.buttonCancel.Dock = DockStyle.Right;
			this.buttonCancel.Margins = new Margins (6, 0, 0, 0);
			this.buttonCancel.Clicked += new MessageEventHandler (this.HandleButtonCancelClicked);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.buttonOk = new Button (footer);
			this.buttonOk.PreferredWidth = 85;
			this.buttonOk.Text = this.ActionButtonName;
			this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
			this.buttonOk.Dock = DockStyle.Right;
			this.buttonOk.Margins = new Margins (6, 0, 0, 0);
			this.buttonOk.Clicked += new MessageEventHandler (this.HandleButtonOkClicked);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.buttonOk.Enable = false;

			this.fieldExtension = new TextField (footer);
			this.fieldExtension.AutoFocus = false;
			this.fieldExtension.TabNavigationMode = TabNavigationMode.None;
			this.fieldExtension.IsReadOnly = true;
			this.fieldExtension.Text = this.NiceFileExtension (true);
			this.fieldExtension.PreferredWidth = this.IsMultipleExtensions ? 100 : 44;
			this.fieldExtension.Margins = new Margins (-1, 10, 0, 0);
			this.fieldExtension.Dock = DockStyle.Right;
			if (this.IsMultipleExtensions)
			{
				ToolTip.Default.SetToolTip (this.fieldExtension, this.NiceFileExtension (false));
			}
		}

		protected virtual void CreateFooterOptions(Widget footer)
		{
		}


		private bool IsMultipleExtensions
		{
			//	Retourne true s'il existe plus d'une extension.
			get
			{
				return this.fileFilterPattern.IndexOf ("|") != -1;
			}
		}

		private string NiceFileExtension(bool summary)
		{
			//	Retourne ce qu'il faut afficher comme extension.
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			int founded = 0;

			for (int i=0; i<this.fileFilterPattern.Length; i++)
			{
				if (this.fileFilterPattern[i] == '*')
				{
					founded++;
					continue;
				}
				else if (this.fileFilterPattern[i] == '|')
				{
					builder.Append (' ');

					if (summary && founded > 3)
					{
						builder.Append ("...");
						break;
					}
				}
				else
				{
					builder.Append (this.fileFilterPattern[i]);
				}
			}

			return builder.ToString ();
		}

		private void SelectFileNameInTable(string fileNameToSelect)
		{
			//	Sélectionne et montre un fichier dans la table.

			FileListItem[] items;

			lock (this.files.SyncRoot)
			{
				items = this.files.ToArray ();
			}

			foreach (FileListItem item in items)
			{
				if (item.FullPath == fileNameToSelect)
				{
					this.filesCollectionView.MoveCurrentTo (item);
					ItemView view = this.table.ItemPanel.GetItemView (item);
					this.table.ItemPanel.SelectItemView (view);
					break;
				}
			}

			this.UpdateButtons ();
		}

		private void UpdateFavorites()
		{
			//	Met à jour le panneau de gauche des favoris.
			this.favoritesBigState.ActiveState = this.FavoritesSettings.UseLargeIcons ? ActiveState.Yes : ActiveState.No;

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is FileButton)
				{
					FileButton f = widget as FileButton;
					f.Clicked -= new MessageEventHandler (this.HandleFavoriteClicked);
				}

				widget.Dispose ();
			}

			this.favoritesList = new List<FolderItem> ();

			this.FavoritesAddApplicationFolders ();

			this.AddFavorite (FolderId.Recent);              // Mes documents récents
			this.AddFavorite (FolderId.VirtualDesktop);      // Bureau
			this.AddFavorite (FolderId.VirtualMyDocuments);  // Mes documents
			this.AddFavorite (FolderId.VirtualMyComputer);   // Poste de travail
			this.AddFavorite (FolderId.VirtualNetwork);      // Favoris réseau

			this.favoritesFixes = this.favoritesList.Count;

			foreach (string dir in this.FavoritesSettings.Items)
			{
				FolderItem item = FileManager.GetFolderItem (dir, FolderQueryMode.NoIcons);
				this.AddFavorite (item.DisplayName, "FileTypeFavorite", dir);
			}
		}

		protected abstract void FavoritesAddApplicationFolders();

		protected void AddFavorite(string text, string icon, string path)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem (path, FolderQueryMode.LargeIcons);

			if (item.IsEmpty && !System.IO.Directory.Exists (path))
			{
				System.IO.Directory.CreateDirectory (path);
				item = FileManager.GetFolderItem (path, FolderQueryMode.LargeIcons);
			}

			FileButton f = new FileButton ();
			f.AutoFocus = false;
			f.DisplayName = text;
			f.IconName = icon;

			this.AddFavorite (item, f);
		}

		protected void AddFavorite(FolderId id)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem (id, FolderQueryMode.LargeIcons);

			if (item.Icon != null)
			{
				FileButton f = new FileButton ();
				
				f.AutoFocus = false;
				f.DisplayName = item.DisplayName;
				f.IconName = item.Icon.ImageName;

				this.AddFavorite (item, f);
			}
		}

		private void AddFavorite(FolderItem item, FileButton f)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			f.PreferredHeight = (this.favoritesBigState.ActiveState == ActiveState.Yes) ? Common.Widgets.FileButton.ExtendedHeight : Common.Widgets.FileButton.CompactHeight;
			f.Name = this.favoritesList.Count.ToString (System.Globalization.CultureInfo.InvariantCulture);
			f.Dock = DockStyle.Top;
			f.Clicked += new MessageEventHandler (this.HandleFavoriteClicked);

			if (string.IsNullOrEmpty (item.FullPath))
			{
				ToolTip.Default.SetToolTip (f, TextLayout.ConvertToTaggedText (f.DisplayName));
			}
			else
			{
				string tooltip = string.Concat (TextLayout.ConvertToTaggedText (f.DisplayName), "<br/><i>", TextLayout.ConvertToTaggedText (item.FullPath), "</i>");
				ToolTip.Default.SetToolTip (f, tooltip);
			}

			this.favorites.Panel.Children.Add (f);
			this.favoritesList.Add (item);
		}

		private void UpdateSelectedFavorites()
		{
			//	Met à jour le favoris sélectionné selon le chemin d'accès en cours.
			this.favoritesSelected = -1;

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is FileButton)
				{
					FileButton f = widget as FileButton;

					int i = System.Int32.Parse (f.Name, System.Globalization.CultureInfo.InvariantCulture);
					bool active = (this.favoritesList[i] == this.initialDirectory);
					f.ActiveState = active ? ActiveState.Yes : ActiveState.No;

					if (active)
					{
						this.favoritesSelected = i;
					}
				}
			}
		}

		private void UpdateTable(int sel)
		{
			//	Met à jour la table des fichiers.
			this.CreateCollectionView ();

			this.filesCollectionView.MoveCurrentToPosition (sel);

			if (this.table != null)
			{
				object select = this.filesCollectionView.CurrentItem;

				if (sel < 0)
				{
					this.table.ItemPanel.DeselectAllItemViews ();
				}
				else
				{
					ItemView view = this.table.ItemPanel.FindItemView (
						delegate (ItemView candidate)
						{
							return candidate.Item == select;
						});

					this.table.ItemPanel.DeselectAllItemViews ();
					this.table.ItemPanel.SelectItemView (view);
				}
			}

			this.UpdateButtons ();
		}

		private void AsyncRefreshCollectionView()
		{
			if (this.refreshRequested)
			{
				this.refreshRequested = false;
				this.filesCollectionView.Refresh ();
			}
		}

		private void UpdateFileList()
		{
			if (this.files != null)
			{
				this.CancelPendingJobs ();
				FileListJob.Start (this, this.UpdateFileList);
			}
		}

		private void RefreshFileList()
		{
			if (this.files != null)
			{
				this.CancelPendingJobs ();
				FileListJob.Start (this, this.RefreshFileList);
			}
		}

		private bool RefreshFileList(CancelCallback cancelCallback)
		{
			if (this.initialDirectory.IsFolder)
			{
				FileListSettings settings = this.GetFileListSettings ();

				string path = settings.Path;

				List<string> added = new List<string> ();
				List<string> deleted = new List<string> ();

				added.AddRange (System.IO.Directory.GetDirectories (path));
				added.AddRange (System.IO.Directory.GetFiles (path));

				Dictionary<string, FileListItem> items = new Dictionary<string, FileListItem> ();

				lock (this.files.SyncRoot)
				{
					foreach (FileListItem item in this.files)
					{
						string name = item.FullPath;
						items[name] = item;

						if (added.Contains (name))
						{
							added.Remove (name);
						}
						else
						{
							deleted.Add (name);
						}
					}
				}

				//	Traite les nouveaux fichiers, ainsi que ceux qui pourraient avoir été
				//	supprimés...

				foreach (string name in added)
				{
					settings.Process (this.files, name);

					if ((cancelCallback != null) &&
						(cancelCallback ()))
					{
						return true;
					}
				}

				foreach (string name in deleted)
				{
					if (settings.Process (name) == null)
					{
						lock (this.files.SyncRoot)
						{
							this.files.Remove (items[name]);
						}
					}

					if ((cancelCallback != null) &&
						(cancelCallback ()))
					{
						return true;
					}
				}
			}

			return false;
		}

		private void AddJob(FileListJob job)
		{
			lock (this.exclusion)
			{
				this.fileListJobs.Add (job);
			}
		}

		private void RemoveJob(FileListJob job)
		{
			lock (this.exclusion)
			{
				this.fileListJobs.Remove (job);
			}
		}

		private void CancelPendingJobs()
		{
			lock (this.exclusion)
			{
				foreach (FileListJob job in this.fileListJobs)
				{
					job.Cancel ();
				}
			}

			this.timer.Stop ();
			this.OnTimerStopped ();
		}

		private delegate bool CancelCallback();
		private delegate bool CancellableProcessCallback(CancelCallback callback);

		private class FileListJob
		{
			public static void Start(AbstractFileDialog dialog, CancellableProcessCallback process)
			{
				FileListJob job = new FileListJob (dialog, process);

				job.Register ();
				job.Begin ();
			}

			private FileListJob(AbstractFileDialog dialog, CancellableProcessCallback process)
			{
				this.dialog = dialog;
				this.process = process;
				this.running = true;
				this.callback = new JobCallback (this.ProcessJob);
			}

			private void Register()
			{
				this.dialog.AddJob (this);
			}

			private void Begin()
			{
				this.dialog.timer.Start ();
				this.callback.BeginInvoke (this.ProcessResult, null);
			}

			public void Cancel()
			{
				this.cancelRequested = true;
			}

			public bool Running
			{
				get
				{
					return this.running;
				}
			}

			public bool Interrupted
			{
				get
				{
					return this.interrupted;
				}
			}

			private void ProcessJob()
			{
				this.interrupted = this.process (
					delegate ()
					{
						return this.cancelRequested;
					});
			}

			private void ProcessResult(System.IAsyncResult result)
			{
				this.running = false;
				this.dialog.RemoveJob (this);
				this.callback.EndInvoke (result);
			}

			delegate void JobCallback();

			AbstractFileDialog dialog;
			CancellableProcessCallback process;
			JobCallback callback;
			bool running;
			bool interrupted;
			volatile bool cancelRequested;
		}


		private void NextProgressIndicator()
		{
			//	Affiche le ProgressIndicator (si nécessaire) puis fait-le avancer.
			
			if (this.progressStartTicks == 0)
			{
				this.progressStartTicks = System.Environment.TickCount;
			}

			int delta = System.Environment.TickCount - this.progressStartTicks;

			if (delta > 200)
			{
				if (!this.progressIndicator.IsVisible)
				{
					this.filenameLabel.Visibility = false;
					this.progressIndicator.Visibility = true;
				}

				this.progressIndicator.UpdateProgress ();
			}
		}

		private void StopProgressIndicator()
		{
			//	Cache le ProgressIndicator.
			if (this.filenameLabel != null)
			{
				this.filenameLabel.Visibility = true;
				this.progressIndicator.Visibility = false;
				this.progressStartTicks = 0;
			}
		}


		private bool UpdateFileList(CancelCallback cancelCallback)
		{
			//	Effectue la liste des fichiers contenus dans le dossier ad hoc.
			this.ClearFileList ();

			//	Ne montre pas les raccourcis si le chemin est distant, car cela n'apporte
			//	rien du tout: on ne sait pas suivre un raccourci distant, car il pointe
			//	sur une ressource locale de la machine distante!

			FileListSettings settings = this.GetFileListSettings ();

			System.Predicate<FileFilterInfo> filter =
				delegate (FileFilterInfo file)
				{
					if (cancelCallback ())
					{
						throw new System.Threading.ThreadInterruptedException ();
					}

					return settings.Filter (file);
				};

			try
			{
				foreach (FolderItem item in FileManager.GetFolderItems (this.initialDirectory, settings.FolderQueryMode, filter))
				{
					settings.Process (this.files, item);
				}
			}
			catch (System.Threading.ThreadInterruptedException)
			{
				return true;
			}

			return false;
		}

		private void ClearFileList()
		{
			//	Crée une liste de fichiers vide (ou presque).
			lock (this.files.SyncRoot)
			{
				this.files.Clear ();

				if (this.FileDialogType == FileDialogType.New)
				{
					this.files.Add (new FileListItem ("manifest:Epsitec.Common.Dialogs.Images.New.icon", AbstractFileDialog.NewEmptyDocument, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.NewEmptyDocument, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.NewEmptyDocument));  // première ligne avec 'nouveau document vide'
				}
			}
		}

		private FileListSettings GetFileListSettings()
		{
			FileListSettings settings;
			settings = new FileListSettings (this);

			settings.FolderQueryMode = (this.itemViewSize >= 32) ? FolderQueryMode.LargeIcons : FolderQueryMode.SmallIcons;
			settings.HideFolders     = this.initialDirectory.Equals (FileManager.GetFolderItem (FolderId.Recent, FolderQueryMode.NoIcons));
			settings.HideShortcuts   = FolderItem.IsNetworkPath (this.initialDirectory.FullPath);

			settings.DefineFilterPattern (this.fileFilterPattern);

			this.CreateFileExtensionDescriptions (settings);

			return settings;
		}


		protected abstract void CreateFileExtensionDescriptions(IFileExtensionDescription settings);

		private void UpdateButtons()
		{
			//	Met à jour les boutons en fonction du fichier sélectionné dans la liste.
			if (this.renameState == null)
			{
				return;
			}
			if (this.table == null)
			{
				return;
			}

			this.toolbarExtend.GlyphShape = this.toolbar.Visibility ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;

			IList<string> list = this.FavoritesSettings.Items;
			int sel = this.favoritesSelected-this.favoritesFixes;
			this.favoritesAddState.Enable = this.IsFavoriteAddPossible;
			this.favoritesRemoveState.Enable = (sel >= 0);
			this.favoritesUpState.Enable = (sel >= 1);
			this.favoritesDownState.Enable = (sel >= 0 && sel < list.Count-1);

			FileListItem item = this.GetCurrentFileListItem ();
			bool enable = (item != null && item.FullPath != AbstractFileDialog.NewEmptyDocument);
			bool okEnable = enable;

			if (item != null && (item.IsDrive || item.IsVirtual || item.IsSynthetic))
			{
				//	on ne peut renommer/supprimer que les fichiers ou les dossiers.
				enable = false;
			}

			string oldPath = this.initialDirectory.FullPath;
			string newPath = this.RedirectPath (oldPath);

			if (oldPath != newPath)
			{
				//	This is a special folder where we may not rename the files.
				enable = false;
			}

			this.deleteState.Enable = enable;

			if (this.table.ItemPanel.GetSelectedItemViews().Count > 1)
			{
				//	Pas de rename lors d'une sélection multiple.
				enable = false;
			}

			this.renameState.Enable = enable;

			FolderItem parent = FileManager.GetParentFolderItem (this.initialDirectory, FolderQueryMode.NoIcons);
			this.parentState.Enable = !parent.IsEmpty;

			this.viewDispositionState.Enable = true;
			this.viewSizeState.Enable = true;

			this.prevState.Enable = (this.directoriesVisitedIndex > 0);
			this.nextState.Enable = (this.directoriesVisitedIndex < this.directoriesVisited.Count-1);

			if (item == null)
			{
				okEnable = this.fieldFileName.Text.Trim ().Length > 0;
			}
			if (okEnable)
			{
				okEnable = this.MultipleSelectionContainsOnlyDataFiles ();
			}

			this.buttonOk.Enable = okEnable || ((item != null) && (item.IsSynthetic));
		}

		private void UpdateInitialDirectory()
		{
			//	Met à jour le chemin d'accès.
			if (this.fieldPath != null)
			{
				string text = TextLayout.ConvertToTaggedText (this.initialDirectory.DisplayName);
				if (this.initialSmallIcon != null)
				{
					text = string.Format ("<img src=\"{0}\"/> {1}", this.initialSmallIcon.ImageName, text);
				}

				this.fieldPath.Text = text;
				this.UpdateSelectedFavorites ();
			}
		}

		private void UpdateInitialFileName()
		{
			//	Met à jour le nom du fichier.
			if (this.fieldFileName != null)
			{
				if (string.IsNullOrEmpty (this.initialFileName))
				{
					this.fieldFileName.Text = "";
				}
				else
				{
					if (this.IsMultipleExtensions)  // plusieurs extensions ?
					{
						this.fieldFileName.Text = TextLayout.ConvertToTaggedText (this.initialFileName);
					}
					else  // une seule extension ?
					{
						this.fieldFileName.Text = TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (this.initialFileName));
					}
				}
			}
		}


		private void NavigatePrev()
		{
			if (this.directoriesVisitedIndex > 0)
			{
				this.SetInitialFolder (this.directoriesVisited[--this.directoriesVisitedIndex], false);
				this.UpdateButtons ();
			}
		}

		private void NavigateNext()
		{
			if (this.directoriesVisitedIndex < this.directoriesVisited.Count-1)
			{
				this.SetInitialFolder (this.directoriesVisited[++this.directoriesVisitedIndex], false);
				this.UpdateButtons ();
			}
		}

		private void ParentDirectory()
		{
			//	Remonte dans le dossier parent.
			FolderItem parent = FileManager.GetParentFolderItem (this.initialDirectory, FolderQueryMode.NoIcons);
			if (parent.IsEmpty)
			{
				return;
			}

			this.SetInitialFolder (parent, true);
		}

		private void NewDirectory()
		{
			//	Crée un nouveau dossier vide.
			string newDir = this.NewDirectoryName;
			if (newDir == null)
			{
				return;
			}

			try
			{
				System.IO.Directory.CreateDirectory (newDir);
			}
			catch
			{
				return;
			}

			this.RefreshFileList (null);
			this.table.ItemPanel.Refresh ();
			this.SelectFileNameInTable (newDir);
			this.RenameStarting ();
		}

		private string NewDirectoryName
		{
			//	Retourne le nom à utiliser pour le nouveau dossier à créer.
			//	On est assuré que le nom retourné n'existe pas déjà.
			get
			{
				for (int i=1; i<100; i++)
				{
					string newDir = string.Concat (this.initialDirectory.FullPath, "\\", Epsitec.Common.Dialogs.Res.Strings.Dialog.File.NewDirectoryName);
					if (i > 1)
					{
						newDir = string.Concat (newDir, " (", i.ToString (), ")");
					}

					bool exist = false;
					foreach (FileListItem item in this.files)
					{
						if (item.IsDirectory && item.FullPath == newDir)
						{
							exist = true;
							break;
						}
					}

					if (!exist)
					{
						return newDir;
					}
				}

				return null;
			}
		}

		private void FileDelete()
		{
			//	Supprime un fichier ou un dossier.
			FileListItem item = this.GetCurrentFileListItem ();
			if (item == null || item.IsSynthetic)
			{
				return;
			}

			//	Construit la liste des fichiers à supprimer.
			string[] selectedNames = this.GetSelectedFileNames ();

			if (selectedNames.Length > 0)
			{
				//	Supprime le ou les fichiers.
				FileOperationMode mode = new FileOperationMode (this.window);
				FileManager.DeleteFiles (mode, selectedNames);

				if (!System.IO.File.Exists (selectedNames[0]))  // fichier n'existe plus (donc bien supprimé) ?
				{
					this.RefreshFileList (null);
				}
			}
		}

		private void ToggleViewDisposition()
		{
			ItemPanelLayout layout = this.GetItemTableLayout ();
			
			if (layout == ItemPanelLayout.VerticalList)
			{
				this.SetItemViewDisposition (this.itemViewSize, ItemPanelLayout.RowsOfTiles);
				this.viewDispositionState.ActiveState = ActiveState.No;
			}
			else
			{
				this.SetItemViewDisposition (this.itemViewSize, ItemPanelLayout.VerticalList);
				this.viewDispositionState.ActiveState = ActiveState.Yes;
			}
		}

		private void ToggleViewSize()
		{
			double size = this.itemViewSize;

			if (size < 25)
			{
				size = 50;
			}
			else if (size < 75)
			{
				size = 100;
			}
			else
			{
				size = 20;
			}

			this.slider.Value = (decimal) size;
		}

		private void Refresh()
		{
			this.UpdateFileList ();
		}

		private void RenameStarting()
		{
			//	Début d'un renommer. Le widget pour éditer le nom est positionné et
			//	rendu visible.
			System.Diagnostics.Debug.Assert (this.fieldRename != null);
			FileListItem item = this.GetCurrentFileListItem ();
			ItemPanel itemPanel = this.table.ItemPanel;
			ItemView view = itemPanel.GetItemView (item);
			itemPanel.Show (view);
			Widget nameWidget = FileListItemViewFactory.FindFileNameWidget (view);

			if (item == null || view == null || nameWidget == null || item.IsSynthetic)
			{
				return;
			}

			Rectangle rect = nameWidget.MapClientToRoot (nameWidget.Client.Bounds);
			rect.Deflate (0, System.Math.Floor ((rect.Height-20)/2));	// force une hauteur de 20
			rect.Left -= 3;												// aligne par rapport au contenu de la ligne éditable
			rect.Width += 32;											// place pour les boutons "v" et "x"

			Rectangle box = this.table.Client.Bounds;
			box.Deflate (this.table.GetPanelPadding ());
			box = this.table.MapClientToRoot (box);
			if (!box.Contains (rect))
			{
				return;
			}

			this.focusedWidgetBeforeRename = this.window.FocusedWidget;

			this.fieldRename.SetManualBounds (rect);
			this.fieldRename.Text = TextLayout.ConvertToTaggedText (item.ShortFileName);
			int i = this.fieldRename.Text.LastIndexOf('.');
			if (i == -1 || FolderItem.HideFileExtensions || !item.IsDataFile)
			{
				this.fieldRename.SelectAll();
			}
			else
			{
				this.fieldRename.CursorFrom = 0;
				this.fieldRename.CursorTo = i;
			}
			this.fieldRename.Visibility = true;
			this.fieldRename.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldRename.StartEdition ();
			this.fieldRename.Focus ();

			this.renameSelected = item;

			this.fieldRename.Window.WindowResizeBeginning += this.HandleWindowResizeBeginning;
		}

		private void RenameEnding(bool accepted)
		{
			//	Fin d'un renommer. Le fichier ou le dossier est renommé (si accepted = true)
			//	et le widget pour éditer le nom est caché.
			if (!this.fieldRename.Visibility)
			{
				return;
			}

			this.fieldRename.Window.WindowResizeBeginning -= this.HandleWindowResizeBeginning;

			this.focusedWidgetBeforeRename.Focus ();
			this.focusedWidgetBeforeRename = null;
			this.fieldRename.Visibility = false;

			if (accepted && this.renameSelected != null)
			{
				FileListItem item = this.renameSelected;
				this.renameSelected = null;
				string srcFileName, dstFileName;
				string newText = TextLayout.ConvertToSimpleText (this.fieldRename.Text);

				if (item.IsDirectory)
				{
					srcFileName = item.FullPath;
					dstFileName = string.Concat (System.IO.Path.GetDirectoryName (srcFileName), "\\", newText);

					FileOperationMode mode = new FileOperationMode (this.window);
					FileManager.RenameFile (mode, srcFileName, dstFileName);

					if (System.IO.Directory.Exists (srcFileName) && !string.Equals (srcFileName, dstFileName, System.StringComparison.CurrentCultureIgnoreCase))
					{
						return;
					}
				}
				else
				{
					srcFileName = item.FullPath;
					if (FolderItem.HideFileExtensions)
					{
						dstFileName = string.Concat (System.IO.Path.GetDirectoryName (srcFileName), "\\", newText, System.IO.Path.GetExtension (srcFileName));
					}
					else
					{
						dstFileName = string.Concat (System.IO.Path.GetDirectoryName (srcFileName), "\\", newText);
					}

					FileOperationMode mode = new FileOperationMode (this.window);
					FileManager.RenameFile (mode, srcFileName, dstFileName);

					if (System.IO.File.Exists (srcFileName) && !string.Equals (srcFileName, dstFileName, System.StringComparison.CurrentCultureIgnoreCase))
					{
						return;
					}
				}

				this.RefreshFileList (null);
				this.table.ItemPanel.Refresh ();
				this.SelectFileNameInTable (dstFileName);
			}
		}

		private bool IsFavoriteAddPossible
		{
			//	Indique si le dossier en cours peut être ajouté aux favoris.
			get
			{
				foreach (FolderItem item in this.favoritesList)
				{
					if (item == this.initialDirectory)
					{
						return false;
					}
				}

				return true;
			}
		}

		private void AddFavorite()
		{
			//	Ajoute un favoris.
			if (this.IsFavoriteAddPossible)
			{
				this.FavoritesSettings.Items.Add (this.InitialDirectory);

				this.UpdateFavorites ();
				this.UpdateSelectedFavorites ();
				this.UpdateButtons ();
			}
		}

		private void FavoritesRemove()
		{
			//	Supprime un favoris.
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 0)
			{
				this.FavoritesSettings.Items.RemoveAt (sel);

				this.UpdateFavorites ();
				this.UpdateSelectedFavorites ();
				this.UpdateButtons ();
			}
		}

		private void FavoritesMoveUp()
		{
			//	Monte un favoris dans la liste.
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 1)
			{
				IList<string> list = this.FavoritesSettings.Items;
				string s = list[sel] as string;
				list.RemoveAt (sel);
				list.Insert (sel-1, s);

				this.UpdateFavorites ();
				this.UpdateSelectedFavorites ();
				this.UpdateButtons ();
			}
		}

		private void FavoritesMoveDown()
		{
			//	Descend un favoris dans la liste.
			IList<string> list = this.FavoritesSettings.Items;
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 0 && sel < list.Count-1)
			{
				string s = list[sel] as string;
				list.RemoveAt (sel);
				list.Insert (sel+1, s);

				this.UpdateFavorites ();
				this.UpdateSelectedFavorites ();
				this.UpdateButtons ();
			}
		}

		private void FavoritesToggleSize()
		{
			//	Modifie la hauteur des favoris.
			if (this.favoritesBigState.ActiveState == ActiveState.No)
			{
				this.favoritesBigState.ActiveState = ActiveState.Yes;
				this.FavoritesSettings.UseLargeIcons = true;
			}
			else
			{
				this.favoritesBigState.ActiveState = ActiveState.No;
				this.FavoritesSettings.UseLargeIcons = false;
			}

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is FileButton)
				{
					FileButton f = widget as FileButton;
					f.PreferredHeight = (this.favoritesBigState.ActiveState == ActiveState.Yes) ? Common.Widgets.FileButton.ExtendedHeight : Common.Widgets.FileButton.CompactHeight;
				}
			}
		}

		private static List<string> SplitFilenames(string name)
		{
			//	Sépare plusieurs noms de fichiers.
			//	a			-> a
			//	a b			-> a b
			//	"a b"		-> a b
			//	"a" "b"		-> a, b
			//	"ab" "cd"	-> ab, cd
			//	"a b" "c d"	-> a b, c d
			List<string> list = new List<string>();
			name = name.Trim();

			if (name.Length != 0 && name[0] == '"')
			{
				int i = 0;
				while (i<name.Length && name[i] != '"')
				{
					i++;
				}
				int start = ++i;
				while (i<name.Length && name[i] != '"')
				{
					i++;
				}
				list.Add(name.Substring(start, i-start));
				i++;
			}
			else
			{
				list.Add(name);
			}

			return list;
		}

		private bool ActionOk()
		{
			//	Effectue l'action lorsque le bouton 'Ouvrir/Enregistrer' est actionné.
			//	Retourne true s'il faut fermer le dialogue.

			FileListItem item = this.GetCurrentFileListItem ();
			string name = TextLayout.ConvertToSimpleText (this.fieldFileName.Text).Trim ();
			bool synthetic = false;

			List<string> list = null;

			if (item == null)
			{
				list = AbstractFileDialog.SplitFilenames(name);
			}
			else
			{
				name = item.GetResolvedFileName ();
				synthetic = item.IsSynthetic;
			}

			string[] selectedNames = this.GetSelectedFileNames ();

			if (selectedNames.Length > 1)
			{
				this.selectedFileNames = selectedNames;
			}
			else
			{
				this.selectedFileNames = null;
			}

			if (synthetic)
			{
				this.selectedFileName = name;
				return true;
			}

			if (list == null || list.Count == 1)
			{
				if (!System.IO.Path.IsPathRooted (name))
				{
					name = System.IO.Path.Combine (this.initialDirectory.FullPath, name);
				}

				if (System.IO.Directory.Exists (name) && item != null)
				{
					FolderItem folderItem = FileManager.GetFolderItem (name, FolderQueryMode.NoIcons);

					if (folderItem.IsFolder)
					{
						this.SetInitialFolder (folderItem, true);
						return false;
					}
				}

				if (this.fileExtension != null &&
					!name.ToLowerInvariant ().EndsWith (this.fileExtension) &&
					!System.IO.File.Exists (name))
				{
					name = string.Concat (name, this.fileExtension);
				}

				this.selectedFileName = name;

				return this.PromptForOverwriting ();
			}

			if (list.Count > 1)
			{
				this.selectedFileName = "*";
				this.selectedFileNames = new string[list.Count];

				for (int i=0; i<list.Count; i++)
				{
					name = list[i];

					if (!System.IO.Path.IsPathRooted(name))
					{
						name = System.IO.Path.Combine(this.initialDirectory.FullPath, name);
					}

					if (System.IO.Directory.Exists(name))
					{
						FolderItem folderItem = FileManager.GetFolderItem(name, FolderQueryMode.NoIcons);

						if (folderItem.IsFolder)
						{
							this.SetInitialFolder(folderItem, true);
							return false;
						}
					}

					if (this.fileExtension != null &&
						!name.ToLowerInvariant().EndsWith(this.fileExtension) &&
						!System.IO.File.Exists(name))
					{
						name = string.Concat(name, this.fileExtension);
					}

					this.selectedFileNames[i] = name;

				}
				return true;
			}

			return false;
		}

		private string[] GetSelectedFileNames()
		{
			IList<ItemView> selectedItemViews = this.table.ItemPanel.GetSelectedItemViews ();

			if (selectedItemViews.Count > 0)
			{
				List<string> selectedNames = new List<string> ();

				foreach (ItemView selectedItemView in selectedItemViews)
				{
					FileListItem selectedItem = selectedItemView.Item as FileListItem;
					selectedNames.Add (selectedItem.GetResolvedFileName ());
				}

				return selectedNames.ToArray ();
			}
			else
			{
				return new string[0];
			}
		}

		private string GetCurrentFileName()
		{
			FileListItem item = this.GetCurrentFileListItem ();

			if (item == null)
			{
				return null;
			}
			else
			{
				return item.FullPath;
			}
		}

		private FileListItem GetCurrentFileListItem()
		{
			FileListItem item = this.filesCollectionView.CurrentItem as FileListItem;
			IList<ItemView> views = this.table.ItemPanel.GetSelectedItemViews (
				delegate (ItemView view)
				{
					if (view.Item == item)
					{
						return true;
					}
					else
					{
						return false;
					}
				});

			return views.Count > 0 ? item : null;
		}

		private bool MultipleSelectionContainsOnlyDataFiles()
		{
			//	Détermine si, dans le cas d'une sélection mutliple, tous les éléments sont
			//	de véritables fichiers; retourne false si cela n'est pas le cas (évite que
			//	l'on ne puisse sélectionner un dossier et un fichier et cliquer ensuite sur
			//	le bouton "Ok").

			IList<ItemView> views = this.table.ItemPanel.GetSelectedItemViews ();

			if (views.Count > 1)
			{
				foreach (ItemView view in views)
				{
					FileListItem item = view.Item as FileListItem;
					if (!item.IsDataFile)
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool PromptForOverwriting()
		{
			//	Si requis, demande s'il faut écraser le fichier ?
			if (this.FileDialogType != FileDialogType.Save && this.selectedFileName != AbstractFileDialog.NewEmptyDocument && !System.IO.File.Exists (this.selectedFileName))  // fichier n'existe pas ?
			{
				string question = Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Open.File;
				string filePath = this.selectedFileName;
				string fileName = FolderItem.GetShortFileName (filePath);
				string message  = string.Format (question, Message.FormatMessage (fileName), Message.FormatMessage (filePath));

				Common.Dialogs.DialogResult result = Message.ShowError (message, this.window);

				this.selectedFileName = null;
				this.selectedFileNames = null;
				return false;  // ne pas fermer le dialogue
			}

			if (this.FileDialogType == FileDialogType.Save && System.IO.File.Exists (this.selectedFileName))  // fichier existe déjà ?
			{
				string question = Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Save.File;
				string filePath = this.selectedFileName;
				string fileName = FolderItem.GetShortFileName (filePath);
				string message  = string.Format (question, Message.FormatMessage (fileName), Message.FormatMessage (filePath));

				Common.Dialogs.DialogResult result = Message.ShowQuestion (message, this.window);

				if (result != Common.Dialogs.DialogResult.Yes)
				{
					this.selectedFileName  = null;
					this.selectedFileNames = null;
					return false;  // ne pas fermer le dialogue
				}
			}

			return true;  // il faudra fermer le dialogue
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


		private VMenu CreateVisitedMenu()
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

		private void HandleWindowResizeBeginning(object sender)
		{
			this.fieldFileName.Focus ();
		}

		private void HandleVisitedMenuPressed(object sender, MessageEventArgs e)
		{
			//	Une case du menu pour choisir un dossier visité a été actionnée.
			MenuItem item = sender as MenuItem;
			this.directoriesVisitedIndex = System.Int32.Parse (item.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetInitialFolder (this.directoriesVisited[this.directoriesVisitedIndex], false);
			this.UpdateButtons ();
		}


		private void HandleNavigateComboClicked(object sender, MessageEventArgs e)
		{
			//	Clic sur le bouton pour le menu de navigation.
			GlyphButton button = sender as GlyphButton;
			if (button == null)
				return;
			VMenu menu = this.CreateVisitedMenu ();
			menu.Host = this.window;
			TextFieldCombo.AdjustComboSize (button, menu, false);
			menu.ShowAsComboList (button, Point.Zero, button);
		}

		private void HandleToolbarExtendClicked(object sender, MessageEventArgs e)
		{
			this.toolbar.Visibility = !this.toolbar.Visibility;
			this.UpdateButtons ();
		}

		private void HandleFavoriteClicked(object sender, MessageEventArgs e)
		{
			//	Favoris cliqué dans le panneau de gauche.
			FileButton f = sender as FileButton;
			int i = System.Int32.Parse (f.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetInitialFolder (this.favoritesList[i], true);
		}

		private void HandleRenameAccepted(object sender)
		{
			//	Le TextFieldEx pour renommer a accepté l'édition.
			this.RenameEnding (true);
		}

		private void HandleRenameRejected(object sender)
		{
			//	Le TextFieldEx pour renommer a refusé l'édition.
			this.RenameEnding (false);
		}

		private void HandleKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Un widget (table ou filename) a pris/perdu le focus.
			bool focused = (bool) e.NewValue;
			Widget widget = sender as Widget;

			if (focused)  // focus pris ?
			{
				if (this.focusedWidget != widget)
				{
					this.focusedWidget = widget;
					this.UpdateButtons ();
				}
			}
		}

		private void HandleFileNameTextEdited(object sender)
		{
			this.filesCollectionView.MoveCurrentToPosition (-1);
			this.table.ItemPanel.DeselectAllItemViews ();

			this.UpdateButtons ();
		}

		private void HandleTableFinalSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste.
			this.UpdateButtons ();
		}

		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Double-clic dans la liste.
			if (this.buttonOk.Enable)
			{
				if (this.ActionOk ())
				{
					this.CloseWindow ();
				}
			}
		}

		private void HandleFieldPathComboOpening(object sender, CancelEventArgs e)
		{
			//	Le menu pour le chemin d'accès va être ouvert.
			this.comboFolders = new List<FileListItem> ();

#if true
			//	Ajoute toutes les unités du bureau et du poste de travail.
			FolderItem desktop = FileManager.GetFolderItem (FolderId.VirtualDesktop, FolderQueryMode.SmallIcons);
			FolderItem computer = FileManager.GetFolderItem (FolderId.VirtualMyComputer, FolderQueryMode.SmallIcons);
			bool showHidden = FolderItem.ShowHiddenFiles;

			this.ComboAdd (desktop, null);
			FileListItem root = this.comboFolders[this.comboFolders.Count-1];

			foreach (FolderItem item in FileManager.GetFolderItems (desktop, FolderQueryMode.SmallIcons))
			{
				if (!item.IsFileSystemNode)
				{
					continue;  // ignore les items qui ne stockent pas des fichiers
				}

				if (item.IsHidden && !showHidden)
				{
					continue;  // ignore les items cachés si l'utilisateur ne veut pas les voir
				}

				if (!item.IsFolder)
				{
					continue;
				}

				this.ComboAdd (item, root);
				FileListItem parent = this.comboFolders[this.comboFolders.Count-1];

				if (item.DisplayName == computer.DisplayName)
				{
					foreach (FolderItem subItem in FileManager.GetFolderItems (item, FolderQueryMode.SmallIcons))
					{
						if (!subItem.IsFileSystemNode)
						{
							continue;  // ignore les items qui ne stockent pas des fichiers
						}

						if (subItem.IsHidden && !showHidden)
						{
							continue;  // ignore les items cachés si l'utilisateur ne veut pas les voir
						}

						if (!subItem.IsFolder)
						{
							continue;
						}

						this.ComboAdd (subItem, parent);
					}
				}
			}
#endif

#if true
			//	Ajoute toutes les unités du chemin courant et de ses parents.
			FolderItem currentFolder = this.initialDirectory;
			int nb = 0;
			while (!currentFolder.IsEmpty)
			{
				this.ComboAdd (currentFolder, null);
				nb++;
				currentFolder = FileManager.GetParentFolderItem (currentFolder, FolderQueryMode.SmallIcons);
			}

			int count = this.comboFolders.Count;
			for (int i=count-nb; i<count-1; i++)
			{
				this.comboFolders[i].Parent = this.comboFolders[i+1];
			}
#endif

			//	Trie la liste obtenue.
			this.comboFolders.Sort ();

			//	Supprime les doublons.
			int index=0;
			while (index < this.comboFolders.Count-1)
			{
				FileListItem i1 = this.comboFolders[index];
				FileListItem i2 = this.comboFolders[index+1];
				if (i1.CompareTo (i2) == 0)
				{
					this.comboFolders.RemoveAt (index+1);
				}
				else
				{
					index++;
				}
			}

			//	Crée le menu-combo.
			this.fieldPath.Items.Clear ();
			this.comboTexts = new List<string> ();

			foreach (FileListItem cell in this.comboFolders)
			{
				string text = string.Format ("<img src=\"{0}\"/> {1}", cell.GetSmallIcon ().ImageName, TextLayout.ConvertToTaggedText (cell.ShortFileName));
				text = AbstractFileDialog.AddStringIndent (text, cell.Depth);

				this.fieldPath.Items.Add (text);
				this.comboTexts.Add (text);
			}

			this.comboSelected = -1;
		}

		private void ComboAdd(FolderItem folderItem, FileListItem parent)
		{
			FileListItem item = new FileListItem (folderItem);

			item.Parent = parent;
			item.SortAccordingToLevel = true;
			this.comboFolders.Add (item);
		}

		private void HandleFieldPathComboClosed(object sender)
		{
			//	Le menu pour le chemin d'accès a été fermé.
			if (this.comboSelected != -1)
			{
				this.SetInitialFolder (this.comboFolders[this.comboSelected].FolderItem, true);
			}
		}

		private void HandleFieldPathTextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Le texte pour le chemin d'accès a changé.
			if ((this.comboTexts == null) ||
				(this.fieldPath == null))
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

			this.comboSelected = this.comboTexts.FindIndex (
				delegate (string path)
				{
					if (path.Trim () == newValue)
					{
						return true;
					}
					else
					{
						return false;
					}
				});

			this.fieldPath.Text = AbstractFileDialog.RemoveStartingIndent (this.fieldPath.Text);
		}

		private void HandleSliderChanged(object sender)
		{
			//	Slider pour la taille des miniatures changé.
			this.SetItemViewDisposition ((double) this.slider.Value);
		}

		private void HandleWindowCloseClicked(object sender)
		{
			//	Fenêtre fermée.
			this.CloseWindow ();
		}

		private void UpdateSlider()
		{
			this.slider.Value = (decimal) this.itemViewSize;
		}

		protected void CloseWindow()
		{
			this.CancelPendingJobs ();
			this.window.Owner.MakeActive ();
			this.window.Hide ();
			this.OnClosed ();
		}

		protected virtual void OnClosed()
		{
			if (this.Closed != null)
			{
				this.Closed (this);
			}
		}

		public event Epsitec.Common.Support.EventHandler Closed;

		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Annuler' cliqué.
			this.CloseWindow ();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Ouvrir/Enregistrer' cliqué.
			if (this.ActionOk ())
			{
				this.CloseWindow ();
			}
		}

		static AbstractFileDialog()
		{
			Res.Initialize ();
			FileManager.Initialize ();
		}

		public static readonly string NewEmptyDocument = "#NewEmptyDocument#";

		private static readonly double LeftColumnWidth = 145;
		private static readonly double TimerRefreshRate = 0.1;

		protected Window					window;

		private GlyphButton					toolbarExtend;
		private HToolBar					toolbar;
		private GlyphButton					navigateCombo;
		private Scrollable					favorites;
		private Epsitec.Common.UI.ItemTable	table;
		private HSlider						slider;
		private TextFieldCombo				fieldPath;
		private StaticText					filenameLabel;
		private ProgressIndicator			progressIndicator;
		private int							progressStartTicks;
		private TextField					fieldFileName;
		private TextField					fieldExtension;
		private TextFieldEx					fieldRename;
		private Button						buttonOk;
		private Button						buttonCancel;
		private Timer						timer;
		private readonly object				exclusion = new object ();
		private volatile bool				refreshRequested;

		private string						fileExtension;
		private string						fileFilterPattern;
			
		protected bool						enableNavigation;
		protected bool						enableMultipleSelection;
			
		private bool						isRedirected;
		private FolderItem					initialDirectory;
		private FolderItemIcon				initialSmallIcon;
		private string						initialFileName;
		private string						selectedFileName;
		private string[]					selectedFileNames;
		private FileListItem				renameSelected;
		private Widget						focusedWidget;
		private Widget						focusedWidgetBeforeRename;

		private List<FolderItem>			favoritesList;
		private int							favoritesFixes;
		private int							favoritesSelected;

		private List<FolderItem>			directoriesVisited;
		private int							directoriesVisitedIndex;
		private List<FileListItem>			comboFolders;
		private List<string>				comboTexts;
		private int							comboSelected;

		private CommandDispatcher			dispatcher;
		private CommandContext				context;
		private CommandState				prevState;
		private CommandState				nextState;
		private CommandState				parentState;
		private CommandState				newState;
		private CommandState				renameState;
		private CommandState				deleteState;
		private CommandState				refreshState;
		private CommandState				viewDispositionState;
		private CommandState				viewSizeState;
		private CommandState				favoritesAddState;
		private CommandState				favoritesRemoveState;
		private CommandState				favoritesUpState;
		private CommandState				favoritesDownState;
		private CommandState				favoritesBigState;

		private ObservableList<FileListItem> files;
		private CollectionView				filesCollectionView;
		private List<FileListJob>			fileListJobs = new List<FileListJob> ();
		private double						itemViewSize;
		private string						title;
	}
}
