//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	static class CoreProgram
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			UI.Initialize ();

			CoreApplication application = new CoreApplication ();

			System.Diagnostics.Debug.Assert (application.ResourceManagerPool.PoolName == "Core");

			application.SetupInterface ();
			application.SetupData ();

			application.Window.Show ();
			application.Window.Run ();

			UI.ShutDown ();
		}
	}
}
