using NUnit.Framework;

namespace Epsitec.Common.Script
{
	[TestFixture] public class AppDomainTest
	{
		[Test] public void CheckAppDomainCreation()
		{
			System.AppDomain domain = System.AppDomain.CreateDomain ("test domain");
			
			Assert.IsNotNull (domain);
		}
		
		[Test] public void CheckCompilerCreation()
		{
			Microsoft.CSharp.CSharpCodeProvider   provider = new Microsoft.CSharp.CSharpCodeProvider ();
			System.CodeDom.Compiler.ICodeCompiler compiler = provider.CreateCompiler ();
			
			Assert.IsNotNull (provider);
			Assert.IsNotNull (compiler);
		}
		
		[Test] public void CheckPaths()
		{
			System.Console.Out.WriteLine ("Base directory: {0}.", Helpers.Paths.BaseDirectory);
			System.Console.Out.WriteLine ("Dynamic Code directory: {0}.", Helpers.Paths.DynamicCodeDirectory);
		}
		
		[Test] public void TestEngineCompile()
		{
			string source =
				"namespace Epsitec.Dynamic.Script {\n" +
				"public class DynamicScript : System.MarshalByRefObject, Epsitec.Common.Script.Glue.IScript\n" +
				"{\n" +
				"public DynamicScript() { System.Diagnostics.Debug.WriteLine(\"Instanciated Epsitec.Dynamic.Script.DynamicScript\"); }\n" +
				"public void SetScriptHost(Epsitec.Common.Script.Glue.IScriptHost host) { System.Diagnostics.Debug.WriteLine(\"Host set to \" + host.Name); }\n" +
				"public bool Execute(string name, object[] in_args) { System.Diagnostics.Debug.WriteLine(\"Executing in Epsitec.Dynamic.Script.DynamicScript: \" + name); return true; }\n" +
				"}\n" +
				"}\n";
			
			Engine engine = new Engine ();
			Script script = engine.Compile (source);
			
			if (script.HasErrors)
			{
				foreach (string error in script.Errors)
				{
					System.Console.Out.WriteLine (error);
				}
			}
			
			script.Execute ("Hello.", null);
			script.Execute ("Good bye.", null);
			
			script.Dispose ();
		}
	}
}
