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
			this.entitySamples = new List<EntitiesEditor.EntitySample> ();
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
				this.WindowInit ("SaveAllImages", 640, 402, true);
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

				this.statistics = new StaticText
				{
					Parent = footer,
					ContentAlignment = Drawing.ContentAlignment.MiddleRight,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 20, 0, 0),
				};

				this.buttonCancel = new Button (footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonApply = new Button (footer);
				this.buttonApply.PreferredWidth = 75;
				this.buttonApply.Text = "Appliquer";
				this.buttonApply.Dock = DockStyle.Right;
				this.buttonApply.Margins = new Margins (0, 16, 0, 0);
				this.buttonApply.Clicked += this.HandleButtonApplyClicked;
				this.buttonApply.TabIndex = 10;
				this.buttonApply.TabNavigationMode = TabNavigationMode.ActivateOnTab;

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


		public List<EntitiesEditor.EntitySample> EntitySamples
		{
			get
			{
				return this.entitySamples;
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

		public System.Windows.Forms.DialogResult Result
		{
			get
			{
				return this.result;
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
				PreferredWidth = 250,
				Dock = DockStyle.Right,
				Margins = new Margins (4, 0, 0, 0),
			};

			//	Rempli la colonne de gauche.
			this.CreateEntitiesUI (leftPane);

			//	Rempli la colonne de droite.
			this.CreateGenerateUI (rightPane);
			this.CreateZoomUI (rightPane);
			this.CreateExtensionUI (rightPane);
			this.CreateBrowseUI (rightPane);
		}

		private void CreateEntitiesUI(Widget parent)
		{
			var group = new GroupBox
			{
				Parent = parent,
				Text = "Entités pour lesquelles il faut générer une image",
				Dock = DockStyle.Fill,
				Padding = new Margins (8),
			};

			this.scrollableEntities = new Scrollable
			{
				Parent = group,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.ShowAlways,
				PaintViewportFrame = false,
				Dock = DockStyle.Fill,
			};

			this.scrollableEntities.Viewport.IsAutoFitting = true;

			new Separator
			{
				Parent = this.scrollableEntities,
				PreferredWidth = 1,
				Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right,
				Margins = new Margins (0, 50+24, 0, 0),
			};

			new Separator
			{
				Parent = this.scrollableEntities,
				PreferredWidth = 1,
				Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right,
				Margins = new Margins (0, 50+55+24, 0, 0),
			};

			var footer = new FrameBox
			{
				Parent = group,
				ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 8, 0),
			};

			this.buttonClear = new Button
			{
				Parent = footer,
				Text = "Aucune",
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 1, 0, 0),
			};

			this.buttonMajor = new Button
			{
				Parent = footer,
				Text = "Importantes",
				Dock = DockStyle.Fill,
				Margins = new Margins (1, 1, 0, 0),
			};

			this.buttonAll = new Button
			{
				Parent = footer,
				Text = "Toutes",
				Dock = DockStyle.Fill,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.buttonClear.Clicked += delegate
			{
				this.selectedEntityNames.Clear ();

				this.UpdateButtons ();
				this.UpdateEntities ();
			};

			this.buttonMajor.Clicked += delegate
			{
				this.selectedEntityNames.Clear ();
				foreach (var sample in this.entitySamples)
				{
					if (sample.IsMajor)
					{
						this.selectedEntityNames.Add (sample.Name);
					}
				}

				this.UpdateButtons ();
				this.UpdateEntities ();
			};

			this.buttonAll.Clicked += delegate
			{
				this.selectedEntityNames.Clear ();
				foreach (var sample in this.entitySamples)
				{
					this.selectedEntityNames.Add (sample.Name);
				}

				this.UpdateButtons ();
				this.UpdateEntities ();
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
			group.Margins = new Margins (0);

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
			this.result = System.Windows.Forms.DialogResult.Cancel;
			this.closed = false;

			this.UpdateEntities ();
			this.UpdateButtons ();
		}

		private void UpdateEntities()
		{
			this.scrollableEntities.Viewport.Children.Clear ();

			for (int i = 0; i < this.entitySamples.Count; i++)
			{
				var name = this.entitySamples[i].Name;

				var line = new FrameBox
				{
					Parent = this.scrollableEntities.Viewport,
					Dock = DockStyle.Top,
				};

				if (i%2 == 0)
				{
					line.BackColor = Color.FromHexa ("f8f5fa");  // une ligne sur deux plus claire
				}

				var button = new CheckButton
				{
					Parent = line,
					Name = name,
					Text = this.entitySamples[i].NameDescription,
					ActiveState = (this.selectedEntityNames.Contains (name)) ? ActiveState.Yes : ActiveState.No,
					Dock = DockStyle.Fill,
				};

				new StaticText
				{
					Parent = line,
					Text = this.entitySamples[i].BoxCountDescription,  // par exemple "2 boîtes"
					ContentAlignment = Drawing.ContentAlignment.MiddleRight,
					PreferredWidth = 50,
					Dock = DockStyle.Right,
					Margins = new Margins (0, 5, 0, 0),
				};

				new StaticText
				{
					Parent = line,
					Text = this.entitySamples[i].FlagsDescription,  // par exemple "Schéma"
					ContentAlignment = Drawing.ContentAlignment.MiddleLeft,
					PreferredWidth = 50,
					Dock = DockStyle.Right,
					Margins = new Margins (5, 0, 0, 0),
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
			}
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

			this.buttonClear.Enable = (this.selectedEntityNames.Count != 0);
			this.buttonMajor.Enable = !this.IsMajor;
			this.buttonAll.Enable = (this.selectedEntityNames.Count != this.entitySamples.Count);

			this.buttonOk.Enable = (!string.IsNullOrEmpty (this.Folder) && this.selectedEntityNames.Count != 0);

			if (this.selectedEntityNames.Count == 0)
			{
				this.statistics.Text = "Aucune image à générer.";
			}
			else if (this.selectedEntityNames.Count == 1)
			{
				this.statistics.Text = "Une image à générer.";
			}
			else
			{
				this.statistics.Text = string.Format ("{0} images à générer.", this.selectedEntityNames.Count.ToString ());
			}
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

		private void HandleButtonApplyClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
			this.result = System.Windows.Forms.DialogResult.Yes;
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
			this.result = System.Windows.Forms.DialogResult.OK;
		}


		private bool IsMajor
		{
			get
			{
				int count = 0;

				foreach (var sample in this.entitySamples)
				{
					if (sample.IsMajor)
					{
						if (!this.selectedEntityNames.Contains (sample.Name))
						{
							return false;
						}

						count++;
					}
				}

				return this.selectedEntityNames.Count == count;
			}
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


		private System.Windows.Forms.DialogResult	result;
		private bool								closed;
		private int									tabIndex;
		private List<EntitiesEditor.EntitySample>	entitySamples;
		private List<string>						selectedEntityNames;

		private Scrollable							scrollableEntities;

		private CheckButton							checkUser;
		private CheckButton							checkDate;
		private CheckButton							checkSamples;

		private RadioButton							radioZoom1;
		private RadioButton							radioZoom2;
		private RadioButton							radioZoom3;
		private RadioButton							radioZoom4;

		private RadioButton							radioPng;
		private RadioButton							radioTif;
		private RadioButton							radioJpg;
		private RadioButton							radioBmp;

		private TextField							fieldFolder;

		private Button								buttonClear;
		private Button								buttonMajor;
		private Button								buttonAll;

		private StaticText							statistics;
		private Button								buttonOk;
		private Button								buttonApply;
		private Button								buttonCancel;
	}
}
