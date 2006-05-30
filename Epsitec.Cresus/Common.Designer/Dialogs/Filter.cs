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
		public Filter(MainWindow mainWindow) : base(mainWindow)
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
				this.WindowInit("Filter", 270, 130, true);
				this.window.Text = Res.Strings.Dialog.Filter.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Filter.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.PreferredWidth = 40;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldFilter = new TextFieldCombo(this.window.Root);
				this.fieldFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldFilter.Margins = new Margins(6+40, 6, 6, 0);
				this.fieldFilter.TabIndex = tabIndex++;
				this.fieldFilter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.radioBegin = new RadioButton(this.window.Root);
				this.radioBegin.PreferredWidth = 120;
				this.radioBegin.Group = "Part";
				this.radioBegin.Text = Res.Strings.Dialog.Filter.Radio.Begin;
				this.radioBegin.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.radioBegin.Margins = new Margins(6, 6, 6+32+16*0, 0);
				this.radioBegin.TabIndex = tabIndex++;
				this.radioBegin.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.radioAny = new RadioButton(this.window.Root);
				this.radioAny.PreferredWidth = 120;
				this.radioAny.ActiveState = ActiveState.Yes;
				this.radioAny.Group = "Part";
				this.radioAny.Text = Res.Strings.Dialog.Filter.Radio.Any;
				this.radioAny.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.radioAny.Margins = new Margins(6, 6, 6+32+16*1, 0);
				this.radioAny.TabIndex = tabIndex++;
				this.radioAny.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.radioJocker = new RadioButton(this.window.Root);
				this.radioJocker.PreferredWidth = 120;
				this.radioJocker.Group = "Part";
				this.radioJocker.Text = Res.Strings.Dialog.Filter.Radio.Jocker;
				this.radioJocker.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.radioJocker.Margins = new Margins(6, 6, 6+32+16*2, 0);
				this.radioJocker.TabIndex = tabIndex++;
				this.radioJocker.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.radioJocker.ActiveStateChanged += new EventHandler(this.HandleRadioJockerActiveStateChanged);

				this.checkCase = new CheckButton(this.window.Root);
				this.checkCase.PreferredWidth = 120;
				this.checkCase.Text = Res.Strings.Dialog.Filter.Check.Case;
				this.checkCase.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.checkCase.Margins = new Margins(6+120, 6, 6+32+16*0, 0);
				this.checkCase.TabIndex = tabIndex++;
				this.checkCase.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.checkWord = new CheckButton(this.window.Root);
				this.checkWord.PreferredWidth = 120;
				this.checkWord.Text = Res.Strings.Dialog.Filter.Check.Word;
				this.checkWord.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				this.checkWord.Margins = new Margins(6+120, 6, 6+32+16*1, 0);
				this.checkWord.TabIndex = tabIndex++;
				this.checkWord.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Filter.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonFilterClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonAll = new Button(this.window.Root);
				buttonAll.PreferredWidth = 75;
				buttonAll.Text = Res.Strings.Dialog.Filter.Button.All;
				buttonAll.Anchor = AnchorStyles.BottomLeft;
				buttonAll.Margins = new Margins(6+75+5, 0, 0, 6);
				buttonAll.Clicked += new MessageEventHandler(this.HandleButtonAllClicked);
				buttonAll.TabIndex = tabIndex++;
				buttonAll.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 6, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
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

		void HandleRadioJockerActiveStateChanged(object sender)
		{
			this.checkWord.Enable = (this.radioJocker.ActiveState == ActiveState.No);
		}

		private void HandleButtonFilterClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = Searcher.SearchingMode.None;
			if (this.radioBegin.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.AtBeginning;
			if (this.radioJocker.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.Jocker;
			if (this.checkCase.ActiveState   == ActiveState.Yes)  mode |= Searcher.SearchingMode.CaseSensitive;
			if (this.checkWord.ActiveState   == ActiveState.Yes)  mode |= Searcher.SearchingMode.WholeWord;
			module.Modifier.ActiveViewer.DoFilter(this.fieldFilter.Text, mode);

			Misc.ComboMenuAdd(this.fieldFilter);
		}

		private void HandleButtonAllClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			module.Modifier.ActiveViewer.DoFilter("", Searcher.SearchingMode.None);
		}


		protected TextFieldCombo				fieldFilter;
		protected RadioButton					radioBegin;
		protected RadioButton					radioAny;
		protected RadioButton					radioJocker;
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;
	}
}
