using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir un caract�re quelconque � ins�rer dans
	/// un texte en �dition.
	/// </summary>
	public class Glyphs : Abstract
	{
		public Glyphs(MainWindow mainWindow) : base(mainWindow)
		{
			Font font = Font.DefaultFont;
			this.fontFace  = font.FaceName;   // "Tahoma"
			this.fontStyle = font.StyleName;  // "Regular"
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Glyphs", 300, 260, true);
				this.window.Text = Res.Strings.Dialog.Glyphs.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				this.array = new MyWidgets.GlyphArray(this.window.Root);
				this.array.Dock = DockStyle.Fill;
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.array.SetFont(this.fontFace, this.fontStyle);
				this.array.SelectedIndex = -1;
				this.array.DoubleClicked += new MessageEventHandler(this.HandleDoubleClicked);
				this.array.ChangeSelected += new EventHandler(this.HandleArraySelected);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Glyphs.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Left;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonInsertClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Dock = DockStyle.Left;
				buttonClose.Margins = new Margins(0, 6, 0, 0);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Barre de statut.
				Widget band = new Widget(this.window.Root);
				band.PreferredHeight = 20;
				band.Margins = new Margins(0, 0, 8, 0);
				band.Dock = DockStyle.Bottom;

				this.status = new TextField(band);
				this.status.Dock = DockStyle.Fill;
				this.status.IsReadOnly = true;

				this.slider = new HSlider(band);
				this.slider.PreferredWidth = 80;
				this.slider.PreferredHeight = 14;
				this.slider.Dock = DockStyle.Right;
				this.slider.Margins = new Margins(6, 0, 3, 3);
				this.slider.TabIndex = tabIndex++;
				this.slider.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 1.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) this.array.CellSize;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Glyphs.Tooltip.ArraySize);
			}

			this.window.Show();
		}


		protected void EditInsert()
		{
			//	Ins�re le glyphe s�lectionn� dans le tableau dans la ligne �ditable
			//	qui avait le focus.
			if ( this.array.SelectedIndex == -1 )  return;

			int code = this.array.IndexToUnicode(array.SelectedIndex);
			char c = (char) code;
			string insert = c.ToString();

			Module module = this.mainWindow.CurrentModule;
			if ( module == null )  return;

			AbstractTextField edit = module.Modifier.ActiveViewer.CurrentTextField;
			if ( edit == null )  return;
			edit.TextNavigator.Selection = insert;
		}


		private void HandleArraySelected(object sender)
		{
			//	Le glyphe dans le tableau est s�lectionn�.
			string text = "";
			if ( this.array.SelectedIndex != -1 )
			{
				int code = this.array.IndexToUnicode(this.array.SelectedIndex);
				text = string.Format("{0}: {1}", code.ToString("X4"), Misc.GetUnicodeName(code));
			}
			this.status.Text = text;
		}

		private void HandleDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Le glyphe est double-cliqu�.
			this.EditInsert();
		}

		private void HandleSliderChanged(object sender)
		{
			HSlider slider = sender as HSlider;
			if ( slider == null )  return;
			this.array.CellSize = (double) slider.Value;
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

		private void HandleButtonInsertClicked(object sender, MessageEventArgs e)
		{
			this.EditInsert();
		}


		protected string					fontFace;
		protected string					fontStyle;

		protected MyWidgets.GlyphArray		array;
		protected TextField					status;
		protected HSlider					slider;
	}
}
