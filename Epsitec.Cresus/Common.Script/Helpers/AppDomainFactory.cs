//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Helpers
{
	/// <summary>
	/// Summary description for AppDomainFactory.
	/// </summary>
	public sealed class AppDomainFactory
	{
		private AppDomainFactory()
		{
		}
		
		
		public static System.AppDomain CreateAppDomain(string name)
		{
			System.AppDomain current = System.AppDomain.CurrentDomain;
			
			System.Security.Policy.Evidence evidence = new System.Security.Policy.Evidence (current.Evidence);
			System.AppDomainSetup           setup    = new System.AppDomainSetup ();
			
			//	On ne veut pas que le AppDomain provoque des "shadow copies", car cela risque de g�n�rer
			//	des tonnes de dossiers/fichiers dans "Documents and Settings\User\Application Data" !
			
			setup.ApplicationName = "Epsitec.Common.Scripts " + name;
			setup.ShadowCopyFiles = "false";
			setup.PrivateBinPath  = Paths.DynamicCodeDirectory;
			setup.ApplicationBase = Paths.BaseDirectory;
			
			return System.AppDomain.CreateDomain (name, evidence, setup);;
		}
	}
}
