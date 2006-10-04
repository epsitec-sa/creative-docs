using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le type d'un Caption.Type.
	/// </summary>
	public class ResourceTypeType : Abstract
	{
		public ResourceTypeType(MainWindow mainWindow) : base(mainWindow)
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
				this.WindowInit("TypeType", 250, 200, true);
				this.window.Text = "Choix du type à créer";  // Res.Strings.Dialog.TypeType.Title;
				this.window.Owner = this.parentWindow;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				this.tabIndex = 0;
				this.radioButtons = new List<RadioButton>();
				this.CreateRadio(ResourceAccess.TypeType.Void,        "Vide");
				this.CreateRadio(ResourceAccess.TypeType.Boolean,     "Booléen");
				this.CreateRadio(ResourceAccess.TypeType.Integer,     "Entier");
				this.CreateRadio(ResourceAccess.TypeType.LongInteger, "Entier long");
				this.CreateRadio(ResourceAccess.TypeType.Decimal,     "Décimal");
				this.CreateRadio(ResourceAccess.TypeType.String,      "Chaîne de caractères");
				this.CreateRadio(ResourceAccess.TypeType.Enum,        "Enumération");
				this.CreateRadio(ResourceAccess.TypeType.Structured,  "Structure");

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Left;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
				buttonOk.TabIndex = this.tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Cancel;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Dock = DockStyle.Left;
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.UpdateRadios();

			this.window.ShowDialog();
		}

		public ResourceAccess.TypeType ContentType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}


		protected void CreateRadio(ResourceAccess.TypeType type, string text)
		{
			RadioButton button = new RadioButton(this.window.Root);
			button.Name = ResourceAccess.ConvTypeType(type);
			button.Text = string.Format("{0} ({1})", text, type.ToString());
			button.Dock = DockStyle.Top;
			button.Margins = new Margins(0, 0, 2, 2);
			button.TabIndex = tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button.Clicked += new MessageEventHandler(this.HandleRadioButtonClicked);

			this.radioButtons.Add(button);
		}

		protected void UpdateRadios()
		{
			string actual = ResourceAccess.ConvTypeType(this.type);

			foreach (RadioButton button in this.radioButtons)
			{
				button.ActiveState = (button.Name == actual) ? ActiveState.Yes : ActiveState.No;
			}
		}


		private void HandleRadioButtonClicked(object sender, MessageEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			this.type = ResourceAccess.ConvTypeType(button.Name);
			this.UpdateRadios();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.type = ResourceAccess.TypeType.None;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOKClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		protected ResourceAccess.TypeType		type;
		protected List<RadioButton>				radioButtons;
		protected int							tabIndex;
	}
}
