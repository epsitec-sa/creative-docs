//	Copyright © 2004-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for AbstractYesNoCancel.
	/// </summary>
	public abstract class AbstractYesNoCancelDialog : AbstractMessageDialog
	{
		public AbstractYesNoCancelDialog(string dialogTitle, string yesText, string noText, string cancelText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			this.dialogTitle         = dialogTitle;
			this.yesText             = yesText;
			this.noText              = noText;
			this.cancelText          = cancelText;
			this.commandYesTemplate = commandYesTemplate;
			this.commandNoTemplate  = commandNoTemplate;
			
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
			Button button2;
			Button button3 = null;
			
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;
			
			dx = System.Math.Max (dx, 3*75+4*8);

			Window dialogWindow = new Window ();

			dialogWindow.Text              = this.dialogTitle;
			dialogWindow.Name              = "Dialog";
			dialogWindow.ClientSize        = new Drawing.Size (dx+2*8, dy+2*16+24+16);
			dialogWindow.PreventAutoClose  = true;

			this.CommandContext.GetCommandState (Res.Commands.Dialog.Generic.Yes).Enable = true;
			this.CommandContext.GetCommandState (Res.Commands.Dialog.Generic.No).Enable = true;
			this.CommandContext.GetCommandState (Res.Commands.Dialog.Generic.Cancel).Enable = true;

			dialogWindow.MakeFixedSizeWindow ();
			dialogWindow.MakeSecondaryWindow ();

			body.SetParent (dialogWindow.Root);
			body.SetManualBounds(new Drawing.Rectangle(8, 16+24+16, dx, dy));
			body.TabIndex      = 1;
			body.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			button1               = new Button (dialogWindow.Root);
			button1.SetManualBounds (new Drawing.Rectangle (dialogWindow.ClientSize.Width - 3*75 - 3*8, 16, 75, button1.PreferredHeight));
			button1.FormattedText = this.yesText.IsNullOrEmpty ? Widgets.Res.Strings.Dialog.Button.Yes : this.yesText;
			button1.CommandObject = Res.Commands.Dialog.Generic.Yes;
			button1.TabIndex      = 2;
			button1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button1.ButtonStyle   = ButtonStyle.DefaultAccept;

			button2               = new Button (dialogWindow.Root);
			button2.SetManualBounds (new Drawing.Rectangle (dialogWindow.ClientSize.Width - 2*75 - 2*8, 16, 75, button2.PreferredHeight));
			button2.FormattedText = this.noText.IsNullOrEmpty ? Widgets.Res.Strings.Dialog.Button.No : this.noText;
			button2.CommandObject = Res.Commands.Dialog.Generic.No;
			button2.TabIndex      = 3;
			button2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			if (this.hideCancel == false)
			{
				button3               = new Button (dialogWindow.Root);
				button3.SetManualBounds (new Drawing.Rectangle (dialogWindow.ClientSize.Width - 1*75 - 1*8, 16, 75, button3.PreferredHeight));
				button3.FormattedText = this.cancelText.IsNullOrEmpty ? Widgets.Res.Strings.Dialog.Button.Cancel : this.cancelText;
				button3.Name          = "Cancel";
				button3.CommandObject = Res.Commands.Dialog.Generic.Cancel;
				button3.TabIndex      = 4;
				button3.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button3.ButtonStyle   = ButtonStyle.DefaultCancel;
			}

			AbstractMessageDialog.LayoutButtons (dialogWindow.ClientSize.Width, button1, button2, button3);

			body.SetFocusOnTabWidget ();

			dialogWindow.WindowCloseClicked +=
				delegate
				{
					this.CommandQuitDialog ();
				};

			return dialogWindow;
		}
		
		
		[Command (Res.CommandIds.Dialog.Generic.Yes)]
		protected void CommandValidateDialogYes()
		{
			this.Result = DialogResult.Yes;
			
			if (this.commandYesTemplate != null)
			{
				this.DispatchWindow.QueueCommand (this.DialogWindow, string.Format (this.commandYesTemplate, this.CommandArgs));
			}
			
			this.CloseDialog ();
		}

		[Command (Res.CommandIds.Dialog.Generic.No)]
		protected void CommandValidateDialogNo()
		{
			this.Result = DialogResult.No;
			
			if (this.commandNoTemplate != null)
			{
				this.DispatchWindow.QueueCommand (this.DialogWindow, string.Format (this.commandNoTemplate, this.CommandArgs));
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
		protected FormattedText					yesText;
		protected FormattedText					noText;
		protected FormattedText					cancelText;
		protected string						commandYesTemplate;
		protected string						commandNoTemplate;
		protected CommandDispatcher				commandDispatcher;
	}
}
