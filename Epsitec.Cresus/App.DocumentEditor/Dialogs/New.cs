using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using FontFaceCombo  = Common.Document.Widgets.FontFaceCombo;

	/// <summary>
	/// Dialogue permettant de choisir un caractère quelconque à insérer dans
	/// un texte en édition.
	/// </summary>
	public class New : Abstract
	{
		public New(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("New", 300, 300, true);
				this.window.Text = Res.Strings.Dialog.New.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				this.table = new CellTable(this.window.Root);
				this.table.DefHeight = 50;
				this.table.HeaderHeight = 20;
				this.table.StyleH = CellArrayStyles.Stretch | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile;
				this.table.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;
				this.table.AlphaSeparator = 0.3;
				this.table.Margins = new Margins(0, 0, 0, 0);
				this.table.Dock = DockStyle.Fill;

				//	Pied.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOpen = new Button(footer);
				buttonOpen.PreferredWidth = 75;
				buttonOpen.Text = Res.Strings.Dialog.New.Button.Open;
				buttonOpen.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOpen.Dock = DockStyle.Left;
				buttonOpen.Margins = new Margins(0, 6, 0, 0);
				buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
				buttonOpen.TabIndex = tabIndex++;
				buttonOpen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonCancel = new Button(footer);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Dock = DockStyle.Left;
				buttonCancel.Margins = new Margins(0, 6, 0, 0);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
				buttonCancel.TabIndex = tabIndex++;
				buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.slider = new HSlider(footer);
				this.slider.PreferredWidth = 80;
				this.slider.Dock = DockStyle.Right;
				this.slider.Margins = new Margins(0, 0, 4, 4);
				this.slider.TabIndex = tabIndex++;
				this.slider.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 100.0M;
				this.slider.SmallChange = 1.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) this.table.DefHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.New.Tooltip.PreviewSize);

			}

			this.selectedFilename = null;
			this.UpdateTable(0);

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("New");
		}


		public string Filename
		{
			get
			{
				return this.selectedFilename;
			}
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

			this.table.SetHeaderTextH(0, Res.Strings.Dialog.New.Header.Preview);
			this.table.SetHeaderTextH(1, Res.Strings.Dialog.New.Header.Filename);
			this.table.SetHeaderTextH(2, Res.Strings.Dialog.New.Header.Description);
			this.table.SetHeaderTextH(3, Res.Strings.Dialog.New.Header.Size);

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
				if (row == 0)  // nouveau document vide ?
				{
					im.FixIcon = Misc.Icon("New");
				}
				else
				{
					im.DrawingImage = this.files[row].Image;
				}

				st = this.table[1, row].Children[0] as StaticText;
				st.Text = this.files[row].ShortFilename;

				st = this.table[2, row].Children[0] as StaticText;
				st.Text = this.files[row].Description;

				st = this.table[3, row].Children[0] as StaticText;
				st.Text = this.files[row].FileSize;

				this.table.SelectRow(row, row==sel);
			}

			if (sel != -1)
			{
				this.table.ShowSelect();  // montre la ligne sélectionnée
			}
		}

		protected void ListFilenames()
		{
			//	Effectue la liste des fichiers .crmod contenus dans le dossier adhoc.
			string[] filenames;

			try
			{
				string path = System.IO.Path.GetDirectoryName(this.globalSettings.NewDocument);
				filenames = System.IO.Directory.GetFiles(path, "*.crdoc", System.IO.SearchOption.TopDirectoryOnly);
			}
			catch
			{
				filenames = null;
			}

			this.files = new List<Item>();
			this.files.Add(new Item(null));  // première ligne avec 'nouveau document vide'

			if (filenames != null)
			{
				foreach (string filename in filenames)
				{
					this.files.Add(new Item(filename));
				}
			}
		}



		private void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.table.SelectedRow;
			if (sel == -1)
			{
				this.selectedFilename = null;
			}
			else
			{
				this.selectedFilename = this.files[sel].Filename;
			}
		}

		private void HandleSliderChanged(object sender)
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
			public Item(string filename)
			{
				this.filename = filename;
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
						return Res.Strings.Dialog.New.EmptyDocument;
					}
					else
					{
						return System.IO.Path.GetFileNameWithoutExtension(this.filename);
					}
				}
			}

			public string FileSize
			{
				//	Taille du fichier en kilo-bytes.
				get
				{
					long size = 0;

					if (this.filename != null)
					{
						using (System.IO.FileStream stream = System.IO.File.OpenRead(this.filename))
						{
							size = stream.Length;
						}
					}

					size = (size+500)/1000;
					return string.Format(Res.Strings.Dialog.New.Size, size.ToString());
				}
			}

			public string Description
			{
				//	Retourne la description du fichier.
				get
				{
					if (this.filename == null)  // nouveau document vide ?
					{
						return "—";
					}
					else
					{
						return "—";
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
						byte[] data = ReadPreview();
						if (data != null)
						{
							return Bitmap.FromData(data);
						}

						return null;
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
		}
		#endregion


		protected CellTable					table;
		protected HSlider					slider;
		protected List<Item>				files;
		protected string					selectedFilename;
	}
}
