//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Compta
{
	public class ComptaApplication : CoreInteractiveApp
	{
		public ComptaApplication()
		{
		}

		public override string					ShortWindowTitle
		{
			get
			{
				return "Crésus MCH-2";
			}
		}
		public override string					ApplicationIdentifier
		{
			get
			{
				return "Cr.MCH-2";
			}
		}

		public override bool StartupLogin()
		{
			return true;
		}

		protected override Window CreateWindow()
		{
			var window = base.CreateWindow ();
			
			window.MakeTitlelessResizableWindow ();
			
			return window;
		}

		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.ExecuteQuit (dispatcher, e);
		}

		protected override CoreAppPolicy CreateDefaultPolicy()
		{
			var policy = base.CreateDefaultPolicy ();

			policy.RequiresCoreCommandHandlers = false;

			return policy;
		}

		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
			initializers.Add (this.InitializeApplication);
		}

		protected override void InitializeEmptyDatabase()
		{
			var businessContext = new BusinessContext (this.Data);
			
			Hack.PopulateUsers (businessContext);

			var compta = businessContext.CreateAndRegisterEntity<ComptaEntity> ();

			businessContext.SaveChanges (LockingPolicy.ReleaseLock);
		}

		protected override System.Xml.Linq.XDocument LoadApplicationState()
		{
			if (System.IO.File.Exists (ComptaApplication.Paths.SettingsPath))
			{
				return XDocument.Load (ComptaApplication.Paths.SettingsPath);
			}
			else
			{
				return null;
			}
		}

		protected override void SaveApplicationState(System.Xml.Linq.XDocument doc)
		{
			doc.Save (ComptaApplication.Paths.SettingsPath);
		}

		private void InitializeApplication()
		{
			this.businessContext = new BusinessContext (this.Data);
			var compta = this.Data.GetRepository<ComptaEntity> ().GetAllEntities ().FirstOrDefault ();

			var window = this.Window;
			//var controller = new DocumentWindowController (this, new List<AbstractController> (), this.businessContext, compta, TypeDeDocumentComptable.Journal);
			//controller.SetupApplicationWindow (window);
			window.Root.BackColor = Common.Drawing.Color.FromName ("White");
//-			window.Root.MinSize = new Size (640, 480);
#if false
#if false
			window.WindowBounds = new Rectangle (100, 50, 880, 600);
#else
			window.WindowBounds = new Rectangle (2560+100, 1600-600-100, 880, 600);  // coin sup/gauche de mon 2ème écran 2560x1600 !!!
#endif
#endif
			this.windowController = new MainWindowController (this);
			this.windowController.CreateUI (window);

			window.Show ();
			window.MakeActive ();
		}

		internal static class Paths
		{
			public static readonly string SettingsPath = System.IO.Path.Combine (Globals.Directories.UserAppData, "Cresus NG settings.xml");
		}
		
		private BusinessContext					businessContext;
		private MainWindowController			windowController;
	}
}
