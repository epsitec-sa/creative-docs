﻿//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	using FormResourceAccessor=Epsitec.Common.Support.ResourceAccessors.FormResourceAccessor;

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

			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
		}

		public static void ShutDown()
		{
			Epsitec.Common.Drawing.ImageManager.ShutDownDefaultCache ();
		}


		public static Panel LoadPanel(Druid id, PanelInteractionMode mode)
		{
			ResourceManager manager = CoreProgram.Application.ResourceManager;
			ResourceBundle  bundle  = manager.GetBundle (id);
			
			switch (bundle.Type)
			{
				case Resources.PanelTypeName:
					return UI.CreateUserInterfaceFromPanel (bundle, mode);

				case Resources.FormTypeName:
					return UI.CreateUserInterfaceFromForm (bundle, mode);

				default:
					return null;
			}
		}

		private static Panel CreateUserInterfaceFromForm(ResourceBundle bundle, PanelInteractionMode mode)
		{
			string xmlSource = bundle[FormResourceAccessor.Strings.XmlSource].AsString;
			Size size = FormResourceAccessor.GetFormDefaultSize (bundle);

			Epsitec.Common.FormEngine.FormDescription formDescription = new Epsitec.Common.FormEngine.FormDescription ();
			Epsitec.Common.FormEngine.IFormResourceProvider provider = new Epsitec.Common.FormEngine.DefaultResourceProvider (bundle.ResourceManager);
			Epsitec.Common.FormEngine.Engine formEngine = new Epsitec.Common.FormEngine.Engine (provider);

			switch (mode)
			{
				case PanelInteractionMode.Search:
					formEngine.EnableSearchMode ();
					break;
			}

			formDescription.Deserialize (xmlSource);

			Panel panel = formEngine.CreateForm (formDescription);

			//formDescription.EntityId;

			return panel;
		}

		private static Panel CreateUserInterfaceFromPanel(ResourceBundle bundle, PanelInteractionMode mode)
		{
			throw new System.NotImplementedException ();
		}
	}
}
