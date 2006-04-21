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
				this.WindowInit("Search", 375, 256, true);
				this.window.Text = Res.Strings.Dialog.Search.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Search.Label;
				label.Alignment = ContentAlignment.MiddleLeft;
				label.Width = 80;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldSearch = new TextFieldCombo(this.window.Root);
				this.fieldSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldSearch.Margins = new Margins(6+80, 6, 6, 0);
				this.fieldSearch.TabIndex = tabIndex++;
				this.fieldSearch.TextChanged += new EventHandler(this.HandleFieldSearchTextChanged);

				label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Search.Replace;
				label.Alignment = ContentAlignment.MiddleLeft;
				label.Width = 80;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+28+3, 0);

				this.fieldReplace = new TextFieldCombo(this.window.Root);
				this.fieldReplace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldReplace.Margins = new Margins(6+80, 6, 6+28, 0);
				this.fieldReplace.TabIndex = tabIndex++;

				this.checkCase = new CheckButton(this.window.Root);
				this.checkCase.Text = Res.Strings.Dialog.Search.Check.Case;
				this.checkCase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkCase.Margins = new Margins(6+80, 6, 6+58+16*0, 0);
				this.checkCase.TabIndex = tabIndex++;

				this.checkWord = new CheckButton(this.window.Root);
				this.checkWord.Text = Res.Strings.Dialog.Search.Check.Word;
				this.checkWord.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkWord.Margins = new Margins(6+80, 6, 6+58+16*1, 0);
				this.checkWord.TabIndex = tabIndex++;

				this.checkLabel = new CheckButton(this.window.Root);
				this.checkLabel.Text = Res.Strings.Dialog.Search.Check.Label;
				this.checkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkLabel.Margins = new Margins(6+80, 6, 6+58+16*2+4, 0);
				this.checkLabel.TabIndex = tabIndex++;

				this.checkPrimaryText = new CheckButton(this.window.Root);
				this.checkPrimaryText.ActiveState = ActiveState.Yes;
				this.checkPrimaryText.Text = Res.Strings.Dialog.Search.Check.PrimaryText;
				this.checkPrimaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkPrimaryText.Margins = new Margins(6+80, 6, 6+58+16*3+4, 0);
				this.checkPrimaryText.TabIndex = tabIndex++;

				this.checkSecondaryText = new CheckButton(this.window.Root);
				this.checkSecondaryText.ActiveState = ActiveState.Yes;
				this.checkSecondaryText.Text = Res.Strings.Dialog.Search.Check.SecondaryText;
				this.checkSecondaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkSecondaryText.Margins = new Margins(6+80, 6, 6+58+16*4+4, 0);
				this.checkSecondaryText.TabIndex = tabIndex++;

				this.checkPrimaryAbout = new CheckButton(this.window.Root);
				this.checkPrimaryAbout.ActiveState = ActiveState.Yes;
				this.checkPrimaryAbout.Text = Res.Strings.Dialog.Search.Check.PrimaryAbout;
				this.checkPrimaryAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkPrimaryAbout.Margins = new Margins(6+80, 6, 6+58+16*5+4, 0);
				this.checkPrimaryAbout.TabIndex = tabIndex++;

				this.checkSecondaryAbout = new CheckButton(this.window.Root);
				this.checkSecondaryAbout.ActiveState = ActiveState.Yes;
				this.checkSecondaryAbout.Text = Res.Strings.Dialog.Search.Check.SecondaryAbout;
				this.checkSecondaryAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkSecondaryAbout.Margins = new Margins(6+80, 6, 6+58+16*6+4, 0);
				this.checkSecondaryAbout.TabIndex = tabIndex++;

				//	Boutons de fermeture.
				this.buttonSearchPrev = new Button(this.window.Root);
				this.buttonSearchPrev.Name = "SearchPrev";
				this.buttonSearchPrev.Width = 85;
				this.buttonSearchPrev.Text = string.Concat(Res.Strings.Dialog.Search.Button.Search, " ", Misc.Image("SearchPrevButton"));
				this.buttonSearchPrev.Anchor = AnchorStyles.BottomLeft;
				this.buttonSearchPrev.Margins = new Margins(6+(85+5)*0, 0, 0, 6+30);
				this.buttonSearchPrev.Clicked += new MessageEventHandler(this.HandleButtonSearchClicked);
				this.buttonSearchPrev.TabIndex = tabIndex++;
				this.buttonSearchPrev.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonReplacePrev = new Button(this.window.Root);
				this.buttonReplacePrev.Name = "ReplacePrev";
				this.buttonReplacePrev.Width = 85;
				this.buttonReplacePrev.Text = string.Concat(Res.Strings.Dialog.Search.Button.Replace, " ", Misc.Image("SearchPrevButton"));
				this.buttonReplacePrev.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplacePrev.Margins = new Margins(6+(85+5)*1, 0, 0, 6+30);
				this.buttonReplacePrev.Clicked += new MessageEventHandler(this.HandleButtonReplaceClicked);
				this.buttonReplacePrev.TabIndex = tabIndex++;
				this.buttonReplacePrev.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCount = new Button(this.window.Root);
				this.buttonCount.Name = "Count";
				this.buttonCount.Width = 85;
				this.buttonCount.Text = Res.Strings.Dialog.Search.Button.Count;
				this.buttonCount.Anchor = AnchorStyles.BottomLeft;
				this.buttonCount.Margins = new Margins(6+(85+5)*2, 0, 0, 6+30);
				this.buttonCount.Clicked += new MessageEventHandler(this.HandleButtonCountClicked);
				this.buttonCount.TabIndex = tabIndex++;
				this.buttonCount.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;


				this.buttonSearchNext = new Button(this.window.Root);
				this.buttonSearchNext.Name = "buttonSearchNext";
				this.buttonSearchNext.Width = 85;
				this.buttonSearchNext.Text = string.Concat(Res.Strings.Dialog.Search.Button.Search, " ", Misc.Image("SearchNextButton"));
				this.buttonSearchNext.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonSearchNext.Anchor = AnchorStyles.BottomLeft;
				this.buttonSearchNext.Margins = new Margins(6+(85+5)*0, 0, 0, 6);
				this.buttonSearchNext.Clicked += new MessageEventHandler(this.HandleButtonSearchClicked);
				this.buttonSearchNext.TabIndex = tabIndex++;
				this.buttonSearchNext.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonReplaceNext = new Button(this.window.Root);
				this.buttonReplaceNext.Name = "ReplaceNext";
				this.buttonReplaceNext.Width = 85;
				this.buttonReplaceNext.Text = string.Concat(Res.Strings.Dialog.Search.Button.Replace, " ", Misc.Image("SearchNextButton"));
				this.buttonReplaceNext.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplaceNext.Margins = new Margins(6+(85+5)*1, 0, 0, 6);
				this.buttonReplaceNext.Clicked += new MessageEventHandler(this.HandleButtonReplaceClicked);
				this.buttonReplaceNext.TabIndex = tabIndex++;
				this.buttonReplaceNext.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonReplaceAll = new Button(this.window.Root);
				this.buttonReplaceAll.Name = "ReplaceAll";
				this.buttonReplaceAll.Width = 85;
				this.buttonReplaceAll.Text = Res.Strings.Dialog.Search.Button.ReplaceAll;
				this.buttonReplaceAll.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplaceAll.Margins = new Margins(6+(85+5)*2, 0, 0, 6);
				this.buttonReplaceAll.Clicked += new MessageEventHandler(this.HandleButtonReplaceAllClicked);
				this.buttonReplaceAll.TabIndex = tabIndex++;
				this.buttonReplaceAll.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 6, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.HandleFieldSearchTextChanged(null);
			}

			this.window.Show();

			this.fieldSearch.Focus();
			this.fieldSearch.SelectAll();
		}


		public string Searching
		{
			get
			{
				if (this.fieldSearch == null)
				{
					return "";
				}
				else
				{
					return this.fieldSearch.Text;
				}
			}
		}

		public Searcher.SearchingMode Mode
		{
			get
			{
				Searcher.SearchingMode mode = Searcher.SearchingMode.None;
				if (this.checkCase != null)
				{
					if (this.checkCase.ActiveState            == ActiveState.Yes)  mode |= Searcher.SearchingMode.CaseSensitive;
					if (this.checkWord.ActiveState            == ActiveState.Yes)  mode |= Searcher.SearchingMode.WholeWord;
					if (this.checkLabel.ActiveState           == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInLabel;
					if (this.checkPrimaryText.ActiveState     == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryText;
					if (this.checkSecondaryText.ActiveState   == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryText;
					if (this.checkPrimaryAbout.ActiveState    == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryAbout;
					if (this.checkSecondaryAbout.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryAbout;
				}
				return mode;
			}
		}

		void HandleFieldSearchTextChanged(object sender)
		{
			Module module = this.mainWindow.CurrentModule;
			if (module == null)  return;

			bool enable = (this.fieldSearch.Text != "");

			this.buttonSearchPrev.Enable = enable;
			this.buttonSearchNext.Enable = enable;
			this.buttonCount.Enable = enable;
			this.buttonReplaceAll.Enable = enable;
			this.buttonReplacePrev.Enable = enable;
			this.buttonReplaceNext.Enable = enable;

			this.mainWindow.GetCommandState("SearchPrev").Enable = enable;
			this.mainWindow.GetCommandState("SearchNext").Enable = enable;
		}

		private void HandleWindowCloseClicked(object sender)
		{
			//	TODO: si on ferme la fen�tre ainsi, on ne peut plus la rouvrir !
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
			Button button = sender as Button;

			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = this.Mode;
			if ( button.Name == "SearchPrev" )
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			module.Modifier.ActiveViewer.DoSearch(this.fieldSearch.Text, mode);

			Misc.ComboMenuAdd(this.fieldSearch, this.fieldSearch.Text);
		}

		private void HandleButtonCountClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoCount(this.fieldSearch.Text, this.Mode);

			Misc.ComboMenuAdd(this.fieldSearch, this.fieldSearch.Text);
		}

		private void HandleButtonReplaceClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;

			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = this.Mode;
			if ( button.Name == "ReplacePrev" )
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			module.Modifier.ActiveViewer.DoReplace(this.fieldSearch.Text, this.fieldReplace.Text, mode);

			Misc.ComboMenuAdd(this.fieldSearch, this.fieldSearch.Text);
			Misc.ComboMenuAdd(this.fieldReplace, this.fieldReplace.Text);
		}

		private void HandleButtonReplaceAllClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoReplaceAll(this.fieldSearch.Text, this.fieldReplace.Text, this.Mode);

			Misc.ComboMenuAdd(this.fieldSearch, this.fieldSearch.Text);
			Misc.ComboMenuAdd(this.fieldReplace, this.fieldReplace.Text);
		}


		protected TextFieldCombo				fieldSearch;
		protected TextFieldCombo				fieldReplace;
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;
		protected CheckButton					checkLabel;
		protected CheckButton					checkPrimaryText;
		protected CheckButton					checkSecondaryText;
		protected CheckButton					checkPrimaryAbout;
		protected CheckButton					checkSecondaryAbout;
		protected Button						buttonSearchPrev;
		protected Button						buttonSearchNext;
		protected Button						buttonCount;
		protected Button						buttonReplaceAll;
		protected Button						buttonReplacePrev;
		protected Button						buttonReplaceNext;
	}
}
