using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Fonts contient tous les panneaux des repères.
	/// </summary>
	[SuppressBundleSupport]
	public class Fonts : Abstract
	{
		public Fonts(Document document) : base(document)
		{
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 2;
			this.toolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			int tabIndex = 0;

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDefault = new IconButton(Misc.Icon("QuickDefault"));
			this.buttonDefault.Clicked += new MessageEventHandler(this.HandleButtonDefault);
			this.buttonDefault.TabIndex = tabIndex++;
			this.buttonDefault.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonDefault);
			ToolTip.Default.SetToolTip(this.buttonDefault, Res.Strings.Dialog.Settings.Fonts.Default);

			toolBar.Items.Add(new IconSeparator());

			this.buttonClear = new IconButton(Misc.Icon("QuickClear"));
			this.buttonClear.Clicked += new MessageEventHandler(this.HandleButtonClear);
			this.buttonClear.TabIndex = tabIndex++;
			this.buttonClear.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonClear);
			ToolTip.Default.SetToolTip(this.buttonClear, Res.Strings.Dialog.Settings.Fonts.Clear);

			toolBar.Items.Add(new IconSeparator());

			this.fontSelector = new Widgets.FontSelector(this);
			this.fontSelector.Width  = 264;
			this.fontSelector.Height = 240;
			this.fontSelector.Anchor = AnchorStyles.Bottom;
			this.fontSelector.AnchorMargins = new Margins(0, 0, 0, 16);
			this.fontSelector.FontList = Misc.GetFontList(false);
			this.fontSelector.Build();
			this.fontSelector.SelectionChanged += new EventHandler(this.HandleFontSelectorSelectionChanged);
		
			this.Update();
			this.UpdateFontsButtons();
		}

		// Met à jour la liste des polices rapides.
		public void Update()
		{
			this.fontSelector.SelectedList = this.document.Settings.QuickFonts;
		}
		

		// Met à jour les boutons des polices rapides.
		protected void UpdateFontsButtons()
		{
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
		}

		private void HandleButtonClear(object sender, MessageEventArgs e)
		{
			this.fontSelector.SelectedList.Clear();
			this.UpdateFontsButtons();
			this.fontSelector.UpdateList();
		}

		private void HandleFontSelectorSelectionChanged(object sender)
		{
			this.UpdateFontsButtons();
		}

		
		protected HToolBar					toolBar;
		protected IconButton				buttonDefault;
		protected IconButton				buttonClear;
		protected Widgets.FontSelector		fontSelector;
	}
}
