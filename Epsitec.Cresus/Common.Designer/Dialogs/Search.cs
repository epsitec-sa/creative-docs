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
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.MakeFixedSizeWindow();
				this.window.MakeToolWindow();
				this.WindowInit("Search", 340, 240, true);
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

				this.checkWord = new CheckButton(this.window.Root);
				this.checkWord.Text = Res.Strings.Dialog.Search.Check.Word;
				this.checkWord.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkWord.Margins = new Margins(6+60, 6, 6+32+16*3+4, 0);
				this.checkWord.TabIndex = tabIndex++;

				this.checkLabel = new CheckButton(this.window.Root);
				this.checkLabel.Text = Res.Strings.Dialog.Search.Check.Label;
				this.checkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkLabel.Margins = new Margins(6+60, 6, 6+32+16*4+8, 0);
				this.checkLabel.TabIndex = tabIndex++;

				this.checkPrimaryText = new CheckButton(this.window.Root);
				this.checkPrimaryText.ActiveState = ActiveState.Yes;
				this.checkPrimaryText.Text = Res.Strings.Dialog.Search.Check.PrimaryText;
				this.checkPrimaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkPrimaryText.Margins = new Margins(6+60, 6, 6+32+16*5+8, 0);
				this.checkPrimaryText.TabIndex = tabIndex++;

				this.checkSecondaryText = new CheckButton(this.window.Root);
				this.checkSecondaryText.ActiveState = ActiveState.Yes;
				this.checkSecondaryText.Text = Res.Strings.Dialog.Search.Check.SecondaryText;
				this.checkSecondaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkSecondaryText.Margins = new Margins(6+60, 6, 6+32+16*6+8, 0);
				this.checkSecondaryText.TabIndex = tabIndex++;

				this.checkPrimaryAbout = new CheckButton(this.window.Root);
				this.checkPrimaryAbout.ActiveState = ActiveState.Yes;
				this.checkPrimaryAbout.Text = Res.Strings.Dialog.Search.Check.PrimaryAbout;
				this.checkPrimaryAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkPrimaryAbout.Margins = new Margins(6+60, 6, 6+32+16*7+8, 0);
				this.checkPrimaryAbout.TabIndex = tabIndex++;

				this.checkSecondaryAbout = new CheckButton(this.window.Root);
				this.checkSecondaryAbout.ActiveState = ActiveState.Yes;
				this.checkSecondaryAbout.Text = Res.Strings.Dialog.Search.Check.SecondaryAbout;
				this.checkSecondaryAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkSecondaryAbout.Margins = new Margins(6+60, 6, 6+32+16*8+8, 0);
				this.checkSecondaryAbout.TabIndex = tabIndex++;

				//	Boutons de fermeture.
				Button buttonSearch = new Button(this.window.Root);
				buttonSearch.Width = 75;
				buttonSearch.Text = Res.Strings.Dialog.Search.Button.Search;
				buttonSearch.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonSearch.Anchor = AnchorStyles.BottomLeft;
				buttonSearch.Margins = new Margins(6, 0, 0, 6);
				buttonSearch.Clicked += new MessageEventHandler(this.HandleButtonSearchClicked);
				buttonSearch.TabIndex = tabIndex++;
				buttonSearch.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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

			Searcher.SearchingMode mode = Searcher.SearchingMode.None;
			if (this.radioReverse.ActiveState         == ActiveState.Yes)  mode |= Searcher.SearchingMode.Reverse;
			if (this.checkCase.ActiveState            == ActiveState.Yes)  mode |= Searcher.SearchingMode.CaseSensitive;
			if (this.checkWord.ActiveState            == ActiveState.Yes)  mode |= Searcher.SearchingMode.WholeWord;
			if (this.checkLabel.ActiveState           == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInLabel;
			if (this.checkPrimaryText.ActiveState     == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryText;
			if (this.checkSecondaryText.ActiveState   == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryText;
			if (this.checkPrimaryAbout.ActiveState    == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryAbout;
			if (this.checkSecondaryAbout.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryAbout;
			module.Modifier.ActiveViewer.DoSearch(this.fieldSearch.Text, mode);

			Misc.ComboMenuAdd(this.fieldSearch, this.fieldSearch.Text);
		}


		protected TextFieldCombo				fieldSearch;
		protected RadioButton					radioReverse;
		protected RadioButton					radioNormal;
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;
		protected CheckButton					checkLabel;
		protected CheckButton					checkPrimaryText;
		protected CheckButton					checkSecondaryText;
		protected CheckButton					checkPrimaryAbout;
		protected CheckButton					checkSecondaryAbout;
	}
}
