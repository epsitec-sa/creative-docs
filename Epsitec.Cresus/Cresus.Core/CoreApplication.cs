//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreApplication</c> class implements the central application
	/// logic.
	/// </summary>
	public partial class CoreApplication : CoreApp
	{
		public CoreApplication()
		{
			CoreProgram.Application = this;
			UI.SetApplication (this);
			
			this.plugIns = new List<PlugIns.ICorePlugIn> ();

			//	CoreData est initialisé dans la classe Factory.CoreData, dans CoreData.cs:563

			this.CreateManualComponents ();

			this.UserManager.AuthenticatedUserChanged += this.HandleAuthenticatedUserChanged;
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
				return this.GetComponent<CoreData> ();
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
				return Res.Strings.ProductAppId.ToSimpleText ();
			}
		}

		public CoreCommandDispatcher			Commands
		{
			get
			{
				return this.GetComponent<CoreCommandDispatcher> ();
			}
		}

		public UserManager						UserManager
		{
			get
			{
				return this.Data.GetComponent<UserManager> ();
			}
		}

		
		public override void SetupApplication()
		{
			base.SetupApplication ();

			this.DiscoverPlugIns ();
			this.CreatePlugIns ();
			this.SetupData ();
			this.CreateUI ();
		}

		internal void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
		}

		internal void SetupData()
		{
			if (this.Data.ForceDatabaseCreation)
			{
				Hack.PopulateUsers (this.Data.CreateDataContext ("hack"));
			}

			this.OnSetupDataDone ();
		}

		internal void DiscoverPlugIns()
		{
			this.plugInFactory = new PlugIns.PlugInFactory (this);
		}

		internal void CreatePlugIns()
		{
			foreach (var attribute in this.plugInFactory.GetPlugInAttributeList ())
			{
				this.plugIns.Add (this.plugInFactory.CreatePlugIn (attribute.Name));
			}
		}

		internal void Shutdown()
		{
			this.OnShutdownStarted ();

			this.plugIns.ForEach (x => x.Dispose ());
			this.plugIns.Clear ();
		}

		
		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}


		private void CreateManualComponents()
		{
			new DataViewOrchestrator (this);
		}

		private void CreateUI()
		{
			this.OnCreatingUI ();
			this.CreateUIMainWindow ();
			this.CreateUIControllers ();
			this.RestoreApplicationState ();
			this.OnCreatedUI ();

			this.IsReady = true;
		}

		private void CreateUIMainWindow()
		{
			string path = System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "app.ico");
			
			Window window = new Window
			{
				Text = this.ShortWindowTitle,
//-				Name = "Application",
				ClientSize = new Epsitec.Common.Drawing.Size (600, 400),
				Icon = Epsitec.Common.Drawing.Bitmap.FromNativeIcon (path, 48, 48)
			};

			this.Window = window;
			this.Window.Root.SizeChanged +=
				delegate
				{
					this.Window.Text = string.Format ("{0} Alpha {1}x{2}", this.ShortWindowTitle, this.Window.ClientSize.Width, this.Window.ClientSize.Height);
				};

			this.PersistenceManager.Register (this.Window);
		}

		private void CreateUIControllers()
		{
			var orchestrator = this.FindComponent<DataViewOrchestrator> ();
			var mainWindowController = orchestrator.MainWindowController;
			
			mainWindowController.CreateUI (this.Window.Root);
		}

		private void RestoreApplicationState()
		{
			var persistenceManager = this.PersistenceManager;
			
			if (System.IO.File.Exists (CoreApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (CoreApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

//-				this.stateManager.RestoreStates (store.Element ("stateManager"));
				UI.RestoreWindowPositions (store.Element ("windowPositions"));
				persistenceManager.Restore (store.Element ("uiSettings"));
				this.SettingsManager.Restore (store.Element ("appSettings"));
			}
			
			persistenceManager.DiscardChanges ();
			persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();

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
						this.PersistenceManager.Save ("uiSettings"),
						this.SettingsManager.Save ("appSettings")));

				doc.Save (CoreApplication.Paths.SettingsPath);
				System.Diagnostics.Debug.WriteLine ("Save done.");
			}
		}



		private void HandleAuthenticatedUserChanged(object sender)
		{
			this.Data.SetActiveUser (this.UserManager.AuthenticatedUser);
			this.Data.ConnectionManager.ReopenConnection ();
		}

		
		private void OnSetupDataDone()
		{
			var handler = this.SetupDataDone;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnCreatingUI()
		{
			var handler = this.CreatingUI;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnCreatedUI()
		{
			var handler = this.CreatedUI;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		private void OnShutdownStarted()
		{
			var handler = this.ShutdownStarted;
			
			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler						SetupDataDone;
		public event EventHandler						CreatingUI;
		public event EventHandler						CreatedUI;
		public event EventHandler						ShutdownStarted;


		private readonly List<PlugIns.ICorePlugIn>		plugIns;

		private readonly CoreData						data;
		private PlugIns.PlugInFactory					plugInFactory;
	}
}
