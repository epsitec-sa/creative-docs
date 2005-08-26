//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe AssemblyLoader permet de charger une "assembly" d'après son nom.
	/// </summary>
	public sealed class AssemblyLoader
	{
		public static System.Reflection.Assembly Load(string name)
		{
			//	Charge l'assembly spécifiée par son nom court, sans extension. Le fichier DLL va être
			//	recherché dans le même dossier que "Common.Support" qui n'est pas nécessairement celui
			//	de l'exécutable (en tout cas quand NUnit est utilisé pour les tests).
			
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
