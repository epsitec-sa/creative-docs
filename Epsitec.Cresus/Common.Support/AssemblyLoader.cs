//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			string file_name = string.Concat (name, ".dll");
			string full_name = System.IO.Path.Combine (Globals.Directories.Executable, file_name);
			
			return System.Reflection.Assembly.Load (full_name);
		}
	}
}
