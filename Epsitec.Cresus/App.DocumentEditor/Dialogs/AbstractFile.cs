using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	/// <summary>
	/// Classe abstraite pour les dialogues FileNew, FileOpen et FileOpenModel.
	/// </summary>
	public abstract class AbstractFile : Abstract
	{
		public AbstractFile(DocumentEditor editor) : base(editor)
		{
			this.directoriesVisited = new List<FolderItem>();
			this.directoriesVisitedIndex = -1;

			this.focusedWidget = null;
		}


		public Common.Dialogs.DialogResult Result
		{
			//	Indique si le dialogue a été fermé avec 'ouvrir' ou 'annuler'.
			get
			{
				if (this.selectedFilename == null)
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
						this.isRedirection = Document.RedirectionDirectory(ref value);
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

		public string InitialFilename
		{
			//	Nom de fichier initial.
			get
			{
				return this.initialFilename;
			}
			set
			{
				if (this.isSave)
				{
					Document.RedirectionFilename(ref value);
				}

				if (this.initialFilename != value)
				{
					this.initialFilename = value;
					this.UpdateInitialFilename();
				}
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

		public string Filename
		{
			//	Retourne le nom du fichier à ouvrir, ou null si l'utilisateur a choisi
			//	le bouton 'annuler'.
			get
			{
				return this.selectedFilename;
			}
		}

		public string[] Filenames
		{
			//	Retourne les noms des fichiers à ouvrir, ou null si l'utilisateur a choisi
			//	le bouton 'annuler'.
			get
			{
				if (this.selectedFilename == null)  // annuler ?
				{
					return null;
				}

				if (this.selectedFilenames == null)
				{
					this.selectedFilenames = new string[1];
					this.selectedFilenames[0] = this.selectedFilename;
				}

				return this.selectedFilenames;
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

			this.UpdateInitialDirectory();
			this.UpdateTable(-1);
			this.UpdateButtons();
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
				if (current != folder)
				{
					while (this.directoriesVisitedIndex < this.directoriesVisited.Count-1)
					{
						this.directoriesVisited.RemoveAt(this.directoriesVisited.Count-1);
					}
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
			this.CreateFavorites();
			this.CreateAccess();
			this.CreateTable(cellHeight);
			this.CreateRename();

			//	Danss l'ordre de bas en haut:
			this.CreateFooter();
			this.CreateOptions();
			this.CreateFilename();
		}

		protected void UpdateAll(int initialSelection, bool focusInFilename)
		{
			//	Mise à jour lorsque les widgets sont déjà créés, avant de montrer le dialogue.
			this.selectedFilename = null;
			this.selectedFilenames = null;
			this.UpdateFavorites();
			this.UpdateTable(initialSelection);
			this.UpdateInitialDirectory();
			this.UpdateInitialFilename();
			this.UpdateButtons();
			this.UpdateFontIncludeMode();
			this.UpdateImageIncludeMode();

			if (focusInFilename)
			{
				this.fieldFilename.SelectAll();
				this.fieldFilename.Focus();  // focus pour frapper le nom du fichier à ouvrir
			}
			else
			{
				this.table.Focus();  // focus dans la liste des modèles
			}
		}


		protected void CreateCommandDispatcher()
		{
			this.dispatcher = new CommandDispatcher();
			this.context = new CommandContext();

#if false
			//	Les Druids utilisés ici sont de la forme [MMDL].
			//	MM: numéro de module, voir dans App.DocumentEditor/Resources/App/module.info
			//	D: numéro du développeur (pour le moment, le numéro du développeur est toujours "0")
			//	L: numéro local, voir dans App.DocumentEditor/Resources/App/Captions.00.resource
			this.prevState            = this.CreateCommandState("[3006]", this.NavigatePrev);
			this.nextState            = this.CreateCommandState("[3007]", this.NavigateNext);
			this.parentState          = this.CreateCommandState("[3001]", this.ParentDirectory);
			this.newState             = this.CreateCommandState("[3003]", this.NewDirectory);
			this.renameState          = this.CreateCommandState("[3004]", this.RenameStarting);
			this.deleteState          = this.CreateCommandState("[3005]", this.FileDelete);
			this.favoritesAddState    = this.CreateCommandState("[3008]", this.FavoriteAdd);
			this.favoritesRemoveState = this.CreateCommandState("[3009]", this.FavoriteRemove);
			this.favoritesUpState     = this.CreateCommandState("[300A]", this.FavoriteUp);
			this.favoritesDownState   = this.CreateCommandState("[300B]", this.FavoriteDown);
			this.favoritesBigState    = this.CreateCommandState("[300C]", this.FavoriteBig);
#else
			this.prevState            = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.NavigatePrev, this.NavigatePrev);
			this.nextState            = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.NavigateNext, this.NavigateNext);
			this.parentState          = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.ParentDirectory, this.ParentDirectory);
			this.newState             = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.NewDirectory, this.NewDirectory);
			this.renameState          = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Rename, this.RenameStarting);
			this.deleteState          = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Delete, this.FileDelete);
			this.favoritesAddState    = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Favorites.Add, this.FavoriteAdd);
			this.favoritesRemoveState = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Favorites.Remove, this.FavoriteRemove);
			this.favoritesUpState     = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Favorites.Up, this.FavoriteUp);
			this.favoritesDownState   = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Favorites.Down, this.FavoriteDown);
			this.favoritesBigState    = this.CreateCommandState(Res.Commands.Cmd.Dialog.File.Favorites.Big, this.FavoriteBig);
#endif

			CommandDispatcher.SetDispatcher(this.window, this.dispatcher);
			CommandContext.SetContext(this.window, this.context);
		}

		protected CommandState CreateCommandState(Command command, SimpleCallback handler)
		{
			this.dispatcher.Register(command, handler);

			return this.context.GetCommandState(command);
		}

		protected CommandState CreateCommandState(string commandDruid, SimpleCallback handler)
		{
			Command command = Command.Get(commandDruid);
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

		protected void CreateFavorites()
		{
			//	Crée le panneau de gauche pour les favoris.
			Widget container = new Widget(this.window.Root);
			container.PreferredWidth = 111;
			container.Dock = DockStyle.Left;
			container.Margins = new Margins(0, 10, 0, 0);
			container.TabNavigation = Widget.TabNavigationMode.Passive;

			Widget header = new Widget(container);
			header.PreferredHeight = 20;
			header.Dock = DockStyle.Top;
			header.Margins = new Margins(0, 0, 0, 8);
			header.TabNavigation = Widget.TabNavigationMode.Passive;

			this.favoritesExtend = new GlyphButton(header);
			this.favoritesExtend.GlyphShape = GlyphShape.ArrowDown;
			this.favoritesExtend.AutoFocus = false;
			this.favoritesExtend.TabNavigation = Widget.TabNavigationMode.Passive;
			this.favoritesExtend.Dock = DockStyle.Left;
			this.favoritesExtend.Clicked += new MessageEventHandler(this.HandleFavoritesExtendClicked);
			ToolTip.Default.SetToolTip(this.favoritesExtend, Res.Strings.Dialog.File.Tooltip.Extend.Favorites);

			StaticText label = new StaticText(header);
			label.Text = this.isSave ? Res.Strings.Dialog.File.LabelPath.Save : Res.Strings.Dialog.File.LabelPath.Open;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Dock = DockStyle.Fill;

			this.favorites = new Scrollable(container);
			this.favorites.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.favorites.VerticalScrollerMode = ScrollableScrollerMode.Auto;
			this.favorites.Panel.IsAutoFitting = true;
			this.favorites.IsForegroundFrame = true;
			this.favorites.Dock = DockStyle.Fill;
		}

		protected void CreateTable(double cellHeight)
		{
			//	Crée la table principale contenant la liste des fichiers et dossiers.
			CellArrayStyles sh = CellArrayStyles.Stretch | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile;
			CellArrayStyles sv = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;

			if (this.isMultipleSelection)
			{
				sv |= CellArrayStyles.SelectMulti;
			}

			this.table = new CellTable(this.window.Root);
			this.table.DefHeight = cellHeight;
			this.table.HeaderHeight = 20;
			this.table.StyleH = sh;
			this.table.StyleV = sv;
			this.table.AlphaSeparator = 0.3;
			this.table.Margins = new Margins(0, 0, 0, 0);
			this.table.Dock = DockStyle.Fill;
			this.table.FinalSelectionChanged += new EventHandler(this.HandleTableFinalSelectionChanged);
			this.table.TabIndex = 2;
			this.table.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.table.DoubleClicked += new MessageEventHandler(this.HandleTableDoubleClicked);
			this.table.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
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
			group.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.fieldPath = new TextFieldCombo(group);
			this.fieldPath.IsReadOnly = true;
			this.fieldPath.Dock = DockStyle.Fill;
			this.fieldPath.Margins = new Margins(0, 5, 0, 0);
			this.fieldPath.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleFieldPathComboOpening);
			this.fieldPath.ComboClosed += new EventHandler(this.HandleFieldPathComboClosed);
			this.fieldPath.TextChanged += new EventHandler(this.HandleFieldPathTextChanged);
			this.fieldPath.TabIndex = 1;
			this.fieldPath.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Il faut créer ces boutons dans l'ordre 'de droite à gauche' !
			IconButton buttonDelete = new IconButton(group);
			buttonDelete.AutoFocus = false;
			buttonDelete.TabNavigation = Widget.TabNavigationMode.Passive;
			buttonDelete.CommandObject = this.deleteState.Command;
			buttonDelete.Dock = DockStyle.Right;

			IconButton buttonRename = new IconButton(group);
			buttonRename.AutoFocus = false;
			buttonRename.TabNavigation = Widget.TabNavigationMode.Passive;
			buttonRename.CommandObject = this.renameState.Command;
			buttonRename.Dock = DockStyle.Right;

			IconButton buttonNew = new IconButton(group);
			buttonNew.AutoFocus = false;
			buttonNew.TabNavigation = Widget.TabNavigationMode.Passive;
			buttonNew.CommandObject = this.newState.Command;
			buttonNew.Dock = DockStyle.Right;

			IconSeparator sep = new IconSeparator(group);
			sep.Dock = DockStyle.Right;

			IconButton buttonParent = new IconButton(group);
			buttonParent.AutoFocus = false;
			buttonParent.TabNavigation = Widget.TabNavigationMode.Passive;
			buttonParent.CommandObject = this.parentState.Command;
			buttonParent.Dock = DockStyle.Right;

#if false
			IconButton buttonNext = new IconButton(group);
			buttonNext.AutoFocus = false;
			buttonNext.TabNavigation = Widget.TabNavigationMode.Passive;
			buttonNext.CommandObject = this.nextState.Command;
			buttonNext.Dock = DockStyle.Right;
#endif

			IconButton buttonPrev = new IconButton(group);
			buttonPrev.AutoFocus = false;
			buttonPrev.TabNavigation = Widget.TabNavigationMode.Passive;
			buttonPrev.CommandObject = this.prevState.Command;
			buttonPrev.Dock = DockStyle.Right;
		}

		protected void CreateFilename()
		{
			//	Crée la partie permettant d'éditer le nom de fichier.
			Widget group = new Widget(this.window.Root);
			group.PreferredHeight = 20;
			group.Margins = new Margins(0, 0, 8, 0);
			group.Dock = DockStyle.Bottom;
			group.TabIndex = 3;
			group.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText label = new StaticText(group);
			label.Text = this.isModel ? Res.Strings.Dialog.File.LabelMod : Res.Strings.Dialog.File.LabelDoc;
			label.PreferredWidth = 80;
			label.Dock = DockStyle.Left;

			this.fieldFilename = new TextField(group);
			this.fieldFilename.Dock = DockStyle.Fill;
			this.fieldFilename.KeyboardFocusChanged += this.HandleKeyboardFocusChanged;
			this.fieldFilename.TabIndex = 1;
			this.fieldFilename.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			TextField ext = new TextField(group);
			ext.AutoFocus = false;
			ext.TabNavigation = Widget.TabNavigationMode.Passive;
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
			this.optionsToolbar.TabNavigation = Widget.TabNavigationMode.Passive;
			this.optionsToolbar.Visibility = false;


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
			footer.TabIndex = 5;
			footer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

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
			this.buttonOK.PreferredWidth = 75;
			this.buttonOK.Text = ok;
			this.buttonOK.ButtonStyle = ButtonStyle.DefaultAccept;
			this.buttonOK.Dock = DockStyle.Left;
			this.buttonOK.Margins = new Margins(0, 6, 0, 0);
			this.buttonOK.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonCancel = new Button(footer);
			this.buttonCancel.PreferredWidth = 75;
			this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
			this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
			this.buttonCancel.Dock = DockStyle.Left;
			this.buttonCancel.Margins = new Margins(0, 12, 0, 0);
			this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			if (this.isSave)
			{
				this.optionsExtend = new GlyphButton(footer);
				this.optionsExtend.AutoFocus = false;
				this.optionsExtend.TabNavigation = Widget.TabNavigationMode.Passive;
				this.optionsExtend.Dock = DockStyle.Left;
				this.optionsExtend.Clicked += new MessageEventHandler(this.HandleOptionsExtendClicked);
				ToolTip.Default.SetToolTip(this.optionsExtend, Res.Strings.Dialog.File.Tooltip.Extend.Include);
			}

			this.slider = new HSlider(footer);
			this.slider.AutoFocus = false;
			this.slider.TabNavigation = Widget.TabNavigationMode.Passive;
			this.slider.PreferredWidth = 110;
			this.slider.IsMinMaxButtons = true;
			this.slider.Dock = DockStyle.Right;
			this.slider.Margins = new Margins(0, 0, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 100.0M;
			this.slider.SmallChange = 1.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.Value = (decimal) this.table.DefHeight;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.File.Tooltip.PreviewSize);
		}


		protected void SelectFilenameTable(string filenameToSelect)
		{
			//	Sélectionne et montre un fichier dans la table.
			for (int i=0; i<this.files.Count; i++)
			{
				Item item = this.files[i];
				this.table.SelectRow(i, item.Filename == filenameToSelect);
			}

			if (filenameToSelect != null)
			{
				this.table.ShowSelect();
			}

			this.UpdateButtons();
		}

		protected void UpdateFavorites()
		{
			//	Met à jour le panneau de gauche des favoris.
			this.favoritesBigState.ActiveState = this.globalSettings.FavoritesBig ? ActiveState.Yes : ActiveState.No;

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is Filename)
				{
					Filename f = widget as Filename;
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

			Filename f = new Filename();
			f.FilenameValue = text;
			f.IconValue = Misc.Icon(icon);

			this.FavoritesAdd(item, f);
		}

		protected void FavoritesAdd(FolderId id)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem(id, FolderQueryMode.LargeIcons);

			Filename f = new Filename();
			f.FilenameValue = item.DisplayName;
			f.ImageValue = item.Icon.Image;

			this.FavoritesAdd(item, f);
		}

		protected void FavoritesAdd(FolderItem item, Filename f)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			f.PreferredHeight = (this.favoritesBigState.ActiveState == ActiveState.Yes) ? Common.Widgets.Filename.ExtendedHeight : Common.Widgets.Filename.CompactedHeight;
			f.Name = this.favoritesList.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
			f.Dock = DockStyle.Top;
			f.Clicked += new MessageEventHandler(this.HandleFavoriteClicked);

			if (string.IsNullOrEmpty(item.FullPath))
			{
				ToolTip.Default.SetToolTip(f, TextLayout.ConvertToTaggedText(f.FilenameValue));
			}
			else
			{
				string tooltip = string.Concat(TextLayout.ConvertToTaggedText(f.FilenameValue), "<br/><i>", TextLayout.ConvertToTaggedText(item.FullPath), "</i>");
				ToolTip.Default.SetToolTip(f, tooltip);
			}

			this.favorites.Panel.Children.Add(f);
			this.favoritesList.Add(item);
		}

		protected static VMenu CreateFavoritesMenu()
		{
			//	Crée le menu des commandes pour manipuler les favoris.
			VMenu menu = new VMenu();

			menu.Items.Add(AbstractFile.CreateMenuItem(Res.Commands.Cmd.Dialog.File.Favorites.Add));
			menu.Items.Add(AbstractFile.CreateMenuItem(Res.Commands.Cmd.Dialog.File.Favorites.Remove));
			menu.Items.Add(new MenuSeparator());
			menu.Items.Add(AbstractFile.CreateMenuItem(Res.Commands.Cmd.Dialog.File.Favorites.Up));
			menu.Items.Add(AbstractFile.CreateMenuItem(Res.Commands.Cmd.Dialog.File.Favorites.Down));
			menu.Items.Add(new MenuSeparator());
			menu.Items.Add(AbstractFile.CreateMenuItem(Res.Commands.Cmd.Dialog.File.Favorites.Big));

			menu.AdjustSize();
			return menu;
		}

		protected static MenuItem CreateMenuItem(Command command)
		{
			//	Crée une case d'un menu contenant une commande.
			MenuItem item = new MenuItem();
			item.CommandObject = command;
			return item;
		}

		protected void UpdateSelectedFavorites()
		{
			//	Met à jour le favoris sélectionné selon le chemin d'accès en cours.
			this.favoritesSelected = -1;

			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is Filename)
				{
					Filename f = widget as Filename;

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
			if (this.table == null)
			{
				return;
			}

			this.ListFilenames();
			int rows = this.files.Count;

			this.table.SetArraySize(4, rows);

			this.table.SetWidthColumn(0, 50);
			this.table.SetWidthColumn(1, 90);
			this.table.SetWidthColumn(2, 90);
			this.table.SetWidthColumn(3, 40);

			this.table.SetHeaderTextH(0, Res.Strings.Dialog.File.Header.Preview);
			this.table.SetHeaderTextH(1, Res.Strings.Dialog.File.Header.Filename);
			this.table.SetHeaderTextH(2, Res.Strings.Dialog.File.Header.Description);
			this.table.SetHeaderTextH(3, Res.Strings.Dialog.File.Header.Size);

			this.table.SelectRow(-1, true);

			StaticText st;
			ImageShower im;
			for (int row=0; row<rows; row++)
			{
				for (int column=0; column<this.table.Columns; column++)
				{
					if (this.table[column, row].IsEmpty)
					{
						if (column == 0)  // miniature ?
						{
							im = new ImageShower();
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
						else if (column == 3)  // taille ?
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

				im = this.table[0, row].Children[0] as ImageShower;
				string fixIcon = this.files[row].FixIcon;
				if (fixIcon == null)
				{
					Image image;
					bool icon;
					this.files[row].GetImage(out image, out icon);
					im.DrawingImage = image;
					im.PaintFrame = !icon;
					im.StretchImage = !icon;
					im.FixIcon = null;
				}
				else
				{
					im.FixIcon = Misc.Icon(fixIcon);
					im.DrawingImage = null;
					im.PaintFrame = false;
				}

				st = this.table[1, row].Children[0] as StaticText;
				st.Text = this.files[row].ShortFilename;

				st = this.table[2, row].Children[0] as StaticText;
				st.Text = this.files[row].Description;

				st = this.table[3, row].Children[0] as StaticText;
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

			this.UpdateButtons();
		}

		protected bool UseLargeIcons
		{
			//	Indique si la hauteur des lignes permet l'usage des grandes icônes.
			get
			{
				return (this.table.DefHeight >= 32);
			}
		}

		protected void ListFilenames()
		{
			//	Effectue la liste des fichiers contenus dans le dossier adhoc.
			this.files = new List<Item>();

			if (this.isNewEmtpyDocument)
			{
				this.files.Add(new Item());  // première ligne avec 'nouveau document vide'
			}

			FolderQueryMode mode = this.UseLargeIcons ? FolderQueryMode.LargeIcons : FolderQueryMode.SmallIcons;
			bool showHidden = FolderItem.ShowHiddenFiles;
			bool skipFolders = this.initialFolder.Equals(FileManager.GetFolderItem(FolderId.Recent, FolderQueryMode.NoIcons));
			foreach (FolderItem item in FileManager.GetFolderItems(this.initialFolder, mode))
			{
				if (!item.IsFileSystemNode)
				{
					continue;  // ignore les items qui ne stockent pas des fichiers
				}

				if (item.IsHidden && !showHidden)
				{
					continue;  // ignore les items cachés si l'utilisateur ne veut pas les voir
				}

				if (item.IsShortcut)
				{
					if (skipFolders)
					{
						//	Filtre tout de suite les fichiers que l'on ne sait pas nous intéresser.
						//	En effet, le nom du raccourci se termine par .crdoc.lnk s'il s'agit d'un
						//	document .crdoc.
						string name = item.FullPath.Substring(0, item.FullPath.Length-4);  // nom sans .lnk

						if (!Misc.IsExtension(name, this.fileExtension))  // autre extension ?
						{
							continue;
						}
					}

					//	Vérifie que le fichier existe plutôt que de montrer des raccourcis cassés
					//	qui ne mènent nulle part:
					FolderItem target = FileManager.ResolveShortcut(item, FolderQueryMode.NoIcons);

					if (target.IsFolder)
					{
						//	On liste le dossier.
						if (skipFolders)
						{
							continue;
						}
					}
					else if (target.IsEmpty)
					{
						continue;
					}
					else
					{
						if (!Misc.IsExtension(target.FullPath, this.fileExtension))  // autre extension ?
						{
							continue;  // oui -> ignore ce fichier
						}
					}
				}
				else if (!item.IsFolder)  // fichier ?
				{
					if (!Misc.IsExtension(item.FullPath, this.fileExtension))  // autre extension ?
					{
						continue;  // oui -> ignore ce fichier
					}
				}

				this.files.Add(new Item(item, this.isModel));  // ajoute une ligne à la liste
			}

			this.files.Sort();  // trie toute la liste
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons en fonction du fichier sélectionné dans la liste.
			if (this.renameState == null)
			{
				return;
			}

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

			sel = this.table.SelectedRow;
			bool enable = (sel != -1 && this.files[sel].Filename != Common.Document.Settings.GlobalSettings.NewEmptyDocument && !this.files[sel].IsShortcut);

			if (string.Equals(this.initialFolder.FullPath, Document.DirectoryOriginalSamples, System.StringComparison.OrdinalIgnoreCase))
			{
				enable = false;
			}
			
			this.renameState.Enable = enable;
			this.deleteState.Enable = enable;

			FolderItem parent = FileManager.GetParentFolderItem(this.initialFolder, FolderQueryMode.NoIcons);
			this.parentState.Enable = !parent.IsEmpty;

			this.prevState.Enable = (this.directoriesVisitedIndex > 0);
			this.nextState.Enable = (this.directoriesVisitedIndex < this.directoriesVisited.Count-1);
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

		protected void UpdateInitialFilename()
		{
			//	Met à jour le nom du fichier.
			if (this.fieldFilename != null)
			{
				this.ignoreChanged = true;

				if (string.IsNullOrEmpty(this.initialFilename))
				{
					this.fieldFilename.Text = "";
				}
				else
				{
					this.fieldFilename.Text = TextLayout.ConvertToTaggedText(System.IO.Path.GetFileNameWithoutExtension(this.initialFilename));
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

			this.UpdateTable(-1);
			this.SelectFilenameTable(newDir);
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
					foreach (Item item in this.files)
					{
						if (item.IsDirectory && item.Filename == newDir)
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
			int sel = this.table.SelectedRow;
			if (sel == -1 || this.files[sel].Filename == Common.Document.Settings.GlobalSettings.NewEmptyDocument)
			{
				return;
			}

			//	Construit la liste des fichiers à supprimer.
			List<string> filenamesToDelete = new List<string>();
			for (int i=0; i<this.table.Rows; i++)
			{
				if (this.table.IsCellSelected(i, 0))
				{
					filenamesToDelete.Add(this.files[i].Filename);
				}
			}

			//	Cherche le nom du fichier qu'il faudra sélectionner après la suppression.
			string filenameToSelect = null;

			if (sel < this.files.Count-1)
			{
				filenameToSelect = this.files[sel+1].Filename;
			}
			else
			{
				if (sel > 0)
				{
					filenameToSelect = this.files[sel-1].Filename;
				}
			}

			//	Supprime le ou les fichiers.
			FileOperationMode mode = new FileOperationMode(this.window);
			FileManager.DeleteFiles(mode, filenamesToDelete);

			if (!System.IO.File.Exists(filenamesToDelete[0]))  // fichier n'existe plus (donc bien supprimé) ?
			{
				this.UpdateTable(-1);
				this.SelectFilenameTable(filenameToSelect);
			}
		}

		protected void RenameStarting()
		{
			//	Début d'un renommer. Le widget pour éditer le nom est positionné et
			//	rendu visible.
			System.Diagnostics.Debug.Assert(this.fieldRename != null);
			int sel = this.table.SelectedRow;
			if (sel == -1 || this.files[sel].Filename == Common.Document.Settings.GlobalSettings.NewEmptyDocument)
			{
				return;
			}

			StaticText st = this.table[1, sel].Children[0] as StaticText;
			Rectangle rect = st.MapClientToRoot(st.Client.Bounds);
			rect.Deflate(0, System.Math.Floor((rect.Height-20)/2));		// force une hauteur de 20
			rect.Inflate(st.Margins);									// tient compte de la marge
			rect.Left -= 1;												// déborde d'un pixel hors du tableau
			rect.Width += 32;											// place pour les boutons "v" et "x"

			Rectangle box = this.table.MapClientToRoot(this.table.Client.Bounds);
			box.Deflate(2);
			box.Top -= this.table.HeaderHeight;
			if (!box.Contains(rect))
			{
				return;
			}

			this.focusedWidgetBeforeRename = this.window.FocusedWidget;

			this.fieldRename.SetManualBounds(rect);
			this.fieldRename.Text = this.files[sel].ShortFilename;
			this.fieldRename.SelectAll();
			this.fieldRename.Visibility = true;
			this.fieldRename.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldRename.StartEdition();
			this.fieldRename.Focus();

			this.renameSelected = sel;
		}

		protected void RenameEnding(bool accepted)
		{
			//	Fin d'un renommer. Le fichier ou le dossier est renommé (si accepted = true)
			//	et le widget pour éditer le nom est caché.
			if (!this.fieldRename.Visibility)
			{
				return;
			}

			this.focusedWidgetBeforeRename.Focus();
			this.focusedWidgetBeforeRename = null;
			this.fieldRename.Visibility = false;

			if (accepted && this.renameSelected != -1)
			{
				int sel = this.renameSelected;
				this.renameSelected = -1;
				string srcFilename, dstFilename;
				string newText = TextLayout.ConvertToSimpleText(this.fieldRename.Text);

				if (this.files[sel].IsDirectory)
				{
					srcFilename = this.files[sel].Filename;
					dstFilename = string.Concat(System.IO.Path.GetDirectoryName(srcFilename), "\\", newText);

					FileOperationMode mode = new FileOperationMode(this.window);
					FileManager.RenameFile(mode, srcFilename, dstFilename);

					if (System.IO.Directory.Exists(srcFilename) && !string.Equals(srcFilename, dstFilename, System.StringComparison.CurrentCultureIgnoreCase))
					{
						return;
					}
				}
				else
				{
					srcFilename = this.files[sel].Filename;
					dstFilename = string.Concat(System.IO.Path.GetDirectoryName(srcFilename), "\\", newText, System.IO.Path.GetExtension(srcFilename));

					FileOperationMode mode = new FileOperationMode(this.window);
					FileManager.RenameFile(mode, srcFilename, dstFilename);

					if (System.IO.File.Exists(srcFilename) && !string.Equals(srcFilename, dstFilename, System.StringComparison.CurrentCultureIgnoreCase))
					{
						return;
					}
				}
				
				FolderItem item = FileManager.GetFolderItem(dstFilename, FolderQueryMode.NoIcons);
				this.files[sel].FolderItem = item;

				StaticText st = this.table[1, sel].Children[0] as StaticText;
				st.Text = this.files[sel].ShortFilename;
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
				if (widget is Filename)
				{
					Filename f = widget as Filename;
					f.PreferredHeight = (this.favoritesBigState.ActiveState == ActiveState.Yes) ? Common.Widgets.Filename.ExtendedHeight : Common.Widgets.Filename.CompactedHeight;
				}
			}
		}

		protected bool ActionOK()
		{
			//	Effectue l'action lorsque le bouton 'Ouvrir/Enregistrer' est actionné.
			//	Retourne true s'il faut fermer le dialogue.
			int sel = this.table.SelectedRow;
			if (sel != -1)
			{
				if (this.files[sel].IsDirectory)  // ouvre un dossier ?
				{
					if (this.files[sel].FolderItem.IsShortcut)
					{
						this.SetInitialFolder(FileManager.ResolveShortcut(this.files[sel].FolderItem, FolderQueryMode.NoIcons), true);
					}
					else
					{
						this.SetInitialFolder(this.files[sel].FolderItem, true);
					}

					return false;  // ne pas fermer le dialogue
				}
				else
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

					this.selectedFilenames = new string[selCount];
					int rank = 0;
					for (int i=0; i<this.table.Rows; i++)
					{
						if (this.table.IsCellSelected(i, 0) && !this.files[i].IsDirectory)
						{
							if (rank == 0)
							{
								this.selectedFilename = this.files[i].Filename;  // premier fichier sélectionné
							}

							this.selectedFilenames[rank++] = this.files[i].Filename;
						}
					}

					return this.PromptForOverwriting();
				}
			}

			return false;  // ne pas fermer le dialogue
		}

		protected bool PromptForOverwriting()
		{
			//	Si requis, demande s'il faut écraser le fichier ?
			if (!this.isSave && this.selectedFilename != Common.Document.Settings.GlobalSettings.NewEmptyDocument && !System.IO.File.Exists(this.selectedFilename))  // fichier n'existe pas ?
			{
				string message = string.Format(Res.Strings.Dialog.Question.Open.File, Misc.ExtractName(this.selectedFilename), this.selectedFilename);
				Common.Dialogs.DialogResult result = this.editor.DialogError(message);
				this.selectedFilename = null;
				this.selectedFilenames = null;
				return false;  // ne pas fermer le dialogue
			}

			if (this.isSave && System.IO.File.Exists(this.selectedFilename))  // fichier existe déjà ?
			{
				string message = string.Format(Res.Strings.Dialog.Question.Save.File, Misc.ExtractName(this.selectedFilename), this.selectedFilename);
				Common.Dialogs.DialogResult result = this.editor.DialogQuestion(message);
				if (result != Common.Dialogs.DialogResult.Yes)
				{
					this.selectedFilename = null;
					this.selectedFilenames = null;
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



		private void HandleFavoritesExtendClicked(object sender, MessageEventArgs e)
		{
			//	Clic sur le bouton pour le menu des favoris.
			GlyphButton button = sender as GlyphButton;
			if (button == null)  return;
			VMenu menu = AbstractFile.CreateFavoritesMenu();
			menu.Host = this.window;
			TextFieldCombo.AdjustComboSize(button, menu);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		private void HandleOptionsExtendClicked(object sender, MessageEventArgs e)
		{
			this.optionsToolbar.Visibility = !this.optionsToolbar.Visibility;
			this.UpdateButtons();
		}

		private void HandleFavoriteClicked(object sender, MessageEventArgs e)
		{
			//	Favoris cliqué dans le panneau de gauche.
			Filename f = sender as Filename;
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
			//	Un widget (table ou filename) à pris/perdu le focus.
			bool focused = (bool) e.NewValue;
			if (focused)  // focus pris ?
			{
				this.focusedWidget = sender as Widget;
			}
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
			this.comboFolders = new List<Item>();

#if true
			//	Ajoute toutes les unités du bureau et du poste de travail.
			FolderItem desktop = FileManager.GetFolderItem(FolderId.VirtualDesktop, FolderQueryMode.SmallIcons);
			FolderItem computer = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.SmallIcons);
			bool showHidden = FolderItem.ShowHiddenFiles;

			this.ComboAdd(desktop, null);
			Item root = this.comboFolders[this.comboFolders.Count-1];

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
				Item parent = this.comboFolders[this.comboFolders.Count-1];

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
				Item i1 = this.comboFolders[index];
				Item i2 = this.comboFolders[index+1];
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

			foreach (Item cell in this.comboFolders)
			{
				FolderItemIcon icon = cell.FolderItem.Icon;
				if (cell.SmallIcon == null)
				{
					cell.SmallIcon = FileManager.GetFolderItemIcon(cell.FolderItem, FolderQueryMode.SmallIcons);
				}
				string text = string.Format("<img src=\"{0}\"/> {1}", cell.SmallIcon.ImageName, cell.ShortFilename);
				text = AbstractFile.AddStringIndent(text, cell.Deep);

				this.fieldPath.Items.Add(text);
				this.comboTexts.Add(text);
			}

			this.comboSelected = -1;
		}

		protected void ComboAdd(FolderItem folderItem, Item parent)
		{
			Item item = new Item(folderItem, this.isModel);
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
				this.UpdateTable(-1);
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

			this.table.DefHeight = (double) this.slider.Value;
			this.table.HeaderHeight = 20;

			for (int i=0; i<this.table.Rows; i++)
			{
				this.table.SetHeightRow(i, this.table.DefHeight);
			}

			if (initialMode == this.UseLargeIcons)
			{
				this.table.ShowSelect();
			}
			else
			{
				this.UpdateTable(this.table.SelectedRow);
			}
		}

		protected void HandleWindowCloseClicked(object sender)
		{
			//	Fenêtre fermée.
			this.CloseWindow();
		}

		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Annuler' cliqué.
			this.CloseWindow();
		}

		private void HandleButtonOKClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Ouvrir/Enregistrer' cliqué.
			if (this.focusedWidget is AbstractTextField)  // focus dans un texte éditable ?
			{
				AbstractTextField field = this.focusedWidget as AbstractTextField;
				if (!string.IsNullOrEmpty(field.Text))
				{
					string filename = string.Concat(TextLayout.ConvertToSimpleText(field.Text), this.fileExtension);

					if (!System.IO.Path.IsPathRooted(filename))
					{
						filename = string.Concat(this.initialFolder.FullPath, "\\", filename);
					}

					this.selectedFilename = filename;
					this.selectedFilenames = null;

					if (this.PromptForOverwriting())
					{
						this.CloseWindow();
					}
					return;
				}
			}

			if (this.ActionOK())
			{
				this.CloseWindow();
			}
		}


		#region Class Item
		//	Cette classe représente une 'ligne' dans la liste, qui peut représenter
		//	un fichier, un dossier ou la commande 'nouveau document vide'.
		protected class Item : System.IComparable
		{
			public Item()
			{
				//	Crée un item pour 'Nouveau document vide'.
				this.isNewEmptyDocument = true;
			}

			public Item(FolderItem folderItem, bool isModel)
			{
				//	Crée un item pour un fichier ou un dossier.
				this.folderItem = folderItem;
				this.isModel = isModel;
				this.isNewEmptyDocument = false;
			}

			public FolderItem FolderItem
			{
				get
				{
					return this.folderItem;
				}
				set
				{
					this.folderItem = value;

					if (this.folderItem.QueryMode.IconSize == FileInfoIconSize.Small)
					{
						this.smallIcon = this.folderItem.Icon;
					}
					else
					{
						this.smallIcon = null;
					}
				}
			}

			public FolderItemIcon SmallIcon
			{
				get
				{
					return this.smallIcon;
				}
				set
				{
					this.smallIcon = value;
				}
			}

			public string Filename
			{
				//	Nom du fichier avec le chemin d'accès complet.
				get
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						return Common.Document.Settings.GlobalSettings.NewEmptyDocument;
					}
					else
					{
						if (this.IsShortcut)
						{
							FolderItem item = FileManager.ResolveShortcut(this.folderItem, FolderQueryMode.NoIcons);
							return item.FullPath;
						}
						else
						{
							return this.folderItem.FullPath;
						}
					}
				}
			}

			public string ShortFilename
			{
				//	Nom du fichier court, sans le chemin d'accès ni l'extension.
				get
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						return "—";
					}
					else
					{
						if (this.IsDirectory)
						{
							return TextLayout.ConvertToTaggedText(this.folderItem.DisplayName);
						}
						else if (this.IsShortcut)
						{
							FolderItem item = FileManager.ResolveShortcut(this.folderItem, FolderQueryMode.NoIcons);
							return TextLayout.ConvertToTaggedText(System.IO.Path.GetFileNameWithoutExtension(item.FullPath));
						}
						else
						{
							return TextLayout.ConvertToTaggedText(System.IO.Path.GetFileNameWithoutExtension(this.folderItem.FullPath));
						}
					}
				}
			}

			public bool IsDirectory
			{
				get
				{
					if (this.isNewEmptyDocument)
					{
						return false;
					}

					return this.folderItem.IsFolder;
				}
			}

			public bool IsDirectoryOrShortcut
			{
				get
				{
					if (this.isNewEmptyDocument)
					{
						return false;
					}

					return this.folderItem.IsFolder || this.folderItem.IsShortcut;
				}
			}

			public bool IsShortcut
			{
				get
				{
					if (this.isNewEmptyDocument)
					{
						return false;
					}

					return this.folderItem.IsShortcut;
				}
			}

			public bool IsDrive
			{
				get
				{
					if (this.isNewEmptyDocument)
					{
						return false;
					}

					return this.folderItem.IsDrive;
				}
			}

			public string FileSize
			{
				//	Taille du fichier en kilo-bytes.
				get
				{
					if (this.isNewEmptyDocument || this.IsDirectoryOrShortcut)
					{
						return "";
					}
					else
					{
						System.IO.FileInfo info = new System.IO.FileInfo(this.folderItem.FullPath);
						if (!info.Exists)
						{
							return "";
						}

						long size = info.Length;

						size = (size+512)/1024;
						if (size < 1024)
						{
							double s = (double) size;
							s = System.Math.Floor(s*1000/1024);  // 0..999 KB
							return string.Format(Res.Strings.Dialog.File.Size.Kilo, s.ToString());
						}

						size = (size+512)/1024;
						if (size < 1024)
						{
							double s = (double) size;
							s = System.Math.Floor(s*1000/1024);  // 0..999 MB
							return string.Format(Res.Strings.Dialog.File.Size.Mega, s.ToString());
						}

						size = (size+512)/1024;
						if (size < 1024)
						{
							double s = (double) size;
							s = System.Math.Floor(s*1000/1024);  // 0..999 GB
							return string.Format(Res.Strings.Dialog.File.Size.Giga, s.ToString());
						}

						return "?";
					}
				}
			}

			public string Description
			{
				//	Retourne la description du fichier, basée sur les statistiques si elles existent.
				get
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						return Res.Strings.Dialog.New.EmptyDocument;
					}
					else
					{
						if (this.IsDirectoryOrShortcut)
						{
							return this.folderItem.TypeName;
						}
						else
						{
							Document.Statistics stat = this.Statistics;
							if (stat == null)
							{
								return this.isModel ? Res.Strings.Dialog.File.Model : Res.Strings.Dialog.File.Document;
							}
							else
							{
								return string.Format(Res.Strings.Dialog.File.Statistics, stat.PageFormat, stat.PagesCount.ToString(), stat.LayersCount.ToString(), stat.ObjectsCount.ToString(), stat.ComplexesCount.ToString(), stat.FontsCount.ToString(), stat.ImagesCount.ToString());
							}
						}
					}
				}
			}

			public string FixIcon
			{
				//	Retourne l'éventuelle icône fixe qui remplace l'image miniature.
				get
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						return "New";
					}
					else
					{
						return null;
					}
				}
			}

			public void GetImage(out Image image, out bool icon)
			{
				//	Donne l'image miniature associée au fichier.
				if (this.isNewEmptyDocument)  // nouveau document vide ?
				{
					image = null;
					icon = false;
				}
				else
				{
					if (this.IsDirectoryOrShortcut)
					{
						image = this.folderItem.Icon.Image;
						icon = true;
					}
					else
					{
						DocumentCache.Add(this.folderItem.FullPath);
						image = DocumentCache.Image(this.folderItem.FullPath);
						if (image == null)
						{
							image = this.folderItem.Icon.Image;
							icon = true;
						}
						else
						{
							icon = false;
						}
					}
				}
			}

			protected Document.Statistics Statistics
			{
				//	Retourne les statistiques associées au fichier.
				get
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						return null;
					}
					else
					{
						if (this.IsDirectoryOrShortcut)
						{
							return null;
						}
						else
						{
							DocumentCache.Add(this.folderItem.FullPath);
							return DocumentCache.Statistics(this.folderItem.FullPath);
						}
					}
				}
			}


			public bool SortAccordingToLevel
			{
				//	Indique si le tri doit tenir compte du niveau (lent).
				get
				{
					return this.sortAccordingToLevel;
				}
				set
				{
					this.sortAccordingToLevel = value;
				}
			}

			public Item Parent
			{
				get
				{
					return this.parent;
				}
				set
				{
					this.parent = value;
				}
			}

			protected Item GetParent(int level)
			{
				//	Retourne un parent.
				//	Le niveau 0 correspond au bureau.
				//	Le niveau Deep correspond à l'objet lui-même.
				//	Un niveau supérieur retourne null.
				int deep = this.Deep;
				if (level <= deep)
				{
					Item current = this;
					for (int i=0; i<deep-level; i++)
					{
						current = current.parent;
					}
					return current;
				}
				else
				{
					return null;
				}
			}

			public int Deep
			{
				//	Retourne la profondeur d'imbrication du dossier.
				//	Pour un dossier du bureau, la profondeur est 0.
				//	Pour un dossier du poste de travail, la profondeur est 1.
				get
				{
					int deep = 0;
					Item current = this;
					while (current.parent != null)
					{
						current = current.parent;
						deep++;
					}
					return deep;
				}
			}


			#region IComparable Members
			public int CompareTo(object obj)
			{
				//	Comparaison simple ou complexe, selon SortAccordingToLevel.
				//	En mode complexe (SortAccordingToLevel = true), on cherche
				//	à obtenir cet ordre:
				//		A		(deep = 0)
				//		B		(deep = 0)
				//		B/1		(deep = 1)
				//		B/1/a	(deep = 2)
				//		B/1/b	(deep = 2)
				//		B/2		(deep = 1)
				//		C		(deep = 0)
				if (this.sortAccordingToLevel)
				{
					Item that = obj as Item;

					for (int level=0; level<100; level++)
					{
						Item p1 = this.GetParent(level);
						Item p2 = that.GetParent(level);

						if (p1 == null && p2 == null)
						{
							return this.BaseCompareTo(obj);
						}

						if (p1 == null && p2 != null)
						{
							return -1;
						}

						if (p1 != null && p2 == null)
						{
							return 1;
						}

						int c = p1.BaseCompareTo(p2);
						if (c != 0)
						{
							return c;
						}
					}
				}

				return this.BaseCompareTo(obj);
			}

			protected int BaseCompareTo(object obj)
			{
				//	Comparaison simple, sans tenir compte du niveau.
				Item that = obj as Item;

				if (this.isNewEmptyDocument != that.isNewEmptyDocument)
				{
					return this.isNewEmptyDocument ? -1 : 1;  // 'nouveau document vide' au début
				}

				if (this.IsDrive != that.IsDrive)
				{
					return this.IsDrive ? -1 : 1;  // unités avant les fichiers
				}

				if (this.IsDirectoryOrShortcut != that.IsDirectoryOrShortcut)
				{
					return this.IsDirectoryOrShortcut ? -1 : 1;  // dossiers avant les fichiers
				}

				if (this.IsDrive)
				{
					int ct = this.folderItem.DriveInfo.Name.CompareTo(that.folderItem.DriveInfo.Name);
					if (ct != 0)
					{
						return ct;
					}
				}
				else
				{
					int ct = this.folderItem.TypeName.CompareTo(that.folderItem.TypeName);
					if (ct != 0)
					{
						return ct;
					}
				}

				string f1 = this.ShortFilename.ToLower();
				string f2 = that.ShortFilename.ToLower();
				return f1.CompareTo(f2);
			}
			#endregion

			protected FolderItem				folderItem;
			protected FolderItemIcon			smallIcon;
			protected Item						parent;
			protected bool						isModel;
			protected bool						isNewEmptyDocument;
			protected bool						sortAccordingToLevel = false;
		}
		#endregion


		protected Scrollable				favorites;
		protected GlyphButton				favoritesExtend;
		protected CellTable					table;
		protected HSlider					slider;
		protected TextFieldCombo			fieldPath;
		protected TextField					fieldFilename;
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

		protected string					fileExtension;
		protected bool						isModel = false;
		protected bool						isNavigationEnabled = false;
		protected bool						isMultipleSelection = false;
		protected bool						isNewEmtpyDocument = false;
		protected bool						isSave = false;
		protected bool						isRedirection = false;
		protected FolderItem				initialFolder;
		protected FolderItemIcon			initialSmallIcon;
		protected string					initialFilename;
		protected Document.FontIncludeMode	fontIncludeMode;
		protected Document.ImageIncludeMode	imageIncludeMode;
		protected List<Item>				files;
		protected string					selectedFilename;
		protected string[]					selectedFilenames;
		protected int						renameSelected = -1;
		protected Widget					focusedWidget;
		protected Widget					focusedWidgetBeforeRename;
		protected bool						ignoreChanged = false;
		protected List<FolderItem>			favoritesList;
		protected int						favoritesFixes;
		protected int						favoritesSelected;
		protected List<FolderItem>			directoriesVisited;
		protected int						directoriesVisitedIndex;
		protected List<Item>				comboFolders;
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
	}
}
