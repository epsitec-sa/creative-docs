using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir la culture � cr�er.
	/// </summary>
	public class NewCulture : Abstract
	{
		public NewCulture(MainWindow mainWindow) : base(mainWindow)
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
				this.window.Root.WindowStyles = WindowStyles.None;
				this.window.PreventAutoClose = true;
				this.WindowInit("NewCulture", 172, 160, true);
				this.window.Text = Res.Strings.Dialog.NewCulture.Title;
				this.window.Owner = this.parentWindow;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.NewCulture.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.Dock = DockStyle.Top;
				label.Margins = new Margins(0, 0, 0, 6);

				this.cultureWidget = new ScrollList(this.window.Root);
				this.cultureWidget.PreferredHeight = 90;
				this.cultureWidget.Dock = DockStyle.Fill;
				this.cultureWidget.TabIndex = tabIndex++;

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
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.NewCulture.Button.Create;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Right;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonFilterClicked);
				buttonOk.TabIndex = tabIndex++;
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
				if ( this.cultureWidget.SelectedIndex == -1 )  return null;
				return this.cultureList[this.cultureWidget.SelectedIndex];
			}
		}


		protected void UpdateList()
		{
			//	Met � jour la ScrollList des cultures, en enlevant celles qui font d�j�
			//	partie du bundle.
			string baseCulture = this.access.GetBaseCultureName();
			List<string> secondaryCultures = this.access.GetSecondaryCultureNames();

			//	Construit la liste des cultures inexistantes dans l'acc�s.
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
			this.cultureWidget.SelectedIndex = 0;  // s�lectionne en priorit� la premi�re culture de la liste
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


		protected ResourceAccess				access;
		protected ScrollList					cultureWidget;
		protected List<string>					cultureList;
	}
}
