//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for AbstractOkCancel.
	/// </summary>
	public abstract class AbstractOkCancel : AbstractMessageDialog
	{
		public AbstractOkCancel(string dialogTitle, string okText, string cancelText, string commandTemplate, CommandDispatcher commandDispatcher)
		{
			this.dialogTitle       = dialogTitle;
			this.okText            = okText;
			this.cancelText        = cancelText;
			
			this.commandTemplate   = commandTemplate;
			this.commandDispatcher = commandDispatcher;

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

			dialogWindow.Text             = this.dialogTitle;
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
			button1.FormattedText = this.okText.IsNullOrEmpty ? Widgets.Res.Strings.Dialog.Button.OK : this.okText;
			button1.CommandObject = Res.Commands.Dialog.Generic.Ok;
			button1.TabIndex      = 2;
			button1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button1.Shortcuts.Add (Widgets.Feel.Factory.Active.AcceptShortcut);
			
			if (this.hideCancel == false)
			{
				button2               = new Button (dialogWindow.Root);
				button2.SetManualBounds (new Drawing.Rectangle (dialogWindow.ClientSize.Width - 1*75 - 1*8, 16, 75, button2.PreferredHeight));
				button2.FormattedText = this.cancelText.IsNullOrEmpty ? Widgets.Res.Strings.Dialog.Button.Cancel : this.cancelText;
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
			this.Result = DialogResult.Accept;
			
			if (! string.IsNullOrEmpty (this.commandTemplate))
			{
				this.DispatchWindow.QueueCommand (this.DialogWindow, string.Format (this.commandTemplate, this.CommandArgs));
			}
			
			this.CloseDialog ();
		}

		[Command (Res.CommandIds.Dialog.Generic.Cancel)]
		protected void CommandQuitDialog()
		{
			this.Result = DialogResult.Cancel;
			
			this.CloseDialog ();
		}
		
		
		protected string						dialogTitle;
		protected FormattedText					okText;
		protected FormattedText					cancelText;
		protected string						commandTemplate;
		protected CommandDispatcher				commandDispatcher;
	}
}
