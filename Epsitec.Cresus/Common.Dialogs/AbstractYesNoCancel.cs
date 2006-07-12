//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for AbstractYesNoCancel.
	/// </summary>
	public abstract class AbstractYesNoCancel : AbstractMessageDialog
	{
		public AbstractYesNoCancel(string dialog_title, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			this.dialog_title         = dialog_title;
			this.command_yes_template = command_yes_template;
			this.command_no_template  = command_no_template;
			
			this.private_dispatcher   = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.private_context      = new CommandContext ();
			
			this.private_dispatcher.RegisterController (this);

			CommandDispatcher.SetDispatcher (this, command_dispatcher);
		}
		
		
		public abstract string[]				CommandArgs
		{
			get;
		}
		
		protected abstract Widget CreateBodyWidget();
		protected override void   CreateWindow()
		{
			Widget body = this.CreateBodyWidget ();
			Button button1;
			Button button2;
			Button button3 = null;
			
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;
			
			dx = System.Math.Max (dx, 3*75+4*8);
			
			this.window = new Window ();
			
			this.window.Text              = this.dialog_title;
			this.window.Name              = "Dialog";
			this.window.ClientSize        = new Drawing.Size (dx+2*8, dy+2*16+24+16);
			this.window.PreventAutoClose  = true;
			
			CommandDispatcher.SetDispatcher (this.window, this.private_dispatcher);
			CommandContext.SetContext (this.window, this.private_context);

			this.private_context.GetCommandState ("ValidateDialogYes").Enable = true;
			this.private_context.GetCommandState ("ValidateDialogNo").Enable = true;
			this.private_context.GetCommandState ("QuitDialog").Enable = true;
			
			this.window.MakeFixedSizeWindow ();
			this.window.MakeSecondaryWindow ();
			
			body.SetParent (this.window.Root);
			body.SetManualBounds(new Drawing.Rectangle(8, 16+24+16, dx, dy));
			body.TabIndex      = 1;
			body.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			button1               = new Button (this.window.Root);
			button1.SetManualBounds(new Drawing.Rectangle(this.window.ClientSize.Width - 3*75 - 3*8, 16, 75, button1.PreferredHeight));
			button1.Text          = Widgets.Res.Strings.Dialog.Button.Yes;
			button1.Command       = "ValidateDialogYes";
			button1.TabIndex      = 2;
			button1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button1.ButtonStyle   = ButtonStyle.DefaultAccept;
			
			button2               = new Button (this.window.Root);
			button2.SetManualBounds(new Drawing.Rectangle(this.window.ClientSize.Width - 2*75 - 2*8, 16, 75, button2.PreferredHeight));
			button2.Text          = Widgets.Res.Strings.Dialog.Button.No;
			button2.Command       = "ValidateDialogNo";
			button2.TabIndex      = 3;
			button2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			if (this.hide_cancel == false)
			{
				button3               = new Button (this.window.Root);
				button3.SetManualBounds(new Drawing.Rectangle(this.window.ClientSize.Width - 1*75 - 1*8, 16, 75, button3.PreferredHeight));
				button3.Text          = Widgets.Res.Strings.Dialog.Button.Cancel;
				button3.Name          = "Cancel";
				button3.Command       = "QuitDialog";
				button3.TabIndex      = 4;
				button3.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button3.ButtonStyle   = ButtonStyle.DefaultCancel;
			}
			
			AbstractMessageDialog.LayoutButtons (this.window.ClientSize.Width, button1, button2, button3);
			
			this.window.FocusedWidget = body.FindTabWidget (Widget.TabNavigationDir.Forwards, Widget.TabNavigationMode.ActivateOnTab);
		}
		
		
		[Command ("ValidateDialogYes")] protected void CommandValidateDialogYes()
		{
			this.result = DialogResult.Yes;
			
			if (this.command_yes_template != null)
			{
				this.DispatchWindow.QueueCommand (this.Window, string.Format (this.command_yes_template, this.CommandArgs));
			}
			
			this.CloseDialog ();
		}
		
		[Command ("ValidateDialogNo")]  protected void CommandValidateDialogNo()
		{
			this.result = DialogResult.No;
			
			if (this.command_no_template != null)
			{
				this.DispatchWindow.QueueCommand (this.Window, string.Format (this.command_no_template, this.CommandArgs));
			}
			
			this.CloseDialog ();
		}
		
		[Command ("QuitDialog")]        protected void CommandQuitDialog()
		{
			this.result = DialogResult.Cancel;
			
			this.CloseDialog ();
		}
		
		
		protected string						dialog_title;
		protected string						command_yes_template;
		protected string						command_no_template;
		protected CommandDispatcher				command_dispatcher;
		protected CommandDispatcher				private_dispatcher;
		protected CommandContext				private_context;
	}
}
