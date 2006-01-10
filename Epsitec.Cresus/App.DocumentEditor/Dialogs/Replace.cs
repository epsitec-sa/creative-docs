using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue chercher/remplacer.
	/// </summary>
	public class Replace : Abstract
	{
		public Replace(DocumentEditor editor) : base(editor)
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
				this.WindowInit("Replace", 275, 190);
				this.window.Text = Res.Strings.Dialog.Replace.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				this.tabIndex = 0;


				//	Bouton Chercher.
				this.buttonFind = new Button(this.window.Root);
				this.buttonFind.Width = 75;
				this.buttonFind.Text = Res.Strings.Dialog.Replace.Button.Find;
				this.buttonFind.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonFind.Anchor = AnchorStyles.BottomLeft;
				this.buttonFind.AnchorMargins = new Margins(10, 0, 0, 10);
				this.buttonFind.Clicked += new MessageEventHandler(this.HandleButtonFindClicked);
				this.buttonFind.TabIndex = this.tabIndex++;
				this.buttonFind.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Bouton Replacer.
				this.buttonReplace = new Button(this.window.Root);
				this.buttonReplace.Width = 75;
				this.buttonReplace.Text = Res.Strings.Dialog.Replace.Button.Replace;
				this.buttonReplace.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplace.AnchorMargins = new Margins(10+75+10, 0, 0, 10);
				this.buttonReplace.Clicked += new MessageEventHandler(this.HandleButtonReplaceClicked);
				this.buttonReplace.TabIndex = this.tabIndex++;
				this.buttonReplace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Bouton annuler.
				this.buttonClose = new Button(this.window.Root);
				this.buttonClose.Width = 75;
				this.buttonClose.Text = Res.Strings.Dialog.Button.Close;
				this.buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonClose.Anchor = AnchorStyles.BottomLeft;
				this.buttonClose.AnchorMargins = new Margins(10+75+10+75+10, 0, 0, 10);
				this.buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonClose.TabIndex = this.tabIndex++;
				this.buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.window.Show();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Replace");
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonFindClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonReplaceClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		protected int					tabIndex;
		protected Button				buttonFind;
		protected Button				buttonReplace;
		protected Button				buttonClose;
	}
}
