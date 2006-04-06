using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Fenêtre principale de l'éditeur de ressources.
	/// </summary>
	public class MainWindow
	{
		public void Show()
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.WindowSize = new Size(400, 400);
				this.window.Text = "Ressources Editor";
				this.window.PreventAutoClose = true;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowAboutCloseClicked);

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = "Close";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleAboutButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.window.ShowDialog();
		}


		private void HandleWindowAboutCloseClicked(object sender)
		{
			this.window.Hide();
		}

		private void HandleAboutButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.window.Hide();
		}


		protected Window						window;
	}
}
