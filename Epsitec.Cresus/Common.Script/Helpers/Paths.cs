//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Helpers
{
	/// <summary>
	/// Summary description for Paths.
	/// </summary>
	public sealed class Paths
	{
		private Paths()
		{
		}
		
		static Paths()
		{
			Paths.base_dir = System.AppDomain.CurrentDomain.BaseDirectory;
			Paths.dynamic_code_dir = System.IO.Directory.GetCurrentDirectory ();
		}
		
		
		public static string				BaseDirectory
		{
			get
			{
				return Paths.base_dir;
			}
		}
		
		public static string				DynamicCodeDirectory
		{
			get
			{
				return Paths.dynamic_code_dir;
			}
		}
		
		
		private static string				base_dir;
		private static string				dynamic_code_dir;
	}
}
