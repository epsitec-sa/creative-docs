using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue des informations sur le document.
	/// </summary>
	public class Key : Abstract
	{
		public Key(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Key", 275, 190);
				this.window.Text = Res.Strings.Dialog.Key.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += this.HandleWindowKeyCloseClicked;

				this.tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Key.Label;
				label.Dock = DockStyle.Top;
				label.Margins = new Margins (6, 6, 6, 6);

				this.radio1 = new RadioButton(this.window.Root);
				this.radio1.PreferredHeight = radio1.PreferredHeight*1.2;
//-				this.radio1.SetClientZoom(1.2);
				this.radio1.Text = @"<font size=""120%"">" + Res.Strings.Dialog.Key.RadioDemo + "</font>";
				this.radio1.Dock = DockStyle.Top;
				this.radio1.Margins = new Margins (6, 6, 0, 0);
				this.radio1.ActiveStateChanged += this.HandleRadioActiveStateChanged;
				this.radio1.TabIndex = this.tabIndex++;
				this.radio1.Index = 1;
				this.radio1.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.radio2 = new RadioButton(this.window.Root);
				this.radio2.PreferredHeight = radio2.PreferredHeight*1.2;
//-				this.radio2.SetClientZoom(1.2);
				this.radio2.Text = @"<font size=""120%"">" + Res.Strings.Dialog.Key.RadioFull + "</font>";
				this.radio2.Dock = DockStyle.Top;
				this.radio2.Margins = new Margins (6, 6, 0, 0);
				this.radio2.ActiveStateChanged += this.HandleRadioActiveStateChanged;
				this.radio2.TabIndex = this.tabIndex++;
				this.radio2.Index = 2;
				this.radio2.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.keyLabel = new StaticText(this.window.Root);
				this.keyLabel.Text = Res.Strings.Dialog.Key.Number;
				this.keyLabel.Dock = DockStyle.Top;
				this.keyLabel.Margins = new Margins (6, 6, 20, 0);

				Viewport keys = new Viewport(this.window.Root);
				keys.PreferredHeight = 20;
				keys.Dock = DockStyle.Top;
				keys.Margins = new Margins (6, 0, 6, 0);
				keys.TabIndex = this.tabIndex++;

				this.key1 = new TextField(keys);
				this.key1.PreferredWidth = 50;
				this.key1.Dock = DockStyle.Left;
				this.key1.TextChanged += this.HandleKeyTextChanged;
				this.key1.TabIndex = this.tabIndex++;
				this.key1.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				label = new StaticText(keys);
				label.PreferredWidth = 15;
				label.Text = "-";
				label.ContentAlignment = ContentAlignment.MiddleCenter;
				label.Dock = DockStyle.Left;

				this.key2 = new TextField(keys);
				this.key2.PreferredWidth = 60;
				this.key2.Dock = DockStyle.Left;
				this.key2.TextChanged += this.HandleKeyTextChanged;
				this.key2.TabIndex = this.tabIndex++;
				this.key2.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				label = new StaticText(keys);
				label.PreferredWidth = 15;
				label.Text = "-";
				label.ContentAlignment = ContentAlignment.MiddleCenter;
				label.Dock = DockStyle.Left;

				this.key3 = new TextField(keys);
				this.key3.PreferredWidth = 40;
				this.key3.Dock = DockStyle.Left;
				this.key3.TextChanged += this.HandleKeyTextChanged;
				this.key3.TabIndex = this.tabIndex++;
				this.key3.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				label = new StaticText(keys);
				label.PreferredWidth = 15;
				label.Text = "-";
				label.ContentAlignment = ContentAlignment.MiddleCenter;
				label.Dock = DockStyle.Left;

				this.key4 = new TextField(keys);
				this.key4.PreferredWidth = 60;
				this.key4.Dock = DockStyle.Left;
				this.key4.TextChanged += this.HandleKeyTextChanged;
				this.key4.TabIndex = this.tabIndex++;
				this.key4.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Bouton OK.
				this.buttonOK = new Button(this.window.Root);
				this.buttonOK.PreferredWidth = 75;
				this.buttonOK.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOK.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOK.Anchor = AnchorStyles.BottomRight;
				this.buttonOK.Margins = new Margins(0, 6+75+6, 0, 6);
				this.buttonOK.Clicked += this.HandleKeyButtonOKClicked;
				this.buttonOK.TabIndex = this.tabIndex++;
				this.buttonOK.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Bouton annuler.
				this.buttonCancel = new Button(this.window.Root);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Anchor = AnchorStyles.BottomRight;
				this.buttonCancel.Margins = new Margins(0, 6, 0, 6);
				this.buttonCancel.Clicked += this.HandleKeyButtonCancelClicked;
				this.buttonCancel.TabIndex = this.tabIndex++;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.KeyFields = Common.Support.SerialAlgorithm.ReadCrDocSerial();
			this.UpdateRadio();
			this.UpdateKeys();
			this.UpdateButton();

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Key");
		}


		protected string KeyFields
		{
			set
			{
				if ( value != null && value.Length == 24 )
				{
					this.validKey = Common.Support.SerialAlgorithm.CheckSerial(value, 40);

					if ( this.validKey )
					{
						this.radio1.Enable = false;
					}
					else
					{
						this.radio1.Text = Res.Strings.Dialog.Key.Expired;
					}
					this.demo = false;
				}
				else
				{
					this.validKey = false;
					this.demo = true;
				}

				if ( this.validKey )
				{
					this.key1.Text = value.Substring( 0, 5);
					this.key2.Text = value.Substring( 6, 6);
					this.key3.Text = value.Substring(13, 4);
					this.key4.Text = value.Substring(18, 6);

					this.buttonOK.Focus();
				}
				else
				{
					this.key1.Text = "";
					this.key2.Text = "";
					this.key3.Text = "";
					this.key4.Text = "";

					this.key1.Focus();
				}
			}

			get
			{
				return string.Format("{0}-{1}-{2}-{3}", this.key1.Text, this.key2.Text, this.key3.Text, this.key4.Text);
			}
		}

		protected void UpdateRadio()
		{
			this.radio1.ActiveState = this.demo ? ActiveState.Yes : ActiveState.No;
			this.radio2.ActiveState = this.demo ? ActiveState.No : ActiveState.Yes;
		}

		protected void UpdateKeys()
		{
			bool enable = !this.demo;

//-			enable = false;

			this.keyLabel.Enable = enable;
			this.key1.Enable = enable;
			this.key2.Enable = enable;
			this.key3.Enable = enable;
			this.key4.Enable = enable;
		}

		protected void UpdateButton()
		{
			this.buttonOK.Enable = (this.demo || this.validKey);
		}



		private void HandleRadioActiveStateChanged(object sender)
		{
			bool demo = (this.radio1.ActiveState == ActiveState.Yes);
			if ( demo == this.demo )  return;
			this.demo = demo;

			this.UpdateKeys();
			this.UpdateButton();

			if ( !this.demo )
			{
				this.key1.Focus();
			}
		}

		private void HandleKeyTextChanged(object sender)
		{
			string key = this.KeyFields;
			this.validKey = Common.Support.SerialAlgorithm.CheckSerial(key, 40);
			this.UpdateButton();

			if ( sender == this.key1 )
			{
				if ( this.key1.Text.Length >= 5 )
				{
					this.key2.SelectAll();
					this.key2.Focus();
				}
			}

			if ( sender == this.key2 )
			{
				if ( this.key2.Text.Length >= 6 )
				{
					this.key3.SelectAll();
					this.key3.Focus();
				}
			}

			if ( sender == this.key3 )
			{
				if ( this.key3.Text.Length >= 4 )
				{
					this.key4.SelectAll();
					this.key4.Focus();
				}
			}

			if ( sender == this.key4 )
			{
				if ( this.key4.Text.Length >= 6 )
				{
					if ( this.validKey )
					{
						this.buttonOK.Focus();
					}
					else
					{
						this.key1.SelectAll();
						this.key1.Focus();
					}
				}
			}
		}

		private void HandleWindowKeyCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleKeyButtonOKClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();

			if ( this.validKey && !this.demo )
			{
				this.editor.InstallType = InstallType.Full;
				Common.Support.SerialAlgorithm.WriteCrDocSerial(this.KeyFields);
			}
			else
			{
				if ( this.editor.InstallType == InstallType.Full )
				{
					this.editor.InstallType = InstallType.Expired;
				}
				else if ( this.editor.InstallType == InstallType.Expired )
				{
					this.editor.InstallType = InstallType.Expired;
				}
				else if ( this.editor.InstallType == InstallType.Demo )
				{
					this.editor.InstallType = InstallType.Demo;
					Common.Support.SerialAlgorithm.WriteCrDocSerial("demo");
				}
			}
		}

		private void HandleKeyButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}


		protected int					tabIndex;
		protected RadioButton			radio1;
		protected RadioButton			radio2;
		protected StaticText			keyLabel;
		protected TextField				key1;
		protected TextField				key2;
		protected TextField				key3;
		protected TextField				key4;
		protected Button				buttonOK;
		protected Button				buttonCancel;
		protected bool					demo = true;
		protected bool					validKey = false;
	}
}
