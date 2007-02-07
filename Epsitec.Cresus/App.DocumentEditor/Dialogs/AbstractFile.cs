using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;

using System.IO;
using System.Collections.Generic;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	/// <summary>
	/// Classe abstraite pour les dialogues FileNew, FileOpen et FileOpenModel.
	/// </summary>
	public abstract class AbstractFile : Abstract
	{
		static AbstractFile()
		{
			FileManager.Initialize ();
		}

		public AbstractFile(DocumentEditor editor) : base(editor)
		{
			this.directoriesVisited = new List<FolderItem> ();
			this.directoriesVisitedIndex = -1;

			this.focusedWidget = null;
		}


		public Common.Dialogs.DialogResult Result
		{
			//	Indique si le dialogue a été fermé avec 'ouvrir' ou 'annuler'.
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

		public Document.FontIncludeMode FontIncludeMode
		{
			//	Mode d'inclusion des polices.
			get
			{
				return this.fontIncludeMode;
			}
			set
			{
				if (this.fontIncludeMode != value)
				{
					this.fontIncludeMode = value;
					this.UpdateFontIncludeMode();
				}
			}
		}

		public Document.ImageIncludeMode ImageIncludeMode
		{
			//	Mode d'inclusion des images.
			get
			{
				return this.imageIncludeMode;
			}
			set
			{
				if (this.imageIncludeMode != value)
				{
					this.imageIncludeMode = value;
					this.UpdateImageIncludeMode();
				}
			}
		}

		public bool IsRedirection
		{
			//	Indique si le dossier passé avec InitialDirectory a dû être
			//	redirigé de 'Exemples originaux' vers 'Mes exemples'.
			get
			{
				return this.isRedirection;
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
			this.UpdateFileList ();
		}

		protected void AddToVisitedDirectories(FolderItem folder)
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
					this.directoriesVisited.RemoveAt(this.directoriesVisited.Count-1);
				}
			}

			this.directoriesVisited.Add(folder);
			this.directoriesVisitedIndex = this.directoriesVisited.Count-1;
		}


		protected void CreateAll(string name, Size windowSize, string title, double cellHeight)
		{
			//	Crée la fenêtre et tous les widgets pour peupler le dialogue.
			this.window = new Window();
			this.window.MakeSecondaryWindow();
			this.window.PreventAutoClose = true;
			this.WindowInit(name, windowSize.Width, windowSize.Height, true);
			this.window.Text = title;
			this.window.Owner = this.editor.Window;
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

		protected void UpdateAll(int initialSelection, bool focusInFileName)
		{
			//	Mise à jour lorsque les widgets sont déjà créés, avant de montrer le dialogue.
			this.selectedFileName = null;
			this.selectedFileNames = null;
			this.UpdateFavorites();
			this.UpdateTable(initialSelection);
			this.UpdateInitialDirectory();
			this.UpdateInitialFileName();
			this.UpdateButtons();
			this.UpdateFontIncludeMode();
			this.UpdateImageIncludeMode();

			if (focusInFileName)
			{
				this.fieldFileName.SelectAll();
				this.fieldFileName.Focus();  // focus pour frapper le nom du fichier à ouvrir
			}
			else
			{
				this.table2.Focus();  // focus dans la liste des fichiers
			}
		}


		protected void CreateCommandDispatcher()
		{
			this.dispatcher = new CommandDispatcher();
			this.context = new CommandContext();

			this.prevState            = this.CreateCommandState(Res.Commands.Dialog.File.NavigatePrev, this.NavigatePrev);
			this.nextState            = this.CreateCommandState(Res.Commands.Dialog.File.NavigateNext, this.NavigateNext);
			this.parentState          = this.CreateCommandState(Res.Commands.Dialog.File.ParentDirectory, this.ParentDirectory);
			this.newState             = this.CreateCommandState(Res.Commands.Dialog.File.NewDirectory, this.NewDirectory);
			this.renameState          = this.CreateCommandState(Res.Commands.Dialog.File.Rename, this.RenameStarting);
			this.deleteState          = this.CreateCommandState(Res.Commands.Dialog.File.Delete, this.FileDelete);
			this.favoritesAddState    = this.CreateCommandState(Res.Commands.Dialog.File.Favorites.Add, this.FavoriteAdd);
			this.favoritesRemoveState = this.CreateCommandState(Res.Commands.Dialog.File.Favorites.Remove, this.FavoriteRemove);
			this.favoritesUpState     = this.CreateCommandState(Res.Commands.Dialog.File.Favorites.Up, this.FavoriteUp);
			this.favoritesDownState   = this.CreateCommandState(Res.Commands.Dialog.File.Favorites.Down, this.FavoriteDown);
			this.favoritesBigState    = this.CreateCommandState(Res.Commands.Dialog.File.Favorites.Big, this.FavoriteBig);

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
			//	Crée l'icône en bas à droite pour signaler que la fenêtre est redimensionnable.
			ResizeKnob resize = new ResizeKnob(this.window.Root);
			resize.Anchor = AnchorStyles.BottomRight;
			resize.Margins = new Margins(0, -8, 0, -8);
			ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);
		}

		protected void CreateTable(double cellHeight)
		{
			//	Crée la table principale contenant la liste des fichiers et dossiers.
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

			//	TODO: (*) En mode Auto, l'ascenseur est caché lors de la première apparition
			//	du dialogue !

			CellArrayStyles sh = CellArrayStyles.Stretch | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile;
			CellArrayStyles sv = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;

			if (this.isMultipleSelection)
			{
				sv |= CellArrayStyles.SelectMulti;
			}

#if false
			this.table = new CellTable(group);
			this.table.DefHeight = cellHeight;
			this.table.HeaderHeight = 20;
			this.table.StyleH = sh;
			this.table.StyleV = sv;
			this.table.AlphaSeparator = 0.3;
			this.table.Margins = new Margins(0, 0, 0, 0);
			this.table.Dock = DockStyle.Fill;
			this.table.FinalSelectionChanged += new EventHandler(this.HandleTableFinalSelectionChanged);
			this.table.TabIndex = 2;
			this.table.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.table.DoubleClicked += new MessageEventHandler(this.HandleTableDoubleClicked);
			this.table.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
#endif
			this.files = new ObservableList<FileListItem> ();
			this.filesCollectionView = new CollectionView (this.files);

			this.filesCollectionView.InvalidationHandler =
				delegate (Epsitec.Common.Types.CollectionView cv)
				{
					Epsitec.Common.Widgets.Application.QueueAsyncCallback (
						delegate ()
						{
							cv.Refresh ();
						});
				};
			
			this.table2 = new Epsitec.Common.UI.ItemTable (group);
			this.table2.Dock = DockStyle.Fill;
			this.table2.TabIndex = 2;
			this.table2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.table2.SourceType = FileListItem.GetStructuredType ();
			this.table2.Items = this.filesCollectionView;
			this.table2.AutoFocus = true;
			this.table2.VerticalScrollMode = ItemTableScrollMode.ItemBased;
			this.table2.ItemPanel.ItemViewDefaultSize = new Size (this.table2.Parent.PreferredWidth, cellHeight);
			this.table2.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("icon", 50));
			this.table2.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("name", 85));
			this.table2.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("type", 95, FileListItem.GetDescriptionPropertyComparer ()));
			this.table2.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("date", 96));
			this.table2.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("size", 50));
			this.table2.ColumnHeader.SetColumnSortable (0, false);
			this.table2.ColumnHeader.SetColumnSort (1, Epsitec.Common.Types.ListSortDirection.Ascending);
			this.table2.ColumnHeader.SetColumnSort (2, Epsitec.Common.Types.ListSortDirection.Ascending);

			this.filesCollectionView.CurrentChanged += this.HandleFilesCollectionViewCurrentChanged;
			this.table2.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
			this.table2.ItemPanel.DoubleClicked += this.HandleTableDoubleClicked;

			this.slider.Value = (decimal) Widget.DefaultFontHeight+4;
			this.useLargeIcons = false;

			this.UpdateFileList ();
		}

		private void HandleFilesCollectionViewCurrentChanged(object sender)
		{
			FileListItem item = this.filesCollectionView.CurrentItem as FileListItem;

			if ((item != null) &&
				(item.IsDataFile))
			{
				this.fieldFileName.Text = TextLayout.ConvertToTaggedText (item.ShortFileName);
				this.fieldFileName.SelectAll ();
			}

			this.UpdateButtons ();
		}

		protected void CreateRename()
		{
			//	Crée le widget permettant de renommer un fichier/dossier.
			//	Normalement, ce widget est caché.
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
			//	Crée la partie controlant le chemin d'accès.
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
			ToolTip.Default.SetToolTip(this.toolbarExtend, Res.Strings.Dialog.File.Tooltip.ExtendToolbar);

			StaticText label = new StaticText(group);
			label.Text = this.isSave ? Res.Strings.Dialog.File.LabelPath.Save : Res.Strings.Dialog.File.LabelPath.Open;
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

			//	Il faut créer ces boutons dans l'ordre 'de droite à gauche'.
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
			ToolTip.Default.SetToolTip(this.navigateCombo, Res.Strings.Dialog.File.Tooltip.VisitedMenu);

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
			//	Crée la grande toolbar.
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
			this.slider.IsMinMaxButtons = true;
			this.slider.Dock = DockStyle.Left;
			this.slider.Margins = new Margins(50, 0, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 100.0M;
			this.slider.SmallChange = 1.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.File.Tooltip.PreviewSize);
			this.toolbar.Items.Add(this.slider);

			//	Dans l'ordre de droite à gauche:
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
			//	Crée la partie permettant d'éditer le nom de fichier.
			Widget group = new Widget(this.window.Root);
			group.PreferredHeight = 20;
			group.Margins = new Margins(0, 0, 8, 0);
			group.Dock = DockStyle.Bottom;
			group.TabIndex = 4;
			group.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText label = new StaticText(group);
			label.Text = this.isModel ? Res.Strings.Dialog.File.LabelMod : Res.Strings.Dialog.File.LabelDoc;
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

		protected void CreateOptions()
		{
			//	Crée le panneau facultatif pour les options d'enregistrement.
			this.optionsToolbar = new Widget(this.window.Root);
			this.optionsToolbar.Margins = new Margins(0, 0, 8, 0);
			this.optionsToolbar.Dock = DockStyle.Bottom;
			this.optionsToolbar.TabNavigationMode = TabNavigationMode.None;
			this.optionsToolbar.Visibility = false;

			//	Options pour les polices.
			GroupBox groupFont = new GroupBox(this.optionsToolbar);
			groupFont.Text = Res.Strings.Dialog.Save.Include.Font.Title;
			groupFont.PreferredWidth = 180;
			groupFont.Padding = new Margins(4, 0, 0, 3);
			groupFont.Dock = DockStyle.Left;
			groupFont.Margins = new Margins(0, 8, 0, 0);

			this.optionsFontNone = new RadioButton(groupFont);
			this.optionsFontNone.Text = Res.Strings.Dialog.Save.Include.Font.None;
			this.optionsFontNone.Dock = DockStyle.Top;
			this.optionsFontNone.Clicked += new MessageEventHandler(this.HandleOptionsFontClicked);

			this.optionsFontUsed = new RadioButton(groupFont);
			this.optionsFontUsed.Text = Res.Strings.Dialog.Save.Include.Font.Used;
			this.optionsFontUsed.Dock = DockStyle.Top;
			this.optionsFontUsed.Clicked += new MessageEventHandler(this.HandleOptionsFontClicked);

			this.optionsFontAll = new RadioButton(groupFont);
			this.optionsFontAll.Text = Res.Strings.Dialog.Save.Include.Font.All;
			this.optionsFontAll.Dock = DockStyle.Top;
			this.optionsFontAll.Clicked += new MessageEventHandler(this.HandleOptionsFontClicked);

			//	Options pour les images.
			GroupBox groupImage = new GroupBox(this.optionsToolbar);
			groupImage.Text = Res.Strings.Dialog.Save.Include.Image.Title;
			groupImage.PreferredWidth = 180;
			groupImage.Padding = new Margins(4, 0, 0, 3);
			groupImage.Dock = DockStyle.Left;
			groupImage.Margins = new Margins(0, 8, 0, 0);

			this.optionsImageNone = new RadioButton(groupImage);
			this.optionsImageNone.Text = Res.Strings.Dialog.Save.Include.Image.None;
			this.optionsImageNone.Dock = DockStyle.Top;
			this.optionsImageNone.Clicked += new MessageEventHandler(this.HandleOptionsImageClicked);

			this.optionsImageDefined = new RadioButton(groupImage);
			this.optionsImageDefined.Text = Res.Strings.Dialog.Save.Include.Image.Defined;
			this.optionsImageDefined.Dock = DockStyle.Top;
			this.optionsImageDefined.Clicked += new MessageEventHandler(this.HandleOptionsImageClicked);

			this.optionsImageAll = new RadioButton(groupImage);
			this.optionsImageAll.Text = Res.Strings.Dialog.Save.Include.Image.All;
			this.optionsImageAll.Dock = DockStyle.Top;
			this.optionsImageAll.Clicked += new MessageEventHandler(this.HandleOptionsImageClicked);
		}

		protected void CreateFooter()
		{
			//	Crée le pied du dialogue, avec les boutons 'ouvrir/enregistrer' et 'annuler'.
			Widget footer = new Widget(this.window.Root);
			footer.PreferredHeight = 22;
			footer.Margins = new Margins(0, 0, 8, 0);
			footer.Dock = DockStyle.Bottom;
			footer.TabIndex = 6;
			footer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			if (this.isSave)
			{
				this.optionsExtend = new GlyphButton(footer);
				this.optionsExtend.PreferredWidth = 16;
				this.optionsExtend.ButtonStyle = ButtonStyle.Slider;
				this.optionsExtend.AutoFocus = false;
				this.optionsExtend.TabNavigationMode = TabNavigationMode.None;
				this.optionsExtend.Dock = DockStyle.Left;
				this.optionsExtend.Margins = new Margins(0, 0, 3, 3);
				this.optionsExtend.Clicked += new MessageEventHandler(this.HandleOptionsExtendClicked);
				ToolTip.Default.SetToolTip(this.optionsExtend, Res.Strings.Dialog.File.Tooltip.ExtendInclude);
			}

			//	Dans l'ordre de droite à gauche:
			this.buttonCancel = new Button(footer);
			this.buttonCancel.PreferredWidth = 75;
			this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
			this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
			this.buttonCancel.Dock = DockStyle.Right;
			this.buttonCancel.Margins = new Margins(6, 0, 0, 0);
			this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			string ok;
			if (this.isNewEmtpyDocument)
			{
				ok = Res.Strings.Dialog.File.Button.New;
			}
			else if (this.isSave)
			{
				ok = Res.Strings.Dialog.File.Button.Save;
			}
			else
			{
				ok = Res.Strings.Dialog.File.Button.Open;
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


		private void SelectFileNameInTable(string fileNameToSelect)
		{
			//	Sélectionne et montre un fichier dans la table.
			foreach (FileListItem item in this.files)
			{
				if (item.FullPath == fileNameToSelect)
				{
					this.filesCollectionView.MoveCurrentTo (item);
					break;
				}
			}

			this.UpdateButtons();
		}

		protected void UpdateFavorites()
		{
			//	Met à jour le panneau de gauche des favoris.
			this.favoritesBigState.ActiveState = this.globalSettings.FavoritesBig ? ActiveState.Yes : ActiveState.No;

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
				this.FavoritesAdd(Document.DisplayOriginalSamples, "FileTypeEpsitecSamples", Document.DirectoryOriginalSamples);
			}

			this.FavoritesAdd(Document.DisplayMySamples, "FileTypeMySamples", Document.DirectoryMySamples);

			this.FavoritesAdd(FolderId.Recent);              // Mes documents récents
			this.FavoritesAdd(FolderId.VirtualDesktop);      // Bureau
			this.FavoritesAdd(FolderId.VirtualMyDocuments);  // Mes documents
			this.FavoritesAdd(FolderId.VirtualMyComputer);   // Poste de travail
			this.FavoritesAdd(FolderId.VirtualNetwork);      // Favoris réseau

			this.favoritesFixes = this.favoritesList.Count;

			System.Collections.ArrayList list = this.globalSettings.FavoritesList;
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
			//	Met à jour le favoris sélectionné selon le chemin d'accès en cours.
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
			//	Met à jour la table des fichiers.
			if (this.table2 == null)
			{
				return;
			}

#if false
			int rows = this.files.Count;

			this.table.SetArraySize(5, rows);

			this.table.SetWidthColumn(0, 50);
			this.table.SetWidthColumn(1, 85);
			this.table.SetWidthColumn(2, 95);
			this.table.SetWidthColumn(3, 80);
			this.table.SetWidthColumn(4, 40);

			this.table.SetHeaderTextH(0, Res.Strings.Dialog.File.Header.Preview);
			this.table.SetHeaderTextH(1, Res.Strings.Dialog.File.Header.FileName);
			this.table.SetHeaderTextH(2, Res.Strings.Dialog.File.Header.Description);
			this.table.SetHeaderTextH(3, Res.Strings.Dialog.File.Header.Date);
			this.table.SetHeaderTextH(4, Res.Strings.Dialog.File.Header.Size);

			this.table.SelectRow(-1, true);

			StaticText st;
			ImagePlaceholder im;
			for (int row=0; row<rows; row++)
			{
				for (int column=0; column<this.table.Columns; column++)
				{
					if (this.table[column, row].IsEmpty)
					{
						if (column == 0)  // miniature ?
						{
							im = new ImagePlaceholder();
							im.Dock = DockStyle.Fill;
							im.Margins = new Margins(1, 1, 1, 1);
							this.table[column, row].Insert(im);
						}
						else if (column == 1)  // filename ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 2)  // résumé ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.TextBreakMode = TextBreakMode.Hyphenate;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 6, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 3)  // date ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 4)  // taille ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleRight;
							st.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(0, 6, 0, 0);
							this.table[column, row].Insert(st);
						}
					}
				}

				im = this.table[0, row].Children[0] as ImagePlaceholder;
				string fixIcon = this.files[row].IconName;
				if (fixIcon == null)
				{
					Image image;
					bool icon;
					this.files[row].GetImage(out image, out icon);
					im.Image = image;
					im.PaintFrame = icon ? false : true;
					im.DisplayMode = icon ? ImageDisplayMode.Center : ImageDisplayMode.Stretch;
					im.IconName = null;
				}
				else
				{
					im.IconName = fixIcon;
					im.Image = null;
					im.PaintFrame = false;
				}

				st = this.table[1, row].Children[0] as StaticText;
				st.Text = this.files[row].ShortFileName;

				st = this.table[2, row].Children[0] as StaticText;
				st.Text = this.files[row].Description;

				st = this.table[3, row].Children[0] as StaticText;
				st.Text = this.files[row].FileDate;

				st = this.table[4, row].Children[0] as StaticText;
				st.Text = this.files[row].FileSize;

				this.table.SelectRow(row, row==sel);
			}

			if (sel == -1)
			{
				if (this.table.Rows > 0)
				{
					this.table.ShowCell(0, 0);  // montre le début de la table
				}
			}
			else
			{
				this.table.ShowSelect();  // montre la ligne sélectionnée
			}
#endif

			this.UpdateButtons();
		}

		protected bool UseLargeIcons
		{
			//	Indique si la hauteur des lignes permet l'usage des grandes icônes.
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
				FileListJob.Start (this);
			}
		}

		protected void UpdateFileListIcons()
		{
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

		private class FileListJob
		{
			public static void Start(AbstractFile dialog)
			{
				new FileListJob (dialog);
			}
			
			public FileListJob(AbstractFile dialog)
			{
				this.dialog = dialog;
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
				this.interrupted = this.dialog.CreateFileList (
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
			JobCallback callback;
			bool running;
			bool interrupted;
			bool cancelRequested;
		}

		private delegate bool CancelCallback();

		private bool CreateFileList(CancelCallback cancelCallback)
		{
			//	Effectue la liste des fichiers contenus dans le dossier ad hoc.
			this.files.Clear ();

			if (this.isNewEmtpyDocument)
			{
				this.files.Add (new FileListItem (Misc.Icon ("New"), Common.Document.Settings.GlobalSettings.NewEmptyDocument, "-", Res.Strings.Dialog.New.EmptyDocument));  // première ligne avec 'nouveau document vide'
			}

			//	Ne montre pas les raccourcis si le chemin est distant, car cela n'apporte
			//	rien du tout: on ne sait pas suivre un raccourci distant, car il pointe
			//	sur une ressource locale de la machine distante!

			FileListSettings settings = new FileListSettings ();

			settings.FolderQueryMode = this.UseLargeIcons ? FolderQueryMode.LargeIcons : FolderQueryMode.SmallIcons;
			settings.HideFolders     = this.initialFolder.Equals (FileManager.GetFolderItem (FolderId.Recent, FolderQueryMode.NoIcons));
			settings.HideShortcuts   = FolderItem.IsNetworkPath (this.initialFolder.FullPath);

			settings.DefineFilterPattern (this.fileFilterPattern);

			settings.AddDefaultDescription (".crdoc", Res.Strings.Dialog.File.Document);
			settings.AddDefaultDescription (".crmod", Res.Strings.Dialog.File.Model); 

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

		protected void UpdateButtons()
		{
			//	Met à jour les boutons en fonction du fichier sélectionné dans la liste.
			if (this.renameState == null)
			{
				return;
			}

			this.toolbarExtend.GlyphShape = this.toolbar.Visibility ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;

			if (this.optionsExtend != null)
			{
				this.optionsExtend.GlyphShape = this.optionsToolbar.Visibility ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			}

			System.Collections.ArrayList list = this.globalSettings.FavoritesList;
			int sel = this.favoritesSelected-this.favoritesFixes;
			this.favoritesAddState.Enable = this.IsFavoriteAddPossible;
			this.favoritesRemoveState.Enable = (sel >= 0);
			this.favoritesUpState.Enable = (sel >= 1);
			this.favoritesDownState.Enable = (sel >= 0 && sel < list.Count-1);

			FileListItem item = this.filesCollectionView.CurrentItem as FileListItem;
			bool enable = (item != null && item.FullPath != Common.Document.Settings.GlobalSettings.NewEmptyDocument);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("UpdateButtons: {0} {1} {2}", this.fieldFileName.Text, this.IsTextFieldFocused, item));

			if (string.Equals(this.initialFolder.FullPath, Document.DirectoryOriginalSamples, System.StringComparison.OrdinalIgnoreCase))
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
				enable = this.fieldFileName.Text.Trim ().Length > 0;
			}
			
			this.buttonOK.Enable = enable;
		}

		protected void UpdateInitialDirectory()
		{
			//	Met à jour le chemin d'accès.
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
			//	Met à jour le nom du fichier.
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

		protected void UpdateFontIncludeMode()
		{
			//	Met à jour le mode d'inclusion des polices.
			if (this.optionsFontNone != null)
			{
				this.optionsFontNone.ActiveState = (this.fontIncludeMode == Document.FontIncludeMode.None) ? ActiveState.Yes : ActiveState.No;
				this.optionsFontUsed.ActiveState = (this.fontIncludeMode == Document.FontIncludeMode.Used) ? ActiveState.Yes : ActiveState.No;
				this.optionsFontAll .ActiveState = (this.fontIncludeMode == Document.FontIncludeMode.All ) ? ActiveState.Yes : ActiveState.No;
			}
		}

		protected void UpdateImageIncludeMode()
		{
			//	Met à jour le mode d'inclusion des images.
			if (this.optionsFontNone != null)
			{
				this.optionsImageNone   .ActiveState = (this.imageIncludeMode == Document.ImageIncludeMode.None   ) ? ActiveState.Yes : ActiveState.No;
				this.optionsImageDefined.ActiveState = (this.imageIncludeMode == Document.ImageIncludeMode.Defined) ? ActiveState.Yes : ActiveState.No;
				this.optionsImageAll    .ActiveState = (this.imageIncludeMode == Document.ImageIncludeMode.All    ) ? ActiveState.Yes : ActiveState.No;
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
			//	Crée un nouveau dossier vide.
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

			//	TODO: modification "sur place" de la liste des fichiers

			this.UpdateTable(-1);
			this.SelectFileNameInTable(newDir);
			this.RenameStarting();
		}

		protected string NewDirectoryName
		{
			//	Retourne le nom à utiliser pour le nouveau dossier à créer.
			//	On est assuré que le nom retourné n'existe pas déjà.
			get
			{
				for (int i=1; i<100; i++)
				{
					string newDir = string.Concat(this.initialFolder.FullPath, "\\", Res.Strings.Dialog.File.NewDirectoryName);
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
			FileListItem item = this.filesCollectionView.CurrentItem as FileListItem;
			if (item == null || item.IsSynthetic)
			{
				return;
			}

			//	Construit la liste des fichiers à supprimer.
			List<string> fileNamesToDelete = new List<string> ();
#if false
			for (int i=0; i<this.table.Rows; i++)
			{
				if (this.table.IsCellSelected(i, 0))
				{
					fileNamesToDelete.Add(this.files[i].FileName);
				}
			}

			//	Cherche le nom du fichier qu'il faudra sélectionner après la suppression.
			string fileNameToSelect = null;

			if (sel < this.files.Count-1)
			{
				fileNameToSelect = this.files[sel+1].FileName;
			}
			else
			{
				if (sel > 0)
				{
					fileNameToSelect = this.files[sel-1].FileName;
				}
			}

			//	Supprime le ou les fichiers.
			FileOperationMode mode = new FileOperationMode(this.window);
			FileManager.DeleteFiles(mode, fileNamesToDelete);

			if (!System.IO.File.Exists(fileNamesToDelete[0]))  // fichier n'existe plus (donc bien supprimé) ?
			{
				this.UpdateTable(-1);
				//	TODO: mettre à jour la liste des fichiers en live
				this.SelectFileNameTable(fileNameToSelect);
			}
#endif
		}

		protected void RenameStarting()
		{
			//	Début d'un renommer. Le widget pour éditer le nom est positionné et
			//	rendu visible.
			System.Diagnostics.Debug.Assert(this.fieldRename != null);
			FileListItem item = this.filesCollectionView.CurrentItem as FileListItem;
			ItemPanel itemPanel = this.table2.ItemPanel;
			ItemView view = itemPanel.GetItemView (item);
			itemPanel.Show (view);
			Widget nameWidget = FileListItemViewFactory.GetFileNameWidget (view);
			
			if (item == null || view == null || nameWidget == null || item.IsSynthetic)
			{
				return;
			}

			Rectangle rect = nameWidget.MapClientToRoot(nameWidget.Client.Bounds);
			rect.Deflate(0, System.Math.Floor((rect.Height-20)/2));		// force une hauteur de 20
			rect.Left -= 3;												// aligne par rapport au contenu de la ligne éditable
			rect.Width += 32;											// place pour les boutons "v" et "x"

			Rectangle box = this.table2.Client.Bounds;
			box.Deflate (this.table2.GetPanelPadding ());
			box = this.table2.MapClientToRoot (box);
			if (!box.Contains(rect))
			{
				return;
			}

			this.focusedWidgetBeforeRename = this.window.FocusedWidget;

			this.fieldRename.SetManualBounds(rect);
			this.fieldRename.Text = item.ShortFileName;
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
			//	Fin d'un renommer. Le fichier ou le dossier est renommé (si accepted = true)
			//	et le widget pour éditer le nom est caché.
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

				FolderItem folderItem = FileManager.GetFolderItem (dstFileName, FolderQueryMode.NoIcons);
				item.FolderItem = folderItem;

				Widget nameWidget = FileListItemViewFactory.GetFileNameWidget (this.table2.ItemPanel.GetItemView (item));

				if (nameWidget != null)
				{
					nameWidget.Text = item.ShortFileName;
				}
			}
		}

		protected bool IsFavoriteAddPossible
		{
			//	Indique si le dossier en cours peut être ajouté aux favoris.
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

		protected void FavoriteAdd()
		{
			//	Ajoute un favoris.
			if (this.IsFavoriteAddPossible)
			{
				System.Collections.ArrayList list = this.globalSettings.FavoritesList;
				list.Add(this.InitialDirectory);

				this.UpdateFavorites();
				this.UpdateSelectedFavorites();
				this.UpdateButtons();
			}
		}

		protected void FavoriteRemove()
		{
			//	Supprime un favoris.
			System.Collections.ArrayList list = this.globalSettings.FavoritesList;
			int sel = this.favoritesSelected-this.favoritesFixes;

			if (sel >= 0)
			{
				list.RemoveAt(sel);

				this.UpdateFavorites();
				this.UpdateSelectedFavorites();
				this.UpdateButtons();
			}
		}

		protected void FavoriteUp()
		{
			//	Monte un favoris dans la liste.
			System.Collections.ArrayList list = this.globalSettings.FavoritesList;
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

		protected void FavoriteDown()
		{
			//	Descend un favoris dans la liste.
			System.Collections.ArrayList list = this.globalSettings.FavoritesList;
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

		protected void FavoriteBig()
		{
			//	Modifie la hauteur des favoris.
			if (this.favoritesBigState.ActiveState == ActiveState.No)
			{
				this.favoritesBigState.ActiveState = ActiveState.Yes;
				this.globalSettings.FavoritesBig = true;
			}
			else
			{
				this.favoritesBigState.ActiveState = ActiveState.No;
				this.globalSettings.FavoritesBig = false;
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
			//	Effectue l'action lorsque le bouton 'Ouvrir/Enregistrer' est actionné.
			//	Retourne true s'il faut fermer le dialogue.
			
			FileListItem item = this.filesCollectionView.CurrentItem as FileListItem;
			string name = TextLayout.ConvertToSimpleText (this.fieldFileName.Text).Trim ();
			
			if (item != null)
			{
				name = item.GetResolvedFileName ();
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
				this.selectedFileNames = null;

				return this.PromptForOverwriting ();
			}
			else
			{
				return false;
			}
#if false
				{
					int selCount = 0;
					for (int i=0; i<this.table.Rows; i++)
					{
						if (this.table.IsCellSelected(i, 0) && !this.files[i].IsDirectory)
						{
							selCount++;
						}
					}

					if (selCount == 0)
					{
						return false;  // ne pas fermer le dialogue
					}

					this.selectedFileNames = new string[selCount];
					int rank = 0;
					for (int i=0; i<this.table.Rows; i++)
					{
						if (this.table.IsCellSelected(i, 0) && !this.files[i].IsDirectory)
						{
							if (rank == 0)
							{
								this.selectedFileName = this.files[i].FileName;  // premier fichier sélectionné
							}

							this.selectedFileNames[rank++] = this.files[i].FileName;
						}
					}
					this.selectedFileNames = new string[1];
					this.selectedFileNames[0] = item.GetResolvedFileName ();

					return this.PromptForOverwriting();
				}
			}

			return false;  // ne pas fermer le dialogue
#endif
		}

		protected bool PromptForOverwriting()
		{
			//	Si requis, demande s'il faut écraser le fichier ?
			if (!this.isSave && this.selectedFileName != Common.Document.Settings.GlobalSettings.NewEmptyDocument && !System.IO.File.Exists(this.selectedFileName))  // fichier n'existe pas ?
			{
				string message = string.Format(Res.Strings.Dialog.Question.Open.File, Misc.ExtractName(this.selectedFileName), this.selectedFileName);
				Common.Dialogs.DialogResult result = this.editor.DialogError(message);
				this.selectedFileName = null;
				this.selectedFileNames = null;
				return false;  // ne pas fermer le dialogue
			}

			if (this.isSave && System.IO.File.Exists(this.selectedFileName))  // fichier existe déjà ?
			{
				string message = string.Format(Res.Strings.Dialog.Question.Save.File, Misc.ExtractName(this.selectedFileName), this.selectedFileName);
				Common.Dialogs.DialogResult result = this.editor.DialogQuestion(message);
				if (result != Common.Dialogs.DialogResult.Yes)
				{
					this.selectedFileName = null;
					this.selectedFileNames = null;
					return false;  // ne pas fermer le dialogue
				}
			}

			return true;  // il faudra fermer le dialogue
		}


		protected static string AddStringIndent(string text, int level)
		{
			//	Ajoute des niveaux d'indentation au début d'un texte.
			while (level > 0)
			{
				text = "   "+text;
				level--;
			}
			return text;
		}

		protected static string RemoveStartingIndent(string text)
		{
			//	Supprime tous les niveaux d'indentation au début d'un texte.
			while (text.StartsWith("   "))
			{
				text = text.Substring(3);
			}

			return text;
		}


		protected VMenu CreateVisitedMenu()
		{
			//	Crée le menu pour choisir un dossier visité.
			VMenu menu = new VMenu();

			int max = 10;  // +/-, donc 20 lignes au maximum
			int end   = System.Math.Min(this.directoriesVisitedIndex+max, this.directoriesVisited.Count-1);
			int start = System.Math.Max(end-max*2, 0);

			if (start > 0)  // commence après le début ?
			{
				menu.Items.Add(this.CreateVisitedMenuItem(0));  // met "1: dossier"

				if (start > 1)
				{
					menu.Items.Add(new MenuSeparator());  // met séparateur "------"
				}
			}

			for (int i=start; i<=end; i++)
			{
				if (i-1 == this.directoriesVisitedIndex)
				{
					menu.Items.Add(new MenuSeparator());  // met séparateur "------"
				}

				menu.Items.Add(this.CreateVisitedMenuItem(i));  // met "n: dossier"
			}

			if (end < this.directoriesVisited.Count-1)  // fini avant la fin ?
			{
				if (end < this.directoriesVisited.Count-2)
				{
					menu.Items.Add(new MenuSeparator());  // met séparateur "------"
				}
				
				menu.Items.Add(this.CreateVisitedMenuItem(this.directoriesVisited.Count-1));  // met "n: dossier"
			}

			menu.AdjustSize();
			return menu;
		}

		protected  MenuItem CreateVisitedMenuItem(int index)
		{
			//	Crée une case du menu pour choisir un dossier visité.
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
			//	Une case du menu pour choisir un dossier visité a été actionnée.
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

		private void HandleOptionsExtendClicked(object sender, MessageEventArgs e)
		{
			this.optionsToolbar.Visibility = !this.optionsToolbar.Visibility;
			this.UpdateButtons();
		}

		private void HandleFavoriteClicked(object sender, MessageEventArgs e)
		{
			//	Favoris cliqué dans le panneau de gauche.
			FileButton f = sender as FileButton;
			int i = System.Int32.Parse(f.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetInitialFolder(this.favoritesList[i], true);
		}

		private void HandleRenameAccepted(object sender)
		{
			//	Le TextFieldEx pour renommer a accepté l'édition.
			this.RenameEnding(true);
		}

		private void HandleRenameRejected(object sender)
		{
			//	Le TextFieldEx pour renommer a refusé l'édition.
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
			this.table2.ItemPanel.DeselectAllItemViews ();

			this.UpdateButtons ();
		}

		private void HandleTableFinalSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste.
			this.UpdateButtons();
		}

		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Double-clic dans la liste.
			if (this.ActionOK())
			{
				this.CloseWindow();
			}
		}

		private void HandleFieldPathComboOpening(object sender, CancelEventArgs e)
		{
			//	Le menu pour le chemin d'accès va être ouvert.
			this.comboFolders = new List<FileListItem>();

#if true
			//	Ajoute toutes les unités du bureau et du poste de travail.
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
					continue;  // ignore les items cachés si l'utilisateur ne veut pas les voir
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
							continue;  // ignore les items cachés si l'utilisateur ne veut pas les voir
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
			//	Ajoute toutes les unités du chemin courant et de ses parents.
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

			//	Crée le menu-combo.
			this.fieldPath.Items.Clear();
			this.comboTexts = new List<string>();

			foreach (FileListItem cell in this.comboFolders)
			{
				string text = string.Format("<img src=\"{0}\"/> {1}", cell.GetSmallIcon ().ImageName, cell.ShortFileName);
				text = AbstractFile.AddStringIndent(text, cell.Depth);

				this.fieldPath.Items.Add(text);
				this.comboTexts.Add(text);
			}

			this.comboSelected = -1;
		}

		protected void ComboAdd(FolderItem folderItem, FileListItem parent)
		{
			FileListItem item = new FileListItem(folderItem);
			item.DefaultDescription = this.isModel ? Res.Strings.Dialog.File.Model : Res.Strings.Dialog.File.Document;
			item.Parent = parent;
			item.SortAccordingToLevel = true;
			this.comboFolders.Add(item);
		}

		private void HandleFieldPathComboClosed(object sender)
		{
			//	Le menu pour le chemin d'accès a été fermé.
			if (this.comboSelected != -1)
			{
				this.SetInitialFolder(this.comboFolders[this.comboSelected].FolderItem, true);
			}
		}

		private void HandleFieldPathTextChanged(object sender)
		{
			//	Le texte pour le chemin d'accès a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.ignoreChanged = true;
			this.comboSelected = this.comboTexts.IndexOf(this.fieldPath.Text);
			this.fieldPath.Text = AbstractFile.RemoveStartingIndent(this.fieldPath.Text);
			this.ignoreChanged = false;
		}

		private void HandleOptionsFontClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des polices a été cliqué.
			if (sender == this.optionsFontNone)
			{
				this.FontIncludeMode = Document.FontIncludeMode.None;
			}
			
			if (sender == this.optionsFontUsed)
			{
				this.FontIncludeMode = Document.FontIncludeMode.Used;
			}
			
			if (sender == this.optionsFontAll)
			{
				this.FontIncludeMode = Document.FontIncludeMode.All;
			}
		}

		private void HandleOptionsImageClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des images a été cliqué.
			if (sender == this.optionsImageNone)
			{
				this.ImageIncludeMode = Document.ImageIncludeMode.None;
			}

			if (sender == this.optionsImageDefined)
			{
				this.ImageIncludeMode = Document.ImageIncludeMode.Defined;
			}

			if (sender == this.optionsImageAll)
			{
				this.ImageIncludeMode = Document.ImageIncludeMode.All;
			}
		}

		private void HandleSliderChanged(object sender)
		{
			//	Slider pour la taille des miniatures changé.
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

			this.table2.ItemPanel.ItemViewDefaultSize = new Size (this.table2.Parent.PreferredWidth, (double) this.slider.Value);

			if (initialMode != this.UseLargeIcons)
			{
				this.UpdateFileListIcons ();
			}
		}

		protected void HandleWindowCloseClicked(object sender)
		{
			//	Fenêtre fermée.
			this.CloseWindow();
		}

		protected override void CloseWindow()
		{
			this.CancelPendingJobs ();
			base.CloseWindow ();
		}

		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Annuler' cliqué.
			this.CloseWindow();
		}

		private bool IsTextFieldFocused
		{
			//	Focus dans un texte éditable ?
			get
			{
				return this.focusedWidget is AbstractTextField;
			}
		}

		private void HandleButtonOKClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Ouvrir/Enregistrer' cliqué.
			if (this.ActionOK ())
			{
				this.CloseWindow();
			}
		}


		protected GlyphButton				toolbarExtend;
		protected HToolBar					toolbar;
		protected GlyphButton				navigateCombo;
		protected Scrollable				favorites;
		protected CellTable					table;
		protected Epsitec.Common.UI.ItemTable table2;
		protected HSlider					slider;
		protected TextFieldCombo			fieldPath;
		protected TextField					fieldFileName;
		protected TextFieldEx				fieldRename;
		protected Button					buttonOK;
		protected Button					buttonCancel;
		protected GlyphButton				optionsExtend;
		protected Widget					optionsToolbar;
		protected RadioButton				optionsFontNone;
		protected RadioButton				optionsFontUsed;
		protected RadioButton				optionsFontAll;
		protected RadioButton				optionsImageNone;
		protected RadioButton				optionsImageDefined;
		protected RadioButton				optionsImageAll;

		private string						fileExtension;
		private string						fileFilterPattern;
		protected bool						isModel = false;
		protected bool						isNavigationEnabled = false;
		protected bool						isMultipleSelection = false;
		protected bool						isNewEmtpyDocument = false;
		protected bool						isSave = false;
		protected bool						isRedirection = false;
		protected FolderItem				initialFolder;
		protected FolderItemIcon			initialSmallIcon;
		protected string					initialFileName;
		protected Document.FontIncludeMode	fontIncludeMode;
		protected Document.ImageIncludeMode	imageIncludeMode;
		protected string					selectedFileName;
		protected string[]					selectedFileNames;
		protected FileListItem				renameSelected;
		protected Widget					focusedWidget;
		protected Widget					focusedWidgetBeforeRename;
		protected bool						ignoreChanged = false;
		protected List<FolderItem>			favoritesList;
		protected int						favoritesFixes;
		protected int						favoritesSelected;
		protected List<FolderItem>			directoriesVisited;
		protected int						directoriesVisitedIndex;
		protected List<FileListItem>		comboFolders;
		protected List<string>				comboTexts;
		protected int						comboSelected;
		protected CommandDispatcher			dispatcher;
		protected CommandContext			context;
		protected CommandState				prevState;
		protected CommandState				nextState;
		protected CommandState				parentState;
		protected CommandState				newState;
		protected CommandState				renameState;
		protected CommandState				deleteState;
		protected CommandState				favoritesAddState;
		protected CommandState				favoritesRemoveState;
		protected CommandState				favoritesUpState;
		protected CommandState				favoritesDownState;
		protected CommandState				favoritesBigState;

		private ObservableList<FileListItem> files;
		private CollectionView				filesCollectionView;
		private List<FileListJob>			fileListJobs = new List<FileListJob> ();
		private bool						useLargeIcons;
	}
}
