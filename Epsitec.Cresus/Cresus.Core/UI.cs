//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>UI</c> static class provides central support for user interface
	/// related tasks.
	/// </summary>
	public static class UI
	{
		/// <summary>
		/// Initializes everything on application startup so that the user
		/// interface related frameworks are all properly configured.
		/// </summary>
		public static void Initialize()
		{
			//	Create a default resource manager pool, used for the UI and
			//	the application.
			
			ResourceManagerPool pool = new ResourceManagerPool ("Core");

			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths ();
			pool.ScanForAllModules ();

			ResourceManagerPool.Default = pool;

			//	Set up the fonts, the widgets, the icon rendering engine, etc.
			
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
		}
	}
}
