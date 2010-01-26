//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.App.BanquePiguet
{

	static class Program
	{

		[System.STAThread]
		static void Main(string[] args)
		{
			UI.Initialize ();

			bool adminMode = args.Any (arg => arg == "-admin");

			using (Application application = new Application(adminMode))
			{
				application.Window.Show ();
				application.Window.Run ();
			}

			UI.ShutDown ();
		}

	}

}
