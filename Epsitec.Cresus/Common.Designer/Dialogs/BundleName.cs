using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Summary description for BundleName.
	/// </summary>
	public class BundleName
	{
		public BundleName(string command_template, CommandDispatcher command_dispatcher)
		{
			this.command_template   = command_template;
			this.command_dispatcher = command_dispatcher;
		}
		
		public void Show()
		{
			if (this.window == null)
			{
				this.CreateWindow ();
			}
			
			this.window.Show ();
		}
		
		protected virtual void CreateWindow()
		{
			this.window = new Window ();
			this.window.Text = "Nom de la ressource";
			this.window.Name = "Dialog";
			this.window.ClientSize = new Drawing.Size (320, 180);
			this.window.PreventAutoClose = true;
			this.window.CommandDispatcher = new CommandDispatcher ("Dialog", true);
			this.window.CommandDispatcher.RegisterController (this);
			
			StaticText label;
			Button     button;
			
			double dx = this.window.ClientSize.Width;
			double dy = this.window.ClientSize.Height;
			
			label        = new StaticText (this.window.Root);
			label.Bounds = new Drawing.Rectangle (5, dy - 5 - 22, 120, 22);
			label.Text   = "Nom de la ressource : ";
			
			this.text        = new TextField (this.window.Root);
			this.text.Bounds = new Drawing.Rectangle (label.Right + 5, label.Bottom, dx - label.Right - 10, label.Height);
			
			button         = new Button (this.window.Root);
			button.Bounds  = new Drawing.Rectangle (5, 10, 80, 24);
			button.Text    = "OK";
			button.Command = "ValidateDialog";
			
			button         = new Button (this.window.Root);
			button.Bounds  = new Drawing.Rectangle (90, 10, 80, 24);
			button.Text    = "Annuler";
			button.Command = "QuitDialog";
		}
		
		[Command ("ValidateDialog")] void CommandValidateDialog()
		{
			string command = string.Format (this.command_template, this.text.Text);
			this.window.QueueCommand (this, command, this.command_dispatcher);
			this.Close ();
		}
		
		[Command ("QuitDialog")] void CommandQuitDialog()
		{
			this.Close ();
		}
		
		
		public void Close()
		{
			this.window.Hide ();
			this.window.CommandDispatcher.Dispose ();
			this.window.CommandDispatcher = null;
			this.window.AsyncDispose ();
		}
		
		
		protected Window						window;
		protected TextField						text;
		protected string						command_template;
		protected Support.CommandDispatcher		command_dispatcher;
	}
}
