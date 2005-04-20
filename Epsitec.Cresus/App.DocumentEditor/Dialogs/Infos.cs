using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue des informations sur le document.
	/// </summary>
	public class Infos : Abstract
	{
		public Infos(DocumentEditor editor) : base(editor)
		{
		}

		// Crée et montre la fenêtre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				this.window = new Window();
				//?this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Infos", 300, 250, true);
				this.window.Text = Res.Strings.Dialog.Infos.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowInfosCloseClicked);
				this.window.Root.MinSize = new Size(160, 100);

				TextFieldMulti multi = new TextFieldMulti(this.window.Root);
				multi.Name = "Infos";
				multi.IsReadOnly = true;
				multi.MaxChar = 10000;
				multi.Dock = DockStyle.Fill;
				multi.DockMargins = new Margins(10, 10, 10, 40);

				// Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleInfosButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);
			}

			this.window.Show();
			this.editor.CurrentDocument.Dialogs.BuildInfos(this.window);
		}

		// Enregistre la position de la fenêtre du dialogue.
		public override void Save()
		{
			this.WindowSave("Infos");
		}

		// Reconstruit le dialogue.
		public override void Rebuild()
		{
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildInfos(this.window);
		}


		private void HandleWindowInfosCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleInfosButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}
	}
}
