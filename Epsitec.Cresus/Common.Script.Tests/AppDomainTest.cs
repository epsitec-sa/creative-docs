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
			System.Console.Out.WriteLine ("Dynamic code directory: {0}.", Helpers.Paths.DynamicCodeDirectory);
		}
		
		[Test] public void TestEngineCompile()
		{
			string source =
				"namespace Epsitec.Dynamic.Script {\n" +
				"public class DynamicScript : System.MarshalByRefObject, Epsitec.Common.Script.Glue.IScript\n" +
				"{\n" +
				"public DynamicScript() { System.Diagnostics.Debug.WriteLine(\"Instanciated Epsitec.Dynamic.Script.DynamicScript\"); }\n" +
				"public void SetScriptHost(Epsitec.Common.Script.Glue.IScriptHost host) { System.Diagnostics.Debug.WriteLine(\"Host set to \" + host.Name); }\n" +
				"public bool Execute(string name) { System.Diagnostics.Debug.WriteLine(\"Executing in Epsitec.Dynamic.Script.DynamicScript: \" + name); return true; }\n" +
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
			
			script.Execute ("Hello.");
			script.Execute ("Good bye.");
			
			script.Dispose ();
		}
		
		[Test] public void CheckGenerateAssembly()
		{
			System.CodeDom.Compiler.ICodeCompiler compiler = Helpers.CompilerFactory.CreateCompiler ();
			
			Assert.IsNotNull (compiler);
			
			string source =
				"namespace X {\n" +
				"public class T : System.MarshalByRefObject, Epsitec.Common.Script.Glue.IScript\n" +
				"{\n" +
				"public T() { System.Diagnostics.Debug.WriteLine(\"Instanciated X.T\"); }\n" +
				"public void SetScriptHost(Epsitec.Common.Script.Glue.IScriptHost host) { System.Diagnostics.Debug.WriteLine(\"Host set to \" + host.Name); }\n" +
				"public bool Execute(string name) { System.Diagnostics.Debug.WriteLine(\"Executing in X.T: \" + name); return true; }\n" +
				"}\n" +
				"}\n";
			
			System.CodeDom.Compiler.CompilerParameters options = Helpers.CompilerFactory.CreateCompilerParameters ("script_dynamic_x_0001");
			System.CodeDom.Compiler.CompilerResults    results = compiler.CompileAssemblyFromSource (options, source);
			
			foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
			{
				System.Console.Out.WriteLine ("{0}, {1}, {2}", error.FileName, error.Line, error.ErrorText);
			}
			
			Assert.IsFalse (results.Errors.HasErrors);
			
			string dynamic_path_name = options.OutputAssembly;
			string dynamic_full_name = results.CompiledAssembly.GetName ().FullName;
			
			System.AppDomain domain = Helpers.AppDomainFactory.CreateAppDomain ("test domain");
			
			Glue.IScript script = domain.CreateInstance (dynamic_full_name, "X.T").Unwrap () as Glue.IScript;
			
			System.IO.File.Delete (dynamic_path_name);
			
			script.Execute ("Hello.");
			script.Execute ("Good bye.");
			
			System.AppDomain.Unload (domain);
		}

		private void domain_AssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine ("Loading " + args.LoadedAssembly.FullName);
		}

		private System.Reflection.Assembly domain_AssemblyResolve(object sender, System.ResolveEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine ("Failed to resolve: " + args.Name);
			return null;
		}
	}
}
