using NUnit.Framework;
using Epsitec.Common.UI.Data;
using Epsitec.Common.Script;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[SetUp] public void SetUp()
		{
			Epsitec.Common.UI.Engine.Initialise ();
			Epsitec.Common.Pictogram.Engine.Initialise ();
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
			
			Field field_a = new Field ("UserName", "Test");
			Field field_b = new Field ("UserAge", 10);
			Field field_c = new Field ("c", Representation.None);
			
			field_a.DefineCaption ("[res:file:strings#data.ValueA]");
			field_b.DefineCaption ("[res:file:strings#data.ValueB]");
			field_c.DefineCaption ("[res:file:strings#data.ValueC]");
			
			field_a.DefineConstraint (new Support.RegexConstraint (Support.PredefinedRegex.Alpha));
			
			field_a.Changed += new Support.EventHandler (this.HandleFieldChanged);
			field_b.Changed += new Support.EventHandler (this.HandleFieldChanged);
			field_c.Changed += new Support.EventHandler (this.HandleFieldChanged);
			
			record.Add (field_a);
			record.Add (field_b);
			record.Add (field_c);
			
			ScriptWrapper script = new ScriptWrapper ();
			
			script.Source = DialogTest.CreateSource (null);
			
			dispatcher.RegisterExtraDispatcher (script);
			
			Dialog dialog = new Dialog ();
			dialog.Load ("file:dialog_with_data");
			dialog.Data = record;
			dialog.ScriptWrapper = script;
			dialog.CommandDispatcher = dispatcher;
			dialog.ShowWindow ();
			
			
			object document = Editor.Engine.CreateDocument (script);
			Editor.Engine.ShowMethod (document, "Main");
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
		
		public static Types.IDataValue[] CreateValues(out Common.UI.Data.Record record)
		{
			record  = new Epsitec.Common.UI.Data.Record ();
			
			Types.IDataValue[]    values  = new Types.IDataValue[1];
			Common.UI.Data.Field  field_1 = new Epsitec.Common.UI.Data.Field ("UserName", "anonymous", new Types.StringType ());
			
			record.Add (field_1);
			
			values[0] = field_1;
			
			return values;
		}
	}
}
