using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le nom d'un champ qui va être créé.
	/// </summary>
	public class FieldName : Abstract
	{
		public FieldName(MainWindow mainWindow) : base(mainWindow)
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
				this.window.PreventAutoClose = true;
				this.WindowInit("FieldName", 300, 100, true);
				this.window.Text = "Création d'un champ";  // Res.Strings.Dialog.FieldName.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				int tabIndex = 0;

				Widget band = new Widget(this.window.Root);
				band.Margins = new Margins(0, 0, 10, 0);
				band.Dock = DockStyle.Top;

				StaticText label = new StaticText(band);
				label.Text = "Nom du champ";
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.PreferredWidth = 90;
				label.Margins = new Margins(0, 5, 0, 0);
				label.Dock = DockStyle.Left;

				this.fieldName = new TextField(band);
				this.fieldName.Dock = DockStyle.Fill;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonOk = new Button(footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = "Créer";
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Left;
				this.buttonOk.Margins = new Margins(0, 10, 0, 0);
				this.buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOkClicked);
				this.buttonOk.TabIndex = tabIndex++;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Left;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = tabIndex++;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateName();

			this.window.ShowDialog();
		}

		public void Initialise(string name)
		{
			this.initialName = name;
			this.selectedName = null;
		}

		public string SelectedName
		{
			get
			{
				return this.selectedName;
			}
		}


		protected void UpdateName()
		{
			this.fieldName.Text = this.initialName;
			this.fieldName.SelectAll();
			this.fieldName.Focus();
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

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.selectedName = this.fieldName.Text;
		}


		protected string						initialName;
		protected string						selectedName;
		protected TextField						fieldName;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
