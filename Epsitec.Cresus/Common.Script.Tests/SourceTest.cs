using NUnit.Framework;

namespace Epsitec.Common.Script
{
	[TestFixture] public class SourceTest
	{
		[Test] public void CheckSourceGeneration()
		{
			Source.Method[]      methods = new Source.Method[2];
			Types.IDataValue[]   values  = new Types.IDataValue[1];
			Source.CodeSection[] code_1  = new Source.CodeSection[1];
			Source.CodeSection[] code_2  = new Source.CodeSection[1];
			Source.ParameterInfo[] par_2 = new Source.ParameterInfo[3];
			
			string code_1_source = "System.Diagnostics.Debug.WriteLine (\"Executing the 'Main' script. UserName set to '\" + this.UserName + \"'.\");\n";
			string code_2_source = "System.Diagnostics.Debug.WriteLine (\"Executing the 'Mysterious' script. arg1=\" + arg1 + \", arg2=\" + arg2);\narg2 = arg2.ToUpper ();\narg3 = arg1 * 2;\nthis.UserName = arg2.ToLower ();\n";
			
			code_1[0]  = new Source.CodeSection (Source.CodeType.Local, code_1_source);
			code_2[0]  = new Source.CodeSection (Source.CodeType.Local, code_2_source);
			
			par_2[0] = new Source.ParameterInfo (Source.ParameterDirection.In, new Types.IntegerType (), "arg1");
			par_2[1] = new Source.ParameterInfo (Source.ParameterDirection.InOut, new Types.StringType (), "arg2");
			par_2[2] = new Source.ParameterInfo (Source.ParameterDirection.Out, new Types.IntegerType (), "arg3");
			
			methods[0] = new Source.Method ("Main", Types.VoidType.Default, null, code_1);
			methods[1] = new Source.Method ("Mysterious", Types.VoidType.Default, par_2, code_2);
			
			Common.UI.Data.Record record  = new Epsitec.Common.UI.Data.Record ();
			Common.UI.Data.Field  field_1 = new Epsitec.Common.UI.Data.Field ("UserName", "anonymous", new Types.StringType ());
			
			record.Add (field_1);
			
			values[0] = field_1;
			
			Source source = new Source ("Hello", methods, values, "");
			
			string script_source = source.GenerateAssemblySource ();
			
			System.Console.Out.WriteLine (script_source);
			
			Engine engine = new Engine ();
			Script script = engine.Compile (script_source);
			
			if (script.HasErrors)
			{
				foreach (string error in script.Errors)
				{
					System.Console.Out.WriteLine (error);
				}
			}
			
			object[] a_in  = { 12, "Hello" };
			object[] a_out;
			
			script.Attach (record);
			
			Assert.IsTrue (script.Execute ("Main"));
			Assert.IsTrue (script.Execute ("Mysterious", a_in, out a_out));
			Assert.IsFalse (script.Execute ("MissingMethod"));
			
			Assert.IsNotNull (a_out);
			Assert.AreEqual (2, a_out.Length);
			Assert.AreEqual ("HELLO", a_out[0]);
			Assert.AreEqual (24, a_out[1]);
			Assert.AreEqual ("hello", record["UserName"].Value);
			
			script.Dispose ();
		}
	}
}
