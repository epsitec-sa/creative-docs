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
				this.WindowInit("Filter", 270, 150, true);
				this.window.Text = Res.Strings.Dialog.Filter.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Filter.Label;
				label.Alignment = ContentAlignment.MiddleLeft;
				label.Width = 40;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldFilter = new TextFieldCombo(this.window.Root);
				this.fieldFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.fieldFilter.Margins = new Margins(6+40, 6, 6, 0);
				this.fieldFilter.TabIndex = tabIndex++;

				this.radioBegin = new RadioButton(this.window.Root);
				this.radioBegin.Group = "Part";
				this.radioBegin.Text = Res.Strings.Dialog.Filter.Radio.Begin;
				this.radioBegin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.radioBegin.Margins = new Margins(6+40, 6, 6+32+16*0, 0);
				this.radioBegin.TabIndex = tabIndex++;

				this.radioAny = new RadioButton(this.window.Root);
				this.radioAny.ActiveState = ActiveState.Yes;
				this.radioAny.Group = "Part";
				this.radioAny.Text = Res.Strings.Dialog.Filter.Radio.Any;
				this.radioAny.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.radioAny.Margins = new Margins(6+40, 6, 6+32+16*1, 0);
				this.radioAny.TabIndex = tabIndex++;

				this.checkCase = new CheckButton(this.window.Root);
				this.checkCase.Text = Res.Strings.Dialog.Filter.Check.Case;
				this.checkCase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkCase.Margins = new Margins(6+40, 6, 6+32+16*2+4, 0);
				this.checkCase.TabIndex = tabIndex++;

				this.checkWord = new CheckButton(this.window.Root);
				this.checkWord.Text = Res.Strings.Dialog.Filter.Check.Word;
				this.checkWord.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				this.checkWord.Margins = new Margins(6+40, 6, 6+32+16*3+4, 0);
				this.checkWord.TabIndex = tabIndex++;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.Filter.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonFilterClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonAll = new Button(this.window.Root);
				buttonAll.Width = 75;
				buttonAll.Text = Res.Strings.Dialog.Filter.Button.All;
				buttonAll.Anchor = AnchorStyles.BottomLeft;
				buttonAll.Margins = new Margins(6+75+5, 0, 0, 6);
				buttonAll.Clicked += new MessageEventHandler(this.HandleButtonAllClicked);
				buttonAll.TabIndex = tabIndex++;
				buttonAll.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
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
			//	TODO: si on ferme la fenêtre ainsi, on ne peut plus la rouvrir !
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

		private void HandleButtonFilterClicked(object sender, MessageEventArgs e)
		{
			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			Searcher.SearchingMode mode = Searcher.SearchingMode.None;
			if (this.radioBegin.ActiveState == ActiveState.Yes)  mode |= Searcher.SearchingMode.AtBeginning;
			if (this.checkCase.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.CaseSensitive;
			if (this.checkWord.ActiveState  == ActiveState.Yes)  mode |= Searcher.SearchingMode.WholeWord;
			module.Modifier.ActiveViewer.DoFilter(this.fieldFilter.Text, mode);

			Misc.ComboMenuAdd(this.fieldFilter, this.fieldFilter.Text);
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
		protected CheckButton					checkCase;
		protected CheckButton					checkWord;
	}
}
