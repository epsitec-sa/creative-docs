using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le nom d'une ressource qui va �tre cr��e.
	/// </summary>
	public class ResourceName : Abstract
	{
		public enum Operation
		{
			Create,
			Modify,
		}

		public enum Type
		{
			Entity,
			Field,
			Value,
		}


		public ResourceName(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("ResourceName", 300, 150, true);
				this.window.Text = "Choix d'un nom";  // Res.Strings.Dialog.ResourceName.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				//	Titre sup�rieur.
				this.title = new StaticText(this.window.Root);
				this.title.ContentAlignment = ContentAlignment.TopLeft;
				this.title.PreferredHeight = 34;
				this.title.Dock = DockStyle.Top;

				Separator sep = new Separator(this.window.Root);  // trait horizontal de s�paration
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Top;

				//	Partie principale.
				Widget band = new Widget(this.window.Root);
				band.Margins = new Margins(0, 0, 20, 0);
				band.Dock = DockStyle.Top;

				StaticText label = new StaticText(band);
				label.Text = "Nom";
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.PreferredWidth = 40;
				label.Margins = new Margins(0, 8, 0, 0);
				label.Dock = DockStyle.Left;

				this.resourceName = new TextField(band);
				this.resourceName.Dock = DockStyle.Fill;
				this.resourceName.TabIndex = 1;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOk = new Button(footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins(0, 6, 0, 0);
				this.buttonOk.Clicked += this.HandleButtonOkClicked;
				this.buttonOk.TabIndex = 10;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateTitle();
			this.UpdateName();

			this.window.ShowDialog();
		}

		public void Initialise(Operation operation, Type type, string name)
		{
			this.operation = operation;
			this.type = type;
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


		protected void UpdateTitle()
		{
			//	Initialise le titre dans la partie sup�rieure du dialogue, qui explique
			//	l'op�ration effectu�e.
			string text, ok;
			switch (this.operation)
			{
				case Operation.Create:
					text = "Cr�ation {0}";
					ok = "Cr�er";
					break;

				case Operation.Modify:
					text = "Modification {0}";
					ok = "Modifier";
					break;

				default:
					text = "Nom {0}";
					ok = "Ok";
					break;
			}

			string type;
			switch (this.type)
			{
				case Type.Entity:
					type = "d'une entit�";
					break;

				case Type.Field:
					type = "d'un champ";
					break;

				case Type.Value:
					type = "d'une valeur";
					break;

				default:
					type = "d'une ressource";
					break;
			}

			text = string.Concat("<font size=\"200%\"><b>", string.Format(text, type), "</b></font>");
			this.title.Text = text;

			this.buttonOk.Text = ok;
		}

		protected void UpdateName()
		{
			this.resourceName.Text = this.initialName;
			this.resourceName.SelectAll();
			this.resourceName.Focus();
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

			this.selectedName = this.resourceName.Text;
		}


		protected Operation						operation;
		protected Type							type;
		protected string						initialName;
		protected string						selectedName;
		protected StaticText					title;
		protected TextField						resourceName;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
