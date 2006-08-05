//	Copyright � 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe AssemblyLoader permet de charger une "assembly" d'apr�s son nom.
	/// </summary>
	public sealed class AssemblyLoader
	{
		public static System.Reflection.Assembly Load(string name)
		{
			//	Charge l'assembly sp�cifi�e par son nom court, sans extension. Le fichier DLL va �tre
			//	recherch� dans le m�me dossier que "Common.Support" qui n'est pas n�cessairement celui
			//	de l'ex�cutable (en tout cas quand NUnit est utilis� pour les tests).
			
			System.Reflection.Assembly current = System.Reflection.Assembly.GetAssembly (typeof (AssemblyLoader));
			
			string file_name = string.Concat (name, ".dll");
			string path_name = Globals.Directories.Executable;
			
			string load_name = current.CodeBase;
			
			if (load_name.StartsWith ("file:///"))
			{
				load_name = load_name.Substring (8);
				load_name = load_name.Replace ('/', System.IO.Path.DirectorySeparatorChar);
				load_name = System.IO.Path.GetDirectoryName (load_name);
				
				if (path_name != load_name)
				{
					if (AssemblyLoader.load_path_differences++ == 0)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Assembly load path different from executable path\n" +
							/**/										   "  DLL load path: '{0}'\n" +
							/**/										   "  EXE load path: '{1}'", load_name, path_name));
					}
					
					path_name = load_name;
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Support assembly not loaded from a file ? CodeBase = '{0}'", load_name));
			}
			
			string full_name = System.IO.Path.Combine (path_name, file_name);
			
			return System.Reflection.Assembly.LoadFrom (full_name);
		}
		
		
		private static int						load_path_differences = 0;
	}
}
