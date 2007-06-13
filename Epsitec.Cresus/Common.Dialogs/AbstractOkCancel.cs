//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for AbstractOkCancel.
	/// </summary>
	public abstract class AbstractOkCancel : AbstractMessageDialog
	{
		public AbstractOkCancel(string dialog_title, string ok_text, string cancel_text, string command_template, CommandDispatcher command_dispatcher)
		{
			this.dialog_title       = dialog_title;
			this.ok_text            = ok_text;
			this.cancel_text        = cancel_text;
			
			this.command_template   = command_template;
			this.command_dispatcher = command_dispatcher;
			
			this.private_dispatcher = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.private_context    = new CommandContext ();
			
			this.private_dispatcher.RegisterController (this);
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

		public CommandContext					CommandContext
		{
			get
			{
				return this.private_context;
			}
		}
		
		
		protected abstract Widget CreateBodyWidget();
		protected override void   CreateWindow()
		{
			Widget body = this.CreateBodyWidget ();
			Button button1;
			Button button2 = null;
			
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;
			
			dx = System.Math.Max (dx, 2*75+3*8);
			
			this.window = new Window ();
			
			this.window.Text             = this.dialog_title;
			this.window.Name             = "Dialog";
			this.window.ClientSize       = new Drawing.Size (dx+2*8, dy+2*16+24+16);
			this.window.PreventAutoClose = true;

			CommandDispatcher.SetDispatcher (this.window, this.private_dispatcher);
			CommandContext.SetContext (this.window, this.private_context);

			this.private_context.GetCommandState ("QuitDialog").Enable = true;
			this.private_context.GetCommandState ("ValidateDialog").Enable = true;
			
			this.window.MakeFixedSizeWindow ();
			this.window.MakeButtonlessWindow ();
			this.window.MakeSecondaryWindow ();
			
			body.SetParent (this.window.Root);
			body.SetManualBounds(new Drawing.Rectangle(8, 16+24+16, dx, dy));
			body.TabIndex      = 1;
			body.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			button1               = new Button (this.window.Root);
			button1.SetManualBounds(new Drawing.Rectangle(this.window.ClientSize.Width - 2*75 - 2*8, 16, 75, button1.PreferredHeight));
			button1.Text          = string.IsNullOrEmpty(this.ok_text) ? Widgets.Res.Strings.Dialog.Button.OK : this.ok_text;
			button1.CommandObject = Dialog.ValidateDialogCommand;
			button1.TabIndex      = 2;
			button1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button1.Shortcuts.Add (Widgets.Feel.Factory.Active.AcceptShortcut);
			
			if (this.hide_cancel == false)
			{
				button2               = new Button (this.window.Root);
				button2.SetManualBounds(new Drawing.Rectangle(this.window.ClientSize.Width - 1*75 - 1*8, 16, 75, button2.PreferredHeight));
				button2.Text          = string.IsNullOrEmpty(this.cancel_text) ? Widgets.Res.Strings.Dialog.Button.Cancel : this.cancel_text;
				button2.Name          = "Cancel";
				button2.CommandObject = Dialog.QuitDialogCommand;
				button2.TabIndex      = 3;
				button2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button2.Shortcuts.Add (Widgets.Feel.Factory.Active.CancelShortcut);
			}
			
			AbstractMessageDialog.LayoutButtons (this.window.ClientSize.Width, button1, button2);
			
			this.window.FocusedWidget = body.FindTabWidget (TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab);
		}
		
		
		[Command ("ValidateDialog")] protected void CommandValidateDialog()
		{
			this.result = DialogResult.Accept;
			
			if (! string.IsNullOrEmpty (this.command_template))
			{
				this.DispatchWindow.QueueCommand (this.Window, string.Format (this.command_template, this.CommandArgs));
			}
			
			this.CloseDialog ();
		}
		
		[Command ("QuitDialog")]     protected void CommandQuitDialog()
		{
			this.result = DialogResult.Cancel;
			
			this.CloseDialog ();
		}
		
		
		protected string						dialog_title;
		protected string						ok_text;
		protected string						cancel_text;
		protected string						command_template;
		protected CommandDispatcher				command_dispatcher;
		protected CommandDispatcher				private_dispatcher;
		protected CommandContext				private_context;
	}
}
