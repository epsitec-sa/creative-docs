using NUnit.Framework;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Data;
using Epsitec.Common.Script;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[SetUp] public void SetUp()
		{
			Epsitec.Common.Document.Engine.Initialise ();
			
			Support.Resources.DefaultManager.SetupApplication ("DialogTest");
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
		
		[Test] public void CheckLoad1Unknown()
		{
			Dialog dialog = new Dialog (Support.Resources.DefaultManager);
			dialog.Load ("unknown_dialog");
			
			Assert.IsTrue (dialog.IsLoaded);
			Assert.IsFalse (dialog.IsReady);
			
			dialog.OpenDialog ();
		}
		
		[Test] public void CheckLoad2SimpleTest()
		{
			Dialog dialog = new Dialog (Support.Resources.DefaultManager);
			dialog.Load ("test");
			dialog.OpenDialog ();
		}
		
		public enum AccessMode
		{
			[Types.Hide] None,
			
			Local, LAN, Internet
		}
		
		[System.Flags]
		public enum LoginOptions
		{
			[Types.Hide] None	= 0,
			RememberUser		= 1,
			AutoLogin			= 2,
		}
		
		[Test] public void CheckLoad3WithData()
		{
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookPastel");
			
			Dialog           dialog     = new Dialog (Support.Resources.DefaultManager, "dialog_with_data");
			DialogController controller = new DialogController (dialog);
			
			ObsoleteRecord record = new ObsoleteRecord ("Rec", "dialog_with_data_strings");
			
			record.AddField ("UserName", "Test", new Types.StringType (), new Support.RegexConstraint (Support.PredefinedRegex.Alpha));
			record.AddField ("UserAge",  10);
			record.AddField ("AccessMode", AccessMode.Local);
			record.AddField ("LoginOptions", LoginOptions.None);
			
			record.FieldChanged += new Support.EventHandler (this.HandleFieldChanged);
			
			dialog.AddRule (record.Validator, "Ok;Apply");
			dialog.AddController (controller);
			
			Assert.AreEqual ("Test", record["UserName"].Value);
			Assert.AreEqual (10, record["UserAge"].Value);
			Assert.AreEqual (AccessMode.Local, record["AccessMode"].Value);
			Assert.AreEqual (LoginOptions.None, record["LoginOptions"].Value);
			
//			ScriptWrapper script = new ScriptWrapper ();
//			script.Source = DialogTest.CreateSource (null);
//			dialog.CommandDispatcher.Register (script);
			
			dialog.Load ();
			dialog.Data = record;
//			dialog.Script = script;
//			dialog.IsModal = false;
			dialog.StoreInitialData ();
			
			//	Ouvre le dialogue modal (ce qui bloque !)
			
			dialog.OpenDialog ();
			
			
//			object document = Editor.Engine.CreateDocument (script);
//			Editor.Engine.ShowMethod (document, "Main");
		}
		
		private class DialogController
		{
			public DialogController(Dialog dialog)
			{
				this.dialog = dialog;
			}
			
			
			[Support.Command ("Ok")]		private void CommandOk()
			{
				System.Diagnostics.Debug.WriteLine ("OK executed.");
			}
			
			[Support.Command ("Cancel")]	private void CommandCancel()
			{
				this.dialog.RestoreInitialData ();
				System.Diagnostics.Debug.WriteLine ("Cancel executed.");
			}
			
			[Support.Command ("Apply")]		private void CommandApply()
			{
				this.dialog.StoreInitialData ();
				System.Diagnostics.Debug.WriteLine ("Apply executed.");
			}
		
			
			private Dialog					dialog;
		}
		
		private void HandleFieldChanged(object sender)
		{
			ObsoleteField field = sender as ObsoleteField;
			
			System.Diagnostics.Debug.WriteLine ("Field " + field.Name + " changed to " + field.Value.ToString () + (field.IsValueValid ? "" : "(invalid)"));
		}
		
		
		public static Source CreateSource(Types.IDataValue[] values)
		{
			Source.Method[]      methods = new Source.Method[2];
			Source.CodeSection[] code_1  = new Source.CodeSection[1];
			Source.CodeSection[] code_2  = new Source.CodeSection[1];
			Source.ParameterInfo[] par_2 = new Source.ParameterInfo[3];
			
			string code_1_source = "System.Diagnostics.Debug.WriteLine (&quot;Executing the &apos;Main&apos; script. UserName set to &apos;&quot; + this.UserName + &quot;&apos;.&quot;);<br/>";
			string code_2_source = "System.Diagnostics.Debug.WriteLine (&quot;Executing the &apos;Mysterious&apos; script. arg1=&quot; + arg1 + &quot;, arg2=&quot; + arg2);<br/>arg2 = arg2.ToUpper ();<br/>arg3 = arg1 * 2;<br/>this.UserName = arg2.ToLower ();<br/>";
			
			code_1[0]  = new Source.CodeSection (Source.CodeType.Local, code_1_source);
			code_2[0]  = new Source.CodeSection (Source.CodeType.Local, code_2_source);
			
			par_2[0] = new Source.ParameterInfo (Source.ParameterDirection.In, new Types.IntegerType (), "arg1");
			par_2[1] = new Source.ParameterInfo (Source.ParameterDirection.InOut, new Types.StringType (), "arg2");
			par_2[2] = new Source.ParameterInfo (Source.ParameterDirection.Out, new Types.IntegerType (), "arg3");
			
			methods[0] = new Source.Method ("Main", Types.VoidType.Default, null, code_1);
			methods[1] = new Source.Method ("Mysterious", Types.VoidType.Default, par_2, code_2);
			
			return new Source ("Hello", methods, values, "");
		}
		
		public static Types.IDataValue[] CreateValues(out Common.UI.Data.ObsoleteRecord record)
		{
			record  = new Epsitec.Common.UI.Data.ObsoleteRecord ();
			
			Types.IDataValue[]    values  = new Types.IDataValue[1];
			Common.UI.Data.ObsoleteField  field_1 = new Epsitec.Common.UI.Data.ObsoleteField ("UserName", "anonymous", new Types.StringType ());
			
			record.Add (field_1);
			
			values[0] = field_1;
			
			return values;
		}
	}
}
