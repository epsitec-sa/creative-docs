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
	/// Classe abstraite pour les dialogues FileNew et FileOpen.
	/// </summary>
	public abstract class AbstractFile : Abstract
	{
		public AbstractFile(DocumentEditor editor) : base(editor)
		{
			this.focusedWidget = null;
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

		public string Filename
		{
			get
			{
				return this.selectedFilename;
			}
		}


		protected void CreateResizer()
		{
			ResizeKnob resize = new ResizeKnob(this.window.Root);
			resize.Anchor = AnchorStyles.BottomRight;
			resize.Margins = new Margins(0, -8, 0, -8);
			ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);
		}

		protected void CreateTable(double cellHeight)
		{
			this.table = new CellTable(this.window.Root);
			this.table.DefHeight = cellHeight;
			this.table.HeaderHeight = 20;
			this.table.StyleH = CellArrayStyles.Stretch | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile;
			this.table.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;
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
			this.fieldRename = new TextFieldEx(this.window.Root);
			this.fieldRename.Visibility = false;
			this.fieldRename.ButtonShowCondition = ShowCondition.Always;
			this.fieldRename.EditionAccepted += new EventHandler(this.HandleRenameAccepted);
			this.fieldRename.EditionRejected += new EventHandler(this.HandleRenameRejected);
			this.fieldRename.IsFocusedChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleRenameFocusChanged);
		}

		protected void CreateFooter()
		{
			//	Crée le pied du dialogue.
			Widget footer = new Widget(this.window.Root);
			footer.PreferredHeight = 22;
			footer.Margins = new Margins(0, 0, 8, 0);
			footer.Dock = DockStyle.Bottom;

			Button buttonOpen = new Button(footer);
			buttonOpen.PreferredWidth = 75;
			buttonOpen.Text = Res.Strings.Dialog.File.Button.Open;
			buttonOpen.ButtonStyle = ButtonStyle.DefaultAccept;
			buttonOpen.Dock = DockStyle.Left;
			buttonOpen.Margins = new Margins(0, 6, 0, 0);
			buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
			buttonOpen.TabIndex = this.tabIndex++;
			buttonOpen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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

			if (this is FileNew)
			{
				this.files.Add(new Item(null, false));  // première ligne avec 'nouveau document vide'
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
						this.files.Add(new Item(directory, true));
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
					this.files.Add(new Item(filename, false));
				}
			}
		}

		protected virtual void UpdateButtons()
		{
			//	Met à jour les boutons en fonction du fichier sélectionné dans la liste.
		}

		protected virtual void UpdateInitialDirectory()
		{
			//	Met à jour le chemin d'accès.
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
			if (sel == -1)
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
			if (sel == -1)
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
					this.selectedFilename = this.files[sel].Filename;
					return true;  // il faudra ferme le dialogue
				}
			}

			return false;  // ne pas fermer le dialogue
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

		private void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'Ouvrir' cliqué.
			if (this.focusedWidget is AbstractTextField)  // focus dans un texte éditable ?
			{
				AbstractTextField field = this.focusedWidget as AbstractTextField;
				if (!string.IsNullOrEmpty(field.Text))
				{
					string filename = string.Concat(this.initialDirectory, "\\", field.Text, this.fileExtension);
					this.selectedFilename = filename;
					this.CloseWindow();
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
			public Item(string filename, bool isDirectory)
			{
				this.filename = filename;
				this.isDirectory = isDirectory;
			}

			public string Filename
			{
				//	Nom du fichier avec le chemin d'accès complet.
				get
				{
					if (this.filename == null)  // nouveau document vide ?
					{
						return "*";
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
								return Res.Strings.Dialog.File.Document;
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
		}
		#endregion


		protected CellTable					table;
		protected HSlider					slider;
		protected TextFieldEx				fieldRename;

		protected string					fileExtension;
		protected bool						isNavigationEnabled = false;
		protected string					initialDirectory;
		protected List<Item>				files;
		protected string					selectedFilename;
		protected int						tabIndex;
		protected int						renameSelected = -1;
		protected Widget					focusedWidget;
		protected bool						ignoreChanged = false;
	}
}
