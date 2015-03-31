//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Splash;

namespace Epsitec.Cresus.Assets
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			using (var splash = new SplashScreen ("logo.png"))
			{
				var rootPath = System.IO.Path.GetDirectoryName (typeof (Program).Assembly.Location);
				System.IO.Directory.SetCurrentDirectory (rootPath);

				Epsitec.Cresus.Core.CoreProgram.Main (args);
			}
		}
	}
}
