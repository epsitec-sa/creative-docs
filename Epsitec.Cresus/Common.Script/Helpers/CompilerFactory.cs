//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Helpers
{
	using CodeDomProvider = System.CodeDom.Compiler.CodeDomProvider;
	
	/// <summary>
	/// Summary description for CompilerFactory.
	/// </summary>
	public sealed class CompilerFactory
	{
		private CompilerFactory()
		{
		}
		
		
		public static System.CodeDom.Compiler.ICodeCompiler CreateCompiler()
		{
			return CompilerFactory.provider.CreateCompiler ();
		}
		
		public static System.CodeDom.Compiler.CompilerParameters CreateCompilerParameters(string assembly_name)
		{
			System.Collections.ArrayList ref_list = new System.Collections.ArrayList ();
			
			string dll_path = Paths.BaseDirectory;
			string out_path = Paths.DynamicCodeDirectory;
			string out_name = string.Concat (out_path, System.IO.Path.DirectorySeparatorChar, assembly_name, ".dll");
			
			ref_list.Add ("System.dll");
			ref_list.Add (string.Concat (dll_path, System.IO.Path.DirectorySeparatorChar, "Common.Script.Glue.dll"));
			ref_list.Add (string.Concat (dll_path, System.IO.Path.DirectorySeparatorChar, "Common.Types.dll"));
			
			string[] ref_names = new string[ref_list.Count];
			ref_list.CopyTo (ref_names, 0);
			
			System.CodeDom.Compiler.CompilerParameters options = new System.CodeDom.Compiler.CompilerParameters (ref_names, out_name);
		
			options.GenerateExecutable      = false;
			options.GenerateInMemory        = false;
			
#if DEBUG
			options.IncludeDebugInformation = true;
#else
			options.IncludeDebugInformation = false;
#endif
			
			return options;
		}
		
		
		private static CodeDomProvider			provider = new Microsoft.CSharp.CSharpCodeProvider ();
	}
}
