using NUnit.Framework;
using Epsitec.Common.UI.Data;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[SetUp] public void SetUp()
		{
			Epsitec.Common.UI.Engine.Initialise ();
		}
		
		[Test] public void CheckLoadDesignerFactory()
		{
			Assert.IsTrue (Dialog.LoadDesignerFactory ());
		}
		
		[Test] public void CheckCreateDesigner()
		{
			IDialogDesigner designer = Dialog.CreateDesigner ();
			
			Assert.IsNotNull (designer);
		}
		
		[Test] public void CheckCreateEmptyDialog()
		{
			IDialogDesigner designer = Dialog.CreateDesigner ();
			Widgets.Window  window   = new Widgets.Window ();
			
			Assert.IsNotNull (designer);
			
			designer.DialogWindow = window;
			designer.StartDesign ();
		}
		
		[Test] public void CheckLoad1Unknwon()
		{
			Dialog dialog = new Dialog ();
			dialog.Load ("file:unknown_dialog");
			
			Assert.IsTrue (dialog.IsLoaded);
			Assert.IsFalse (dialog.IsReady);
			
			dialog.ShowWindow ();
		}
		
		[Test] public void CheckLoad2SimpleTest()
		{
			Dialog dialog = new Dialog ();
			dialog.Load ("file:test");
			dialog.ShowWindow ();
		}
		
		[Test] public void CheckLoad3WithData()
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive ("LookPastel");
			
			Support.CommandDispatcher dispatcher = new Support.CommandDispatcher ("CheckLoad3WithData");
			
			dispatcher.RegisterController (this);
			
			Record record = new Record ();
			
			Field field_a = new Field ("a", "Test");
			Field field_b = new Field ("b", 10);
			Field field_c = new Field ("c", Representation.None);
			
			field_a.DefineCaption ("[res:file:strings#data.ValueA]");
			field_b.DefineCaption ("[res:file:strings#data.ValueB]");
			field_c.DefineCaption ("[res:file:strings#data.ValueC]");
			
			field_a.Changed += new Support.EventHandler (this.HandleFieldChanged);
			field_b.Changed += new Support.EventHandler (this.HandleFieldChanged);
			field_c.Changed += new Support.EventHandler (this.HandleFieldChanged);
			
			record.Add (field_a);
			record.Add (field_b);
			record.Add (field_c);
			
			Dialog dialog = new Dialog ();
			dialog.Load ("file:dialog_with_data");
			dialog.Data = record;
			dialog.CommandDispatcher = dispatcher;
			dialog.ShowWindow ();
		}
		
		
		[Support.Command ("Ok")] private void CommandOk()
		{
			System.Diagnostics.Debug.WriteLine ("OK executed.");
		}
		
		[Support.Command ("Cancel")] private void CommandCancel()
		{
			System.Diagnostics.Debug.WriteLine ("Cancel executed.");
		}
		
		[Support.Command ("Apply")] private void CommandApply()
		{
			System.Diagnostics.Debug.WriteLine ("Apply executed.");
		}
		
		private void HandleFieldChanged(object sender)
		{
			Field field = sender as Field;
			System.Diagnostics.Debug.WriteLine ("Field " + field.Name + " changed to " + field.Value.ToString () + (field.IsValueValid ? "" : "(invalid)"));
		}
	}
}
