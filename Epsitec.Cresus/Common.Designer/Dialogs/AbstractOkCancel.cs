using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Summary description for AbstractOkCancel.
	/// </summary>
	public abstract class AbstractOkCancel
	{
		public AbstractOkCancel(string command_template, CommandDispatcher command_dispatcher)
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
		
		public void Close()
		{
			this.window.Hide ();
			this.window.CommandDispatcher.Dispose ();
			this.window.CommandDispatcher = null;
			this.window.AsyncDispose ();
		}
		
		
		public abstract string[]				CommandArgs
		{
			get;
		}
		
		
		protected abstract Widget CreateBodyWidget();
		
		protected virtual void CreateWindow()
		{
			Widget body = this.CreateBodyWidget ();
			Button button;
			
			double dx = body.Width;
			double dy = body.Height;
			
			this.window = new Window ();
			this.window.Text = "Nom de la ressource";
			this.window.Name = "Dialog";
			this.window.ClientSize = new Drawing.Size (dx+2*8, dy+2*16+24+16);
			this.window.PreventAutoClose = true;
			this.window.CommandDispatcher = new CommandDispatcher ("Dialog", true);
			this.window.CommandDispatcher.RegisterController (this);
			this.window.MakeFixedSizeWindow ();
			this.window.MakeSecondaryWindow ();
			
			body.Parent          = this.window.Root;
			body.Bounds          = new Drawing.Rectangle (8, 16+24+16, dx, dy);
			body.TabIndex        = 1;
			body.TabNavigation   = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			
			button               = new Button (this.window.Root);
			button.Bounds        = new Drawing.Rectangle (this.window.Root.Width - 2*80 - 2*8, 16, 80, 24);
			button.Text          = "OK";
			button.Command       = "ValidateDialog";
			button.ButtonStyle   = ButtonStyle.DefaultActive;
			button.TabIndex      = 2;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button               = new Button (this.window.Root);
			button.Bounds        = new Drawing.Rectangle (this.window.Root.Width - 80 - 8, 16, 80, 24);
			button.Text          = "Annuler";
			button.Command       = "QuitDialog";
			button.TabIndex      = 3;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			this.window.FocusedWidget = body.FindTabWidget (Widget.TabNavigationDir.Forwards, Widget.TabNavigationMode.ActivateOnTab);
		}
		
		
		[Command ("ValidateDialog")] protected void CommandValidateDialog()
		{
			this.window.QueueCommand (this, string.Format (this.command_template, this.CommandArgs), this.command_dispatcher);
			this.Close ();
		}
		
		[Command ("QuitDialog")]     protected void CommandQuitDialog()
		{
			this.Close ();
		}
		
		
		
		protected Window						window;
		protected string						command_template;
		protected Support.CommandDispatcher		command_dispatcher;
	}
}
