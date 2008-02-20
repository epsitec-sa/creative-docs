//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

			CoreProgram.application = new CoreApplication ();

			System.Diagnostics.Debug.Assert (CoreProgram.application.ResourceManagerPool.PoolName == "Core");

			CoreProgram.application.SetupInterface ();
			CoreProgram.application.SetupData ();

			CoreProgram.application.Window.Show ();
			CoreProgram.application.Window.Run ();

			UI.ShutDown ();

			CoreProgram.application.Dispose ();
			CoreProgram.application = null;
		}

		public static CoreApplication Application
		{
			get
			{
				return CoreProgram.application;
			}
		}

		
		private static CoreApplication application;
	}
}
