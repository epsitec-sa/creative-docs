//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.CoreLibrary;
using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreApplication</c> class implements the central application
	/// logic.
	/// </summary>
	public partial class CoreApplication : Application
	{
		public CoreApplication()
		{
			this.controllers = new List<CoreController> ();
			this.persistenceManager = new PersistenceManager ();

			this.data = new CoreData ();
			this.exceptionManager = new ExceptionManager ();
			this.commands = new CoreCommands (this);
		}


		public bool								IsReady
		{
			get;
			private set;
		}

		public CoreData							Data
		{
			get
			{
				return this.data;
			}
		}

		public IExceptionManager				ExceptionManager
		{
			get
			{
				return this.exceptionManager;
			}
		}

		public PersistenceManager				PersistanceManager
		{
			get
			{
				return this.persistenceManager;
			}
		}
		
		public override string					ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName.ToSimpleText ();
			}
		}

		public override string					ApplicationIdentifier
		{
			get
			{
				return "EpCresusCore";		//	TODO: Res.Strings.ProductAppId.ToSimpleText ();
			}
		}


		internal void CreateUI()
		{
			this.CreateUIMainWindow ();
			this.CreateUIRootBoxes ();
			this.CreateUIControllers ();
			
			this.RestoreApplicationState ();

			this.IsReady = true;
		}

		internal void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
		}

		internal void SetupData()
		{
			var entities = this.data.GetSamplePersons ();

			foreach (var entity in entities)
			{
				System.Diagnostics.Debug.WriteLine (entity.Dump ());
				System.Diagnostics.Debug.WriteLine ("---------------------------------------------------");
			}

			//?this.data.SetupDatabase ();
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.controllers.ForEach (controller => controller.Dispose ());
				this.controllers.Clear ();

				if (this.data != null)
				{
					this.data.Dispose ();
					this.data = null;
				}
				if (this.exceptionManager != null)
				{
					this.exceptionManager.Dispose ();
					this.exceptionManager = null;
				}
			}

			base.Dispose (disposing);
		}


		private void CreateUIMainWindow()
		{
			Window window = new Window
			{
				Text = this.ShortWindowTitle,
				ClientSize = new Epsitec.Common.Drawing.Size (600, 400)
			};

			this.Window = window;
		}

		private void CreateUIRootBoxes()
		{
			this.ribbonBox = new FrameBox ()
			{
				Parent = this.Window.Root,
				Name = "RibbonBox",
				Dock = DockStyle.Top,
			};

			this.contentBox = new FrameBox ()
			{
				Parent = this.Window.Root,
				Name = "ContentBox",
				Dock = DockStyle.Fill,
			};
		}

		private void CreateUIControllers()
		{
			var ribbonController   = new RibbonViewController ();

			var entities = new List<AbstractEntity> (this.data.GetSamplePersons ());
			var roles = new List<Entities.ContactRoleEntity> (this.data.GetSampleRoles ());
			var locations = new List<Entities.LocationEntity> (this.data.GetSampleLocations ());
			var countries = new List<Entities.CountryEntity> (this.data.GetSampleCountries ());
			var mainViewController = new MainViewController (entities, roles, locations, countries);

			this.controllers.Add (ribbonController);
			this.controllers.Add (mainViewController);

			ribbonController.CreateUI (this.ribbonBox);
			mainViewController.CreateUI (this.contentBox);
		}

		private void RestoreApplicationState()
		{
			if (System.IO.File.Exists (CoreApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (CoreApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

//-				this.stateManager.RestoreStates (store.Element ("stateManager"));
				UI.RestoreWindowPositions (store.Element ("windowPositions"));
				this.persistenceManager.Restore (store.Element ("uiSettings"));
			}
			
			this.persistenceManager.DiscardChanges ();
			this.persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();

//-			this.UpdateCommandsAfterStateChange ();
		}

		private void SaveApplicationState()
		{
			if (this.IsReady)
			{
				System.Diagnostics.Debug.WriteLine ("Saving application state.");
				System.DateTime now = System.DateTime.Now.ToUniversalTime ();
				string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

				XDocument doc = new XDocument (
					new XDeclaration ("1.0", "utf-8", "yes"),
					new XComment ("Saved on " + timeStamp),
					new XElement ("store",
//-						this.StateManager.SaveStates ("stateManager"),
						UI.SaveWindowPositions ("windowPositions"),
						this.persistenceManager.Save ("uiSettings")));

				doc.Save (CoreApplication.Paths.SettingsPath);
				System.Diagnostics.Debug.WriteLine ("Save done.");
			}
		}


		private PersistenceManager						persistenceManager;
		private CoreData								data;
		private ExceptionManager						exceptionManager;
		private CoreCommands							commands;
		private readonly List<CoreController>			controllers;

		private FrameBox								ribbonBox;
		private FrameBox								contentBox;
	}
}
