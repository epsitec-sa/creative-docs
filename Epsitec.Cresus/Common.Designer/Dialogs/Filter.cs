using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le filtre pour les ressources.
	/// </summary>
	public class Filter : Abstract
	{
		public Filter(MainWindow mainWindow) : base(mainWindow)
		{
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.MakeFixedSizeWindow();
				this.window.MakeToolWindow();
				this.WindowInit("Filter", 250, 80, true);
				this.window.Text = Res.Strings.Dialog.Filter.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Filter.Label;
				label.Width = 40;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6, 0);

				this.fieldFilter = new TextFieldCombo(this.window.Root);
				this.fieldFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldFilter.Margins = new Margins(6+40, 6, 6, 0);
				this.fieldFilter.TabIndex = tabIndex++;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.Filter.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonFilterClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.window.Show();
		}


		public string StringFilter
		{
			get
			{
				return this.fieldFilter.Text;
			}

			set
			{
				this.fieldFilter.Text = value;
			}
		}



		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonFilterClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			this.fieldFilter.Items.Add(this.fieldFilter.Text);
			module.Modifier.ActiveViewer.ChangeFilter(this.fieldFilter.Text);
		}


		protected TextFieldCombo				fieldFilter;
	}
}
