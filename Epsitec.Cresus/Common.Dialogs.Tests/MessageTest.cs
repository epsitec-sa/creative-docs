using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	using CommandAttribute = Support.CommandAttribute;
	
	[TestFixture] public class MessageTest
	{
		[SetUp] public void Initialise()
		{
			Common.Widgets.Widget.Initialise ();
			Common.Pictogram.Engine.Initialise ();
			Common.Widgets.Adorner.Factory.SetActive ("LookMetal");
		}
		
		public MessageTest()
		{
			this.dispatcher = new Support.CommandDispatcher ("MessageTestDispatcher", true);
			this.dispatcher.RegisterController (this);
		}
		
		[Test] public void CheckYesNoCancel()
		{
			IDialog dialog = Message.CreateYesNoCancel ("Notepad", "manifest:Epsitec.Common.Dialogs.Images.Warning.icon", "The text in the <i>c:\\Documents and Settings\\Tester\\Desktop\\dummy file.txt</i> file has changed.<br/><br/>Do you want to save the changes ?", "ExecYes", "ExecNo", this.dispatcher);
			
			dialog.OpenDialog ();
		}

		[Test] public void CheckYesNo()
		{
			IDialog dialog = Message.CreateYesNo ("Notepad", "manifest:Epsitec.Common.Dialogs.Images.Warning.icon", "The text in the <i>c:\\Documents and Settings\\Tester\\Desktop\\dummy file.txt</i> file has changed.<br/><br/>Do you want to save the changes ?", "ExecYes", "ExecNo", this.dispatcher);
			
			dialog.OpenDialog ();
		}

		[Test] public void CheckOkCancel()
		{
			IDialog dialog = Message.CreateOkCancel ("Notepad", "manifest:Epsitec.Common.Dialogs.Images.Warning.icon", "About to mogrify this document...", "ExecYes", this.dispatcher);
			
			dialog.OpenDialog ();
		}

		[Test] public void CheckOk()
		{
			IDialog dialog = Message.CreateOk ("Notepad", "manifest:Epsitec.Common.Dialogs.Images.Warning.icon", "About to mogrify this document...", "ExecYes", this.dispatcher);
			
			dialog.OpenDialog ();
		}

		
		[Command ("ExecYes")] private void CommandExecYes()
		{
			System.Diagnostics.Debug.WriteLine ("Yes...");
		}
		
		[Command ("ExecNo")]  private void CommandExecNo()
		{
			System.Diagnostics.Debug.WriteLine ("No...");
		}
		
		
		private Support.CommandDispatcher		dispatcher;
	}
}
