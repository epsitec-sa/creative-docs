//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs.Helpers;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	internal sealed class YesNoCancelDialog : AbstractYesNoCancelDialog
	{
		internal YesNoCancelDialog(string dialogTitle, string yesText, string noText, string cancelText, string messageIcon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
			: base (dialogTitle, yesText, noText, cancelText, commandYesTemplate, commandNoTemplate, commandDispatcher)
		{
			this.messageIcon = messageIcon;
			this.messageText = messageText;
		}
		
		public override string[] CommandArgs
		{
			get
			{
				return new string[0];
			}
		}
		
		protected override void OnDialogOpened()
		{
			base.OnDialogOpened ();
			Platform.Beep.MessageBeep (Platform.Beep.MessageType.Warning);
		}
		
		protected override Widgets.Widget CreateBodyWidget()
		{
			return MessageBuilder.CreateIconAndText (this.messageIcon, this.messageText);
		}
		
		private readonly string messageIcon;
		private readonly string messageText;
	}
}
