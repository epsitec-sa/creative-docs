using NUnit.Framework;

namespace Epsitec.Common.Script
{
	[TestFixture] public class SourceTest
	{
		[Test] public void CheckSourceGeneration()
		{
			Source.Method[]      methods = new Source.Method[2];
			Types.IDataValue[]   values  = new Types.IDataValue[0];
			Source.CodeSection[] code_1  = new Source.CodeSection[1];
			Source.CodeSection[] code_2  = new Source.CodeSection[1];
			Source.ParameterInfo[] par_2 = new Source.ParameterInfo[2];
			
			string code_1_source = "System.Diagnostics.Debug.WriteLine (\"Executing the 'Main' script.\");\n";
			string code_2_source = "System.Diagnostics.Debug.WriteLine (\"Executing the 'Hello' script. arg1=\" + arg1 + \", arg2=\" + arg2);\n";
			
			code_1[0]  = new Source.CodeSection (Source.CodeLocation.Local, code_1_source);
			code_2[0]  = new Source.CodeSection (Source.CodeLocation.Local, code_2_source);
			
			par_2[0] = new Source.ParameterInfo (Source.ParameterDirection.In, new Types.IntegerType (), "arg1");
			par_2[1] = new Source.ParameterInfo (Source.ParameterDirection.In, new Types.StringType (), "arg2");
			
			methods[0] = new Source.Method ("Main", Types.VoidType.Default, null, code_1);
			methods[1] = new Source.Method ("Hello", Types.VoidType.Default, par_2, code_2);
			
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
			
			Assert.IsTrue (script.Execute ("Main", null));
			Assert.IsFalse (script.Execute ("MissingMethod", null));
			
			script.Dispose ();
		}
	}
}
