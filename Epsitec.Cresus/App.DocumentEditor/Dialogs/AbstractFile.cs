using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;
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
	using Document=Epsitec.Common.Document.Document;
	
	/// <summary>
	/// Classe abstraite pour les dialogues FileNew, FileOpen et FileOpenModel.
	/// </summary>
	public abstract class AbstractFile
	{
		public AbstractFile(IFavoritesSettings favoritesSettings)
		{
			this.favoritesSettings = favoritesSettings;
			this.directoriesVisited = new List<FolderItem> ();
			this.directoriesVisitedIndex = -1;

			this.focusedWidget = null;
		}


		public Common.Dialogs.DialogResult Result
		{
			//	Indique si le dialogue a �t� ferm� avec 'ouvrir' ou 'annuler'.
			get
			{
				if (this.selectedFileName == null)
				{
					return Common.Dialogs.DialogResult.Cancel;
				}
				else
				{
					return Common.Dialogs.DialogResult.Accept;
				}
			}
		}

		public string InitialDirectory
		{
			//	Dossier initial.
			get
			{
				return this.initialFolder.FullPath;
			}
			set
			{
				this.isRedirection = false;
				FolderItem folder;

				if (value == "")  // poste de travail ?
				{
					folder = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
				}
				else
				{
					if (this.isSave)
					{
						this.isRedirection = Document.RedirectDirectory(ref value);
					}

					folder = FileManager.GetFolderItem(value, FolderQueryMode.NoIcons);

					if (folder.IsEmpty)
					{
						folder = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
					}
				}

				this.SetInitialFolder(folder, true);
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
				if (this.isSave)
				{
					Document.RedirectFileName(ref value);
				}

				if (this.initialFileName != value)
				{
					this.initialFileName = value;
					this.UpdateInitialFileName();
				}
			}
		}

		public string FileExtension
		{
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
				}
				else
				{
					throw new System.FormatException ("Incorrect file extension: " + value);
				}
			}
		}

		public string FileFilterPattern
		{
			get
			{
				return this.fileFilterPattern;
			}
			set
			{
				this.fileFilterPattern = value;
			}
		}

		public bool IsRedirection
		{
			//	Indique si le dossier pass� avec InitialDirectory a d� �tre
			//	redirig� de 'Exemples originaux' vers 'Mes exemples'.
			get
			{
				return this.isRedirection;
			}
		}

		public string FileName
		{
			//	Retourne le nom du fichier � ouvrir, ou null si l'utilisateur a choisi
			//	le bouton 'annuler'.
			get
			{
				return this.selectedFileName;
			}
		}

		public string[] FileNames
		{
			//	Retourne les noms des fichiers � ouvrir, ou null si l'utilisateur a choisi
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

		public abstract void Show();

		public void Hide()
		{
			if (this.window != null)
			{
				this.window.Hide ();
			}
		}

		public abstract void Save();


		protected void SetInitialFolder(FolderItem folder, bool updateVisited)
		{
			//	Change le dossier courant.
			if (folder.IsEmpty)
			{
				this.initialFolder = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
			}
			else
			{
				this.initialFolder = folder;
			}

			this.initialSmallIcon = FileManager.GetFolderItemIcon(this.initialFolder, FolderQueryMode.SmallIcons);

			if (updateVisited)
			{
				this.AddToVisitedDirectories(this.initialFolder);
			}

			this.UpdateInitialDirectory ();
			this.UpdateTable (-1);

			if (this.table != null)
			{
				this.UpdateFileList ();
			}
		}

		protected void AddToVisitedDirectories(FolderItem folder)
		{
			if (folder.IsEmpty)
			{
				return;  // on n'ins�re jamais un folder vide
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
					this.directoriesVisited.RemoveAt(this.directoriesVisited.Count-1);
				}
			}

			this.directoriesVisited.Add(folder);
			this.directoriesVisitedIndex = this.directoriesVisited.Count-1;
		}


		protected void CreateAll(string name, Size windowSize, string title, double cellHeight)
		{
			//	Cr�e la fen�tre et tous les widgets pour peupler le dialogue.
			this.window = new Window();
			this.window.MakeSecondaryWindow();
			this.window.PreventAutoClose = true;
			this.WindowInit(name, windowSize.Width, windowSize.Height, true);
			this.window.Text = title;
			this.window.Owner = this.GetWindowOwner ();
			this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
			this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
			this.window.Root.MinSize = new Size(400, 200);
			this.window.Root.Padding = new Margins(8, 8, 8, 8);

			this.CreateCommandDispatcher();
			this.CreateResizer();
			this.CreateAccess();
			this.CreateToolbar();
			this.CreateTable(cellHeight);
			this.CreateRename();

			//	Danss l'ordre de bas en haut:
			this.CreateFooter();
			this.CreateOptions();
			this.CreateFileName();
		}

		protected virtual void CreateOptions()
		{
		}

		protected void UpdateAll(int initialSelection, bool focusInFileName)
		{
			//	Mise � jour lorsque les widgets sont d�j� cr��s, avant de montrer le dialogue.
			this.selectedFileName = null;
			this.selectedFileNames = null;
			this.UpdateFavorites();
			this.UpdateTable(initialSelection);
			this.UpdateInitialDirectory();
			this.UpdateInitialFileName();
			this.UpdateButtons();
			this.UpdateOptions ();

			if (focusInFileName)
			{
				this.fieldFileName.SelectAll();
				this.fieldFileName.Focus();  // focus pour frapper le nom du fichier � ouvrir
			}
			else
			{
				this.table.Focus();  // focus dans la liste des fichiers
			}
		}

		protected virtual void UpdateOptions()
		{
		}

		protected void WindowInit(string name, double dx, double dy, bool resizable)
		{
			this.window.ClientSize = new Size (dx, dy);
			dx = this.window.WindowSize.Width;
			dy = this.window.WindowSize.Height;  // taille avec le cadre

			Point location = new Point ();
			Size size = new Size ();
			if (this.UpdateWindowBounds (name, ref location, ref size))
			{
				if (resizable)
				{
					this.window.ClientSize = size;
					dx = this.window.WindowSize.Width;
					dy = this.window.WindowSize.Height;  // taille avec le cadre
				}

				Rectangle rect = new Rectangle (location, new Size (dx, dy));
				rect = ScreenInfo.FitIntoWorkingArea (rect);
				this.window.WindowBounds = rect;
			}
			else
			{
				Rectangle cb = this.GetCurrentBounds ();
				Rectangle rect = new Rectangle (cb.Center.X-dx/2, cb.Center.Y-dy/2, dx, dy);
				this.window.WindowBounds = rect;
			}
		}

		protected abstract bool UpdateWindowBounds(string name, ref Point location, ref Size size);

		protected abstract Window GetWindowOwner();

		protected abstract Rectangle GetCurrentBounds();

		protected void CreateCommandDispatcher()
		{
			this.dispatcher = new CommandDispatcher();
			this.context = new CommandContext();

			this.prevState            = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NavigatePrev, this.NavigatePrev);
			this.nextState            = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NavigateNext, this.NavigateNext);
			this.parentState          = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.ParentFolder, this.ParentDirectory);
			this.newState             = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.NewFolder, this.NewDirectory);
			this.renameState          = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Rename, this.RenameStarting);
			this.deleteState          = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Delete, this.FileDelete);
			this.favoritesAddState    = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Add, this.FavoritesAdd);
			this.favoritesRemoveState = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Remove, this.FavoritesRemove);
			this.favoritesUpState     = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Up, this.FavoritesMoveUp);
			this.favoritesDownState   = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.Down, this.FavoritesMoveDown);
			this.favoritesBigState    = this.CreateCommandState (Epsitec.Common.Dialogs.Res.Commands.Dialog.File.Favorites.ToggleSize, this.FavoritesToggleSize);

			CommandDispatcher.SetDispatcher(this.window, this.dispatcher);
			CommandContext.SetContext(this.window, this.context);
		}

		protected CommandState CreateCommandState(Command command, SimpleCallback handler)
		{
			this.dispatcher.Register(command, handler);
			return this.context.GetCommandState(command);
		}


		protected void CreateResizer()
		{
			//	Cr�e l'ic�ne en bas � droite pour signaler que la fen�tre est redimensionnable.
			ResizeKnob resize = new ResizeKnob(this.window.Root);
			resize.Anchor = AnchorStyles.BottomRight;
			resize.Margins = new Margins(0, -8, 0, -8);
			ToolTip.Default.SetToolTip (resize, Epsitec.Common.Dialogs.Res.Strings.Dialog.Tooltip.Resize);
		}

		protected void CreateTable(double cellHeight)
		{
			//	Cr�e la table principale contenant la liste des fichiers et dossiers.
			Widget group = new Widget(this.window.Root);
			group.Dock = DockStyle.Fill;
			group.TabIndex = 3;
			group.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.favorites = new Scrollable(group);
			this.favorites.PreferredWidth = 140;
			this.favorites.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.favorites.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;  // (*)
			this.favorites.Panel.IsAutoFitting = true;
			this.favorites.IsForegroundFrame = true;
			this.favorites.Margins = new Margins(0, -1, 0, 0);
			this.favorites.Dock = DockStyle.Left;

			//	TODO: (*) En mode Auto, l'ascenseur est cach� lors de la premi�re apparition
			//	du dialogue !

			this.CreateCollectionView ();

			this.table = new Epsitec.Common.UI.ItemTable (group);
			this.table.Dock = DockStyle.Fill;
			this.table.TabIndex = 2;
			this.table.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.table.SourceType = FileListItem.GetStructuredType ();
			this.table.Items = this.filesCollectionView;
			this.table.AutoFocus = true;
			this.table.VerticalScrollMode = ItemTableScrollMode.ItemBased;
			this.table.ItemPanel.ItemViewDefaultSize = new Size (this.table.Parent.PreferredWidth, cellHeight);
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("icon", 72));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("name", 200));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("type", 120, FileListItem.GetDescriptionPropertyComparer ()));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("date", 96));
			this.table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("size", 54));
			this.table.ColumnHeader.SetColumnSortable (0, false);
			this.table.ColumnHeader.SetColumnSort (1, Epsitec.Common.Types.ListSortDirection.Ascending);
			this.table.ColumnHeader.SetColumnSort (2, Epsitec.Common.Types.ListSortDirection.Ascending);
			this.table.ItemPanel.ItemSelectionMode = this.enableMultipleSelection ? ItemPanelSelectionMode.Multiple : ItemPanelSelectionMode.ZeroOrOne;
			this.table.ItemPanel.SelectionBehavior = ItemPanelSelectionBehavior.ManualOne;

			this.table.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
			this.table.ItemPanel.DoubleClicked += this.HandleTableDoubleClicked;
			this.table.ItemPanel.SelectionChanged += this.HandleFilesItemTableSelectionChanged;

			this.slider.Value = (decimal) Widget.DefaultFontHeight+4;
			this.useLargeIcons = false;

			this.UpdateFileList ();
		}

		private void CreateCollectionView()
		{
			if (this.filesCollectionView == null)
			{
				this.files = new ObservableList<FileListItem> ();
				this.ClearFileList ();
				this.filesCollectionView = new CollectionView (this.files);
				this.filesCollectionView.CurrentChanged += this.HandleFilesCollectionViewCurrentChanged;

				this.filesCollectionView.InvalidationHandler =
				delegate (Epsitec.Common.Types.CollectionView cv)
					{
						Epsitec.Common.Widgets.Application.QueueAsyncCallback (
							delegate ()
							{
								cv.Refresh ();
							});
					};
			}
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
				string name = item.ShortFileName;

				if (name.EndsWith (this.fileExtension))
				{
					name = name.Substring (0, name.Length - this.fileExtension.Length);
				}

				IList<ItemView> selectedItemViews = this.table.ItemPanel.GetSelectedItemViews ();

				if (selectedItemViews.Count > 1)
				{
					name = "";
				}

				this.fieldFileName.Text = TextLayout.ConvertToTaggedText (name);
				this.fieldFileName.SelectAll ();
			}

			this.UpdateButtons ();
		}

		protected void CreateRename()
		{
			//	Cr�e le widget permettant de renommer un fichier/dossier.
			//	Normalement, ce widget est cach�.
			this.fieldRename = new TextFieldEx(this.window.Root);
			this.fieldRename.Visibility = false;
			this.fieldRename.ButtonShowCondition = ShowCondition.Always;
			this.fieldRename.EditionAccepted += new EventHandler(this.HandleRenameAccepted);
			this.fieldRename.EditionRejected += new EventHandler(this.HandleRenameRejected);
			this.fieldRename.SwallowEscape = true;
			this.fieldRename.SwallowReturn = true;
			this.fieldRename.IsModal = true;
		}

		protected void CreateAccess()
		{
			//	Cr�e la partie controlant le chemin d'acc�s.
			Widget group = new Widget(this.window.Root);
			group.PreferredHeight = 20;
			group.Margins = new Margins(0, 0, 0, 8);
			group.Dock = DockStyle.Top;
			group.TabIndex = 1;
			group.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.toolbarExtend = new GlyphButton(group);
			this.toolbarExtend.PreferredWidth = 16;
			this.toolbarExtend.ButtonStyle = ButtonStyle.Slider;
			this.toolbarExtend.AutoFocus = false;
			this.toolbarExtend.TabNavigationMode = TabNavigationMode.None;
			this.toolbarExtend.Dock = DockStyle.Left;
			this.toolbarExtend.Margins = new Margins(0, 0, 2, 2);
			this.toolbarExtend.Clicked += new MessageEventHandler(this.HandleToolbarExtendClicked);
			ToolTip.Default.SetToolTip (this.toolbarExtend, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.ExtendToolbar);

			StaticText label = new StaticText(group);
			label.Text = this.isSave ? Epsitec.Common.Dialogs.Res.Strings.Dialog.File.LabelPath.Save : Epsitec.Common.Dialogs.Res.Strings.Dialog.File.LabelPath.Open;
			label.PreferredWidth = 140-16-10-1;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Dock = DockStyle.Left;
			label.Margins = new Margins(0, 10, 0, 0);

			this.fieldPath = new TextFieldCombo(group);
			this.fieldPath.IsReadOnly = true;
			this.fieldPath.Dock = DockStyle.Fill;
			this.fieldPath.Margins = new Margins(0, 5, 0, 0);
			this.fieldPath.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleFieldPathComboOpening);
			this.fieldPath.ComboClosed += new EventHandler(this.HandleFieldPathComboClosed);
			this.fieldPath.TextChanged += new EventHandler(this.HandleFieldPathTextChanged);
			this.fieldPath.TabIndex = 1;
			this.fieldPath.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Il faut cr�er ces boutons dans l'ordre 'de droite � gauche'.
			IconButton buttonParent = new IconButton(group);
			buttonParent.AutoFocus = false;
			buttonParent.TabNavigationMode = TabNavigationMode.None;
			buttonParent.CommandObject = this.parentState.Command;
			buttonParent.Dock = DockStyle.Right;

			this.navigateCombo = new GlyphButton(group);
			this.navigateCombo.PreferredWidth = 12;
			this.navigateCombo.GlyphShape = GlyphShape.ArrowDown;
			this.navigateCombo.ButtonStyle = ButtonStyle.ToolItem;
			this.navigateCombo.AutoFocus = false;
			this.navigateCombo.TabNavigationMode = TabNavigationMode.None;
			this.navigateCombo.Dock = DockStyle.Right;
			this.navigateCombo.Margins = new Margins(0, 10, 0, 0);
			this.navigateCombo.Clicked += new MessageEventHandler(this.HandleNavigateComboClicked);
			ToolTip.Default.SetToolTip (this.navigateCombo, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.VisitedMenu);

			IconButton buttonNext = new IconButton(group);
			buttonNext.AutoFocus = false;
			buttonNext.TabNavigationMode = TabNavigationMode.None;
			buttonNext.CommandObject = this.nextState.Command;
			buttonNext.Dock = DockStyle.Right;

			IconButton buttonPrev = new IconButton(group);
			buttonPrev.AutoFocus = false;
			buttonPrev.TabNavigationMode = TabNavigationMode.None;
			buttonPrev.CommandObject = this.prevState.Command;
			buttonPrev.Dock = DockStyle.Right;
		}

		protected void CreateToolbar()
		{
			//	Cr�e la grande toolbar.
			this.toolbar = new HToolBar(this.window.Root);
			this.toolbar.Margins = new Margins(0, 0, 0, -1);
			this.toolbar.Dock = DockStyle.Top;
			this.toolbar.TabNavigationMode = TabNavigationMode.None;
			this.toolbar.Visibility = false;

			IconButton buttonFavoritesAdd = new IconButton();
			buttonFavoritesAdd.AutoFocus = false;
			buttonFavoritesAdd.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesAdd.CommandObject = this.favoritesAddState.Command;
			this.toolbar.Items.Add(buttonFavoritesAdd);

			IconButton buttonFavoritesRemove = new IconButton();
			buttonFavoritesRemove.AutoFocus = false;
			buttonFavoritesRemove.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesRemove.CommandObject = this.favoritesRemoveState.Command;
			this.toolbar.Items.Add(buttonFavoritesRemove);

			this.toolbar.Items.Add(new IconSeparator());

			IconButton buttonFavoritesUp = new IconButton();
			buttonFavoritesUp.AutoFocus = false;
			buttonFavoritesUp.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesUp.CommandObject = this.favoritesUpState.Command;
			this.toolbar.Items.Add(buttonFavoritesUp);

			IconButton buttonFavoritesDown = new IconButton();
			buttonFavoritesDown.AutoFocus = false;
			buttonFavoritesDown.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesDown.CommandObject = this.favoritesDownState.Command;
			this.toolbar.Items.Add(buttonFavoritesDown);

			this.toolbar.Items.Add(new IconSeparator());

			IconButton buttonFavoritesBig = new IconButton();
			buttonFavoritesBig.AutoFocus = false;
			buttonFavoritesBig.TabNavigationMode = TabNavigationMode.None;
			buttonFavoritesBig.CommandObject = this.favoritesBigState.Command;
			this.toolbar.Items.Add(buttonFavoritesBig);


			this.slider = new HSlider();
			this.slider.AutoFocus = false;
			this.slider.TabNavigationMode = TabNavigationMode.None;
			this.slider.PreferredWidth = 110;
			this.slider.ShowMinMaxButtons = true;
			this.slider.Dock = DockStyle.Left;
			this.slider.Margins = new Margins(50, 0, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 100.0M;
			this.slider.SmallChange = 1.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			ToolTip.Default.SetToolTip (this.slider, Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Tooltip.PreviewSize);
			this.toolbar.Items.Add(this.slider);

			//	Dans l'ordre de droite � gauche:
			IconButton buttonDelete = new IconButton();
			buttonDelete.AutoFocus = false;
			buttonDelete.TabNavigationMode = TabNavigationMode.None;
			buttonDelete.CommandObject = this.deleteState.Command;
			buttonDelete.Dock = DockStyle.Right;
			this.toolbar.Items.Add(buttonDelete);

			IconButton buttonRename = new IconButton();
			buttonRename.AutoFocus = false;
			buttonRename.TabNavigationMode = TabNavigationMode.None;
			buttonRename.CommandObject = this.renameState.Command;
			buttonRename.Dock = DockStyle.Right;
			this.toolbar.Items.Add(buttonRename);

			IconButton buttonNew = new IconButton();
			buttonNew.AutoFocus = false;
			buttonNew.TabNavigationMode = TabNavigationMode.None;
			buttonNew.CommandObject = this.newState.Command;
			buttonNew.Dock = DockStyle.Right;
			this.toolbar.Items.Add(buttonNew);
		}

		protected void CreateFileName()
		{
			//	Cr�e la partie permettant d'�diter le nom de fichier.
			Widget group = new Widget(this.window.Root);
			group.PreferredHeight = 20;
			group.Margins = new Margins(0, 0, 8, 0);
			group.Dock = DockStyle.Bottom;
			group.TabIndex = 4;
			group.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText label = new StaticText(group);
			label.Text = this.isModel ? Epsitec.Common.Dialogs.Res.Strings.Dialog.File.LabelMod : Epsitec.Common.Dialogs.Res.Strings.Dialog.File.LabelDoc;
			label.PreferredWidth = 140-10-1;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Dock = DockStyle.Left;
			label.Margins = new Margins(0, 10, 0, 0);

			this.fieldFileName = new TextField(group);
			this.fieldFileName.Dock = DockStyle.Fill;
			this.fieldFileName.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
			this.fieldFileName.TextEdited += this.HandleFileNameTextEdited;
			this.fieldFileName.TabIndex = 1;
			this.fieldFileName.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			TextField ext = new TextField(group);
			ext.AutoFocus = false;
			ext.TabNavigationMode = TabNavigationMode.None;
			ext.IsReadOnly = true;
			ext.Text = this.fileExtension;
			ext.PreferredWidth = 50;
			ext.Margins = new Margins(1, 0, 0, 0);
			ext.Dock = DockStyle.Right;
		}

		protected void CreateFooter()
		{
			//	Cr�e le pied du dialogue, avec les boutons 'ouvrir/enregistrer' et 'annuler'.
			Widget footer = new Widget(this.window.Root);
			footer.PreferredHeight = 22;
			footer.Margins = new Margins(0, 0, 8, 0);
			footer.Dock = DockStyle.Bottom;
			footer.TabIndex = 6;
			footer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.CreateFooterOptions(footer);

			//	Dans l'ordre de droite � gauche:
			this.buttonCancel = new Button(footer);
			this.buttonCancel.PreferredWidth = 75;
			this.buttonCancel.Text = Epsitec.Common.Dialogs.Res.Strings.Dialog.Generic.Button.Cancel;
			this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
			this.buttonCancel.Dock = DockStyle.Right;
			this.buttonCancel.Margins = new Margins(6, 0, 0, 0);
			this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			string ok;
			if (this.displayNewEmtpyDocument)
			{
				ok = Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.New;
			}
			else if (this.isSave)
			{
				ok = Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.Save;
			}
			else
			{
				ok = Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.Open;
			}

			this.buttonOK = new Button(footer);
			this.buttonOK.PreferredWidth = 85;
			this.buttonOK.Text = ok;
			this.buttonOK.ButtonStyle = ButtonStyle.DefaultAccept;
			this.buttonOK.Dock = DockStyle.Right;
			this.buttonOK.Margins = new Margins(6, 0, 0, 0);
			this.buttonOK.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.buttonOK.Enable = false;
		}

		protected virtual void CreateFooterOptions(Widget footer)
		{
		}


		private void SelectFileNameInTable(string fileNameToSelect)
		{
			//	S�lectionne et montre un fichier dans la table.

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

			this.UpdateButtons();
		}

		protected void UpdateFavorites()
		{
			//	Met � jour le panneau de gauche des favoris.
			this.favoritesBigState.ActiveState = this.favoritesSettings.FavoritesBig ? ActiveState.Yes : ActiveState.No;

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is FileButton)
				{
					FileButton f = widget as FileButton;
					f.Clicked -= new MessageEventHandler(this.HandleFavoriteClicked);
				}

				widget.Dispose();
			}

			this.favoritesList = new List<FolderItem>();

			if (!this.isSave)
			{
				this.FavoritesAdd(Document.OriginalSamplesDisplayName, "FileTypeEpsitecSamples", Document.OriginalSamplesPath);
			}

			this.FavoritesAdd(Document.MySamplesDisplayName, "FileTypeMySamples", Document.MySamplesPath);

			this.FavoritesAdd(FolderId.Recent);              // Mes documents r�cents
			this.FavoritesAdd(FolderId.VirtualDesktop);      // Bureau
			this.FavoritesAdd(FolderId.VirtualMyDocuments);  // Mes documents
			this.FavoritesAdd(FolderId.VirtualMyComputer);   // Poste de travail
			this.FavoritesAdd(FolderId.VirtualNetwork);      // Favoris r�seau

			this.favoritesFixes = this.favoritesList.Count;

			System.Collections.ArrayList list = this.favoritesSettings.FavoritesList;
			foreach (string dir in list)
			{
				FolderItem item = FileManager.GetFolderItem(dir, FolderQueryMode.NoIcons);
				this.FavoritesAdd(item.DisplayName, "FileTypeFavorite", dir);
			}
		}

		protected void FavoritesAdd(string text, string icon, string path)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem(path, FolderQueryMode.LargeIcons);

			if (item.IsEmpty && !System.IO.Directory.Exists(path))
			{
				System.IO.Directory.CreateDirectory(path);
				item = FileManager.GetFolderItem(path, FolderQueryMode.LargeIcons);
			}

			FileButton f = new FileButton();
			f.DisplayName = text;
			f.IconName = Misc.Icon (icon);

			this.FavoritesAdd(item, f);
		}

		protected void FavoritesAdd(FolderId id)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem(id, FolderQueryMode.LargeIcons);

			FileButton f = new FileButton();
			f.DisplayName = item.DisplayName;
			f.IconName = item.Icon == null ? null : item.Icon.ImageName;

			this.FavoritesAdd(item, f);
		}

		protected void FavoritesAdd(FolderItem item, FileButton f)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			f.PreferredHeight = (this.favoritesBigState.ActiveState == ActiveState.Yes) ? Common.Widgets.FileButton.ExtendedHeight : Common.Widgets.FileButton.CompactHeight;
			f.Name = this.favoritesList.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
			f.Dock = DockStyle.Top;
			f.Clicked += new MessageEventHandler(this.HandleFavoriteClicked);

			if (string.IsNullOrEmpty(item.FullPath))
			{
				ToolTip.Default.SetToolTip(f, TextLayout.ConvertToTaggedText(f.DisplayName));
			}
			else
			{
				string tooltip = string.Concat(TextLayout.ConvertToTaggedText(f.DisplayName), "<br/><i>", TextLayout.ConvertToTaggedText(item.FullPath), "</i>");
				ToolTip.Default.SetToolTip(f, tooltip);
			}

			this.favorites.Panel.Children.Add(f);
			this.favoritesList.Add(item);
		}

		protected void UpdateSelectedFavorites()
		{
			//	Met � jour le favoris s�lectionn� selon le chemin d'acc�s en cours.
			this.favoritesSelected = -1;

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is FileButton)
				{
					FileButton f = widget as FileButton;

					int i = System.Int32.Parse(f.Name, System.Globalization.CultureInfo.InvariantCulture);
					bool active = (this.favoritesList[i] == this.initialFolder);
					f.ActiveState = active ? ActiveState.Yes : ActiveState.No;

					if (active)
					{
						this.favoritesSelected = i;
					}
				}
			}
		}

		protected void UpdateTable(int sel)
		{
			//	Met � jour la table des fichiers.
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
			
			this.UpdateButtons();
		}

		protected bool UseLargeIcons
		{
			//	Indique si la hauteur des lignes permet l'usage des grandes ic�nes.
			get
			{
				return this.useLargeIcons;
			}
		}

		protected void UpdateFileList()
		{
			if (this.files != null)
			{
				this.CancelPendingJobs ();
				FileListJob.Start (this, this.UpdateFileList);
			}
		}

		protected void RefreshFileList()
		{
			if (this.files != null)
			{
				FileListJob.Start (this, this.RefreshFileList);
			}
		}

		private bool RefreshFileList(CancelCallback cancelCallback)
		{
			if (this.initialFolder.IsFolder)
			{
				FileListSettings settings = this.GetFileListSettings ();

				string path = this.initialFolder.FullPath;

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

				//	Traite les nouveaux fichiers, ainsi que ceux qui pourraient avoir �t�
				//	supprim�s...

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
			lock (this.fileListJobs)
			{
				this.fileListJobs.Add (job);
			}
		}

		private void RemoveJob(FileListJob job)
		{
			lock (this.fileListJobs)
			{
				this.fileListJobs.Remove (job);
			}
		}

		private void CancelPendingJobs()
		{
			lock (this.fileListJobs)
			{
				foreach (FileListJob job in this.fileListJobs)
				{
					job.Cancel ();
				}
			}
		}

		private delegate bool CancelCallback();
		private delegate bool CancellableProcessCallback(CancelCallback callback);

		private class FileListJob
		{
			public static void Start(AbstractFile dialog, CancellableProcessCallback process)
			{
				new FileListJob (dialog, process);
			}

			public FileListJob(AbstractFile dialog, CancellableProcessCallback process)
			{
				this.dialog = dialog;
				this.process = process;
				this.running = true;
				this.callback = new JobCallback (this.ProcessJob);
				
				this.dialog.AddJob (this);
				
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

			AbstractFile dialog;
			CancellableProcessCallback process;
			JobCallback callback;
			bool running;
			bool interrupted;
			bool cancelRequested;
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
				foreach (FolderItem item in FileManager.GetFolderItems (this.initialFolder, settings.FolderQueryMode, filter))
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
			//	Cr�e une liste de fichiers vide (ou presque).
			lock (this.files.SyncRoot)
			{
				this.files.Clear ();

				if (this.displayNewEmtpyDocument)
				{
					this.files.Add (new FileListItem (Misc.Icon ("New"), Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument, "-", Epsitec.Common.Dialogs.Res.Strings.Dialog.File.NewEmptyDocument));  // premi�re ligne avec 'nouveau document vide'
				}
			}
		}

		private FileListSettings GetFileListSettings()
		{
			FileListSettings settings;
			settings = new FileListSettings ();

			settings.FolderQueryMode = this.UseLargeIcons ? FolderQueryMode.LargeIcons : FolderQueryMode.SmallIcons;
			settings.HideFolders     = this.initialFolder.Equals (FileManager.GetFolderItem (FolderId.Recent, FolderQueryMode.NoIcons));
			settings.HideShortcuts   = FolderItem.IsNetworkPath (this.initialFolder.FullPath);

			settings.DefineFilterPattern (this.fileFilterPattern);

			settings.AddDefaultDescription (".crdoc", Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Document);
			settings.AddDefaultDescription (".crmod", Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Model);
			return settings;
		}

		protected void UpdateButtons()
		{
			//	Met � jour les boutons en fonction du fichier s�lectionn� dans la liste.
			if (this.renameState == null)
			{
				return;
			}
			if (this.table == null)
			{
				return;
			}

			this.toolbarExtend.GlyphShape = this.toolbar.Visibility ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;

			System.Collections.ArrayList list = this.favoritesSettings.FavoritesList;
			int sel = this.favoritesSelected-this.favoritesFixes;
			this.favoritesAddState.Enable = this.IsFavoriteAddPossible;
			this.favoritesRemoveState.Enable = (sel >= 0);
			this.favoritesUpState.Enable = (sel >= 1);
			this.favoritesDownState.Enable = (sel >= 0 && sel < list.Count-1);

			FileListItem item = this.GetCurrentFileListItem ();
			bool enable = (item != null && item.FullPath != Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument);
			bool okEnable = enable;
			
			System.Diagnostics.Debug.WriteLine (string.Format ("UpdateButtons: {0} {1} {2}", this.fieldFileName.Text, this.IsTextFieldFocused, item));

			if (string.Equals(this.initialFolder.FullPath, Document.OriginalSamplesPath, System.StringComparison.OrdinalIgnoreCase))
			{
				enable = false;
			}

			System.Diagnostics.Debug.WriteLine ("Rename enable="+enable);
			
			this.renameState.Enable = enable;
			this.deleteState.Enable = enable;

			FolderItem parent = FileManager.GetParentFolderItem(this.initialFolder, FolderQueryMode.NoIcons);
			this.parentState.Enable = !parent.IsEmpty;

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
			
			this.buttonOK.Enable = okEnable || ((item != null) && (item.IsSynthetic));
		}

		protected void UpdateInitialDirectory()
		{
			//	Met � jour le chemin d'acc�s.
			if (this.fieldPath != null)
			{
				this.ignoreChanged = true;

				string text = TextLayout.ConvertToTaggedText(this.initialFolder.DisplayName);
				if (this.initialSmallIcon != null)
				{
					text = string.Format("<img src=\"{0}\"/> {1}", this.initialSmallIcon.ImageName, text);
				}
				
				this.fieldPath.Text = text;
				this.UpdateSelectedFavorites();

				this.ignoreChanged = false;
			}
		}

		protected void UpdateInitialFileName()
		{
			//	Met � jour le nom du fichier.
			if (this.fieldFileName != null)
			{
				this.ignoreChanged = true;

				if (string.IsNullOrEmpty(this.initialFileName))
				{
					this.fieldFileName.Text = "";
				}
				else
				{
					this.fieldFileName.Text = TextLayout.ConvertToTaggedText(System.IO.Path.GetFileNameWithoutExtension(this.initialFileName));
				}

				this.ignoreChanged = false;
			}
		}


		protected void NavigatePrev()
		{
			if (this.directoriesVisitedIndex > 0)
			{
				this.SetInitialFolder(this.directoriesVisited[--this.directoriesVisitedIndex], false);
				this.UpdateButtons();
			}
		}

		protected void NavigateNext()
		{
			if (this.directoriesVisitedIndex < this.directoriesVisited.Count-1)
			{
				this.SetInitialFolder(this.directoriesVisited[++this.directoriesVisitedIndex], false);
				this.UpdateButtons();
			}
		}

		protected void ParentDirectory()
		{
			//	Remonte dans le dossier parent.
			FolderItem parent = FileManager.GetParentFolderItem(this.initialFolder, FolderQueryMode.NoIcons);
			if (parent.IsEmpty)
			{
				return;
			}

			this.SetInitialFolder(parent, true);
		}

		protected void NewDirectory()
		{
			//	Cr�e un nouveau dossier vide.
			string newDir = this.NewDirectoryName;
			if (newDir == null)
			{
				return;
			}

			try
			{
				System.IO.Directory.CreateDirectory(newDir);
			}
			catch
			{
				return;
			}

			this.RefreshFileList (null);
			this.table.ItemPanel.Refresh ();
			this.SelectFileNameInTable(newDir);
			this.RenameStarting();
		}

		protected string NewDirectoryName
		{
			//	Retourne le nom � utiliser pour le nouveau dossier � cr�er.
			//	On est assur� que le nom retourn� n'existe pas d�j�.
			get
			{
				for (int i=1; i<100; i++)
				{
					string newDir = string.Concat (this.initialFolder.FullPath, "\\", Epsitec.Common.Dialogs.Res.Strings.Dialog.File.NewDirectoryName);
					if (i > 1)
					{
						newDir = string.Concat(newDir, " (", i.ToString(), ")");
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

		protected void FileDelete()
		{
			//	Supprime un fichier ou un dossier.
			FileListItem item = this.GetCurrentFileListItem ();
			if (item == null || item.IsSynthetic)
			{
				return;
			}

			//	Construit la liste des fichiers � supprimer.
			string[] selectedNames = this.GetSelectedFileNames ();

			if (selectedNames.Length > 0)
			{
				//	Supprime le ou les fichiers.
				FileOperationMode mode = new FileOperationMode (this.window);
				FileManager.DeleteFiles (mode, selectedNames);

				if (!System.IO.File.Exists (selectedNames[0]))  // fichier n'existe plus (donc bien supprim�) ?
				{
					this.RefreshFileList (null);
				}
			}
		}

		protected void RenameStarting()
		{
			//	D�but d'un renommer. Le widget pour �diter le nom est positionn� et
			//	rendu visible.
			System.Diagnostics.Debug.Assert(this.fieldRename != null);
			FileListItem item = this.GetCurrentFileListItem ();
			ItemPanel itemPanel = this.table.ItemPanel;
			ItemView view = itemPanel.GetItemView (item);
			itemPanel.Show (view);
			Widget nameWidget = FileListItemViewFactory.GetFileNameWidget (view);
			
			if (item == null || view == null || nameWidget == null || item.IsSynthetic)
			{
				return;
			}

			Rectangle rect = nameWidget.MapClientToRoot(nameWidget.Client.Bounds);
			rect.Deflate(0, System.Math.Floor((rect.Height-20)/2));		// force une hauteur de 20
			rect.Left -= 3;												// aligne par rapport au contenu de la ligne �ditable
			rect.Width += 32;											// place pour les boutons "v" et "x"

			Rectangle box = this.table.Client.Bounds;
			box.Deflate (this.table.GetPanelPadding ());
			box = this.table.MapClientToRoot (box);
			if (!box.Contains(rect))
			{
				return;
			}

			this.focusedWidgetBeforeRename = this.window.FocusedWidget;

			this.fieldRename.SetManualBounds(rect);
			this.fieldRename.Text = TextLayout.ConvertToTaggedText (item.ShortFileName);
			this.fieldRename.SelectAll();
			this.fieldRename.Visibility = true;
			this.fieldRename.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldRename.StartEdition();
			this.fieldRename.Focus();

			this.renameSelected = item;

			this.fieldRename.Window.WindowResizeBeginning += this.HandleWindowResizeBeginning;
		}

		protected void RenameEnding(bool accepted)
		{
			//	Fin d'un renommer. Le fichier ou le dossier est renomm� (si accepted = true)
			//	et le widget pour �diter le nom est cach�.
			if (!this.fieldRename.Visibility)
			{
				return;
			}

			this.fieldRename.Window.WindowResizeBeginning -= this.HandleWindowResizeBeginning;

			this.focusedWidgetBeforeRename.Focus();
			this.focusedWidgetBeforeRename = null;
			this.fieldRename.Visibility = false;

			if (accepted && this.renameSelected != null)
			{
				FileListItem item = this.renameSelected;
				this.renameSelected = null;
				string srcFileName, dstFileName;
				string newText = TextLayout.ConvertToSimpleText(this.fieldRename.Text);

				if (item.IsDirectory)
				{
					srcFileName = item.FullPath;
					dstFileName = string.Concat (System.IO.Path.GetDirectoryName (srcFileName), "\\", newText);

					FileOperationMode mode = new FileOperationMode(this.window);
					FileManager.RenameFile (mode, srcFileName, dstFileName);

					if (System.IO.Directory.Exists (srcFileName) && !string.Equals (srcFileName, dstFileName, System.StringComparison.CurrentCultureIgnoreCase))
					{
						return;
					}
				}
				else
				{
					srcFileName = item.FullPath;
					dstFileName = string.Concat (System.IO.Path.GetDirectoryName (srcFileName), "\\", newText, System.IO.Path.GetExtension (srcFileName));

					FileOperationMode mode = new FileOperationMode(this.window);
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

		protected bool IsFavoriteAddPossible
		{
			//	Indique si le dossier en cours peut �tre ajout� aux favoris.
			get
			{
				foreach (FolderItem item in this.favoritesList)
				{
					if (item == this.initialFolder)
					{
						return false;
					}
				}

				return true;
			}
		}

		protected void FavoritesAdd()
		{
			//	Ajoute un favoris.
			if (this.IsFavoriteAddPossible)
			{
				System.Collections.ArrayList list = this.favoritesSettings.FavoritesList;
				list.Add(this.InitialDirectory);

				this.UpdateFavorites();
				this.UpdateSelectedFavorites();
				this.UpdateButtons();
			}
		}

		protected void FavoritesRemove()
		{
			//	Supprime un favoris.
			System.Collections.ArrayList list = this.favoritesSettings.FavoritesList;
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 0)
			{
				list.RemoveAt(sel);

				this.UpdateFavorites();
				this.UpdateSelectedFavorites();
				this.UpdateButtons();
			}
		}

		protected void FavoritesMoveUp()
		{
			//	Monte un favoris dans la liste.
			System.Collections.ArrayList list = this.favoritesSettings.FavoritesList;
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 1)
			{
				string s = list[sel] as string;
				list.RemoveAt(sel);
				list.Insert(sel-1, s);

				this.UpdateFavorites();
				this.UpdateSelectedFavorites();
				this.UpdateButtons();
			}
		}

		protected void FavoritesMoveDown()
		{
			//	Descend un favoris dans la liste.
			System.Collections.ArrayList list = this.favoritesSettings.FavoritesList;
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 0 && sel < list.Count-1)
			{
				string s = list[sel] as string;
				list.RemoveAt(sel);
				list.Insert(sel+1, s);

				this.UpdateFavorites();
				this.UpdateSelectedFavorites();
				this.UpdateButtons();
			}
		}

		protected void FavoritesToggleSize()
		{
			//	Modifie la hauteur des favoris.
			if (this.favoritesBigState.ActiveState == ActiveState.No)
			{
				this.favoritesBigState.ActiveState = ActiveState.Yes;
				this.favoritesSettings.FavoritesBig = true;
			}
			else
			{
				this.favoritesBigState.ActiveState = ActiveState.No;
				this.favoritesSettings.FavoritesBig = false;
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

		protected bool ActionOK()
		{
			//	Effectue l'action lorsque le bouton 'Ouvrir/Enregistrer' est actionn�.
			//	Retourne true s'il faut fermer le dialogue.
			
			FileListItem item = this.GetCurrentFileListItem ();
			string name = TextLayout.ConvertToSimpleText (this.fieldFileName.Text).Trim ();
			bool synthetic = false;
			
			if (item != null)
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

			if (name.Length > 0)
			{
				if (!System.IO.Path.IsPathRooted (name))
				{
					name = System.IO.Path.Combine (this.initialFolder.FullPath, name);
				}

				if (System.IO.Directory.Exists (name))
				{
					FolderItem folderItem = FileManager.GetFolderItem (name, FolderQueryMode.NoIcons);

					if (folderItem.IsFolder)
					{
						this.SetInitialFolder (folderItem, true);
						return false;
					}
				}

				if ((!name.ToLowerInvariant ().EndsWith (this.fileExtension)) &&
					(!System.IO.File.Exists (name)))
				{
					name = string.Concat (name, this.fileExtension);
				}

				this.selectedFileName = name;

				return this.PromptForOverwriting ();
			}
			else
			{
				return false;
			}
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
			//	D�termine si, dans le cas d'une s�lection mutliple, tous les �l�ments sont
			//	de v�ritables fichiers; retourne false si cela n'est pas le cas (�vite que
			//	l'on ne puisse s�lectionner un dossier et un fichier et cliquer ensuite sur
			//	le bouton "OK").

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

		protected bool PromptForOverwriting()
		{
			//	Si requis, demande s'il faut �craser le fichier ?
			if (!this.isSave && this.selectedFileName != Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument && !System.IO.File.Exists (this.selectedFileName))  // fichier n'existe pas ?
			{
#if false //#fix
				string message = string.Format(Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Open.File, Misc.ExtractName(this.selectedFileName), this.selectedFileName);
				Common.Dialogs.DialogResult result = this.editor.DialogError(message);
				this.selectedFileName = null;
				this.selectedFileNames = null;
				return false;  // ne pas fermer le dialogue
#endif
			}

			if (this.isSave && System.IO.File.Exists(this.selectedFileName))  // fichier existe d�j� ?
			{
#if false //#fix
				string message = string.Format(Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Save.File, Misc.ExtractName(this.selectedFileName), this.selectedFileName);
				Common.Dialogs.DialogResult result = this.editor.DialogQuestion(message);
				if (result != Common.Dialogs.DialogResult.Yes)
				{
					this.selectedFileName = null;
					this.selectedFileNames = null;
					return false;  // ne pas fermer le dialogue
				}
#endif
			}

			return true;  // il faudra fermer le dialogue
		}


		protected static string AddStringIndent(string text, int level)
		{
			//	Ajoute des niveaux d'indentation au d�but d'un texte.
			while (level > 0)
			{
				text = "   "+text;
				level--;
			}
			return text;
		}

		protected static string RemoveStartingIndent(string text)
		{
			//	Supprime tous les niveaux d'indentation au d�but d'un texte.
			while (text.StartsWith("   "))
			{
				text = text.Substring(3);
			}

			return text;
		}


		protected VMenu CreateVisitedMenu()
		{
			//	Cr�e le menu pour choisir un dossier visit�.
			VMenu menu = new VMenu();

			int max = 10;  // +/-, donc 20 lignes au maximum
			int end   = System.Math.Min(this.directoriesVisitedIndex+max, this.directoriesVisited.Count-1);
			int start = System.Math.Max(end-max*2, 0);

			if (start > 0)  // commence apr�s le d�but ?
			{
				menu.Items.Add(this.CreateVisitedMenuItem(0));  // met "1: dossier"

				if (start > 1)
				{
					menu.Items.Add(new MenuSeparator());  // met s�parateur "------"
				}
			}

			for (int i=start; i<=end; i++)
			{
				if (i-1 == this.directoriesVisitedIndex)
				{
					menu.Items.Add(new MenuSeparator());  // met s�parateur "------"
				}

				menu.Items.Add(this.CreateVisitedMenuItem(i));  // met "n: dossier"
			}

			if (end < this.directoriesVisited.Count-1)  // fini avant la fin ?
			{
				if (end < this.directoriesVisited.Count-2)
				{
					menu.Items.Add(new MenuSeparator());  // met s�parateur "------"
				}
				
				menu.Items.Add(this.CreateVisitedMenuItem(this.directoriesVisited.Count-1));  // met "n: dossier"
			}

			menu.AdjustSize();
			return menu;
		}

		protected  MenuItem CreateVisitedMenuItem(int index)
		{
			//	Cr�e une case du menu pour choisir un dossier visit�.
			if (index == -1)
			{
				return new MenuItem("ChangeVisitedDirectory", "", "...", null);
			}
			else
			{
				FolderItem folder = this.directoriesVisited[index];

				bool isCurrent = (index == this.directoriesVisitedIndex);
				bool isNext    = (index >  this.directoriesVisitedIndex);

				string icon = "";
				if (!isNext)
				{
					icon = isCurrent ? Misc.Icon("ActiveCurrent") : Misc.Icon("ActiveNo");
				}

				string text = TextLayout.ConvertToTaggedText(folder.DisplayName);
				if (isNext)
				{
					text = Misc.Italic(text);
				}
				text = string.Format("{0}: {1}", (index+1).ToString(), text);

				string tooltip = TextLayout.ConvertToTaggedText(folder.FullPath);

				string name = index.ToString(System.Globalization.CultureInfo.InvariantCulture);

				MenuItem item = new MenuItem("ChangeVisitedDirectory", icon, text, null, name);
				item.Pressed += new MessageEventHandler(this.HandleVisitedMenuPressed);
				ToolTip.Default.SetToolTip(item, tooltip);

				return item;
			}
		}

		void HandleWindowResizeBeginning(object sender)
		{
			this.fieldFileName.Focus ();
		}

		void HandleVisitedMenuPressed(object sender, MessageEventArgs e)
		{
			//	Une case du menu pour choisir un dossier visit� a �t� actionn�e.
			MenuItem item = sender as MenuItem;
			this.directoriesVisitedIndex = System.Int32.Parse(item.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetInitialFolder(this.directoriesVisited[this.directoriesVisitedIndex], false);
			this.UpdateButtons();
		}


		private void HandleNavigateComboClicked(object sender, MessageEventArgs e)
		{
			//	Clic sur le bouton pour le menu de navigation.
			GlyphButton button = sender as GlyphButton;
			if (button == null)  return;
			VMenu menu = this.CreateVisitedMenu();
			menu.Host = this.window;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		private void HandleToolbarExtendClicked(object sender, MessageEventArgs e)
		{
			this.toolbar.Visibility = !this.toolbar.Visibility;
			this.UpdateButtons();
		}

		private void HandleFavoriteClicked(object sender, MessageEventArgs e)
		{
			//	Favoris cliqu� dans le panneau de gauche.
			FileButton f = sender as FileButton;
			int i = System.Int32.Parse(f.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetInitialFolder(this.favoritesList[i], true);
		}

		private void HandleRenameAccepted(object sender)
		{
			//	Le TextFieldEx pour renommer a accept� l'�dition.
			this.RenameEnding(true);
		}

		private void HandleRenameRejected(object sender)
		{
			//	Le TextFieldEx pour renommer a refus� l'�dition.
			this.RenameEnding(false);
		}

		protected void HandleKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
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
			//	S�lection chang�e dans la liste.
			this.UpdateButtons();
		}

		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Double-clic dans la liste.
			if (this.buttonOK.Enable)
			{
				if (this.ActionOK ())
				{
					this.CloseWindow ();
				}
			}
		}

		private void HandleFieldPathComboOpening(object sender, CancelEventArgs e)
		{
			//	Le menu pour le chemin d'acc�s va �tre ouvert.
			this.comboFolders = new List<FileListItem>();

#if true
			//	Ajoute toutes les unit�s du bureau et du poste de travail.
			FolderItem desktop = FileManager.GetFolderItem(FolderId.VirtualDesktop, FolderQueryMode.SmallIcons);
			FolderItem computer = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.SmallIcons);
			bool showHidden = FolderItem.ShowHiddenFiles;

			this.ComboAdd(desktop, null);
			FileListItem root = this.comboFolders[this.comboFolders.Count-1];

			foreach (FolderItem item in FileManager.GetFolderItems(desktop, FolderQueryMode.SmallIcons))
			{
				if (!item.IsFileSystemNode)
				{
					continue;  // ignore les items qui ne stockent pas des fichiers
				}

				if (item.IsHidden && !showHidden)
				{
					continue;  // ignore les items cach�s si l'utilisateur ne veut pas les voir
				}

				if (!item.IsFolder)
				{
					continue;
				}

				this.ComboAdd(item, root);
				FileListItem parent = this.comboFolders[this.comboFolders.Count-1];

				if (item.DisplayName == computer.DisplayName)
				{
					foreach (FolderItem subItem in FileManager.GetFolderItems(item, FolderQueryMode.SmallIcons))
					{
						if (!subItem.IsFileSystemNode)
						{
							continue;  // ignore les items qui ne stockent pas des fichiers
						}

						if (subItem.IsHidden && !showHidden)
						{
							continue;  // ignore les items cach�s si l'utilisateur ne veut pas les voir
						}

						if (!subItem.IsFolder)
						{
							continue;
						}

						this.ComboAdd(subItem, parent);
					}
				}
			}
#endif

#if true
			//	Ajoute toutes les unit�s du chemin courant et de ses parents.
			FolderItem currentFolder = this.initialFolder;
			int nb = 0;
			while (!currentFolder.IsEmpty)
			{
				this.ComboAdd(currentFolder, null);
				nb++;
				currentFolder = FileManager.GetParentFolderItem(currentFolder, FolderQueryMode.SmallIcons);
			}

			int count = this.comboFolders.Count;
			for (int i=count-nb; i<count-1; i++)
			{
				this.comboFolders[i].Parent = this.comboFolders[i+1];
			}
#endif

			//	Trie la liste obtenue.
			this.comboFolders.Sort();

			//	Supprime les doublons.
			int index=0;
			while (index < this.comboFolders.Count-1)
			{
				FileListItem i1 = this.comboFolders[index];
				FileListItem i2 = this.comboFolders[index+1];
				if (i1.CompareTo(i2) == 0)
				{
					this.comboFolders.RemoveAt(index+1);
				}
				else
				{
					index++;
				}
			}

			//	Cr�e le menu-combo.
			this.fieldPath.Items.Clear();
			this.comboTexts = new List<string>();

			foreach (FileListItem cell in this.comboFolders)
			{
				string text = string.Format("<img src=\"{0}\"/> {1}", cell.GetSmallIcon ().ImageName, TextLayout.ConvertToTaggedText (cell.ShortFileName));
				text = AbstractFile.AddStringIndent(text, cell.Depth);

				this.fieldPath.Items.Add(text);
				this.comboTexts.Add(text);
			}

			this.comboSelected = -1;
		}

		protected void ComboAdd(FolderItem folderItem, FileListItem parent)
		{
			FileListItem item = new FileListItem(folderItem);
			item.DefaultDescription = this.isModel ? Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Model : Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Document;
			item.Parent = parent;
			item.SortAccordingToLevel = true;
			this.comboFolders.Add(item);
		}

		private void HandleFieldPathComboClosed(object sender)
		{
			//	Le menu pour le chemin d'acc�s a �t� ferm�.
			if (this.comboSelected != -1)
			{
				this.SetInitialFolder(this.comboFolders[this.comboSelected].FolderItem, true);
			}
		}

		private void HandleFieldPathTextChanged(object sender)
		{
			//	Le texte pour le chemin d'acc�s a chang�.
			if (this.ignoreChanged)
			{
				return;
			}

			this.ignoreChanged = true;
			this.comboSelected = this.comboTexts.IndexOf(this.fieldPath.Text);
			this.fieldPath.Text = AbstractFile.RemoveStartingIndent(this.fieldPath.Text);
			this.ignoreChanged = false;
		}

		private void HandleSliderChanged(object sender)
		{
			//	Slider pour la taille des miniatures chang�.
			bool initialMode = this.UseLargeIcons;

			if (this.slider.Value >= 32)
			{
				this.useLargeIcons = true;
			}
			else
			{
				this.useLargeIcons = false;
			}

			//#this.table.DefHeight = (double) this.slider.Value;
			//#this.table.HeaderHeight = 20;

			this.table.ItemPanel.ItemViewDefaultSize = new Size (this.table.Parent.PreferredWidth, (double) this.slider.Value);
		}

		protected void HandleWindowCloseClicked(object sender)
		{
			//	Fen�tre ferm�e.
			this.CloseWindow();
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
			//	Bouton 'Annuler' cliqu�.
			this.CloseWindow();
		}

		private bool IsTextFieldFocused
		{
			//	Focus dans un texte �ditable ?
			get
			{
				return this.focusedWidget is AbstractTextField;
			}
		}

		private void HandleButtonOKClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Ouvrir/Enregistrer' cliqu�.
			if (this.ActionOK ())
			{
				this.CloseWindow();
			}
		}

		static AbstractFile()
		{
			FileManager.Initialize ();
		}

		
		protected Window window;
		private IFavoritesSettings			favoritesSettings;
		private GlyphButton					toolbarExtend;
		private HToolBar					toolbar;
		private GlyphButton					navigateCombo;
		private Scrollable					favorites;
		private Epsitec.Common.UI.ItemTable table;
		private HSlider						slider;
		private TextFieldCombo				fieldPath;
		private TextField					fieldFileName;
		private TextFieldEx					fieldRename;
		private Button						buttonOK;
		private Button						buttonCancel;

		private string						fileExtension;
		private string						fileFilterPattern;
		
		protected bool						isModel;
		protected bool						isSave;
		
		protected bool						enableNavigation;
		protected bool						enableMultipleSelection;
		protected bool						displayNewEmtpyDocument;
		
		private bool						isRedirection;
		private FolderItem					initialFolder;
		private FolderItemIcon				initialSmallIcon;
		private string						initialFileName;
		private string						selectedFileName;
		private string[]					selectedFileNames;
		private FileListItem				renameSelected;
		private Widget						focusedWidget;
		private Widget						focusedWidgetBeforeRename;
		private bool						ignoreChanged;

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
		private CommandState				favoritesAddState;
		private CommandState				favoritesRemoveState;
		private CommandState				favoritesUpState;
		private CommandState				favoritesDownState;
		private CommandState				favoritesBigState;

		private ObservableList<FileListItem> files;
		private CollectionView				filesCollectionView;
		private List<FileListJob>			fileListJobs = new List<FileListJob> ();
		private bool						useLargeIcons;
	}
}
