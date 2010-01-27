//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core;
using Epsitec.Common.Printing;

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
			
			using (Application application = new Application(args.Contains ("-admin")))
			{
				application.Window.Show ();
				application.DisplayPrintersManager (true);
				application.Window.Run ();
			}

			UI.ShutDown ();
		}

	}

}
