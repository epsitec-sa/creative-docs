//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for AbstractOkCancel.
	/// </summary>
	public abstract class AbstractOkCancel : AbstractDialog
	{
		public AbstractOkCancel(string dialog_title, string command_template, CommandDispatcher command_dispatcher)
		{
			this.dialog_title       = dialog_title;
			this.command_template   = command_template;
			this.command_dispatcher = command_dispatcher;
			this.private_dispatcher = new CommandDispatcher ("Dialog", true);
		}
		
		
		public abstract string[]				CommandArgs
		{
			get;
		}
		
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return this.private_dispatcher;
			}
		}
		
		
		protected abstract Widget CreateBodyWidget();
		protected override void   CreateWindow()
		{
			Widget body = this.CreateBodyWidget ();
			Button button1;
			Button button2;
			
			double dx = body.Width;
			double dy = body.Height;
			
			dx = System.Math.Max (dx, 2*75+3*8);
			
			this.window = new Window ();
			
			this.window.Text              = this.dialog_title;
			this.window.Name              = "Dialog";
			this.window.ClientSize        = new Drawing.Size (dx+2*8, dy+2*16+24+16);
			this.window.PreventAutoClose  = true;
			this.window.CommandDispatcher = this.private_dispatcher;
			this.window.CommandDispatcher.RegisterController (this);
			this.window.MakeFixedSizeWindow ();
			this.window.MakeButtonlessWindow ();
			this.window.MakeSecondaryWindow ();
			
			body.Parent          = this.window.Root;
			body.Bounds          = new Drawing.Rectangle (8, 16+24+16, dx, dy);
			body.TabIndex        = 1;
			body.TabNavigation   = Widget.TabNavigationMode.ForwardTabPassive;
			
			button1               = new Button (this.window.Root);
			button1.Bounds        = new Drawing.Rectangle (this.window.Root.Width - 2*75 - 2*8, 16, 75, button1.Height);
			button1.Text          = "OK";
			button1.Command       = "ValidateDialog";
			button1.TabIndex      = 2;
			button1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button1.Shortcut      = Widgets.Feel.Factory.Active.AcceptShortcut;
			
			button2               = new Button (this.window.Root);
			button2.Bounds        = new Drawing.Rectangle (this.window.Root.Width - 75 - 8, 16, 75, button2.Height);
			button2.Text          = "Annuler";
			button2.Command       = "QuitDialog";
			button2.TabIndex      = 3;
			button2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button2.Shortcut      = Widgets.Feel.Factory.Active.CancelShortcut;
			
			AbstractDialog.LayoutButtons (this.window.Root.Width, button1, button2);
			
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
		
		
		protected string						dialog_title;
		protected string						command_template;
		protected Support.CommandDispatcher		command_dispatcher;
		protected Support.CommandDispatcher		private_dispatcher;
	}
}
