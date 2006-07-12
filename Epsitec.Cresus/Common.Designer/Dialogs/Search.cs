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
				this.window.PreventAutoClose = true;
				this.WindowInit("Search", 375, 200, true);
				this.window.Text = Res.Strings.Dialog.Search.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Search.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.PreferredWidth = 80;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldSearch = new TextFieldCombo(this.window.Root);
				this.fieldSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldSearch.Margins = new Margins(6+80, 6, 6, 0);
				this.fieldSearch.TabIndex = tabIndex++;
				this.fieldSearch.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.fieldSearch.TextChanged += new EventHandler(this.HandleFieldSearchTextChanged);

				label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Search.Replace;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.PreferredWidth = 80;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+28+3, 0);

				this.fieldReplace = new TextFieldCombo(this.window.Root);
				this.fieldReplace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldReplace.Margins = new Margins(6+80, 6, 6+28, 0);
				this.fieldReplace.TabIndex = tabIndex++;
				this.fieldReplace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Crée le groupe pour le type 'Strings'.
				this.groupStrings = new GroupBox(this.window.Root);
				this.groupStrings.Text = Res.Strings.Dialog.Search.Check.Who;
				this.groupStrings.PreferredWidth = 160;
				this.groupStrings.PreferredHeight = 60;
				this.groupStrings.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.groupStrings.Margins = new Margins(6, 0, 6+58+16*0, 0);
				this.groupStrings.Padding = new Margins(5, 5, 5, 5);
				this.groupStrings.TabIndex = tabIndex++;
				this.groupStrings.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

				label = new StaticText(this.groupStrings);
				label.PreferredWidth = 80;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Text = Res.Strings.Viewers.Strings.Edit;
				label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				label.Margins = new Margins(0, 0, 0, 0);

				label = new StaticText(this.groupStrings);
				label.PreferredWidth = 80;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Text = Res.Strings.Viewers.Strings.About;
				label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				label.Margins = new Margins(0, 0, 16, 0);

				this.stringsCheckLabel = new CheckButton(this.groupStrings);
				this.stringsCheckLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.stringsCheckLabel.PreferredWidth = this.stringsCheckLabel.PreferredHeight;
				this.stringsCheckLabel.Margins = new Margins(90+20*0, 0, 0, 0);
				this.stringsCheckLabel.TabIndex = tabIndex++;
				this.stringsCheckLabel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.stringsCheckLabel.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.stringsCheckLabel, Res.Strings.Dialog.Search.Check.Label);

				this.stringsCheckPrimaryText = new CheckButton(this.groupStrings);
				this.stringsCheckPrimaryText.ActiveState = ActiveState.Yes;
				this.stringsCheckPrimaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.stringsCheckPrimaryText.PreferredWidth = this.stringsCheckPrimaryText.PreferredHeight;
				this.stringsCheckPrimaryText.Margins = new Margins(90+20*1, 0, 0, 0);
				this.stringsCheckPrimaryText.TabIndex = tabIndex++;
				this.stringsCheckPrimaryText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.stringsCheckPrimaryText.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.stringsCheckPrimaryText, Res.Strings.Dialog.Search.Check.PrimaryText);

				this.stringsCheckSecondaryText = new CheckButton(this.groupStrings);
				this.stringsCheckSecondaryText.ActiveState = ActiveState.Yes;
				this.stringsCheckSecondaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.stringsCheckSecondaryText.PreferredWidth = this.stringsCheckSecondaryText.PreferredHeight;
				this.stringsCheckSecondaryText.Margins = new Margins(90+20*2, 0, 0, 0);
				this.stringsCheckSecondaryText.TabIndex = tabIndex++;
				this.stringsCheckSecondaryText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.stringsCheckSecondaryText.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.stringsCheckSecondaryText, Res.Strings.Dialog.Search.Check.SecondaryText);

				this.stringsCheckPrimaryAbout = new CheckButton(this.groupStrings);
				this.stringsCheckPrimaryAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.stringsCheckPrimaryAbout.PreferredWidth = this.stringsCheckPrimaryAbout.PreferredHeight;
				this.stringsCheckPrimaryAbout.Margins = new Margins(90+20*1, 0, 16, 0);
				this.stringsCheckPrimaryAbout.TabIndex = tabIndex++;
				this.stringsCheckPrimaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.stringsCheckPrimaryAbout.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.stringsCheckPrimaryAbout, Res.Strings.Dialog.Search.Check.PrimaryAbout);

				this.stringsCheckSecondaryAbout = new CheckButton(this.groupStrings);
				this.stringsCheckSecondaryAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.stringsCheckSecondaryAbout.PreferredWidth = this.stringsCheckSecondaryAbout.PreferredHeight;
				this.stringsCheckSecondaryAbout.Margins = new Margins(90+20*2, 0, 16, 0);
				this.stringsCheckSecondaryAbout.TabIndex = tabIndex++;
				this.stringsCheckSecondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.stringsCheckSecondaryAbout.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.stringsCheckSecondaryAbout, Res.Strings.Dialog.Search.Check.SecondaryAbout);

				//	Crée le groupe pour le type 'Captions'.
				this.groupCaptions = new GroupBox(this.window.Root);
				this.groupCaptions.Text = Res.Strings.Dialog.Search.Check.Who;
				this.groupCaptions.PreferredWidth = 160;
				this.groupCaptions.PreferredHeight = 60;
				this.groupCaptions.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.groupCaptions.Margins = new Margins(6, 0, 6+58+16*0, 0);
				this.groupCaptions.Padding = new Margins(5, 5, 5, 5);
				this.groupCaptions.TabIndex = tabIndex++;
				this.groupCaptions.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

				label = new StaticText(this.groupCaptions);
				label.PreferredWidth = 80;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Text = Res.Strings.Viewers.Strings.Edit;
				label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				label.Margins = new Margins(0, 0, 0, 0);

				this.captionsCheckLabel = new CheckButton(this.groupCaptions);
				this.captionsCheckLabel.ActiveState = ActiveState.Yes;
				this.captionsCheckLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.captionsCheckLabel.PreferredWidth = this.captionsCheckLabel.PreferredHeight;
				this.captionsCheckLabel.Margins = new Margins(90+20*0, 0, 0, 0);
				this.captionsCheckLabel.TabIndex = tabIndex++;
				this.captionsCheckLabel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.captionsCheckLabel.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.captionsCheckLabel, Res.Strings.Dialog.Search.Check.Label);

				this.captionsCheckPrimaryText = new CheckButton(this.groupCaptions);
				//this.captionsCheckPrimaryText.ActiveState = ActiveState.Yes;
				this.captionsCheckPrimaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.captionsCheckPrimaryText.PreferredWidth = this.captionsCheckPrimaryText.PreferredHeight;
				this.captionsCheckPrimaryText.Margins = new Margins(90+20*1, 0, 0, 0);
				this.captionsCheckPrimaryText.TabIndex = tabIndex++;
				this.captionsCheckPrimaryText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.captionsCheckPrimaryText.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.captionsCheckPrimaryText, Res.Strings.Dialog.Search.Check.PrimaryText);

				this.captionsCheckSecondaryText = new CheckButton(this.groupCaptions);
				//this.captionsCheckSecondaryText.ActiveState = ActiveState.Yes;
				this.captionsCheckSecondaryText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.captionsCheckSecondaryText.PreferredWidth = this.captionsCheckSecondaryText.PreferredHeight;
				this.captionsCheckSecondaryText.Margins = new Margins(90+20*2, 0, 0, 0);
				this.captionsCheckSecondaryText.TabIndex = tabIndex++;
				this.captionsCheckSecondaryText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.captionsCheckSecondaryText.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
				ToolTip.Default.SetToolTip(this.captionsCheckSecondaryText, Res.Strings.Dialog.Search.Check.SecondaryText);


				this.checkCase = new CheckButton(this.window.Root);
				this.checkCase.PreferredWidth = 130;
				this.checkCase.Text = Res.Strings.Dialog.Search.Check.Case;
				this.checkCase.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.checkCase.Margins = new Margins(6+180, 0, 6+79+16*0, 0);
				this.checkCase.TabIndex = tabIndex++;
				this.checkCase.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.checkWord = new CheckButton(this.window.Root);
				this.checkWord.PreferredWidth = 130;
				this.checkWord.Text = Res.Strings.Dialog.Search.Check.Word;
				this.checkWord.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.checkWord.Margins = new Margins(6+180, 0, 6+79+16*1, 0);
				this.checkWord.TabIndex = tabIndex++;
				this.checkWord.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Boutons de fermeture.
				this.buttonSearchPrev = new Button(this.window.Root);
				this.buttonSearchPrev.Name = "SearchPrev";
				this.buttonSearchPrev.PreferredWidth = 85;
				this.buttonSearchPrev.Text = string.Concat(Res.Strings.Dialog.Search.Button.Search, " ", Misc.Image("SearchPrevButton"));
				this.buttonSearchPrev.Anchor = AnchorStyles.BottomLeft;
				this.buttonSearchPrev.Margins = new Margins(6+(85+5)*0, 0, 0, 6+30);
				this.buttonSearchPrev.Clicked += new MessageEventHandler(this.HandleButtonSearchClicked);
				this.buttonSearchPrev.TabIndex = tabIndex++;
				this.buttonSearchPrev.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonReplacePrev = new Button(this.window.Root);
				this.buttonReplacePrev.Name = "ReplacePrev";
				this.buttonReplacePrev.PreferredWidth = 85;
				this.buttonReplacePrev.Text = string.Concat(Res.Strings.Dialog.Search.Button.Replace, " ", Misc.Image("SearchPrevButton"));
				this.buttonReplacePrev.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplacePrev.Margins = new Margins(6+(85+5)*1, 0, 0, 6+30);
				this.buttonReplacePrev.Clicked += new MessageEventHandler(this.HandleButtonReplaceClicked);
				this.buttonReplacePrev.TabIndex = tabIndex++;
				this.buttonReplacePrev.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCount = new Button(this.window.Root);
				this.buttonCount.Name = "Count";
				this.buttonCount.PreferredWidth = 85;
				this.buttonCount.Text = Res.Strings.Dialog.Search.Button.Count;
				this.buttonCount.Anchor = AnchorStyles.BottomLeft;
				this.buttonCount.Margins = new Margins(6+(85+5)*2, 0, 0, 6+30);
				this.buttonCount.Clicked += new MessageEventHandler(this.HandleButtonCountClicked);
				this.buttonCount.TabIndex = tabIndex++;
				this.buttonCount.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;


				this.buttonSearchNext = new Button(this.window.Root);
				this.buttonSearchNext.Name = "SearchNext";
				this.buttonSearchNext.PreferredWidth = 85;
				this.buttonSearchNext.Text = string.Concat(Res.Strings.Dialog.Search.Button.Search, " ", Misc.Image("SearchNextButton"));
				this.buttonSearchNext.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonSearchNext.Anchor = AnchorStyles.BottomLeft;
				this.buttonSearchNext.Margins = new Margins(6+(85+5)*0, 0, 0, 6);
				this.buttonSearchNext.Clicked += new MessageEventHandler(this.HandleButtonSearchClicked);
				this.buttonSearchNext.TabIndex = tabIndex++;
				this.buttonSearchNext.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonReplaceNext = new Button(this.window.Root);
				this.buttonReplaceNext.Name = "ReplaceNext";
				this.buttonReplaceNext.PreferredWidth = 85;
				this.buttonReplaceNext.Text = string.Concat(Res.Strings.Dialog.Search.Button.Replace, " ", Misc.Image("SearchNextButton"));
				this.buttonReplaceNext.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplaceNext.Margins = new Margins(6+(85+5)*1, 0, 0, 6);
				this.buttonReplaceNext.Clicked += new MessageEventHandler(this.HandleButtonReplaceClicked);
				this.buttonReplaceNext.TabIndex = tabIndex++;
				this.buttonReplaceNext.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonReplaceAll = new Button(this.window.Root);
				this.buttonReplaceAll.Name = "ReplaceAll";
				this.buttonReplaceAll.PreferredWidth = 85;
				this.buttonReplaceAll.Text = Res.Strings.Dialog.Search.Button.ReplaceAll;
				this.buttonReplaceAll.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplaceAll.Margins = new Margins(6+(85+5)*2, 0, 0, 6);
				this.buttonReplaceAll.Clicked += new MessageEventHandler(this.HandleButtonReplaceAllClicked);
				this.buttonReplaceAll.TabIndex = tabIndex++;
				this.buttonReplaceAll.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 6, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.HandleFieldSearchTextChanged(null);
				this.UpdateBundleType();
			}

			this.window.Show();

			this.fieldSearch.Focus();
			this.fieldSearch.SelectAll();
		}


		public void Adapt(Module.BundleType type)
		{
			//	Adapte le dialogue en fonction du type du viewer actif.
			if (this.bundleType != type)
			{
				this.bundleType = type;
				this.UpdateBundleType();
				this.UpdateButtons();
			}
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
					if (this.checkCase.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.CaseSensitive;
					if (this.checkWord.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.WholeWord;

					if (this.bundleType == Module.BundleType.Strings)
					{
						if (this.stringsCheckLabel.ActiveState          == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInLabel;
						if (this.stringsCheckPrimaryText.ActiveState    == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryText;
						if (this.stringsCheckSecondaryText.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryText;
						if (this.stringsCheckPrimaryAbout.ActiveState   == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryAbout;
						if (this.stringsCheckSecondaryAbout.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryAbout;
					}
					else if (this.bundleType == Module.BundleType.Captions)
					{
						if (this.captionsCheckLabel.ActiveState         == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInLabel;
						if (this.captionsCheckPrimaryText.ActiveState   == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInPrimaryText;
						if (this.captionsCheckSecondaryText.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.SearchInSecondaryText;
					}
					else
					{
						mode |= Searcher.SearchingMode.SearchInLabel;
					}
				}

				return mode;
			}
		}


		public bool IsActionsEnabled
		{
			//	Indique si l'ensemble des actions de recherches est actif ou non.
			get
			{
				if (this.fieldSearch == null )  return false;
				if (this.fieldSearch.Text == "")  return false;

				if (this.bundleType == Module.BundleType.Strings)
				{
					return (this.stringsCheckLabel.ActiveState == ActiveState.Yes ||
							this.stringsCheckPrimaryText.ActiveState == ActiveState.Yes ||
							this.stringsCheckSecondaryText.ActiveState == ActiveState.Yes ||
							this.stringsCheckPrimaryAbout.ActiveState == ActiveState.Yes ||
							this.stringsCheckSecondaryAbout.ActiveState == ActiveState.Yes);
				}
				else if (this.bundleType == Module.BundleType.Captions)
				{
					return (this.captionsCheckLabel.ActiveState == ActiveState.Yes ||
							this.captionsCheckPrimaryText.ActiveState == ActiveState.Yes ||
							this.captionsCheckSecondaryText.ActiveState == ActiveState.Yes);
				}
				else
				{
					return false;
				}
			}
		}

		protected void UpdateButtons()
		{
			if (this.window == null)  return;

			Module module = this.mainWindow.CurrentModule;
			if (module == null)  return;

			bool enable = this.IsActionsEnabled;

			this.buttonSearchPrev.Enable = enable;
			this.buttonSearchNext.Enable = enable;
			this.buttonCount.Enable = enable;
			this.buttonReplaceAll.Enable = enable;
			this.buttonReplacePrev.Enable = enable;
			this.buttonReplaceNext.Enable = enable;

			this.mainWindow.GetCommandState("SearchPrev").Enable = enable;
			this.mainWindow.GetCommandState("SearchNext").Enable = enable;
		}

		protected void UpdateBundleType()
		{
			if (this.window != null)
			{
				this.groupStrings.Visibility = (this.bundleType == Module.BundleType.Strings);
				this.groupCaptions.Visibility = (this.bundleType == Module.BundleType.Captions);
			}
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

		void HandleFieldSearchTextChanged(object sender)
		{
			//	Le texte à chercher a changé.
			this.UpdateButtons();
		}

		void HandleCheckActiveStateChanged(object sender)
		{
			//	Un bouton 'où rechercher' a été cliqué.
			this.UpdateButtons();
		}

		private void HandleButtonSearchClicked(object sender, MessageEventArgs e)
		{
			//	Rechercher en avant ou en arrière.
			Button button = sender as Button;

			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = this.Mode;
			if ( button.Name == "SearchPrev" )
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			module.Modifier.ActiveViewer.DoSearch(this.fieldSearch.Text, mode);

			Misc.ComboMenuAdd(this.fieldSearch);
		}

		private void HandleButtonCountClicked(object sender, MessageEventArgs e)
		{
			//	Compter.
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoCount(this.fieldSearch.Text, this.Mode);

			Misc.ComboMenuAdd(this.fieldSearch);
		}

		private void HandleButtonReplaceClicked(object sender, MessageEventArgs e)
		{
			//	Remplacer en avant ou en arrière.
			Button button = sender as Button;

			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = this.Mode;
			if ( button.Name == "ReplacePrev" )
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			module.Modifier.ActiveViewer.DoReplace(this.fieldSearch.Text, this.fieldReplace.Text, mode);

			Misc.ComboMenuAdd(this.fieldSearch);
			Misc.ComboMenuAdd(this.fieldReplace);
		}

		private void HandleButtonReplaceAllClicked(object sender, MessageEventArgs e)
		{
			//	Rechercher tout.
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoReplaceAll(this.fieldSearch.Text, this.fieldReplace.Text, this.Mode);

			Misc.ComboMenuAdd(this.fieldSearch);
			Misc.ComboMenuAdd(this.fieldReplace);
		}


		protected Module.BundleType				bundleType = Module.BundleType.Unknow;

		protected TextFieldCombo				fieldSearch;
		protected TextFieldCombo				fieldReplace;
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;

		protected GroupBox						groupStrings;
		protected CheckButton					stringsCheckLabel;
		protected CheckButton					stringsCheckPrimaryText;
		protected CheckButton					stringsCheckSecondaryText;
		protected CheckButton					stringsCheckPrimaryAbout;
		protected CheckButton					stringsCheckSecondaryAbout;

		protected GroupBox						groupCaptions;
		protected CheckButton					captionsCheckLabel;
		protected CheckButton					captionsCheckPrimaryText;
		protected CheckButton					captionsCheckSecondaryText;

		protected Button						buttonSearchPrev;
		protected Button						buttonSearchNext;
		protected Button						buttonCount;
		protected Button						buttonReplaceAll;
		protected Button						buttonReplacePrev;
		protected Button						buttonReplaceNext;
	}
}
