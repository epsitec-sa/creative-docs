using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe Application initialise tout ce qui est en relation avec le
	/// "designer".
	/// </summary>
	public class Application : Support.ICommandDispatcherHost
	{
		public Application()
		{
			this.Initialise ();
		}
		
		
		public Window					MainWindow
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
		
		public CommandDispatcher		CommandDispatcher
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
		
		
		public BundleEditController		BundleEditController
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.is_initialised);
				
				return this.bundle_edit_controller;
			}
		}
		
		public StringEditController		StringEditController
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.is_initialised);
				
				if (this.string_edit_controller == null)
				{
					this.string_edit_controller = new StringEditController (this.CommandDispatcher);
				}
				
				return this.string_edit_controller;
			}
		}
		
		protected void Initialise()
		{
			if ((this.is_initialised) ||
				(this.is_initialising))
			{
				return;
			}
			
			this.is_initialising = true;
			
			Widgets.Widget.Initialise ();
			Pictogram.Engine.Initialise ();
			
			this.CreateMainWindow ();
			this.RegisterCommands ();
			
			this.is_initialised  = true;
			this.is_initialising = false;
			
			this.bundle_edit_controller = new BundleEditController (this);
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
		}
		
		private void RegisterCommands()
		{
			this.dispatcher.RegisterController (this);
		}
		
		
		
		
		protected bool							is_initialised;
		protected bool							is_initialising;
		
		protected Window						main_window;
		protected Support.CommandDispatcher		dispatcher;
		protected StringEditController			string_edit_controller;
		protected BundleEditController			bundle_edit_controller;
	}
}
