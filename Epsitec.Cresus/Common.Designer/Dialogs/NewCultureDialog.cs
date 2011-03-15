using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir la culture à créer.
	/// </summary>
	public class NewCultureDialog : AbstractDialog
	{
		public NewCultureDialog(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow();
				this.window.Root.WindowStyles = WindowStyles.None;
				this.window.PreventAutoClose = true;
				this.WindowInit("NewCulture", 172, 160, true);
				this.window.Text = Res.Strings.Dialog.NewCulture.Title;
				this.window.Owner = this.parentWindow;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.NewCulture.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.Dock = DockStyle.Top;
				label.Margins = new Margins(0, 0, 0, 6);

				this.cultureWidget = new ScrollList(this.window.Root);
				this.cultureWidget.PreferredHeight = 90;
				this.cultureWidget.Dock = DockStyle.Fill;
				this.cultureWidget.TabIndex = 1;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonClose = new Button(footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Cancel;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Dock = DockStyle.Right;
				buttonClose.Clicked += this.HandleButtonCloseClicked;
				buttonClose.TabIndex = 11;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.NewCulture.Button.Create;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Right;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += this.HandleButtonFilterClicked;
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateList();

			this.window.ShowDialog();
		}

		public void SetAccess(ResourceAccess access)
		{
			this.access = access;
		}

		public string Culture
		{
			//	Retourne la culture choisie.
			get
			{
				if (this.cultureWidget.SelectedItemIndex == -1)
					return null;
				return this.cultureList[this.cultureWidget.SelectedItemIndex];
			}
		}


		protected void UpdateList()
		{
			//	Met à jour la ScrollList des cultures, en enlevant celles qui font déjà
			//	partie du bundle.
			string baseCulture = this.access.GetPrimaryCultureName();
			List<string> secondaryCultures = this.access.GetSecondaryCultureNames();

			//	Construit la liste des cultures inexistantes dans l'accès.
			this.cultureList = new List<string>();
			foreach (string name in Misc.Cultures)
			{
				if (name != baseCulture && !secondaryCultures.Contains(name))
				{
					this.cultureList.Add(name);
				}
			}

			//	Remplit la ScrollList.
			this.cultureWidget.Items.Clear();
			foreach (string name in this.cultureList)
			{
				System.Globalization.CultureInfo culture = Resources.FindSpecificCultureInfo(name);
				this.cultureWidget.Items.Add(Misc.CultureLongName(culture));
			}
			this.cultureWidget.SelectedItemIndex = 0;  // sélectionne en priorité la première culture de la liste
		}


		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.cultureWidget.SelectedItemIndex = -1;

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


		protected ResourceAccess				access;
		protected ScrollList					cultureWidget;
		protected List<string>					cultureList;
	}
}
