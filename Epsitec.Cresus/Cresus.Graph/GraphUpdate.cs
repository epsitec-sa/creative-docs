//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Graph
{
	class GraphUpdate
	{
		public static void CheckUpdate()
		{
			var checker = VersionChecker.CheckUpdate ("Cresus_Graphe", GraphUpdate.GetInstalledVersion ());

			checker.VersionInformationChanged +=
				delegate
				{
					GraphProgram.Application.NotifyNewerVersion (checker);
				};
		}

		public static string GetInstalledVersion()
		{
			var version = (string) Microsoft.Win32.Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus Graphe\Setup", "Version", "");
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ().FullName.Split (',')[1].Split ('=')[1];

			return version ?? assembly;
		}
	}
}
