using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'éditer les paramètres d'une entité.
	/// </summary>
	public class EntityParameters : Abstract
	{
		public EntityParameters(DesignerApplication designerApplication)
			: base (designerApplication)
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
				this.window.PreventAutoClose = true;
				this.WindowInit ("EntityParameters", 350, 250, true);
				this.window.Text = "Paramètres";  // Res.Strings.Dialog.EntityComment.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size(200, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Crée la toolbar et son contenu.
				this.toolbar = new HToolBar(this.window.Root);
				this.toolbar.Margins = new Margins(0, 0, 0, 3);
				this.toolbar.Dock = DockStyle.Top;

				this.CreateButton("FontBold");
				this.CreateButton("FontItalic");
				this.CreateButton("FontUnderline");

				//	Crée le grand pavé de texte éditable.
				this.fieldText = new TextFieldMulti(this.window.Root);
				this.fieldText.Dock = DockStyle.Fill;
				this.fieldText.TabIndex = 1;

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

			this.UpdateText();

			this.window.ShowDialog();
		}

		public void Initialise(string name)
		{
			this.initialText = name;
			this.selectedText = null;
		}

		public string SelectedText
		{
			get
			{
				return this.selectedText;
			}
		}


		protected void CreateButton(string name)
		{
			IconButton button = new IconButton();
			button.Name = name;
			button.IconUri = Misc.Icon(name);
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Clicked += this.HandleButtonClicked;

			this.toolbar.Items.Add(button);
		}

		protected void UpdateText()
		{
			this.fieldText.Text = this.initialText;
			this.fieldText.SelectAll();
			this.fieldText.Focus();
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

			this.selectedText = this.fieldText.Text;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			if (button.Name == "FontBold")
			{
				this.fieldText.TextNavigator.SelectionBold = !this.fieldText.TextNavigator.SelectionBold;
			}

			if (button.Name == "FontItalic")
			{
				this.fieldText.TextNavigator.SelectionItalic = !this.fieldText.TextNavigator.SelectionItalic;
			}

			if (button.Name == "FontUnderline")
			{
				this.fieldText.TextNavigator.SelectionUnderline = !this.fieldText.TextNavigator.SelectionUnderline;
			}
		}


		protected string						initialText;
		protected string						selectedText;
		protected HToolBar						toolbar;
		protected TextFieldMulti				fieldText;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
