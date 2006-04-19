using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le filtre pour les ressources.
	/// </summary>
	public class NewCulture : Abstract
	{
		public NewCulture(MainWindow mainWindow) : base(mainWindow)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.MakeFixedSizeWindow();
				this.window.Root.WindowStyles = WindowStyles.None;
				this.window.PreventAutoClose = true;
				this.WindowInit("NewCulture", 172, 160, true);
				this.window.Text = Res.Strings.Dialog.NewCulture.Title;
				this.window.Owner = this.parentWindow;

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.NewCulture.Label;
				label.Alignment = ContentAlignment.MiddleLeft;
				label.Width = 40;
				label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				label.Margins = new Margins(6, 6, 6+3, 0);

				this.cultureWidget = new ScrollList(this.window.Root);
				this.cultureWidget.Height = 90;
				this.cultureWidget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.cultureWidget.Margins = new Margins(6, 6, 6+22, 0);
				this.cultureWidget.TabIndex = tabIndex++;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.NewCulture.Button.Create;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonFilterClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Cancel;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.UpdateList();

			this.window.ShowDialog();
		}

		public string Culture
		{
			//	Retourne la culture choisie.
			get
			{
				if ( this.cultureWidget.SelectedIndex == -1 )  return null;
				return this.cultureList[this.cultureWidget.SelectedIndex];
			}
		}


		protected void UpdateList()
		{
			//	Met à jour la ScrollList des cultures, en enlevant celles qui font déjà
			//	partie du bundle.
			Module module = this.mainWindow.CurrentModule;

			//	Construit la liste des cultures inexistantes dans le bundle.
			this.cultureList = new List<string>();
			foreach (string name in NewCulture.Cultures)
			{
				if (!module.IsExistingCulture(name))
				{
					this.cultureList.Add(name);
				}
			}

			//	Remplit la ScrollList.
			this.cultureWidget.Items.Clear();
			foreach (string name in this.cultureList)
			{
				System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(name);
				this.cultureWidget.Items.Add(Misc.CultureLongName(culture));
			}
			this.cultureWidget.SelectedIndex = 0;  // sélectionne en priorité la première culture de la liste
		}


		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.cultureWidget.SelectedIndex = -1;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonFilterClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		public static string[] Cultures = { "fr", "en", "de", "it", "es", "pt" };

		
		protected ScrollList cultureWidget;
		protected List<string>					cultureList;
	}
}
