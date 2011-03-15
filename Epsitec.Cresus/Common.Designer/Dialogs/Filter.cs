using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le filtre pour les ressources.
	/// </summary>
	public class Filter : Abstract
	{
		public Filter(DesignerApplication designerApplication) : base(designerApplication)
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
				this.window.MakeFixedSizeWindow();
				this.window.MakeToolWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Filter", 270, 130, true);
				this.window.Text = Res.Strings.Dialog.Filter.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				int tabIndex = 1;

				//	Partie supérieure.
				Widget header = new Widget(this.window.Root);
				header.PreferredHeight = 20;
				header.Margins = new Margins(0, 0, 0, 8);
				header.Dock = DockStyle.Top;

				StaticText label = new StaticText(header);
				label.Text = Res.Strings.Dialog.Filter.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.PreferredWidth = 45;
				label.Dock = DockStyle.Left;

				this.fieldFilter = new TextFieldCombo(header);
				this.fieldFilter.Dock = DockStyle.Fill;
				this.fieldFilter.TabIndex = tabIndex++;
				this.fieldFilter.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Boutons radio de gauche.
				Widget main = new Widget(this.window.Root);
				main.Dock = DockStyle.Fill;

				Widget left = new Widget(main);
				left.PreferredWidth = 120;
				left.Margins = new Margins(0, 8, 0, 0);
				left.Dock = DockStyle.Left;

				this.radioBegin = new RadioButton(left);
				this.radioBegin.Group = "Part";
				this.radioBegin.Text = Res.Strings.Dialog.Filter.Radio.Begin;
				this.radioBegin.Dock = DockStyle.Top;
				this.radioBegin.Margins = new Margins(0, 0, 0, 3);
				this.radioBegin.TabIndex = tabIndex++;
				this.radioBegin.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.radioAny = new RadioButton(left);
				this.radioAny.ActiveState = ActiveState.Yes;
				this.radioAny.Group = "Part";
				this.radioAny.Text = Res.Strings.Dialog.Filter.Radio.Any;
				this.radioAny.Dock = DockStyle.Top;
				this.radioAny.Margins = new Margins(0, 0, 0, 3);
				this.radioAny.TabIndex = tabIndex++;
				this.radioAny.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.radioJoker = new RadioButton(left);
				this.radioJoker.Group = "Part";
				this.radioJoker.Text = Res.Strings.Dialog.Filter.Radio.Joker;
				this.radioJoker.Dock = DockStyle.Top;
				this.radioJoker.Margins = new Margins(0, 0, 0, 3);
				this.radioJoker.TabIndex = tabIndex++;
				this.radioJoker.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.radioJoker.ActiveStateChanged += this.HandleRadioJokerActiveStateChanged;

				//	Boutons à cocher de droite.
				Widget right = new Widget(main);
				right.PreferredWidth = 120;
				right.Margins = new Margins(0, 8, 0, 0);
				right.Dock = DockStyle.Left;

				this.checkCase = new CheckButton(right);
				this.checkCase.PreferredWidth = 120;
				this.checkCase.Text = Res.Strings.Dialog.Filter.Check.Case;
				this.checkCase.Dock = DockStyle.Top;
				this.checkCase.Margins = new Margins(0, 0, 0, 3);
				this.checkCase.TabIndex = tabIndex++;
				this.checkCase.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.checkWord = new CheckButton(right);
				this.checkWord.PreferredWidth = 120;
				this.checkWord.Text = Res.Strings.Dialog.Filter.Check.Word;
				this.checkWord.Dock = DockStyle.Top;
				this.checkWord.Margins = new Margins(0, 0, 0, 3);
				this.checkWord.TabIndex = tabIndex++;
				this.checkWord.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Pied.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Filter.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Left;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += this.HandleButtonFilterClicked;
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Button buttonAll = new Button(footer);
				buttonAll.PreferredWidth = 75;
				buttonAll.Text = Res.Strings.Dialog.Filter.Button.All;
				buttonAll.Dock = DockStyle.Left;
				buttonAll.Margins = new Margins(0, 6, 0, 0);
				buttonAll.Clicked += this.HandleButtonAllClicked;
				buttonAll.TabIndex = tabIndex++;
				buttonAll.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Dock = DockStyle.Right;
				buttonClose.Clicked += this.HandleButtonCloseClicked;
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.window.Show();

			this.fieldFilter.Focus();
			this.fieldFilter.SelectAll();
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

		void HandleRadioJokerActiveStateChanged(object sender)
		{
			this.checkWord.Enable = (this.radioJoker.ActiveState == ActiveState.No);
		}

		private void HandleButtonFilterClicked(object sender, MessageEventArgs e)
		{
			Module module = this.designerApplication.CurrentModule;
			if (module == null)
			{
				return;
			}

			Searcher.SearchingMode mode = Searcher.SearchingMode.None;
			if (this.radioBegin.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.AtBeginning;
			if (this.radioJoker.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.Joker;
			if (this.checkCase.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.CaseSensitive;
			if (this.checkWord.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.WholeWord;
			module.Modifier.ActiveViewer.DoFilter(this.fieldFilter.Text, mode);

			Misc.ComboMenuAdd(this.fieldFilter);
		}

		private void HandleButtonAllClicked(object sender, MessageEventArgs e)
		{
			Module module = this.designerApplication.CurrentModule;
			if (module == null)
			{
				return;
			}

			module.Modifier.ActiveViewer.DoFilter("", Searcher.SearchingMode.None);
		}


		protected TextFieldCombo				fieldFilter;
		protected RadioButton					radioBegin;
		protected RadioButton					radioAny;
		protected RadioButton					radioJoker;
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;
	}
}
