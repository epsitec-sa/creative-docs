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
		}


		public virtual string InitialDirectory
		{
			get
			{
				return this.initialDirectory;
			}
			set
			{
				this.initialDirectory = value;
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

		protected void CreateTable()
		{
			this.table = new CellTable(this.window.Root);
			this.table.DefHeight = 50;
			this.table.HeaderHeight = 20;
			this.table.StyleH = CellArrayStyles.Stretch | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile;
			this.table.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;
			this.table.AlphaSeparator = 0.3;
			this.table.Margins = new Margins(0, 0, 0, 0);
			this.table.Dock = DockStyle.Fill;
			this.table.FinalSelectionChanged += new EventHandler(this.HandleTableFinalSelectionChanged);
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
				this.table.ShowCell(0, 0);  // montre le début de la table
			}
			else
			{
				this.table.ShowSelect();  // montre la ligne sélectionnée
			}
		}

		protected void ListFilenames()
		{
			//	Effectue la liste des fichiers .crmod contenus dans le dossier adhoc.
			this.files = new List<Item>();

			if (this is FileNew)
			{
				this.files.Add(new Item(null, false));  // première ligne avec 'nouveau document vide'
			}

			if (this.IsNavigationEnabled)
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
				filenames = System.IO.Directory.GetFiles(this.initialDirectory, "*"+this.Extension, System.IO.SearchOption.TopDirectoryOnly);
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

		protected virtual bool IsNavigationEnabled
		{
			get
			{
				return false;
			}
		}

		protected virtual string Extension
		{
			get
			{
				return ".xxx";
			}
		}

		protected virtual string SelectedFilename
		{
			get
			{
				int sel = this.table.SelectedRow;
				if (sel == -1)
				{
					return null;
				}
				else
				{
					return this.files[sel].Filename;
				}
			}
		}



		protected virtual void HandleTableFinalSelectionChanged(object sender)
		{
		}

		protected void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			int sel = this.table.SelectedRow;
			if (sel != -1 && this.files[sel].IsDirectory)
			{
				this.InitialDirectory = this.files[sel].Filename;
				this.UpdateTable(-1);
				return;
			}

			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.selectedFilename = this.SelectedFilename;
		}

		protected void HandleSliderChanged(object sender)
		{
			this.table.DefHeight = (double) this.slider.Value;
			this.table.HeaderHeight = 20;

			for (int i=0; i<this.table.Rows; i++)
			{
				this.table.SetHeightRow(i, this.table.DefHeight);
			}
		}


		#region Class Item
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
				//	Retourne la description du fichier.
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
							return Res.Strings.Dialog.File.Document;
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
							return "Directory";
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

			protected string					filename;
			protected bool						isDirectory;
		}
		#endregion


		protected CellTable					table;
		protected HSlider					slider;

		protected string					initialDirectory;
		protected List<Item>				files;
		protected string					selectedFilename;
		protected int						tabIndex;
	}
}
