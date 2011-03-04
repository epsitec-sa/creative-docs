using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'éditer les informations d'un module.
	/// </summary>
	public class SaveAllImages : Abstract
	{
		public SaveAllImages(DesignerApplication designerApplication)
			: base (designerApplication)
		{
			this.allEntityNames = new List<string> ();
			this.selectedEntityNames = new List<string> ();
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("SaveAllImages", 640, 420, true);
				this.window.Text = "Génération en série d'images bitmap";  // Res.Strings.Dialog.SaveAllImages.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob (this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins (0, -8, 0, -8);
				ToolTip.Default.SetToolTip (resize, Res.Strings.Dialog.Tooltip.Resize);

				this.CreateUI (this.window.Root);

				//	Boutons de fermeture.
				Widget footer = new Widget (this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins (0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button (footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOk = new Button (footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = "Générer";
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins (0, 6, 0, 0);
				this.buttonOk.Clicked += this.HandleButtonOkClicked;
				this.buttonOk.TabIndex = 10;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.Update ();

			this.window.ShowDialog ();
		}


		public List<string> AllEntityNames
		{
			get
			{
				return this.allEntityNames;
			}
		}

		public List<string> SelectedEntityNames
		{
			get
			{
				return this.selectedEntityNames;
			}
		}

		public string Folder
		{
			get;
			set;
		}

		public string Extension
		{
			get;
			set;
		}

		public EntitiesEditor.BitmapParameters BitmapParameters
		{
			get;
			set;
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
		}


		private void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var leftPane = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 4, 0, 0),
			};

			var rightPane = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Fill,
				Margins = new Margins (4, 0, 0, 0),
			};

			//	Rempli la colonne de gauche.
			this.CreateTableUI (leftPane);

			//	Rempli la colonne de droite.
			this.CreateGenerateUI (rightPane);
			this.CreateZoomUI (rightPane);
			this.CreateExtensionUI (rightPane);
			this.CreateBrowseUI (rightPane);
		}

		private void CreateTableUI(Widget parent)
		{
			this.table = new CellTable
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				StyleH = CellArrayStyles.Separator,
				StyleV = CellArrayStyles.Separator | CellArrayStyles.ScrollNorm,
			};

			var group = new FrameBox
			{
				Parent = parent,
				ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 5, 0),
			};

			var clearButton = new Button
			{
				Parent = group,
				Text = "Aucune entité",
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 1, 0, 0),
			};

			var setButton = new Button
			{
				Parent = group,
				Text = "Toutes les entités",
				Dock = DockStyle.Fill,
				Margins = new Margins (1, 0, 0, 0),
			};

			clearButton.Clicked += delegate
			{
				this.selectedEntityNames.Clear ();

				this.UpdateButtons ();
				this.UpdateTable ();
			};

			setButton.Clicked += delegate
			{
				this.selectedEntityNames.Clear ();
				this.selectedEntityNames.AddRange (this.allEntityNames);
				
				this.UpdateButtons ();
				this.UpdateTable ();
			};
		}

		private void CreateGenerateUI(Widget parent)
		{
			var group = this.CreateGroupBox (parent, "Options pour les cartouches en bas à gauche");

			this.checkUser    = this.CreateCheckButton (group, "user",    "Met le nom de l'utilisateur");
			this.checkDate    = this.CreateCheckButton (group, "date",    "Met la date");
			this.checkSamples = this.CreateCheckButton (group, "samples", "Met les exemples");
		}

		private void CreateZoomUI(Widget parent)
		{
			var group = this.CreateGroupBox (parent, "Zoom des images à générer");

			this.radioZoom1 = this.CreateRadioButton (group, "zoom1", "100%");
			this.radioZoom2 = this.CreateRadioButton (group, "zoom2", "200%");
			this.radioZoom3 = this.CreateRadioButton (group, "zoom3", "300%");
			this.radioZoom4 = this.CreateRadioButton (group, "zoom4", "400%");
		}

		private void CreateExtensionUI(Widget parent)
		{
			var group = this.CreateGroupBox (parent, "Type des images à générer");

			this.radioPng = this.CreateRadioButton (group, "png", "Images PNG (comprimées sans pertes)");
			this.radioTif = this.CreateRadioButton (group, "tif", "Images TIFF (comprimées sans pertes)");
			this.radioJpg = this.CreateRadioButton (group, "jpg", "Images JPEG (comprimées avec pertes)");
			this.radioBmp = this.CreateRadioButton (group, "bmp", "Images BMP (non comprimées)");
		}

		private void CreateBrowseUI(Widget parent)
		{
			var group = this.CreateGroupBox (parent, "Dossier où mettre les images");

			this.fieldFolder = new TextField
			{
				Parent = group,
				Dock = DockStyle.Fill,
			};

			var browseButton = new Button
			{
				Parent = group,
				Text = "Parcourir...",
				PreferredWidth = 75,
				Dock = DockStyle.Right,
				Margins = new Margins (2, 0, 0, 0),
			};

			this.fieldFolder.TextChanged += delegate
			{
				this.Folder = this.fieldFolder.Text;
				this.UpdateButtons ();
			};

			browseButton.Clicked += delegate
			{
				this.Folder = this.FolderBrowse (this.Folder);
				this.UpdateButtons ();
			};
		}

		private GroupBox CreateGroupBox(Widget parent, string text)
		{
			var group = new GroupBox
			{
				Parent = parent,
				Text = text,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 8),
				Padding = new Margins (8),
			};

			return group;
		}

		private CheckButton CreateCheckButton(Widget parent, string name, string text)
		{
			var button = new CheckButton
			{
				Parent = parent,
				Name = name,
				Text = text,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			button.Clicked += delegate
			{
				this.ButtonClicked (button.Name);
			};

			return button;
		}

		private RadioButton CreateRadioButton(Widget parent, string name, string text)
		{
			var button = new RadioButton
			{
				Parent = parent,
				Name = name,
				Text = text,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			button.Clicked += delegate
			{
				this.ButtonClicked (button.Name);
			};

			return button;
		}

		private void ButtonClicked(string name)
		{
			switch (name)
			{
				case "user":
					this.BitmapParameters.GenerateUserCartridge = !this.BitmapParameters.GenerateUserCartridge;
					break;

				case "date":
					this.BitmapParameters.GenerateDateCartridge = !this.BitmapParameters.GenerateDateCartridge;
					break;

				case "samples":
					this.BitmapParameters.GenerateSamplesCartridge = !this.BitmapParameters.GenerateSamplesCartridge;
					break;


				case "zoom1":
					this.BitmapParameters.Zoom = 1;
					break;

				case "zoom2":
					this.BitmapParameters.Zoom = 2;
					break;

				case "zoom3":
					this.BitmapParameters.Zoom = 3;
					break;

				case "zoom4":
					this.BitmapParameters.Zoom = 4;
					break;


				case "png":
					this.Extension = ".png";
					break;

				case "tif":
					this.Extension = ".tif";
					break;

				case "jpg":
					this.Extension = ".jpg";
					break;

				case "bmp":
					this.Extension = ".bmp";
					break;
			}

			this.UpdateButtons ();
		}


		private void Update()
		{
			this.isEditOk = false;
			this.closed = false;

			this.UpdateTable ();
			this.UpdateButtons ();
		}

		private void UpdateTable()
		{
			this.table.SetArraySize (2, this.allEntityNames.Count);

			this.table.SetWidthColumn (0, 24);
			this.table.SetWidthColumn (1, 300);

			this.table.SetHeaderTextH (1, "Entité");

			for (int row = 0; row < this.allEntityNames.Count; row++)
			{
				this.UpdateTableFillRow (row);
				this.UpdateTableContentRow (row);
			}
		}

		private void UpdateTableFillRow(int row)
		{
			for (int column=0; column<this.table.Columns; column++)
			{
				if (this.table[column, row].IsEmpty)
				{
					if (column == 0)
					{
						var button = new CheckButton
						{
							Name = this.allEntityNames[row],
							Dock = DockStyle.Fill,
							Margins = new Margins (4, 4, 0, 0),
						};

						button.Clicked += delegate
						{
							if (this.selectedEntityNames.Contains (button.Name))
							{
								this.selectedEntityNames.Remove (button.Name);
							}
							else
							{
								this.selectedEntityNames.Add (button.Name);
							}

							this.UpdateButtons ();
						};

						this.table[column, row].Insert (button);
					}
					else
					{
						var st = new StaticText
						{
							ContentAlignment = ContentAlignment.MiddleLeft,
							Dock = DockStyle.Fill,
							Margins = new Margins (4, 4, 0, 0),
						};

						this.table[column, row].Insert (st);
					}
				}
			}
		}

		private void UpdateTableContentRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			this.UpdateTableContentCell (row, 0, this.selectedEntityNames.Contains (this.allEntityNames[row]));
			this.UpdateTableContentCell (row, 1, this.allEntityNames[row]);

			this.table.SelectRow (row, false);
		}

		private void UpdateTableContentCell(int row, int column, bool state)
		{
			var button = this.table[column, row].Children[0] as CheckButton;
			button.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdateTableContentCell(int row, int column, string text)
		{
			var st = this.table[column, row].Children[0] as StaticText;
			st.Text = text;
		}

		private void UpdateButtons()
		{
			this.checkUser.ActiveState    = (this.BitmapParameters.GenerateUserCartridge   ) ? ActiveState.Yes : ActiveState.No;
			this.checkDate.ActiveState    = (this.BitmapParameters.GenerateDateCartridge   ) ? ActiveState.Yes : ActiveState.No;
			this.checkSamples.ActiveState = (this.BitmapParameters.GenerateSamplesCartridge) ? ActiveState.Yes : ActiveState.No;

			this.radioZoom1.ActiveState = (this.BitmapParameters.Zoom == 1) ? ActiveState.Yes : ActiveState.No;
			this.radioZoom2.ActiveState = (this.BitmapParameters.Zoom == 2) ? ActiveState.Yes : ActiveState.No;
			this.radioZoom3.ActiveState = (this.BitmapParameters.Zoom == 3) ? ActiveState.Yes : ActiveState.No;
			this.radioZoom4.ActiveState = (this.BitmapParameters.Zoom == 4) ? ActiveState.Yes : ActiveState.No;

			this.radioPng.ActiveState = (this.Extension == ".png") ? ActiveState.Yes : ActiveState.No;
			this.radioTif.ActiveState = (this.Extension == ".tif") ? ActiveState.Yes : ActiveState.No;
			this.radioJpg.ActiveState = (this.Extension == ".jpg") ? ActiveState.Yes : ActiveState.No;
			this.radioBmp.ActiveState = (this.Extension == ".bmp") ? ActiveState.Yes : ActiveState.No;

			this.fieldFolder.Text = this.Folder;

			this.buttonOk.Enable = (!string.IsNullOrEmpty (this.Folder) && this.selectedEntityNames.Count != 0);
		}

		private void Accept()
		{
		}


		public void Close()
		{
			if (this.closed)
			{
				return;
			}

			if (this.buttonOk != null)  // mode "dialogue" (par opposition au mode "volet") ?
			{
				this.parentWindow.MakeActive ();
				this.window.Hide ();
				this.OnClosed ();
			}

			this.closed = true;
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.Close ();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.Accept ();
			this.Close ();
			this.isEditOk = true;
		}


		private string FolderBrowse(string folder)
		{
			//	Choix d'un dossier.
			var dialog = new System.Windows.Forms.FolderBrowserDialog ();

			dialog.Description = "Choisisser le dossier où mettre les images bitmap générées automatiquement.\nAttention: Les images présentes dans ce dossier seront détruites !";
			dialog.ShowNewFolderButton = true;
			dialog.SelectedPath = folder;
			
			var result = dialog.ShowDialog ();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				folder = dialog.SelectedPath;
			}

			return folder;
		}


		private bool							isEditOk;
		private bool							closed;
		private int								tabIndex;
		private List<string>					allEntityNames;
		private List<string>					selectedEntityNames;

		private CellTable						table;

		private CheckButton						checkUser;
		private CheckButton						checkDate;
		private CheckButton						checkSamples;

		private RadioButton						radioZoom1;
		private RadioButton						radioZoom2;
		private RadioButton						radioZoom3;
		private RadioButton						radioZoom4;

		private RadioButton						radioPng;
		private RadioButton						radioTif;
		private RadioButton						radioJpg;
		private RadioButton						radioBmp;

		private TextField						fieldFolder;

		private Button							buttonOk;
		private Button							buttonCancel;
	}
}
