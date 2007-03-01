using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
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

			this.shortDescriptions = new List<string>();
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.PDF);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.JPG);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.GIF);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.PNG);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.TIF);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.BMP);
			this.shortDescriptions.Add(Res.Strings.Dialog.Export.Short.Type.ICO);

			this.fullDescriptions = new List<string>();
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.PDF);
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.JPG);
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.GIF);
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.PNG);
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.TIF);
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.BMP);
			this.fullDescriptions.Add(Res.Strings.Dialog.Export.Long.Type.ICO);
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("ExportType", 350, 230);
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.Text = Res.Strings.Dialog.Export.Type.Title;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowExportCloseClicked);

				Panel panel = new Panel(this.window.Root);
				panel.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				panel.Margins = new Margins(10, 10, 10, 40);

				this.radioButtons = new List<RadioButton>();
				for (int i=0; i<this.extensions.Count; i++)
				{
					RadioButton radio = new RadioButton(panel);
					radio.Name = this.extensions[i];
					radio.Text = this.shortDescriptions[i];
					radio.ActiveStateChanged += new EventHandler(this.HandleRadioActiveStateChanged);
					radio.Dock = DockStyle.Top;
					this.radioButtons.Add(radio);
				}
				this.UpdateRadio();

				this.fullDescription = new StaticText(panel);
				this.fullDescription.Margins = new Margins(0, 0, 5, 5);
				this.fullDescription.Dock = DockStyle.Fill;
				this.UpdateDescription();

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Export.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(10, 0, 0, 10);
				buttonOk.Clicked += new MessageEventHandler(this.HandleExportButtonOkClicked);
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Export.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Export.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Anchor = AnchorStyles.BottomLeft;
				buttonCancel.Margins = new Margins(10+75+10, 0, 0, 10);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleExportButtonCancelClicked);
				buttonCancel.TabIndex = 11;
				buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.Export.Tooltip.Cancel);
			}

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
					this.fullDescription.Text = this.fullDescriptions[i];
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
		protected List<string>				shortDescriptions;
		protected List<string>				fullDescriptions;
		protected bool						ignoreChange;
		protected bool						isOKclicked;

		protected List<RadioButton>			radioButtons;
		protected StaticText				fullDescription;
	}
}
