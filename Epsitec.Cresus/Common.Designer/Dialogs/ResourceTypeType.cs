using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

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
				this.WindowInit("TypeType", 172, 190, true);
				this.window.Text = Res.Strings.Dialog.TypeType.Title;
				this.window.Owner = this.parentWindow;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				this.tabIndex = 0;
				this.index = 0;
				this.radioButtons = new List<RadioButton>();
				this.CreateRadio(Res.Captions.Types.Type.Void);
				this.CreateRadio(Res.Captions.Types.Type.Boolean);
				this.CreateRadio(Res.Captions.Types.Type.Integer);
				this.CreateRadio(Res.Captions.Types.Type.LongInteger);
				this.CreateRadio(Res.Captions.Types.Type.Decimal);
				this.CreateRadio(Res.Captions.Types.Type.String);
				this.CreateRadio(Res.Captions.Types.Type.Enum);
				this.CreateRadio(Res.Captions.Types.Type.Structured);
				this.tabIndex++;

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
				buttonClose.TabIndex = this.tabIndex++;
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


		protected void CreateRadio(Caption caption)
		{
			//	Crée un bouton radio.
			RadioButton button = new RadioButton(this.window.Root);
			button.CaptionDruid = caption.Druid;
			System.Diagnostics.Debug.Assert(button.Name.StartsWith("Types.Type."));
			button.Dock = DockStyle.Top;
			button.Margins = new Margins(0, 0, 2, 2);
			button.Index = this.index++;
			button.TabIndex = this.tabIndex;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button.ActiveStateChanged += new EventHandler(this.HandleRadioButtonActiveStateChanged);

			this.radioButtons.Add(button);
		}

		protected void UpdateRadios()
		{
			//	Met à jour le bouton radio enfoncé en fonction du type.
			string actual = ResourceAccess.ConvTypeType(this.type);

			this.ignoreChanged = true;
			foreach (RadioButton button in this.radioButtons)
			{
				string name = button.Name;
				name = name.Substring(name.LastIndexOf('.')+1);  // enlève "Res.Captions.Types.Type."

				button.ActiveState = (name == actual) ? ActiveState.Yes : ActiveState.No;

				if (name == actual)
				{
					button.Focus();
				}
			}
			this.ignoreChanged = false;
		}


		private void HandleRadioButtonActiveStateChanged(object sender)
		{
			if (this.ignoreChanged)
			{
				return;
			}

			RadioButton button = sender as RadioButton;
			string name = button.Name;
			name = name.Substring(name.LastIndexOf('.')+1);  // enlève "Res.Captions.Types.Type."
			this.type = ResourceAccess.ConvTypeType(name);
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
		protected int							index;
		protected int							tabIndex;
	}
}
