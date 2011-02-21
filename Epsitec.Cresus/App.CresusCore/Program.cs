//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Splash;

using Epsitec.Cresus.Core;

namespace Epsitec.Cresus.Core
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			using (var splash = new SplashScreen ("logo.png"))
			{
				Program.ExecuteCoreProgram (splash);
			}
		}


		static void ExecuteCoreProgram(SplashScreen splash)
		{
		}
	}
}
