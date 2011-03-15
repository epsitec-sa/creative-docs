using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le texte de recherche.
	/// </summary>
	public class SearchDialog : AbstractDialog
	{
		public SearchDialog(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.designerApplication = designerApplication;
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow();
				this.window.MakeToolWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Search", 375, 216, true);
				this.window.Text = Res.Strings.Dialog.Search.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				int tabIndex = 1;

				//	Partie supérieure.
				Widget header1 = new Widget(this.window.Root);
				header1.PreferredHeight = 20;
				header1.Margins = new Margins(0, 0, 0, 6);
				header1.Dock = DockStyle.Top;

				StaticText label = new StaticText(header1);
				label.Text = Res.Strings.Dialog.Search.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.PreferredWidth = 80;
				label.Dock = DockStyle.Left;

				this.fieldSearch = new TextFieldCombo(header1);
				this.fieldSearch.Dock = DockStyle.Fill;
				this.fieldSearch.TabIndex = tabIndex++;
				this.fieldSearch.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.fieldSearch.TextChanged += this.HandleFieldSearchTextChanged;

				Widget header2 = new Widget(this.window.Root);
				header2.PreferredHeight = 20;
				header2.Margins = new Margins(0, 0, 0, 10);
				header2.Dock = DockStyle.Top;

				label = new StaticText(header2);
				label.Text = Res.Strings.Dialog.Search.Replace;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.PreferredWidth = 80;
				label.Dock = DockStyle.Left;

				this.fieldReplace = new TextFieldCombo(header2);
				this.fieldReplace.Dock = DockStyle.Fill;
				this.fieldReplace.TabIndex = tabIndex++;
				this.fieldReplace.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Partie principale.
				Widget main = new Widget(this.window.Root);
				main.Dock = DockStyle.Fill;

				Widget left = new Widget(main);
				left.PreferredWidth = 120;
				left.Dock = DockStyle.Left;

				//	Crée le groupe pour le type 'Strings2'.
				this.groupStrings = new GroupBox(left);
				this.groupStrings.Text = Res.Strings.Dialog.Search.Check.Who;
				this.groupStrings.PreferredWidth = 160;
				this.groupStrings.PreferredHeight = 28+16*2;
				this.groupStrings.Dock = DockStyle.Top;
				this.groupStrings.Padding = new Margins(5, 5, 5, 5);
				this.groupStrings.TabIndex = tabIndex++;
				this.groupStrings.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

				Viewers.Abstract.SearchCreateFilterGroup(this.groupStrings, this.HandleCheckActiveStateChanged, ResourceAccess.Type.Strings);

				//	Crée le groupe pour le type 'Captions2'.
				this.groupCaptions = new GroupBox(left);
				this.groupCaptions.Text = Res.Strings.Dialog.Search.Check.Who;
				this.groupCaptions.PreferredWidth = 160;
				this.groupCaptions.PreferredHeight = 28+16*2;
				this.groupCaptions.Dock = DockStyle.Top;
				this.groupCaptions.Padding = new Margins(5, 5, 5, 5);
				this.groupCaptions.TabIndex = tabIndex++;
				this.groupCaptions.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

				Viewers.Abstract.SearchCreateFilterGroup(this.groupCaptions, this.HandleCheckActiveStateChanged, ResourceAccess.Type.Captions);

				//	Boutons à cocher de droite.
				Widget right = new Widget(main);
				right.PreferredWidth = 120;
				right.Margins = new Margins(0, 60, 21, 0);
				right.Dock = DockStyle.Right;

				this.checkCase = new CheckButton(right);
				this.checkCase.PreferredWidth = 130;
				this.checkCase.Text = Res.Strings.Dialog.Search.Check.Case;
				this.checkCase.Dock = DockStyle.Top;
				this.checkCase.Margins = new Margins(0, 0, 0, 3);
				this.checkCase.TabIndex = tabIndex++;
				this.checkCase.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.checkWord = new CheckButton(right);
				this.checkWord.PreferredWidth = 130;
				this.checkWord.Text = Res.Strings.Dialog.Search.Check.Word;
				this.checkWord.Dock = DockStyle.Top;
				this.checkWord.Margins = new Margins(0, 0, 0, 3);
				this.checkWord.TabIndex = tabIndex++;
				this.checkWord.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Boutons de fermeture.
				Widget footer2 = new Widget(this.window.Root);
				footer2.PreferredHeight = 22;
				footer2.Margins = new Margins(0, 0, 8, 0);
				footer2.Dock = DockStyle.Bottom;

				this.buttonSearchNext = new Button(footer2);
				this.buttonSearchNext.Name = "SearchNext";
				this.buttonSearchNext.PreferredWidth = 85;
				this.buttonSearchNext.Text = string.Concat(Res.Strings.Dialog.Search.Button.Search, " ", Misc.Image("SearchNextButton"));
				this.buttonSearchNext.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonSearchNext.Dock = DockStyle.Left;
				this.buttonSearchNext.Margins = new Margins(0, 6, 0, 0);
				this.buttonSearchNext.Clicked += this.HandleButtonSearchClicked;
				this.buttonSearchNext.TabIndex = tabIndex++;
				this.buttonSearchNext.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonReplaceNext = new Button(footer2);
				this.buttonReplaceNext.Name = "ReplaceNext";
				this.buttonReplaceNext.PreferredWidth = 85;
				this.buttonReplaceNext.Text = string.Concat(Res.Strings.Dialog.Search.Button.Replace, " ", Misc.Image("SearchNextButton"));
				this.buttonReplaceNext.Dock = DockStyle.Left;
				this.buttonReplaceNext.Margins = new Margins(0, 6, 0, 0);
				this.buttonReplaceNext.Clicked += this.HandleButtonReplaceClicked;
				this.buttonReplaceNext.TabIndex = tabIndex++;
				this.buttonReplaceNext.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonReplaceAll = new Button(footer2);
				this.buttonReplaceAll.Name = "ReplaceAll";
				this.buttonReplaceAll.PreferredWidth = 85;
				this.buttonReplaceAll.Text = Res.Strings.Dialog.Search.Button.ReplaceAll;
				this.buttonReplaceAll.Dock = DockStyle.Left;
				this.buttonReplaceAll.Margins = new Margins(0, 6, 0, 0);
				this.buttonReplaceAll.Clicked += this.HandleButtonReplaceAllClicked;
				this.buttonReplaceAll.TabIndex = tabIndex++;
				this.buttonReplaceAll.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(footer2);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Dock = DockStyle.Right;
				buttonClose.Clicked += this.HandleButtonCloseClicked;
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Widget footer1 = new Widget(this.window.Root);
				footer1.PreferredHeight = 22;
				footer1.Margins = new Margins(0, 0, 8, 0);
				footer1.Dock = DockStyle.Bottom;

				this.buttonSearchPrev = new Button(footer1);
				this.buttonSearchPrev.Name = "SearchPrev";
				this.buttonSearchPrev.PreferredWidth = 85;
				this.buttonSearchPrev.Text = string.Concat(Res.Strings.Dialog.Search.Button.Search, " ", Misc.Image("SearchPrevButton"));
				this.buttonSearchPrev.Dock = DockStyle.Left;
				this.buttonSearchPrev.Margins = new Margins(0, 6, 0, 0);
				this.buttonSearchPrev.Clicked += this.HandleButtonSearchClicked;
				this.buttonSearchPrev.TabIndex = tabIndex++;
				this.buttonSearchPrev.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonReplacePrev = new Button(footer1);
				this.buttonReplacePrev.Name = "ReplacePrev";
				this.buttonReplacePrev.PreferredWidth = 85;
				this.buttonReplacePrev.Text = string.Concat(Res.Strings.Dialog.Search.Button.Replace, " ", Misc.Image("SearchPrevButton"));
				this.buttonReplacePrev.Dock = DockStyle.Left;
				this.buttonReplacePrev.Margins = new Margins(0, 6, 0, 0);
				this.buttonReplacePrev.Clicked += this.HandleButtonReplaceClicked;
				this.buttonReplacePrev.TabIndex = tabIndex++;
				this.buttonReplacePrev.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCount = new Button(footer1);
				this.buttonCount.Name = "Count";
				this.buttonCount.PreferredWidth = 85;
				this.buttonCount.Text = Res.Strings.Dialog.Search.Button.Count;
				this.buttonCount.Dock = DockStyle.Left;
				this.buttonCount.Margins = new Margins(0, 6, 0, 0);
				this.buttonCount.Clicked += this.HandleButtonCountClicked;
				this.buttonCount.TabIndex = tabIndex++;
				this.buttonCount.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.HandleFieldSearchTextChanged(null);
				this.UpdateBundleType();
			}

			this.window.Show();

			this.fieldSearch.Focus();
			this.fieldSearch.SelectAll();
		}


		public void Adapt(ResourceAccess.Type type)
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
				}

				return mode;
			}
		}

		public List<int> FilterList
		{
			//	Retourne la liste des index autorisés par le filtre.
			get
			{
				if (this.bundleType == ResourceAccess.Type.Strings)
				{
					return Viewers.Abstract.SearchGetFilterGroup(this.groupStrings, ResourceAccess.Type.Strings);
				}
				else if (this.bundleType == ResourceAccess.Type.Captions || this.bundleType == ResourceAccess.Type.Commands)
				{
					return Viewers.Abstract.SearchGetFilterGroup(this.groupCaptions, ResourceAccess.Type.Captions);
				}
				else
				{
					List<int> filter = new List<int>();
					filter.Add(0);  // autorisé la recherche dans 'Name'
					return filter;
				}
			}
		}


		public bool IsActionsEnabled
		{
			//	Indique si l'ensemble des actions de recherches est actif ou non.
			get
			{
				if (this.fieldSearch == null )  return false;
				if (this.fieldSearch.Text == "")  return false;

				List<int> filter = this.FilterList;
				return (filter != null && filter.Count > 0);
			}
		}

		protected void UpdateButtons()
		{
			if (this.window == null)  return;

			Module module = this.designerApplication.CurrentModule;
			if (module == null)  return;

			bool enable = this.IsActionsEnabled;

			this.buttonSearchPrev.Enable = enable;
			this.buttonSearchNext.Enable = enable;
			this.buttonCount.Enable = enable;
			this.buttonReplaceAll.Enable = enable;
			this.buttonReplacePrev.Enable = enable;
			this.buttonReplaceNext.Enable = enable;

			this.designerApplication.GetCommandState("SearchPrev").Enable = enable;
			this.designerApplication.GetCommandState("SearchNext").Enable = enable;
		}

		protected void UpdateBundleType()
		{
			if (this.window != null)
			{
				this.groupStrings.Visibility = (this.bundleType == ResourceAccess.Type.Strings);
				this.groupCaptions.Visibility = (this.bundleType == ResourceAccess.Type.Captions || this.bundleType == ResourceAccess.Type.Commands);
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

			Module module = this.designerApplication.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = this.Mode;
			if ( button.Name == "SearchPrev" )
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			module.Modifier.ActiveViewer.DoSearch(this.fieldSearch.Text, mode, this.FilterList);

			Misc.ComboMenuAdd(this.fieldSearch);
		}

		private void HandleButtonCountClicked(object sender, MessageEventArgs e)
		{
			//	Compter.
			Module module = this.designerApplication.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoCount(this.fieldSearch.Text, this.Mode, this.FilterList);

			Misc.ComboMenuAdd(this.fieldSearch);
		}

		private void HandleButtonReplaceClicked(object sender, MessageEventArgs e)
		{
			//	Remplacer en avant ou en arrière.
			Button button = sender as Button;

			Module module = this.designerApplication.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = this.Mode;
			if ( button.Name == "ReplacePrev" )
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			module.Modifier.ActiveViewer.DoReplace(this.fieldSearch.Text, this.fieldReplace.Text, mode, this.FilterList);

			Misc.ComboMenuAdd(this.fieldSearch);
			Misc.ComboMenuAdd(this.fieldReplace);
		}

		private void HandleButtonReplaceAllClicked(object sender, MessageEventArgs e)
		{
			//	Rechercher tout.
			Module module = this.designerApplication.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoReplaceAll(this.fieldSearch.Text, this.fieldReplace.Text, this.Mode, this.FilterList);

			Misc.ComboMenuAdd(this.fieldSearch);
			Misc.ComboMenuAdd(this.fieldReplace);
		}


		protected ResourceAccess.Type			bundleType = ResourceAccess.Type.Unknown;

		protected TextFieldCombo				fieldSearch;
		protected TextFieldCombo				fieldReplace;
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;
		protected GroupBox						groupStrings;
		protected GroupBox						groupCaptions;
		protected Button						buttonSearchPrev;
		protected Button						buttonSearchNext;
		protected Button						buttonCount;
		protected Button						buttonReplaceAll;
		protected Button						buttonReplacePrev;
		protected Button						buttonReplaceNext;
	}
}
