//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class Message
	{
		public static IDialog CreateYesNoCancel(string dialog_title, string message_icon, string message_text, string command_yes_template, string command_no_template, Support.CommandDispatcher command_dispatcher)
		{
			return new Message.YesNoCancel (dialog_title, message_icon, message_text, command_yes_template, command_no_template, command_dispatcher);
		}
		
		public static IDialog CreateOkCancel(string dialog_title, string message_icon, string message_text, string command_ok_template, Support.CommandDispatcher command_dispatcher)
		{
			return new Message.OkCancel (dialog_title, message_icon, message_text, command_ok_template, command_dispatcher);
		}
		
		
		
		public class YesNoCancel : AbstractYesNoCancel
		{
			public YesNoCancel(string dialog_title, string message_icon, string message_text, string command_yes_template, string command_no_template, Support.CommandDispatcher command_dispatcher) : base (dialog_title, command_yes_template, command_no_template, command_dispatcher)
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
			
			
			protected override Widgets.Widget CreateBodyWidget()
			{
				return Helpers.MessageBuilder.CreateIconAndText (this.message_icon, this.message_text);
			}
			
			
			private string						message_icon;
			private string						message_text;
		}
		
		public class OkCancel : AbstractOkCancel
		{
			public OkCancel(string dialog_title, string message_icon, string message_text, string command_template, Support.CommandDispatcher command_dispatcher) : base (dialog_title, command_template, command_dispatcher)
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
			
			
			protected override Widgets.Widget CreateBodyWidget()
			{
				return Helpers.MessageBuilder.CreateIconAndText (this.message_icon, this.message_text);
			}
			
			
			private string						message_icon;
			private string						message_text;
		}
	}
}
