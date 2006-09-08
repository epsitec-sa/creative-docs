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
			this.favoritesVisited = new List<FolderItem>();
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
				FolderItem folder;

				if (value == "")  // poste de travail ?
				{
					folder = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
				}
				else
				{
					folder = FileManager.GetFolderItem(value, FolderQueryMode.NoIcons);
				}

				this.SetInitialFolder(folder);
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
				if (this.initialFilename != value)
				{
					this.initialFilename = value;
					this.UpdateInitialFilename();
				}
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


		protected void SetInitialFolder(FolderItem folder)
		{
			//	Change le dossier courant.
			if (!this.IsFavoritesEndWith(this.initialFolder))
			{
				this.favoritesVisited.Add(this.initialFolder);
			}

			this.initialFolder = folder;

			this.UpdateInitialDirectory();
			this.UpdateTable(-1);
			this.UpdateButtons();
		}

		protected bool IsFavoritesEndWith(FolderItem folder)
		{
			if (folder.IsEmpty)
			{
				return true;  // on n'insère jamais un folder vide
			}

			if (this.favoritesVisited.Count == 0)
			{
				return false;
			}
			else
			{
				FolderItem last = this.favoritesVisited[this.favoritesVisited.Count-1];
				//?return (last == folder);  // TODO: dès que Pierre le permettra !
				return false;
			}
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
			this.CreateFooter();
			this.CreateFilename();

			this.UpdateFavorites();
		}

		protected void UpdateAll(int initialSelection, bool focusInFilename)
		{
			//	Mise à jour lorsque les widgets sont déjà créés, avant de montrer le dialogue.
			this.selectedFilename = null;
			this.selectedFilenames = null;
			this.UpdateTable(initialSelection);
			this.UpdateInitialDirectory();
			this.UpdateInitialFilename();
			this.UpdateButtons();

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

			//	Les Druids utilisés ici sont de la forme [MMDL].
			//	MM: numéro de module, voir dans App.DocumentEditor/Resources/App/module.info
			//	D: numéro du développeur (pour le moment, le numéro du développeur est toujours "0")
			//	L: numéro local, voir dans App.DocumentEditor/Resources/App/Captions.00.resource
			this.prevState   = this.CreateCommandState("[3006]", this.NavigatePrev);
			this.nextState   = this.CreateCommandState("[3007]", this.NavigateNext);
			this.parentState = this.CreateCommandState("[3001]", this.ParentDirectory);
			this.newState    = this.CreateCommandState("[3003]", this.NewDirectory);
			this.renameState = this.CreateCommandState("[3004]", this.RenameStarting);
			this.deleteState = this.CreateCommandState("[3005]", this.FileDelete);

			CommandDispatcher.SetDispatcher(this.window, this.dispatcher);
			CommandContext.SetContext(this.window, this.context);
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
			this.favorites = new Scrollable(this.window.Root);
			this.favorites.PreferredWidth = 100;
			this.favorites.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.favorites.VerticalScrollerMode = ScrollableScrollerMode.Auto;
			this.favorites.Panel.IsAutoFitting = true;
			this.favorites.IsForegroundFrame = true;
			this.favorites.Dock = DockStyle.Left;
			this.favorites.Margins = new Margins(0, 10, 0, 0);
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
			this.table.TabIndex = this.tabIndex++;
			this.table.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.table.DoubleClicked += new MessageEventHandler(this.HandleTableDoubleClicked);
			this.table.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleKeyboardFocusChanged);
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
			this.fieldRename.IsFocusedChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleRenameFocusChanged);
		}

		protected void CreateAccess()
		{
			//	Crée la partie controlant le chemin d'accès.
			Widget group = new Widget(this.window.Root);
			group.PreferredHeight = 20;
			group.Margins = new Margins(0, 0, 0, 8);
			group.Dock = DockStyle.Top;

			StaticText label = new StaticText(group);
			label.Text = Res.Strings.Dialog.Open.LabelPath;
			label.PreferredWidth = 80;
			label.Dock = DockStyle.Left;

			this.fieldPath = new TextFieldCombo(group);
			this.fieldPath.IsReadOnly = true;
			this.fieldPath.Dock = DockStyle.Fill;
			this.fieldPath.Margins = new Margins(0, 5, 0, 0);
			this.fieldPath.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleFieldPathComboOpening);
			this.fieldPath.ComboClosed += new EventHandler(this.HandleFieldPathComboClosed);
			this.fieldPath.TextChanged += new EventHandler(this.HandleFieldPathTextChanged);

			//	Il faut créer ces boutons dans l'ordre 'de droite à gauche' !
			IconButton buttonDelete = new IconButton(group);
			buttonDelete.CommandObject = this.deleteState.Command;
			buttonDelete.Dock = DockStyle.Right;

			IconButton buttonRename = new IconButton(group);
			buttonRename.CommandObject = this.renameState.Command;
			buttonRename.Dock = DockStyle.Right;

			IconButton buttonNew = new IconButton(group);
			buttonNew.CommandObject = this.newState.Command;
			buttonNew.Dock = DockStyle.Right;

			IconButton buttonParent = new IconButton(group);
			buttonParent.CommandObject = this.parentState.Command;
			buttonParent.Dock = DockStyle.Right;

			//IconButton buttonNext = new IconButton(group);
			//buttonNext.CommandObject = this.nextState.Command;
			//buttonNext.Dock = DockStyle.Right;

			IconButton buttonPrev = new IconButton(group);
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

			StaticText label = new StaticText(group);
			label.Text = this.isModel ? Res.Strings.Dialog.Open.LabelMod : Res.Strings.Dialog.Open.LabelDoc;
			label.PreferredWidth = 80;
			label.Dock = DockStyle.Left;

			this.fieldFilename = new TextField(group);
			this.fieldFilename.Dock = DockStyle.Fill;
			this.fieldFilename.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleKeyboardFocusChanged);

			TextField ext = new TextField(group);
			ext.IsReadOnly = true;
			ext.Text = this.fileExtension;
			ext.PreferredWidth = 50;
			ext.Margins = new Margins(1, 0, 0, 0);
			ext.Dock = DockStyle.Right;
		}

		protected void CreateFooter()
		{
			//	Crée le pied du dialogue, avec les boutons 'ouvrir/enregistrer' et 'annuler'.
			Widget footer = new Widget(this.window.Root);
			footer.PreferredHeight = 22;
			footer.Margins = new Margins(0, 0, 8, 0);
			footer.Dock = DockStyle.Bottom;

			this.buttonOK = new Button(footer);
			this.buttonOK.PreferredWidth = 75;
			this.buttonOK.Text = this.isSave ? Res.Strings.Dialog.File.Button.Save : Res.Strings.Dialog.File.Button.Open;
			this.buttonOK.ButtonStyle = ButtonStyle.DefaultAccept;
			this.buttonOK.Dock = DockStyle.Left;
			this.buttonOK.Margins = new Margins(0, 6, 0, 0);
			this.buttonOK.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
			this.buttonOK.TabIndex = this.tabIndex++;
			this.buttonOK.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonCancel = new Button(footer);
			this.buttonCancel.PreferredWidth = 75;
			this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
			this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
			this.buttonCancel.Dock = DockStyle.Left;
			this.buttonCancel.Margins = new Margins(0, 6, 0, 0);
			this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
			this.buttonCancel.TabIndex = this.tabIndex++;
			this.buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.slider = new HSlider(footer);
			this.slider.PreferredWidth = 80;
			this.slider.Dock = DockStyle.Right;
			this.slider.Margins = new Margins(0, 0, 4, 4);
			this.slider.TabIndex = this.tabIndex++;
			this.slider.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
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

			string path;

			if (!this.isSave)
			{
				path = string.Concat(Common.Support.Globals.Directories.Executable, "\\Samples");
				this.FavoritesAdd("Exemples Epsitec", "FileTypeEpsitecSamples", path);
			}

			path = Common.Support.Globals.Directories.UserAppData;
			int i = path.LastIndexOf("\\");
			if (i > 0)
			{
				path = path.Substring(0, i);  // supprime le dossier "1.0.0.0" à la fin
			}
			this.FavoritesAdd("Mes exemples", "FileTypeMySamples", path);

			this.FavoritesAdd(FolderId.VirtualDesktop);      // Bureau
			this.FavoritesAdd(FolderId.VirtualMyDocuments);  // Mes documents
			this.FavoritesAdd(FolderId.VirtualMyComputer);   // Poste de travail
			this.FavoritesAdd(FolderId.VirtualNetwork);      // Favoris réseau
		}

		protected void FavoritesAdd(string text, string icon, string path)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem(path, FolderQueryMode.LargeIcons);

			Filename f = new Filename();
			f.Name = this.favoritesList.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
			f.FilenameValue = text;
			f.IconValue = Misc.Icon(icon);
			f.Dock = DockStyle.Top;
			f.Clicked += new MessageEventHandler(this.HandleFavoriteClicked);
			this.favorites.Panel.Children.Add(f);

			this.favoritesList.Add(item);
		}

		protected void FavoritesAdd(FolderId id)
		{
			//	Ajoute un favoris dans le panneau de gauche.
			FolderItem item = FileManager.GetFolderItem(id, FolderQueryMode.LargeIcons);

			Filename f = new Filename();
			f.Name = this.favoritesList.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
			f.FilenameValue = item.DisplayName;
			f.ImageValue = item.Icon;
			f.Dock = DockStyle.Top;
			f.Clicked += new MessageEventHandler(this.HandleFavoriteClicked);
			this.favorites.Panel.Children.Add(f);

			this.favoritesList.Add(item);
		}

		protected void UpdateSelectedFavorites()
		{
			//	Met à jour le favoris sélectionné selon le chemin d'accès en cours.
			foreach (Widget widget in this.favorites.Panel.Children.Widgets)
			{
				if (widget is Filename)
				{
					Filename f = widget as Filename;

					int i = System.Int32.Parse(f.Name, System.Globalization.CultureInfo.InvariantCulture);
					bool active = (this.favoritesList[i].FullPath == this.initialFolder.FullPath);
					f.ActiveState = active ? ActiveState.Yes : ActiveState.No;
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
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 2)  // résumé ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 6, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 3)  // taille ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleRight;
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
					im.DrawingImage = this.files[row].Image;
					im.PaintFrame = !this.files[row].IsDirectory;
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

		protected void ListFilenames()
		{
			//	Effectue la liste des fichiers contenus dans le dossier adhoc.
			this.files = new List<Item>();

			if (this.isNewEmtpyDocument)
			{
				this.files.Add(new Item());  // première ligne avec 'nouveau document vide'
			}

			foreach (FolderItem item in FileManager.GetFolderItems(this.initialFolder, FolderQueryMode.SmallIcons))
			{
				if (!item.IsFolder)  // fichier ?
				{
					string ext = System.IO.Path.GetExtension(item.FullPath);
					if (ext != this.fileExtension)  // autre extension ?
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
			if (this.renameState != null &&
				this.deleteState != null)
			{
				int sel = this.table.SelectedRow;
				bool enable = (sel != -1 && this.files[sel].Filename != Common.Document.Settings.GlobalSettings.NewEmptyDocument);
				
				this.renameState.Enable = enable;
				this.deleteState.Enable = enable;
			}

			if (this.parentState != null)
			{
				FolderItem parent = FileManager.GetParentFolderItem(this.initialFolder, FolderQueryMode.NoIcons);
				this.parentState.Enable = !parent.IsEmpty;
			}

			if (this.prevState != null)
			{
				this.prevState.Enable = (this.favoritesVisited.Count > 0);
			}
		}

		protected void UpdateInitialDirectory()
		{
			//	Met à jour le chemin d'accès.
			if (this.fieldPath != null)
			{
				this.ignoreChanged = true;
				//?this.fieldPath.Text = AbstractFile.RemoveStartingIndent(AbstractFile.GetIllustredPath(this.initialFolder.FullPath));
				this.fieldPath.Text = this.initialFolder.DisplayName;
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


		protected void NavigatePrev()
		{
			if (this.favoritesVisited.Count == 0)
			{
				return;
			}

			this.SetInitialFolder(this.favoritesVisited[this.favoritesVisited.Count-1]);
			this.favoritesVisited.RemoveAt(this.favoritesVisited.Count-1);
			this.favoritesVisited.RemoveAt(this.favoritesVisited.Count-1);
			this.UpdateButtons();
		}

		protected void NavigateNext()
		{
		}

		protected void ParentDirectory()
		{
			//	Remonte dans le dossier parent.
			FolderItem parent = FileManager.GetParentFolderItem(this.initialFolder, FolderQueryMode.NoIcons);
			if (parent.IsEmpty)
			{
				return;
			}

			this.SetInitialFolder(parent);
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

			string message;
			if (this.files[sel].IsDirectory)
			{
				message = string.Format(Res.Strings.Dialog.Delete.Directory, this.files[sel].ShortFilename, this.files[sel].Filename);
			}
			else
			{
				message = string.Format(Res.Strings.Dialog.Delete.File, this.files[sel].ShortFilename, this.files[sel].Filename);
			}

			Common.Dialogs.DialogResult result = this.editor.DialogQuestion(this.editor.CommandDispatcher, message);
			if ( result != Common.Dialogs.DialogResult.Yes )
			{
				return;
			}

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

			//	TODO: comment supprimer en mettant dans la corbeille ?
			if (this.files[sel].IsDirectory)
			{
				string directory = this.files[sel].Filename;
				System.IO.Directory.Delete(directory, true);
			}
			else
			{
				string filename = this.files[sel].Filename;
				System.IO.File.Delete(filename);
			}

			this.UpdateTable(-1);
			this.SelectFilenameTable(filenameToSelect);
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
			Rectangle rect = st.MapClientToRoot(st.ActualBounds);
			rect.Deflate(0, System.Math.Floor((rect.Height-20)/2));  // force une hauteur de 20
			rect.Offset(-13, 0);  // TODO: mystère...
			rect.Width += 38;  // place pour les boutons "v" et "x"

			//Rectangle box = this.table.MapClientToRoot(this.table.ActualBounds);  // TODO: pourquoi ça ne marche pas ???
			Rectangle box = this.table.ActualBounds;
			box.Deflate(2);
			box.Top -= this.table.HeaderHeight;
			if (!box.Contains(rect))
			{
				return;
			}

			this.buttonOK.ButtonStyle = ButtonStyle.Normal;
			this.buttonCancel.ButtonStyle = ButtonStyle.Normal;  // TODO: ne fonctionne pas !

			this.fieldRename.SetManualBounds(rect);
			this.fieldRename.Text = this.files[sel].ShortFilename;
			this.fieldRename.SelectAll();
			this.fieldRename.Visibility = true;
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

			this.fieldRename.Visibility = false;

			this.buttonOK.ButtonStyle = ButtonStyle.DefaultAccept;
			this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;

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

					try
					{
						System.IO.Directory.Move(srcFilename, dstFilename);
					}
					catch
					{
						return;
					}
				}
				else
				{
					srcFilename = this.files[sel].Filename;
					dstFilename = string.Concat(System.IO.Path.GetDirectoryName(srcFilename), "\\", newText, System.IO.Path.GetExtension(srcFilename));

					try
					{
						System.IO.File.Move(srcFilename, dstFilename);
					}
					catch
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

		protected bool ActionOK()
		{
			//	Effectue l'action lorsque le bouton 'Ouvrir/Enregistrer' est actionné.
			//	Retourne true s'il faut fermer le dialogue.
			int sel = this.table.SelectedRow;
			if (sel != -1)
			{
				if (this.files[sel].IsDirectory)  // ouvre un dossier ?
				{
					this.SetInitialFolder(this.files[sel].FolderItem);
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
			if (this.isSave && System.IO.File.Exists(this.selectedFilename))  // fichier existe déjà ?
			{
				string message = string.Format(Res.Strings.Dialog.Save.File, Misc.ExtractName(this.selectedFilename), this.selectedFilename);
				Common.Dialogs.DialogResult result = this.editor.DialogQuestion(this.editor.CommandDispatcher, message);
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



		private void HandleFavoriteClicked(object sender, MessageEventArgs e)
		{
			//	Favoris cliqué dans le panneau de gauche.
			Filename f = sender as Filename;
			int i = System.Int32.Parse(f.Name, System.Globalization.CultureInfo.InvariantCulture);
			this.SetInitialFolder(this.favoritesList[i]);
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

		private void HandleRenameFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Le TextFieldEx pour renommer a pris/perdu le focus.
			bool focused = (bool) e.NewValue;
			if (!focused)  // focus perdu ?
			{
				this.RenameEnding(true);
			}

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
			this.comboTexts = new List<string>();
			this.comboFolders = new List<FolderItem>();

			this.fieldPath.Items.Clear();

			FolderItem computer = FileManager.GetFolderItem(FolderId.VirtualMyComputer, FolderQueryMode.NoIcons);
			FolderItem desktop = FileManager.GetFolderItem(FolderId.VirtualDesktop, FolderQueryMode.NoIcons);
			foreach (FolderItem item in FileManager.GetFolderItems(desktop, FolderQueryMode.NoIcons))
			{
				if (!item.IsFolder)
				{
					continue;
				}

				string text = item.DisplayName;
				this.fieldPath.Items.Add(text);
				this.comboTexts.Add(text);
				this.comboFolders.Add(item);

				if (item.DisplayName == computer.DisplayName)
				{
					foreach (FolderItem subItem in FileManager.GetFolderItems(item, FolderQueryMode.NoIcons))
					{
						if (!subItem.IsFolder)
						{
							continue;
						}

						text = AbstractFile.AddStringIndent(subItem.DisplayName, 1);
						this.fieldPath.Items.Add(text);
						this.comboTexts.Add(text);
						this.comboFolders.Add(subItem);
					}
				}

#if false
				if (this.initialFolder.FullPath.StartsWith(item.FullPath))
				{
					int index = this.fieldPath.Items.Count;
					int indent = 1;
					FolderItem current = this.initialFolder;
					while (item.FullPath != current.FullPath)
					{
						text = AbstractFile.AddStringIndent(current.DisplayName, indent++);
						this.fieldPath.Items.Insert(index, text);
						this.comboTexts.Insert(index, text);
						this.comboFolders.Insert(index, current);

						current = FileManager.GetParentFolderItem(current, FolderQueryMode.NoIcons);
					}
				}
#endif
			}

			this.comboSelected = -1;
		}

		private void HandleFieldPathComboClosed(object sender)
		{
			//	Le menu pour le chemin d'accès a été fermé.
			if (this.comboSelected != -1)
			{
				this.SetInitialFolder(this.comboFolders[this.comboSelected]);
				this.UpdateTable(-1);
			}
		}

		void HandleFieldPathTextChanged(object sender)
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

		private void HandleSliderChanged(object sender)
		{
			//	Slider pour la taille des miniatures changé.
			this.table.DefHeight = (double) this.slider.Value;
			this.table.HeaderHeight = 20;

			for (int i=0; i<this.table.Rows; i++)
			{
				this.table.SetHeightRow(i, this.table.DefHeight);
			}

			this.table.ShowSelect();
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
					string filename = string.Concat(this.initialFolder.FullPath, "\\", TextLayout.ConvertToSimpleText(field.Text), this.fileExtension);
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
						return this.folderItem.FullPath;
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
						if (this.folderItem.IsFolder)
						{
							return TextLayout.ConvertToTaggedText(this.folderItem.DisplayName);
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

			public string FileSize
			{
				//	Taille du fichier en kilo-bytes.
				get
				{
					if (this.isNewEmptyDocument || this.IsDirectory)
					{
						return "";
					}
					else
					{
						System.IO.FileInfo info = new System.IO.FileInfo(this.folderItem.FullPath);

						long size = info.Exists ? info.Length : 0;
						
						size = (size+500)/1024;
						return string.Format(Res.Strings.Dialog.File.Size, size.ToString());
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
						if (this.IsDirectory)
						{
							return Res.Strings.Dialog.File.Directory;
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

			public Image Image
			{
				//	Retourne l'image miniature associée au fichier.
				get
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						return null;
					}
					else
					{
						if (this.IsDirectory)
						{
							return this.folderItem.Icon;
						}
						else
						{
							byte[] data = this.ReadPreview();
							if (data != null)
							{
								return Bitmap.FromData(data);
							}

							return null;
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
						if (this.IsDirectory)
						{
							return null;
						}
						else
						{
							byte[] data = this.ReadStatistics();
							if (data != null)
							{
								Document.Statistics stat = new Document.Statistics();
								stat = Serialization.DeserializeFromMemory(data) as Document.Statistics;
								return stat;
							}

							return null;
						}
					}
				}
			}

			protected byte[] ReadPreview()
			{
				//	Lit les données de l'image miniature associée au fichier.
				ZipFile zip = new ZipFile();

				if (zip.TryLoadFile(this.folderItem.FullPath))
				{
					try
					{
						return zip["preview.png"].Data;  // lit les données dans le fichier zip
					}
					catch
					{
						return null;
					}
				}

				return null;
			}

			protected byte[] ReadStatistics()
			{
				//	Lit les données des statistiques associée au fichier.
				ZipFile zip = new ZipFile();

				if (zip.TryLoadFile(this.folderItem.FullPath))
				{
					try
					{
						return zip["statistics.data"].Data;  // lit les données dans le fichier zip
					}
					catch
					{
						return null;
					}
				}

				return null;
			}

			#region IComparable Members
			public int CompareTo(object obj)
			{
				Item that = obj as Item;

				if (this.isNewEmptyDocument != that.isNewEmptyDocument)
				{
					return this.isNewEmptyDocument ? -1 : 1;  // 'nouveau document vide' au début
				}

				if (this.IsDirectory != that.IsDirectory)
				{
					return this.IsDirectory ? -1 : 1;  // dossiers avant les fichiers
				}

				int r = this.folderItem.TypeName.CompareTo(that.folderItem.TypeName);
				if (r != 0)
				{
					return r;
				}

				string f1 = this.ShortFilename.ToLower();
				string f2 = that.ShortFilename.ToLower();
				return f1.CompareTo(f2);
			}
			#endregion

			protected FolderItem				folderItem;
			protected bool						isModel;
			protected bool						isNewEmptyDocument;
		}
		#endregion


		protected Scrollable				favorites;
		protected CellTable					table;
		protected HSlider					slider;
		protected TextFieldCombo			fieldPath;
		protected TextField					fieldFilename;
		protected TextFieldEx				fieldRename;
		protected Button					buttonOK;
		protected Button					buttonCancel;

		protected string					fileExtension;
		protected bool						isModel = false;
		protected bool						isNavigationEnabled = false;
		protected bool						isMultipleSelection = false;
		protected bool						isNewEmtpyDocument = false;
		protected bool						isSave = false;
		protected FolderItem				initialFolder;
		protected string					initialFilename;
		protected List<Item>				files;
		protected string					selectedFilename;
		protected string[]					selectedFilenames;
		protected int						tabIndex;
		protected int						renameSelected = -1;
		protected Widget					focusedWidget;
		protected bool						ignoreChanged = false;
		protected List<FolderItem>			favoritesList;
		protected List<FolderItem>			favoritesVisited;
		protected List<FolderItem>			comboFolders;
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
	}
}
