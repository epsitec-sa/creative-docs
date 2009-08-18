using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue pour choisir le type du fichier à exporter.
	/// </summary>
	public class ExportType : Abstract
	{
		public ExportType(DocumentEditor editor) : base(editor)
		{
			this.extensions = new List<string>();
			this.extensions.Add(".pdf");
			this.extensions.Add(".jpg");
			this.extensions.Add(".gif");
			this.extensions.Add(".png");
			this.extensions.Add(".tif");
			this.extensions.Add(".bmp");
			this.extensions.Add(".ico");

			this.formatDescriptions = new List<string>();
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.PDF);
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.JPG);
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.GIF);
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.PNG);
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.TIF);
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.BMP);
			this.formatDescriptions.Add(Res.Strings.Dialog.Export.Format.Type.ICO);

			this.shortDescriptions = new List<string>();
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.PDF);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.JPG);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.GIF);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.PNG);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.TIF);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.BMP);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.ICO);

			this.longDescriptions = new List<string>();
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.PDF);
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.JPG);
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.GIF);
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.PNG);
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.TIF);
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.BMP);
			this.longDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.ICO);
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("ExportType", 350, 310);
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.Text = Res.Strings.Dialog.Export.Type.Title;
				this.window.WindowCloseClicked += this.HandleWindowExportCloseClicked;

				Viewport panel = new Viewport(this.window.Root);
				panel.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				panel.Margins = new Margins(6, 6, 6, 32);

				this.radioButtons = new List<RadioButton>();
				for (int i=0; i<this.extensions.Count; i++)
				{
					RadioButton radio = new RadioButton(panel);
					radio.Name = this.extensions[i];
					radio.Text = string.Format(Res.Strings.Dialog.Export.FormatDescription, this.formatDescriptions[i], this.shortDescriptions[i]);
					radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
					radio.Entered += this.HandleRadioEntered;
					radio.Exited += this.HandleRadioExited;
					radio.Dock = DockStyle.Top;
					this.radioButtons.Add(radio);
				}

				this.fullDescription = new TextFieldMulti(panel);
				this.fullDescription.ScrollerVisibility = false;
				this.fullDescription.IsReadOnly = true;
				this.fullDescription.Margins = new Margins(0, 0, 12, 8);
				this.fullDescription.Dock = DockStyle.Fill;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Export.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomRight;
				buttonOk.Margins = new Margins(0, 6+75+6, 0, 6);
				buttonOk.Clicked += this.HandleExportButtonOkClicked;
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Export.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Export.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Anchor = AnchorStyles.BottomRight;
				buttonCancel.Margins = new Margins(0, 6, 0, 6);
				buttonCancel.Clicked += this.HandleExportButtonCancelClicked;
				buttonCancel.TabIndex = 11;
				buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.Export.Tooltip.Cancel);
			}

			this.UpdateRadio();
			this.UpdateDescription();

			this.isOKclicked = false;
			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("ExportType");
		}


		public string FileType
		{
			//	Choix du type de fichier (extension avec le point).
			get
			{
				return this.fileType;
			}
			set
			{
				if (value == ".jpeg")  value = ".jpg";
				if (value == ".tiff")  value = ".tif";

				if (this.fileType != value)
				{
					this.fileType = value;
					this.UpdateRadio();
					this.UpdateDescription();
				}
			}
		}

		public bool IsOKclicked
		{
			get
			{
				return this.isOKclicked;
			}
		}


		protected void UpdateRadio()
		{
			if (this.radioButtons != null)
			{
				this.ignoreChange = true;

				for (int i=0; i<this.radioButtons.Count; i++)
				{
					RadioButton radio = this.radioButtons[i];
					radio.ActiveState = (radio.Name == this.fileType) ? ActiveState.Yes : ActiveState.No;
				}

				this.ignoreChange = false;
			}
		}

		protected void UpdateDescription()
		{
			if (this.fullDescription != null)
			{
				int i = this.GetExtensionIndex(this.fileType);
				if (i == -1)
				{
					this.fullDescription.Text = "";
				}
				else
				{
					this.fullDescription.Text = string.Concat("<b><font size=\"140%\">", this.formatDescriptions[i], "</font></b><br/>", this.longDescriptions[i]);
				}
			}
		}

		protected int GetExtensionIndex(string extension)
		{
			for (int i=0; i<this.extensions.Count; i++)
			{
				if (this.extensions[i] == extension)
				{
					return i;
				}
			}
			return -1;
		}


		private void HandleRadioEntered(object sender, MessageEventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			string ift = this.fileType;
			this.fileType = radio.Name;
			this.UpdateDescription();
			this.fileType = ift;
		}

		private void HandleRadioExited(object sender, MessageEventArgs e)
		{
			this.UpdateDescription();
		}

		private void HandleRadioActiveStateChanged(object sender)
		{
			if (this.ignoreChange)  return;

			RadioButton radio = sender as RadioButton;
			this.FileType = radio.Name;
		}

		private void HandleWindowExportCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleExportButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}

		private void HandleExportButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
			this.isOKclicked = true;
		}


		protected string					fileType;

		protected List<string>				extensions;
		protected List<string>				formatDescriptions;
		protected List<string>				shortDescriptions;
		protected List<string>				longDescriptions;
		protected bool						ignoreChange;
		protected bool						isOKclicked;

		protected List<RadioButton>			radioButtons;
		protected TextFieldMulti			fullDescription;
	}
}
