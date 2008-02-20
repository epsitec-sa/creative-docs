//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

			this.RegisterController (this);
		}
		
		
		public abstract string[]				CommandArgs
		{
			get;
		}
		
		protected abstract Widget CreateBodyWidget();
		protected override Window CreateWindow()
		{
			Widget body = this.CreateBodyWidget ();
			Button button1;
			Button button2 = null;
			
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;
			
			dx = System.Math.Max (dx, 2*75+3*8);
			
			Window dialogWindow = new Window ();

			dialogWindow.Text             = this.dialog_title;
			dialogWindow.Name             = "Dialog";
			dialogWindow.ClientSize       = new Drawing.Size (dx+2*8, dy+2*16+24+16);
			dialogWindow.PreventAutoClose = true;

			this.CommandContext.GetCommandState (Res.Commands.Dialog.Generic.Cancel).Enable = true;
			this.CommandContext.GetCommandState (Res.Commands.Dialog.Generic.Ok).Enable = true;

			dialogWindow.MakeFixedSizeWindow ();
			dialogWindow.MakeButtonlessWindow ();
			dialogWindow.MakeSecondaryWindow ();

			body.SetParent (dialogWindow.Root);
			body.SetManualBounds(new Drawing.Rectangle(8, 16+24+16, dx, dy));
			body.TabIndex      = 1;
			body.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			button1               = new Button (dialogWindow.Root);
			button1.SetManualBounds (new Drawing.Rectangle (dialogWindow.ClientSize.Width - 2*75 - 2*8, 16, 75, button1.PreferredHeight));
			button1.Text          = string.IsNullOrEmpty(this.ok_text) ? Widgets.Res.Strings.Dialog.Button.OK : this.ok_text;
			button1.CommandObject = Res.Commands.Dialog.Generic.Ok;
			button1.TabIndex      = 2;
			button1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button1.Shortcuts.Add (Widgets.Feel.Factory.Active.AcceptShortcut);
			
			if (this.hide_cancel == false)
			{
				button2               = new Button (dialogWindow.Root);
				button2.SetManualBounds (new Drawing.Rectangle (dialogWindow.ClientSize.Width - 1*75 - 1*8, 16, 75, button2.PreferredHeight));
				button2.Text          = string.IsNullOrEmpty(this.cancel_text) ? Widgets.Res.Strings.Dialog.Button.Cancel : this.cancel_text;
				button2.Name          = "Cancel";
				button2.CommandObject = Res.Commands.Dialog.Generic.Cancel;
				button2.TabIndex      = 3;
				button2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button2.Shortcuts.Add (Widgets.Feel.Factory.Active.CancelShortcut);
			}

			AbstractMessageDialog.LayoutButtons (dialogWindow.ClientSize.Width, button1, button2);

			body.SetFocusOnTabWidget ();

			dialogWindow.WindowCloseClicked +=
				delegate
				{
					this.CommandQuitDialog ();
				};

			return dialogWindow;
		}


		[Command (Res.CommandIds.Dialog.Generic.Ok)]
		protected void CommandValidateDialog()
		{
			this.DialogResult = DialogResult.Accept;
			
			if (! string.IsNullOrEmpty (this.command_template))
			{
				this.DispatchWindow.QueueCommand (this.DialogWindow, string.Format (this.command_template, this.CommandArgs));
			}
			
			this.CloseDialog ();
		}

		[Command (Res.CommandIds.Dialog.Generic.Cancel)]
		protected void CommandQuitDialog()
		{
			this.DialogResult = DialogResult.Cancel;
			
			this.CloseDialog ();
		}
		
		
		protected string						dialog_title;
		protected string						ok_text;
		protected string						cancel_text;
		protected string						command_template;
		protected CommandDispatcher				command_dispatcher;
	}
}
