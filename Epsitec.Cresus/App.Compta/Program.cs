//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Splash;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Compta
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
#if false
			if (args.Length > 0)
			{
				Program.ExecuteCoreProgram (args, null);
			}
			else
			{
				using (var splash = new SplashScreen ("logo.png"))
				{
					Program.ExecuteCoreProgram (args, splash);
				}
			}
#else
			Program.ExecuteCoreProgram (args, null);
#endif
		}


		static void ExecuteCoreProgram(string[] args, SplashScreen splash)
		{
			Epsitec.Cresus.Core.CoreProgram.Main (args);
//-			Epsitec.Cresus.Compta.Application.Start ("N");
		}
	}
}
