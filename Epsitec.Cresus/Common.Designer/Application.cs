//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe Application initialise tout ce qui est en relation avec le
	/// "designer".
	/// </summary>
	public class Application : Support.ICommandDispatcherHost
	{
		static Application()
		{
			Epsitec.Common.Widgets.Widget.Initialise ();
			Epsitec.Common.Pictogram.Engine.Initialise ();
			
			ObjectBundler.RegisterAssembly (System.Reflection.Assembly.GetExecutingAssembly ());
		}
		
		public Application()
		{
			this.name = "Designer";
			this.Initialise ();
		}
		
		
		public Window							MainWindow
		{
			get
			{
				if (this.main_window == null)
				{
					this.Initialise ();
				}
				
				return this.main_window;
			}
		}
		
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				if (this.dispatcher == null)
				{
					this.Initialise ();
				}
				
				return this.dispatcher;
			}
			set
			{
				throw new System.InvalidOperationException ("CommandDispatcher cannot be set.");
			}
		}
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public StringEditController				StringEditController
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.is_initialised);
				return this.string_edit_controller;
			}
		}
		
		public InterfaceEditController			InterfaceEditController
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.is_initialised);
				return this.interf_edit_controller;
			}
		}
		
		
		internal void OpenStringPicker(string bundle, string field, Support.EventHandler accept_handler)
		{
			Context.Save (this);
			
			this.switcher.Mode         = Widgets.SwitcherMode.AcceptReject;
			this.switcher.SelectedName = PanelName.StringEdit.ToString ();
			
			this.switcher_accept_handler = accept_handler;
			
			this.StringEditController.SelectBundleField (bundle, field);
		}
		
		
		internal void Initialise()
		{
			if ((this.is_initialised) ||
				(this.is_initialising))
			{
				return;
			}
			
			System.Diagnostics.Debug.WriteLine ("Initialising designer application object.");
			
			this.is_initialising = true;
			
			this.CreateMainWindow ();
			this.RegisterCommands ();
			
			this.is_initialised  = true;
			this.is_initialising = false;
			
			this.interf_edit_controller = new InterfaceEditController (this);
			this.string_edit_controller = new StringEditController (this);
			
			this.interf_edit_controller.Initialise ();
			this.string_edit_controller.Initialise ();
			
			this.CreateToolBar ();
			this.CreateSwitcher ();
			
			Widget panel;
			
			panel = this.interf_edit_controller.MainPanel;
			
			panel.Dock   = DockStyle.Fill;
			panel.Parent = this.main_window.Root;
			
			panel = this.string_edit_controller.MainPanel;
			
			panel.Dock   = DockStyle.Fill;
			panel.Parent = this.main_window.Root;
			
			this.main_window.MakeFixedSizeWindow ();
			this.main_window.ClientSize = new Drawing.Size (312, 704);
		}
		
		internal void Kill()
		{
			this.string_edit_controller.Dispose ();
			this.interf_edit_controller.Dispose ();
			
			this.main_window            = null;
			this.string_edit_controller = null;
			this.interf_edit_controller = null;
			this.tool_bar               = null;
			this.lang_swap              = null;
			this.lang_combo             = null;
			this.switcher               = null;
			this.dispatcher             = null;
			this.context_stack          = null;
			
			this.is_initialised  = false;
			this.is_initialising = false;
			
			System.Diagnostics.Debug.WriteLine ("Killed designer application object.");
		}
		
		
		private void CreateSwitcher()
		{
			this.switcher = new Widgets.Switcher (this.main_window.Root);
			
			this.switcher.Items.Add (PanelName.InterfaceEdit.ToString (), "Interface utilisateur");
			this.switcher.Items.Add (PanelName.StringEdit.ToString (),    "Ressources (textes)");
			
			this.switcher.Dock   = DockStyle.Top;
			this.switcher.Mode   = Widgets.SwitcherMode.Select;
			
			this.switcher.SelectedIndexChanged += new EventHandler (this.HandleSwitcherSelectedIndexChanged);
			this.switcher.AcceptClicked        += new EventHandler (this.HandleSwitcherAcceptClicked);
			this.switcher.RejectClicked        += new EventHandler (this.HandleSwitcherRejectClicked);
			
			this.switcher.SelectedName = PanelName.InterfaceEdit.ToString ();
		}
		
		private void CreateToolBar()
		{
			this.tool_bar = new HToolBar (this.main_window.Root);
			
			this.tool_bar.Dock = DockStyle.Top;
			
			this.lang_sep   = new IconSeparator ();
			this.lang_swap  = new IconButton ("manifest:Epsitec.Common.Designer.Images.SwapLang.icon");
			this.lang_combo = new TextFieldCombo ();
			
			this.lang_sep.Dock         = this.tool_bar.OppositeIconDockStyle;
			this.lang_swap.Dock        = this.tool_bar.OppositeIconDockStyle;
			this.lang_combo.Dock       = this.tool_bar.OppositeIconDockStyle;
			this.lang_combo.IsReadOnly = true;
			this.lang_combo.Width      = 80;
			
			this.lang_swap.Clicked               += new MessageEventHandler (this.HandleLangSwapClicked);
			this.lang_combo.SelectedIndexChanged += new EventHandler (this.HandleLanguageComboSelectedIndexChanged);
			
			this.FillLangCombo ();
		}
		
		private void FillLangCombo()
		{
			this.lang_combo.Items.Clear ();
			
			foreach (CultureInfo culture in Resources.Cultures)
			{
				this.lang_combo.Items.Add (culture.TwoLetterISOLanguageName, culture.DisplayName);
			}
		}
		
		private void CreateMainWindow()
		{
			this.dispatcher  = new CommandDispatcher ("Designer");
			
			this.main_window = new Window ();
			this.main_window.CommandDispatcher = this.dispatcher;
			this.main_window.Text = "Interface Designer";
			this.main_window.Name = "Designer";
			this.main_window.ClientSize = new Drawing.Size (400, 300);
			this.main_window.PreventAutoClose = true;
			this.main_window.WindowClosed   += new EventHandler (this.HandleMainWindowClosed);
			this.main_window.WindowDisposed += new EventHandler (this.HandleMainWindowDisposed);
		}
		
		private void RegisterCommands()
		{
			this.dispatcher.RegisterController (this);
		}
		
		
		private void UpdateVisiblePanel()
		{
			bool show_string_edit = false;
			bool show_interf_edit = false;
			
			PanelName panel = (PanelName) System.Enum.Parse (typeof (PanelName), this.switcher.SelectedName);
			
			this.tool_bar.Items.Clear ();
			
			switch (panel)
			{
				case PanelName.InterfaceEdit:
					this.interf_edit_controller.FillToolBar (this.tool_bar);
					show_interf_edit = true;
					break;
				
				case PanelName.StringEdit:
					this.string_edit_controller.FillToolBar (this.tool_bar);
					show_string_edit = true;
					break;
			}
			
			this.FillToolBar (this.tool_bar);
			
			this.interf_edit_controller.MainPanel.SetVisible (show_interf_edit);
			this.string_edit_controller.MainPanel.SetVisible (show_string_edit);
		}
		
		private void FillToolBar (AbstractToolBar tool_bar)
		{
			tool_bar.Items.Add (this.lang_combo);
			tool_bar.Items.Add (this.lang_swap);
			tool_bar.Items.Add (this.lang_sep);
		}
		
		
		private void HandleLanguageComboSelectedIndexChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.lang_combo == sender);
			
			string suffix = this.lang_combo.SelectedName;
			
			if (suffix != this.lang_suffix_1)
			{
				CultureInfo culture = Resources.FindSpecificCultureInfo (suffix);
				
				this.lang_suffix_2 = this.lang_suffix_1;
				this.lang_suffix_1 = suffix;
				
				Resources.Culture = culture;
				
				System.Threading.Thread.CurrentThread.CurrentCulture   = culture;
				System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
				
				Window.InvalidateAll (Window.InvalidateReason.CultureChanged);
			}
		}
		
		private void HandleLangSwapClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.lang_swap == sender);
			
			if ((this.lang_suffix_2 != null) &&
				(this.lang_suffix_1 != this.lang_suffix_2))
			{
				this.lang_combo.SelectedName = this.lang_suffix_2;
			}
		}
		
		private void HandleSwitcherSelectedIndexChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.switcher == sender);
			
			this.UpdateVisiblePanel ();
		}
		
		private void HandleSwitcherAcceptClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (this.switcher == sender);
			
			Support.EventHandler accept_handler = this.switcher_accept_handler;
			
			Context.Restore (this);
			
			if (accept_handler != null)
			{
				accept_handler (this);
			}
		}
		
		private void HandleSwitcherRejectClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (this.switcher == sender);
			
			Context.Restore (this);
		}
		
		
		private void HandleMainWindowClosed(object sender)
		{
			if (this.is_initialised)
			{
				System.Diagnostics.Debug.Assert (this.main_window == sender);
				
				this.Kill ();
			}
		}
		
		private void HandleMainWindowDisposed(object sender)
		{
			if (this.is_initialised)
			{
				System.Diagnostics.Debug.Assert (this.main_window == sender);
				System.Diagnostics.Debug.WriteLine ("Main window disposed.");
				
				this.Kill ();
			}
		}
		
		#region Application Commands
		[Command ("QuitDesigner")]	void CommandQuitDesigner()
		{
			this.MainWindow.Close ();
		}
		#endregion
		
		protected enum PanelName
		{
			None,
			InterfaceEdit,
			StringEdit
		}
		
		private class Context
		{
			private Context(Application app)
			{
				this.switcher_mode = app.switcher.Mode;
				this.switcher_sel  = app.switcher.SelectedName;
				
				this.switcher_accept_handler = app.switcher_accept_handler;
			}
			
			
			public static void Save(Application app)
			{
				app.context_stack.Push (new Context (app));
			}
			
			public static void Restore(Application app)
			{
				Context context = app.context_stack.Pop () as Context;
				
				app.switcher.Mode           = context.switcher_mode;
				app.switcher.SelectedName   = context.switcher_sel;
				app.switcher_accept_handler = context.switcher_accept_handler;
			}
			
			
			private Widgets.SwitcherMode		switcher_mode;
			private string						switcher_sel;
			private Support.EventHandler		switcher_accept_handler;
		}
		
		
		protected bool							is_initialised;
		protected bool							is_initialising;
		
		protected Window						main_window;
		protected Support.CommandDispatcher		dispatcher;
		
		protected StringEditController			string_edit_controller;
		protected InterfaceEditController		interf_edit_controller;
		
		protected string						name;
		
		protected AbstractToolBar				tool_bar;
		
		protected IconSeparator					lang_sep;
		protected IconButton					lang_swap;
		protected TextFieldCombo				lang_combo;
		protected string						lang_suffix_1;
		protected string						lang_suffix_2;
		
		protected Widgets.Switcher				switcher;
		protected Support.EventHandler			switcher_accept_handler;
		
		private System.Collections.Stack		context_stack = new System.Collections.Stack ();
	}
}
