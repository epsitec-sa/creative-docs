using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{
		public Application()
		{
			Widgets.Widget.Initialise ();
			Pictogram.Engine.Initialise ();
		}
		
		
		public Window					MainWindow
		{
			get
			{
				if (this.main_window == null)
				{
					this.CreateMainWindow ();
				}
				
				return this.main_window;
			}
		}
		
		private void CreateMainWindow()
		{
			this.dispatcher  = new CommandDispatcher ("Designer");
			
			this.main_window = new Window ();
			this.main_window.CommandDispatcher = this.dispatcher;
			this.main_window.Text = "Designer";
			this.main_window.Name = "Designer";
			this.main_window.ClientSize = new Drawing.Size (400, 300);
			this.main_window.PreventAutoClose = true;
			
			this.RegisterCommands ();
		}
		
		private void RegisterCommands()
		{
			this.dispatcher.RegisterController (this);
		}
		
		
		protected Window				main_window;
		protected CommandDispatcher		dispatcher;
	}
}
