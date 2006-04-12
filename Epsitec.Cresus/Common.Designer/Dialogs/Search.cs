using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le texte de recherche.
	/// </summary>
	public class Search : Abstract
	{
		public Search(MainWindow mainWindow) : base(mainWindow)
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
				this.window.MakeToolWindow();
				this.WindowInit("Search", 257, 140, true);
				this.window.Text = Res.Strings.Dialog.Search.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Search.Label;
				label.Alignment = ContentAlignment.MiddleLeft;
				label.Width = 60;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldSearch = new TextFieldCombo(this.window.Root);
				this.fieldSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldSearch.Margins = new Margins(6+60, 6, 6, 0);
				this.fieldSearch.TabIndex = tabIndex++;

				this.radioReverse = new RadioButton(this.window.Root);
				this.radioReverse.Group = "Direction";
				this.radioReverse.Text = Res.Strings.Dialog.Search.Radio.Reverse;
				this.radioReverse.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.radioReverse.Margins = new Margins(6+60, 6, 6+32+16*0, 0);
				this.radioReverse.TabIndex = tabIndex++;

				this.radioNormal = new RadioButton(this.window.Root);
				this.radioNormal.ActiveState = ActiveState.Yes;
				this.radioNormal.Group = "Direction";
				this.radioNormal.Text = Res.Strings.Dialog.Search.Radio.Normal;
				this.radioNormal.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.radioNormal.Margins = new Margins(6+60, 6, 6+32+16*1, 0);
				this.radioNormal.TabIndex = tabIndex++;

				this.checkCase = new CheckButton(this.window.Root);
				this.checkCase.Text = Res.Strings.Dialog.Search.Check.Case;
				this.checkCase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkCase.Margins = new Margins(6+60, 6, 6+32+16*2+4, 0);
				this.checkCase.TabIndex = tabIndex++;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.Search.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonSearchClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.window.Show();

			this.fieldSearch.Focus();
			this.fieldSearch.SelectAll();
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

		private void HandleButtonSearchClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			bool isReverse = (this.radioReverse.ActiveState == ActiveState.Yes);
			bool isCase    = (this.checkCase.ActiveState == ActiveState.Yes);
			module.Modifier.ActiveViewer.DoSearch(this.fieldSearch.Text, isReverse, isCase);

			Misc.ComboMenuAdd(this.fieldSearch, this.fieldSearch.Text);
		}


		protected TextFieldCombo				fieldSearch;
		protected RadioButton					radioReverse;
		protected RadioButton					radioNormal;
		protected CheckButton					checkCase;
	}
}
