//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.CodeDom.Compiler;

namespace Epsitec.Common.Script
{
	/// <summary>
	/// Summary description for Engine.
	/// </summary>
	public class Engine
	{
		public Engine()
		{
		}
		
		public Script Compile(Source source)
		{
			return this.Compile (source.Name, source.GenerateAssemblySource ());
		}
		
		public Script Compile(string name, string source)
		{
			int build_id  = Engine.GetNextBuildId ();
			int random_id = Engine.GetRandomId ();
			
			string short_name = string.Format (System.Globalization.CultureInfo.InvariantCulture, "script_{0}_{1}", build_id, random_id);
			string long_name  = string.Concat (short_name, ":", name);
			
			CodeDomProvider    compiler = Helpers.CompilerFactory.CreateCompiler ();
			CompilerParameters options  = Helpers.CompilerFactory.CreateCompilerParameters (short_name);
			
			//	.NET 2.0 n'aime pas l'usage du CreateCompiler () et .NET 1.1 n'aime pas son absence. Pour
			//	que cela compile proprement, il faudrait ici une compilation conditionnelle :-(
			
			CompilerResults    results  = compiler.CreateCompiler ().CompileAssemblyFromSource (options, source);
			
			Script script = new Script ();
			
			if (results.Errors.HasErrors)
			{
				string[] errors = new string[results.Errors.Count];
				
				for (int i = 0; i < results.Errors.Count; i++)
				{
					errors[i] = string.Format ("{0}.{1}: {2}", results.Errors[i].Line, results.Errors[i].Column, results.Errors[i].ErrorText);
				}
				
				script.DefineErrors (errors);
				
				return script;
			}
			
			string dll_file_name = options.OutputAssembly;
			string pdb_file_name = System.IO.Path.GetDirectoryName (dll_file_name) + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension (dll_file_name) + ".pdb";
			string assembly_name = results.CompiledAssembly.FullName;
			
			System.AppDomain domain = Helpers.AppDomainFactory.CreateAppDomain (long_name);
			
			script.DefineDllFileName (dll_file_name);
			script.DefinePdbFileName (options.IncludeDebugInformation ? pdb_file_name : null);
			script.DefineAppDomain (domain);
			
			object       remote   = domain.CreateInstanceAndUnwrap (assembly_name, "Epsitec.Dynamic.Script.DynamicScript", null);
			Glue.IScript i_script = remote as Glue.IScript;
			
			script.DefineScript (i_script);
			
			return script;
		}
		
		
		private static int GetNextBuildId()
		{
			lock (typeof (Engine))
			{
				return ++Engine.next_build_id % 1000000;
			}
		}
		
		private static int GetRandomId()
		{
			lock (typeof (Engine))
			{
				return Engine.randomizer.Next (1000000);
			}
		}
		
		
		private static int						next_build_id;
		private static System.Random			randomizer = new System.Random ((int)(System.DateTime.UtcNow.Ticks % 1000000000L));
	}
}
