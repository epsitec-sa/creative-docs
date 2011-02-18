//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class MessageDialog
	{
		public static IDialog CreateYesNoCancel(string dialogTitle, string yesText, string noText, string cancelText, string messageIcon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			return new MessageDialog.YesNoCancel (dialogTitle, yesText, noText, cancelText, messageIcon, messageText, commandYesTemplate, commandNoTemplate, commandDispatcher);
		}

		public static IDialog CreateYesNoCancel(string dialogTitle, string messageIcon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			return new MessageDialog.YesNoCancel (dialogTitle, null, null, null, messageIcon, messageText, commandYesTemplate, commandNoTemplate, commandDispatcher);
		}

		public static IDialog CreateYesNoCancel(string dialogTitle, DialogIcon icon, string messageText)
		{
			return MessageDialog.CreateYesNoCancel (dialogTitle, icon, messageText, null, null, null);
		}

		public static IDialog CreateYesNoCancel(string dialogTitle, DialogIcon icon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			string messageIcon = MessageDialog.GetIconUri (icon);
			
			return MessageDialog.CreateYesNoCancel (dialogTitle, messageIcon, messageText, commandYesTemplate, commandNoTemplate, commandDispatcher);
		}
		
		public static IDialog CreateYesNo(string dialogTitle, string yesText, string noText, string cancelText, string messageIcon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			AbstractMessageDialog dialog = new MessageDialog.YesNoCancel (dialogTitle, yesText, noText, cancelText, messageIcon, messageText, commandYesTemplate, commandNoTemplate, commandDispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateYesNo(string dialogTitle, string messageIcon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			AbstractMessageDialog dialog = new MessageDialog.YesNoCancel (dialogTitle, null, null, null, messageIcon, messageText, commandYesTemplate, commandNoTemplate, commandDispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateYesNo(string dialogTitle, DialogIcon icon, string messageText)
		{
			return MessageDialog.CreateYesNo (dialogTitle, icon, messageText, null, null, null);
		}

		public static IDialog CreateYesNo(string dialogTitle, DialogIcon icon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher)
		{
			string messageIcon = MessageDialog.GetIconUri (icon);
			
			return MessageDialog.CreateYesNo (dialogTitle, messageIcon, messageText, commandYesTemplate, commandNoTemplate, commandDispatcher);
		}
		
		public static IDialog CreateOkCancel(string dialogTitle, string okText, string cancelText, string messageIcon, string messageText, string commandOkTemplate, CommandDispatcher commandDispatcher)
		{
			return new MessageDialog.OkCancel (dialogTitle, okText, cancelText, messageIcon, messageText, commandOkTemplate, commandDispatcher);
		}

		public static IDialog CreateOkCancel(string dialogTitle, string messageIcon, string messageText, string commandOkTemplate, CommandDispatcher commandDispatcher)
		{
			return new MessageDialog.OkCancel (dialogTitle, null, null, messageIcon, messageText, commandOkTemplate, commandDispatcher);
		}

		public static IDialog CreateOkCancel(string dialogTitle, DialogIcon icon, string messageText)
		{
			return MessageDialog.CreateOkCancel (dialogTitle, icon, messageText, null, null);
		}

		public static IDialog CreateOkCancel(string dialogTitle, DialogIcon icon, string messageText, string commandOkTemplate, CommandDispatcher commandDispatcher)
		{
			string messageIcon = MessageDialog.GetIconUri (icon);
			
			return MessageDialog.CreateOkCancel (dialogTitle, messageIcon, messageText, commandOkTemplate, commandDispatcher);
		}
		
		public static IDialog CreateOk(string dialogTitle, string okText, string cancelText, string messageIcon, string messageText, string commandOkTemplate, CommandDispatcher commandDispatcher)
		{
			AbstractMessageDialog dialog = new MessageDialog.OkCancel (dialogTitle, okText, cancelText, messageIcon, messageText, commandOkTemplate, commandDispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateOk(string dialogTitle, string messageIcon, string messageText, string commandOkTemplate, CommandDispatcher commandDispatcher)
		{
			AbstractMessageDialog dialog = new MessageDialog.OkCancel (dialogTitle, null, null, messageIcon, messageText, commandOkTemplate, commandDispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateOk(string dialogTitle, DialogIcon icon, string messageText)
		{
			return MessageDialog.CreateOk (dialogTitle, icon, messageText, null, null);
		}
		
		public static IDialog CreateOk(string dialogTitle, DialogIcon icon, string messageText, string commandOkTemplate, CommandDispatcher commandDispatcher)
		{
			string messageIcon = MessageDialog.GetIconUri (icon);
			
			return MessageDialog.CreateOk (dialogTitle, messageIcon, messageText, commandOkTemplate, commandDispatcher);
		}

		public static IDialog CreateConfirmation(string title, string header, List<string> questions, bool hasCancelButton)
		{
			ConfirmationDialog dialog = new ConfirmationDialog(title, header, questions, hasCancelButton);
			return dialog;
		}

		public static string FormatMessage(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return "";
			}
			else
			{
				text = text.Replace ("\r", "");
				text = TextLayout.ConvertToTaggedText (text);

				return text;
			}
		}

		public static string GetDialogTitle(Window owner)
		{
			Application application = Widgets.Helpers.VisualTree.GetApplication (owner);
			
			if (application == null)
			{
				return Res.Strings.Dialog.Generic.Title.ToSimpleText ();
			}
			else
			{
				return application.ShortWindowTitle;
			}
		}

		public static DialogResult ShowError(string formattedErrorMessage, Window owner)
		{
			return MessageDialog.ShowError (formattedErrorMessage, null, owner);
		}

		public static DialogResult ShowError(string formattedErrorMessage, string title, Window owner)
		{
			if (string.IsNullOrEmpty (formattedErrorMessage))
			{
				return DialogResult.None;
			}

			IDialog dialog = MessageDialog.CreateOk (title ?? MessageDialog.GetDialogTitle (owner), DialogIcon.Warning, formattedErrorMessage);

			dialog.OwnerWindow = owner;
			dialog.OpenDialog ();

			return dialog.Result;
		}

		public static DialogResult ShowError(FormattedText formattedErrorMessage, string title, Window owner)
		{
			return MessageDialog.ShowError (formattedErrorMessage.ToString (), title, owner);
		}

		public static DialogResult ShowMessage(string formattedMessage, Window owner)
		{
			return MessageDialog.ShowMessage (formattedMessage, null, owner);
		}
		
		public static DialogResult ShowMessage(string formattedMessage, string title, Window owner)
		{
			if (string.IsNullOrEmpty (formattedMessage))
			{
				return DialogResult.None;
			}

			IDialog dialog = MessageDialog.CreateOk (title ?? MessageDialog.GetDialogTitle (owner), DialogIcon.Warning, formattedMessage);

			dialog.OwnerWindow = owner;
			dialog.OpenDialog ();

			return dialog.Result;
		}

		public static DialogResult ShowQuestion(string formattedQuestion, Window owner)
		{
			if (string.IsNullOrEmpty (formattedQuestion))
			{
				return DialogResult.None;
			}

			IDialog dialog = MessageDialog.CreateYesNo (MessageDialog.GetDialogTitle (owner), DialogIcon.Question, formattedQuestion);

			dialog.OwnerWindow = owner;
			dialog.OpenDialog ();

			return dialog.Result;
		}
		
		protected static string GetIconUri(DialogIcon icon)
		{
			switch (icon)
			{
				case DialogIcon.Warning:
				case DialogIcon.Question:
					return string.Concat ("manifest:Epsitec.Common.Dialogs.Images.", icon, ".icon");
			}
			
			return null;
		}
		
		
		public class YesNoCancel : AbstractYesNoCancel
		{
			public YesNoCancel(string dialogTitle, string yesText, string noText, string cancelText, string messageIcon, string messageText, string commandYesTemplate, string commandNoTemplate, CommandDispatcher commandDispatcher) : base (dialogTitle, yesText, noText, cancelText, commandYesTemplate, commandNoTemplate, commandDispatcher)
			{
				this.messageIcon = messageIcon;
				this.messageText = messageText;
			}
			
			
			public override string[]				CommandArgs
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
				return Helpers.MessageBuilder.CreateIconAndText (this.messageIcon, this.messageText);
			}
			
			
			private string						messageIcon;
			private string						messageText;
		}
		
		public class OkCancel : AbstractOkCancel
		{
			public OkCancel(string dialogTitle, string okText, string cancelText, string messageIcon, string messageText, string commandTemplate, CommandDispatcher commandDispatcher) : base (dialogTitle, okText, cancelText, commandTemplate, commandDispatcher)
			{
				this.messageIcon = messageIcon;
				this.messageText = messageText;
			}
			
			
			public override string[]				CommandArgs
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
				return Helpers.MessageBuilder.CreateIconAndText (this.messageIcon, this.messageText);
			}
			
			
			private string						messageIcon;
			private string						messageText;
		}
	}
}
