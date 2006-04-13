using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir un caractère quelconque à insérer dans
	/// un texte en édition.
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
			//	Crée et montre la fenêtre du dialogue.
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

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, 0, 0, 0);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				this.array = new GlyphArray(this.window.Root);
				this.array.Dock = DockStyle.Fill;
				this.array.Margins = new Margins (6, 6, 6, 6+20+4+30);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.array.SetFont(this.fontFace, this.fontStyle);
				this.array.SelectedIndex = -1;
				this.array.DoubleClicked += new MessageEventHandler(this.HandleDoubleClicked);
				this.array.ChangeSelected += new EventHandler(this.HandleArraySelected);

				this.status = new TextField(this.window.Root);
				this.status.Anchor = AnchorStyles.Bottom|AnchorStyles.LeftAndRight;
				this.status.Margins = new Margins(6, 4+80+6, 0, 6+30);
				this.status.IsReadOnly = true;

				this.slider = new HSlider(this.window.Root);
				this.slider.Width = 80;
				this.slider.Height = 14;
				this.slider.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.slider.Margins = new Margins(6, 6, 0, 9+30);
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

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.Glyphs.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonInsertClicked);
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
		}


		protected void EditInsert()
		{
			//	Insère le glyphe sélectionné dans le tableau dans la ligne éditable
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
			//	Le glyphe dans le tableau est sélectionné.
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
			//	Le glyphe est double-cliqué.
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

		protected GlyphArray				array;
		protected TextField					status;
		protected HSlider					slider;
	}
}
