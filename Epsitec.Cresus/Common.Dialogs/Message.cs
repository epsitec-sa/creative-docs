//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class Message
	{
		public static IDialog CreateYesNoCancel(string dialog_title, string yes_text, string no_text, string cancel_text, string message_icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			return new Message.YesNoCancel (dialog_title, yes_text, no_text, cancel_text, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
		}

		public static IDialog CreateYesNoCancel(string dialog_title, string message_icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			return new Message.YesNoCancel (dialog_title, null, null, null, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
		}

		public static IDialog CreateYesNoCancel(string dialog_title, DialogIcon icon, string message_text)
		{
			return Message.CreateYesNoCancel (dialog_title, icon, message_text, null, null, null);
		}

		public static IDialog CreateYesNoCancel(string dialog_title, DialogIcon icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			string message_icon = Message.GetIconName (icon);
			
			return Message.CreateYesNoCancel (dialog_title, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
		}
		
		public static IDialog CreateYesNo(string dialog_title, string yes_text, string no_text, string cancel_text, string message_icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			AbstractMessageDialog dialog = new Message.YesNoCancel (dialog_title, yes_text, no_text, cancel_text, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateYesNo(string dialog_title, string message_icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			AbstractMessageDialog dialog = new Message.YesNoCancel (dialog_title, null, null, null, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateYesNo(string dialog_title, DialogIcon icon, string message_text)
		{
			return Message.CreateYesNo (dialog_title, icon, message_text, null, null, null);
		}

		public static IDialog CreateYesNo(string dialog_title, DialogIcon icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher)
		{
			string message_icon = Message.GetIconName (icon);
			
			return Message.CreateYesNo (dialog_title, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
		}
		
		public static IDialog CreateOkCancel(string dialog_title, string ok_text, string cancel_text, string message_icon, string message_text, string command_ok_template, CommandDispatcher command_dispatcher)
		{
			return new Message.OkCancel (dialog_title, ok_text, cancel_text, message_icon, message_text, command_ok_template, command_dispatcher);
		}

		public static IDialog CreateOkCancel(string dialog_title, string message_icon, string message_text, string command_ok_template, CommandDispatcher command_dispatcher)
		{
			return new Message.OkCancel (dialog_title, null, null, message_icon, message_text, command_ok_template, command_dispatcher);
		}

		public static IDialog CreateOkCancel(string dialog_title, DialogIcon icon, string message_text)
		{
			return Message.CreateOkCancel (dialog_title, icon, message_text, null, null);
		}

		public static IDialog CreateOkCancel(string dialog_title, DialogIcon icon, string message_text, string command_ok_template, CommandDispatcher command_dispatcher)
		{
			string message_icon = Message.GetIconName (icon);
			
			return Message.CreateOkCancel (dialog_title, message_icon, message_text, command_ok_template, command_dispatcher);
		}
		
		public static IDialog CreateOk(string dialog_title, string ok_text, string cancel_text, string message_icon, string message_text, string command_ok_template, CommandDispatcher command_dispatcher)
		{
			AbstractMessageDialog dialog = new Message.OkCancel (dialog_title, ok_text, cancel_text, message_icon, message_text, command_ok_template, command_dispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateOk(string dialog_title, string message_icon, string message_text, string command_ok_template, CommandDispatcher command_dispatcher)
		{
			AbstractMessageDialog dialog = new Message.OkCancel (dialog_title, null, null, message_icon, message_text, command_ok_template, command_dispatcher);
			dialog.HideCancelButton ();
			return dialog;
		}

		public static IDialog CreateOk(string dialog_title, DialogIcon icon, string message_text)
		{
			return Message.CreateOk (dialog_title, icon, message_text, null, null);
		}
		
		public static IDialog CreateOk(string dialog_title, DialogIcon icon, string message_text, string command_ok_template, CommandDispatcher command_dispatcher)
		{
			string message_icon = Message.GetIconName (icon);
			
			return Message.CreateOk (dialog_title, message_icon, message_text, command_ok_template, command_dispatcher);
		}

		public static IDialog CreateConfirmation(string title, string header, List<string> questions)
		{
			ConfirmationDialog dialog = new ConfirmationDialog(title, header, questions);
			return dialog;
		}

		public static string FormatMessage(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}

			text = TextLayout.ConvertToTaggedText (text);
			text = text.Replace ("\n", "<br/>");
			text = text.Replace ("\r", "");

			return text;
		}

		public static string GetDialogTitle(Window owner)
		{
			Application application = Widgets.Helpers.VisualTree.GetApplication (owner);
			
			if (application == null)
			{
				return Res.Strings.Dialog.Generic.Title;
			}
			else
			{
				return application.ShortWindowTitle;
			}
		}

		public static DialogResult ShowError(string formattedErrorMessage, Window owner)
		{
			if (string.IsNullOrEmpty (formattedErrorMessage))
			{
				return DialogResult.None;
			}

			IDialog dialog = Message.CreateOk (Message.GetDialogTitle (owner), DialogIcon.Warning, formattedErrorMessage);

			dialog.Owner = owner;
			dialog.OpenDialog ();

			return dialog.Result;
		}

		public static DialogResult ShowQuestion(string formattedQuestion, Window owner)
		{
			if (string.IsNullOrEmpty (formattedQuestion))
			{
				return DialogResult.None;
			}

			IDialog dialog = Message.CreateYesNo (Message.GetDialogTitle (owner), DialogIcon.Question, formattedQuestion);

			dialog.Owner = owner;
			dialog.OpenDialog ();

			return dialog.Result;
		}
		
		protected static string GetIconName(DialogIcon icon)
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
			public YesNoCancel(string dialog_title, string yes_text, string no_text, string cancel_text, string message_icon, string message_text, string command_yes_template, string command_no_template, CommandDispatcher command_dispatcher) : base (dialog_title, yes_text, no_text, cancel_text, command_yes_template, command_no_template, command_dispatcher)
			{
				this.message_icon = message_icon;
				this.message_text = message_text;
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
				return Helpers.MessageBuilder.CreateIconAndText (this.message_icon, this.message_text);
			}
			
			
			private string						message_icon;
			private string						message_text;
		}
		
		public class OkCancel : AbstractOkCancel
		{
			public OkCancel(string dialog_title, string ok_text, string cancel_text, string message_icon, string message_text, string command_template, CommandDispatcher command_dispatcher) : base (dialog_title, ok_text, cancel_text, command_template, command_dispatcher)
			{
				this.message_icon = message_icon;
				this.message_text = message_text;
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
				return Helpers.MessageBuilder.CreateIconAndText (this.message_icon, this.message_text);
			}
			
			
			private string						message_icon;
			private string						message_text;
		}
	}
}
