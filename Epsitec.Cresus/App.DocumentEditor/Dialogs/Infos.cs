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
				double dx = 300;
				double dy = 250;
				this.window = new Window();
				//?this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				if ( this.globalSettings.InfosLocation.IsEmpty )
				{
					Rectangle wrect = this.CurrentBounds;
					this.window.ClientSize = new Size(dx, dy);
					this.window.WindowLocation = new Point(wrect.Center.X-dx/2, wrect.Center.Y-dy/2);
				}
				else
				{
					this.window.ClientSize = this.globalSettings.InfosSize;
					this.window.WindowLocation = this.globalSettings.InfosLocation;
				}
				this.window.Text = "Informations";
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
				buttonClose.Text = "Fermer";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleInfosButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, "Fermer ce dialogue");
			}

			this.window.Show();
			this.editor.CurrentDocument.Dialogs.BuildInfos(this.window);
		}

		// Enregistre la position de la fenêtre du dialogue.
		public override void Save()
		{
			if ( this.window == null )  return;
			this.globalSettings.InfosLocation = this.window.WindowLocation;
			this.globalSettings.InfosSize = this.window.ClientSize;
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
		}

		private void HandleInfosButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
		}

	}
}
