//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Assets
{
	static class AssetsProgram
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			AssetsProgram.ExecuteCoreProgram ();
		}


		static void ExecuteCoreProgram()
		{
			Epsitec.Common.Debug.GeneralExceptionCatcher.Setup ();

			UI.Initialize ();

			//Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookBlue");

			AssetsProgram.Application = new AssetsApplication ();

			System.Diagnostics.Debug.Assert (AssetsProgram.Application.ResourceManagerPool.PoolName == "Core");

//			AssetsProgram.Application.SetupUI ();
//			AssetsProgram.Application.SetupDefaultDocument ();

//			splash.NotifyIsRunning ();

//			AssetsProgram.Application.Window.MakeActive ();

//			AssetsUpdate.CheckUpdate ();
//			AssetsSerial.CheckLicense (AssetsProgram.Application.Window);

//			AssetsProgram.Application.ProcessCommandLine ();

//			AssetsProgram.Application.Window.Show ();
//			AssetsProgram.Application.Window.Run ();

			UI.ShutDown ();

			AssetsProgram.Application.Dispose ();
			AssetsProgram.Application = null;
		}


		public static AssetsApplication Application
		{
			get;
			private set;
		}
	}
}
