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
			get
			{
				return this.initialDirectory;
			}
			set
			{
				if (this.initialDirectory != value)
				{
					this.initialDirectory = value;
					this.UpdateInitialDirectory();
				}
			}
		}

		public string InitialFilename
		{
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


		protected void CreateCommandDispatcher()
		{
			return;  //?

			this.dispatcher = new CommandDispatcher();
			this.context = new CommandContext();

			this.renameState = this.CreateCommandState("Cmd.Dialog.File.Rename", this.HandleCommandRename);

			CommandDispatcher.SetDispatcher(this.window, this.dispatcher);
		}

		protected CommandState CreateCommandState(string commandName, CommandEventHandler handler)
		{
			Command command = Command.Get("[04]");
			this.dispatcher.Register(command, handler);

			return this.context.GetCommandState(commandName);
		}

		private void HandleCommandRename(CommandDispatcher d, CommandEventArgs e)
		{
			int i=123;
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

			this.buttonDelete = new IconButton(group);
			this.buttonDelete.IconName = Misc.Icon("FileDelete");
			this.buttonDelete.Dock = DockStyle.Right;
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDeleteClicked);
			ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Dialog.Open.Delete);

			this.buttonRename = new IconButton(group);
			this.buttonRename.IconName = Misc.Icon("FileRename");
			this.buttonRename.Dock = DockStyle.Right;
			this.buttonRename.Clicked += new MessageEventHandler(this.HandleButtonRenameClicked);
			ToolTip.Default.SetToolTip(this.buttonRename, Res.Strings.Dialog.Open.Rename);

			this.buttonNew = new IconButton(group);
			this.buttonNew.IconName = Misc.Icon("NewDirectory");
			this.buttonNew.Dock = DockStyle.Right;
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNewClicked);
			ToolTip.Default.SetToolTip(this.buttonNew, Res.Strings.Dialog.Open.NewDirectory);

			this.buttonParent = new IconButton(group);
			this.buttonParent.IconName = Misc.Icon("ParentDirectory");
			this.buttonParent.Dock = DockStyle.Right;
			this.buttonParent.Clicked += new MessageEventHandler(this.HandleButtonParentClicked);
			ToolTip.Default.SetToolTip(this.buttonParent, Res.Strings.Dialog.Open.ParentDirectory);
		}

		protected void CreateFilename()
		{
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
			//	Crée le pied du dialogue, avec les boutons 'ouvrir' et 'annuler'.
			Widget footer = new Widget(this.window.Root);
			footer.PreferredHeight = 22;
			footer.Margins = new Margins(0, 0, 8, 0);
			footer.Dock = DockStyle.Bottom;

			Button buttonOK = new Button(footer);
			buttonOK.PreferredWidth = 75;
			buttonOK.Text = this.isSave ? Res.Strings.Dialog.File.Button.Save : Res.Strings.Dialog.File.Button.Open;
			buttonOK.ButtonStyle = ButtonStyle.DefaultAccept;
			buttonOK.Dock = DockStyle.Left;
			buttonOK.Margins = new Margins(0, 6, 0, 0);
			buttonOK.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
			buttonOK.TabIndex = this.tabIndex++;
			buttonOK.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			Button buttonCancel = new Button(footer);
			buttonCancel.PreferredWidth = 75;
			buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
			buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
			buttonCancel.Dock = DockStyle.Left;
			buttonCancel.Margins = new Margins(0, 6, 0, 0);
			buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
			buttonCancel.TabIndex = this.tabIndex++;
			buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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

		protected void UpdateTable(int sel)
		{
			//	Met à jour la table des fichiers.
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
							im.CrossIfNoImage = false;
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
					im.FixIcon = null;
				}
				else
				{
					im.FixIcon = Misc.Icon(fixIcon);
					im.DrawingImage = null;
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
			//	Effectue la liste des fichiers .crmod contenus dans le dossier adhoc.
			this.files = new List<Item>();

			if (this.isNewEmtpyDocument)
			{
				this.files.Add(new Item(null, false, this.isModel));  // première ligne avec 'nouveau document vide'
			}

			if (this.isNavigationEnabled)
			{
				string[] directories;

				try
				{
					directories = System.IO.Directory.GetDirectories(this.initialDirectory, "*", SearchOption.TopDirectoryOnly);
				}
				catch
				{
					directories = null;
				}

				if (directories != null)
				{
					foreach (string directory in directories)
					{
						this.files.Add(new Item(directory, true, this.isModel));
					}
				}
			}

			string[] filenames;

			try
			{
				filenames = System.IO.Directory.GetFiles(this.initialDirectory, "*"+this.fileExtension, System.IO.SearchOption.TopDirectoryOnly);
			}
			catch
			{
				filenames = null;
			}

			if (filenames != null)
			{
				foreach (string filename in filenames)
				{
					this.files.Add(new Item(filename, false, this.isModel));
				}
			}
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons en fonction du fichier sélectionné dans la liste.
			if (this.buttonRename != null)
			{
				int sel = this.table.SelectedRow;
				bool enable = (sel != -1 && this.files[sel].Filename != Common.Document.Settings.GlobalSettings.NewEmptyDocument);
				this.buttonRename.Enable = enable;
				this.buttonDelete.Enable = enable;
			}
		}

		protected void UpdateInitialDirectory()
		{
			//	Met à jour le chemin d'accès.
			if (this.fieldPath != null)
			{
				this.ignoreChanged = true;
				this.fieldPath.Text = AbstractFile.RemoveStartingSpaces(AbstractFile.GetIllustredPath(this.initialDirectory));
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
					this.fieldFilename.Text = System.IO.Path.GetFileNameWithoutExtension(this.initialFilename);
				}

				this.ignoreChanged = false;
			}
		}


		protected void ParentDirectory()
		{
			//	Remonte dans le dossier parent.
			int index = this.initialDirectory.LastIndexOf("\\");
			if (index != -1)
			{
				string dir = this.initialDirectory.Substring(0, index);
				if (dir.Length == 2)  // "C:" ?
				{
					dir += "\\";  // toujours la forme "C:\\"
				}

				this.InitialDirectory = dir;
				this.UpdateTable(-1);
			}
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
					string newDir = string.Concat(this.initialDirectory, "\\", Res.Strings.Dialog.File.NewDirectoryName);
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
			this.fieldRename.Visibility = false;

			if (accepted && this.renameSelected != -1)
			{
				int sel = this.renameSelected;
				this.renameSelected = -1;
				string srcFilename, dstFilename;

				if (this.files[sel].IsDirectory)
				{
					srcFilename = this.files[sel].Filename;
					dstFilename = string.Concat(System.IO.Path.GetDirectoryName(srcFilename), "\\", this.fieldRename.Text);

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
					dstFilename = string.Concat(System.IO.Path.GetDirectoryName(srcFilename), "\\", this.fieldRename.Text, System.IO.Path.GetExtension(srcFilename));

					try
					{
						System.IO.File.Move(srcFilename, dstFilename);
					}
					catch
					{
						return;
					}
				}

				this.files[sel].Filename = dstFilename;

				StaticText st = this.table[1, sel].Children[0] as StaticText;
				st.Text = this.files[sel].ShortFilename;
			}
		}

		protected bool ActionOpen()
		{
			//	Effectue l'action lorsque le bouton 'Ouvrir' est actionné.
			//	Retourne true s'il faut fermer le dialogue.
			int sel = this.table.SelectedRow;
			if (sel != -1)
			{
				if (this.files[sel].IsDirectory)  // ouvre un dossier ?
				{
					this.InitialDirectory = this.files[sel].Filename;
					this.UpdateTable(-1);
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


		protected static string GetIllustredPath(string path)
		{
			//	Retourne le chemin illustré.
			if (path.Length == 3 && path.EndsWith(":\\"))  // "C:\" ?
			{
				System.IO.DriveType type = AbstractFile.GetDriveType(path);
				return string.Concat(AbstractFile.GetImageDriveType(type), " ", path);
			}

			if (path.EndsWith(":\\)"))  // "Travail (D:\)" ?
			{
				string drive = path.Substring(path.Length-4, 3);  // garde "D:\"
				System.IO.DriveType type = AbstractFile.GetDriveType(drive);
				return string.Concat(AbstractFile.GetImageDriveType(type), " ", path);
			}

			string[] dirs = path.Split('\\');
			if (dirs.Length != 0)
			{
				string text = "";
				for (int i=0; i<dirs.Length-1; i++)
				{
					text += "   ";
				}

				text += Misc.Image("FileTypeDirectory");
				text += " ";
				text += dirs[dirs.Length-1];
				return text;
			}

			return path;
		}

		protected static System.IO.DriveType GetDriveType(string drive)
		{
			return DriveType.Fixed;
		}

		protected static string GetImageDriveType(System.IO.DriveType type)
		{
			switch (type)
			{
				case DriveType.CDRom:
					return Misc.Image("FileTypeCDRom");

				case DriveType.Network:
					return Misc.Image("FileTypeNetword");

				case DriveType.Removable:
					return Misc.Image("FileTypeRemovable");
			}

			return Misc.Image("FileTypeFixed");
		}

		protected static string GetIllustredDriveString(System.IO.DriveInfo drive)
		{
			//	Retourne le texte illustré à utiliser pour un drive donné.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			builder.Append(AbstractFile.GetImageDriveType(drive.DriveType));
			builder.Append(" ");

			try
			{
				builder.Append(drive.VolumeLabel);
				builder.Append(" (");
				builder.Append(drive.Name);
				builder.Append(")");
			}
			catch
			{
				builder.Append(drive.Name);
			}

			return builder.ToString();
		}

		protected static string RemoveStartingSpaces(string text)
		{
			//	Supprime tous les espaces au début d'un texte.
			while (text.StartsWith(" "))
			{
				text = text.Substring(1);
			}

			return text;
		}

		protected static string RemoveTagImage(string text)
		{
			//	Supprime le tag "<img ... />" contenu dans un texte.
			int start = text.IndexOf("<img ");
			if (start != -1)
			{
				int end = text.IndexOf("/>", start);
				if (end != -1)
				{
					text = text.Remove(start, end-start+2);
				}
			}

			return text;
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
			if (this.ActionOpen())
			{
				this.CloseWindow();
			}
		}

		private void HandleFieldPathComboOpening(object sender, CancelEventArgs e)
		{
			//	Le menu pour le chemin d'accès va être ouvert.
			this.comboTexts = new List<string>();
			this.comboDirectories = new List<string>();

			System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

			this.fieldPath.Items.Clear();

			foreach (System.IO.DriveInfo drive in drives)
			{
				string text = AbstractFile.GetIllustredDriveString(drive);
				this.fieldPath.Items.Add(text);
				this.comboTexts.Add(text);
				this.comboDirectories.Add(drive.Name);

				if (this.initialDirectory.Length > 3 && this.initialDirectory.StartsWith(drive.Name))
				{
					string[] dirs = this.initialDirectory.Split('\\');
					for (int i=1; i<dirs.Length; i++)
					{
						string dir = "";
						text = "";
						for (int j=0; j<=i; j++)
						{
							dir += dirs[j]+"\\";
							text += "   ";
						}
						text += Misc.Image("FileTypeDirectory");
						text += " ";
						text += dirs[i];

						this.fieldPath.Items.Add(text);
						this.comboTexts.Add(text);

						if (dir.Length > 3 && dir.EndsWith("\\"))
						{
							dir = dir.Substring(0, dir.Length-1);
						}
						this.comboDirectories.Add(dir);
					}
				}
			}

			this.comboSelected = -1;
		}

		private void HandleFieldPathComboClosed(object sender)
		{
			//	Le menu pour le chemin d'accès a été fermé.
			if (this.comboSelected != -1)
			{
				this.InitialDirectory = this.comboDirectories[this.comboSelected];
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
			this.fieldPath.Text = AbstractFile.RemoveStartingSpaces(this.fieldPath.Text);
			this.ignoreChanged = false;
		}

		private void HandleButtonParentClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'dossier parent' a été cliqué.
			this.ParentDirectory();
		}

		private void HandleButtonNewClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'nouveau dossier' a été cliqué.
			this.NewDirectory();
		}

		private void HandleButtonRenameClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'renommer' a été cliqué.
			this.RenameStarting();
		}

		private void HandleButtonDeleteClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'supprimer' a été cliqué.
			this.FileDelete();
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
					string filename = string.Concat(this.initialDirectory, "\\", field.Text, this.fileExtension);
					this.selectedFilename = filename;
					this.selectedFilenames = null;

					if (this.PromptForOverwriting())
					{
						this.CloseWindow();
					}
					return;
				}
			}

			if (this.ActionOpen())
			{
				this.CloseWindow();
			}
		}


		#region Class Item
		//	Cette classe représente une 'ligne' dans la liste, qui peut représenter
		//	un fichier, un dossier ou la commande 'nouveau document vide'.
		protected class Item
		{
			public Item(string filename, bool isDirectory, bool isModel)
			{
				this.filename = filename;
				this.isDirectory = isDirectory;
				this.isModel = isModel;
			}

			public string Filename
			{
				//	Nom du fichier avec le chemin d'accès complet.
				get
				{
					if (this.filename == null)  // nouveau document vide ?
					{
						return Common.Document.Settings.GlobalSettings.NewEmptyDocument;
					}
					else
					{
						return this.filename;
					}
				}
				set
				{
					this.filename = value;
				}
			}

			public string ShortFilename
			{
				//	Nom du fichier court, sans le chemin d'accès ni l'extension.
				get
				{
					if (this.filename == null)  // nouveau document vide ?
					{
						return "—";
					}
					else
					{
						if (this.isDirectory)
						{
							int index = this.filename.LastIndexOf("\\");
							if (index == -1)
							{
								return this.filename;
							}
							else
							{
								return this.filename.Substring(index+1);
							}
						}
						else
						{
							return System.IO.Path.GetFileNameWithoutExtension(this.filename);
						}
					}
				}
			}

			public bool IsDirectory
			{
				get
				{
					return this.isDirectory;
				}
			}

			public string FileSize
			{
				//	Taille du fichier en kilo-bytes.
				get
				{
					if (this.filename == null || this.isDirectory)
					{
						return "";
					}
					else
					{
						long size = 0;

						using (System.IO.FileStream stream = System.IO.File.OpenRead(this.filename))
						{
							size = stream.Length;
						}

						size = (size+500)/1000;
						return string.Format(Res.Strings.Dialog.File.Size, size.ToString());
					}
				}
			}

			public string Description
			{
				//	Retourne la description du fichier, basée sur les statistiques si elles existent.
				get
				{
					if (this.filename == null)  // nouveau document vide ?
					{
						return Res.Strings.Dialog.New.EmptyDocument;
					}
					else
					{
						if (this.isDirectory)
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
					if (this.filename == null)  // nouveau document vide ?
					{
						return "New";
					}
					else
					{
						if (this.isDirectory)
						{
							return "FileTypeDirectory";
						}
						else
						{
							return null;
						}
					}
				}
			}

			public Image Image
			{
				//	Retourne l'image miniature associée au fichier.
				get
				{
					if (this.filename == null)  // nouveau document vide ?
					{
						return null;
					}
					else
					{
						if (this.isDirectory)
						{
							return null;
						}
						else
						{
							byte[] data = ReadPreview();
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
					if (this.filename == null)  // nouveau document vide ?
					{
						return null;
					}
					else
					{
						if (this.isDirectory)
						{
							return null;
						}
						else
						{
							byte[] data = ReadStatistics();
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

				if (zip.TryLoadFile(this.filename))
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

				if (zip.TryLoadFile(this.filename))
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

			protected string					filename;
			protected bool						isDirectory;
			protected bool						isModel;
		}
		#endregion


		protected CellTable					table;
		protected HSlider					slider;
		protected TextFieldCombo			fieldPath;
		protected IconButton				buttonParent;
		protected IconButton				buttonNew;
		protected IconButton				buttonRename;
		protected IconButton				buttonDelete;
		protected TextField					fieldFilename;
		protected TextFieldEx				fieldRename;

		protected string					fileExtension;
		protected bool						isModel = false;
		protected bool						isNavigationEnabled = false;
		protected bool						isMultipleSelection = false;
		protected bool						isNewEmtpyDocument = false;
		protected bool						isSave = false;
		protected string					initialDirectory;
		protected string					initialFilename;
		protected List<Item>				files;
		protected string					selectedFilename;
		protected string[]					selectedFilenames;
		protected int						tabIndex;
		protected int						renameSelected = -1;
		protected Widget					focusedWidget;
		protected bool						ignoreChanged = false;
		protected List<string>				comboDirectories;
		protected List<string>				comboTexts;
		protected int						comboSelected;
		protected CommandDispatcher			dispatcher;
		protected CommandContext			context;
		protected CommandState				renameState;
	}
}
