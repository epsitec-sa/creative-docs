using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Fonts contient tous les panneaux des repères.
	/// </summary>
	public class Fonts : Abstract
	{
		public Fonts(Document document) : base(document)
		{
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.Margins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 2;
			this.toolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			int tabIndex = 0;

			this.buttonDefault = new IconButton(Misc.Icon("QuickDefault"));
			this.buttonDefault.Clicked += this.HandleButtonDefault;
			this.buttonDefault.TabIndex = tabIndex++;
			this.buttonDefault.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonDefault);
			ToolTip.Default.SetToolTip(this.buttonDefault, Res.Strings.Dialog.Settings.Fonts.Default);

			toolBar.Items.Add(new IconSeparator());

			this.buttonClear = new IconButton(Misc.Icon("QuickClear"));
			this.buttonClear.Clicked += this.HandleButtonClear;
			this.buttonClear.TabIndex = tabIndex++;
			this.buttonClear.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonClear);
			ToolTip.Default.SetToolTip(this.buttonClear, Res.Strings.Dialog.Settings.Fonts.Clear);

			toolBar.Items.Add(new IconSeparator());

			this.slider = new HSlider();
			this.slider.PreferredWidth = 100;
			this.slider.Margins = new Margins(0, 0, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 60.0M;
			this.slider.SmallChange = 1.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += this.HandleSliderChanged;
			this.toolBar.Items.Add(this.slider);
			ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Double.TextFontSampleHeight);

			this.buttonAbc = new IconButton(Misc.Icon("TextFontSampleAbc"));
			this.buttonAbc.CommandObject = Command.Get("TextFontSampleAbc");
			this.buttonAbc.Margins = new Margins(10, 0, 0, 0);
			this.buttonAbc.TabIndex = tabIndex++;
			this.buttonAbc.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAbc);
			ToolTip.Default.SetToolTip(this.buttonAbc, Res.Strings.Dialog.Bool.TextFontSampleAbc);

			this.fontSelector = new Widgets.FontSelector(this);
			this.fontSelector.Dock = DockStyle.Fill;
			this.fontSelector.FontList = Misc.GetFontList(false);
			this.fontSelector.SelectionChanged += this.HandleFontSelectorSelectionChanged;
		
			this.UpdateList();
			this.UpdateFontsButtons();
		}

		public void UpdateListAdded()
		{
			//	Met à jour la liste des polices lorsque de nouvelles polices sont apparues.
			//	Ceci peut arriver après l'ouverture d'un document qui contenait des polices
			//	non installées.
			this.fontSelector.FontList = Misc.GetFontList(false);
		}

		public void UpdateList()
		{
			//	Met à jour la liste des polices rapides.
			this.ignoreChange = true;
			this.fontSelector.SelectedList = this.document.Settings.QuickFonts;
			this.fontSelector.SampleHeight = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight;
			this.fontSelector.SampleAbc    = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleAbc;
			this.slider.Value = (decimal) this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight;
			this.ignoreChange = false;
		}
		

		protected void UpdateFontsButtons()
		{
			//	Met à jour les boutons des polices rapides.
			bool def = false;
			bool clr = false;

			if ( this.fontSelector.SelectedList.Count == 0 )
			{
				def = true;
				clr = false;
			}
			else
			{
				clr = true;
				System.Collections.ArrayList defList = new System.Collections.ArrayList();
				Settings.Settings.DefaultQuickFonts(defList);
				foreach ( string font in this.fontSelector.SelectedList )
				{
					if ( !defList.Contains(font) )  def = true;
				}
			}
			
			this.buttonDefault.Enable = def;
			this.buttonClear.Enable = clr;
		}

		private void HandleButtonDefault(object sender, MessageEventArgs e)
		{
			Settings.Settings.DefaultQuickFonts(this.fontSelector.SelectedList);
			this.UpdateFontsButtons();
			this.fontSelector.UpdateList();
			this.document.Notifier.NotifyFontsSettingsChanged();
		}

		private void HandleButtonClear(object sender, MessageEventArgs e)
		{
			this.fontSelector.SelectedList.Clear();
			this.UpdateFontsButtons();
			this.fontSelector.UpdateList();
			this.document.Notifier.NotifyFontsSettingsChanged();
		}

		private void HandleSliderChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			HSlider slider = sender as HSlider;
			if ( slider == null )  return;
			this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight = (double) slider.Value;
			this.UpdateList();
		}

		private void HandleFontSelectorSelectionChanged(object sender)
		{
			this.UpdateFontsButtons();
			this.document.Notifier.NotifyFontsSettingsChanged();
		}

		
		protected HToolBar					toolBar;
		protected IconButton				buttonDefault;
		protected IconButton				buttonClear;
		protected HSlider					slider;
		protected IconButton				buttonAbc;
		protected Widgets.FontSelector		fontSelector;
		protected bool						ignoreChange = false;
	}
}
