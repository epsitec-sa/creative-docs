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
			Library.UI.Services.SetApplication (this);
			
			this.plugIns = new List<PlugIns.ICorePlugIn> ();

			//	CoreData est initialisé dans la classe Factory.CoreData, dans CoreData.cs:563
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

			this.ProbeTemplateDatabase ();

			var initializers = new List<System.Action> ();

			this.CreateManualComponents (initializers);
			this.RegisterEventHandlers ();

			this.DiscoverPlugIns ();
			this.CreatePlugIns ();
			this.SetupData ();
			this.CreateUI (initializers);

			this.CommandContext.UpdateCommandStates (this);
		}

		private void ProbeTemplateDatabase()
		{
			var pool = this.Data.DataContextPool;
			var allUserGroups = this.UserManager.GetAllUserGroups ();

			if (allUserGroups.Any (x => pool.FindEntityKey (x).Value.IsTemplate))
			{
				CoreContext.DatabaseType = CoreDatabaseType.UserData;
			}
			else
			{
				CoreContext.DatabaseType = CoreDatabaseType.PureTemplateData;
			}
		}

		public void ShutdownApplication()
		{
			this.OnShutdownStarted ();
			this.DisposePlugIns ();
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		
		private void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
		}

		private void SetupData()
		{
			if (this.Data.ForceDatabaseCreation)
			{
				Hack.PopulateUsers (this.Data.CreateDataContext ("hack"));
			}

			this.OnSetupDataDone ();
		}

		
		private void DiscoverPlugIns()
		{
			this.plugInFactory = new PlugIns.PlugInFactory (this);
		}

		private void CreatePlugIns()
		{
			foreach (var attribute in this.plugInFactory.GetPlugInAttributeList ())
			{
				this.plugIns.Add (this.plugInFactory.CreatePlugIn (attribute.Name));
			}
		}

		private void DisposePlugIns()
		{
			this.plugIns.ForEach (x => x.Dispose ());
			this.plugIns.Clear ();
		}
		
		private void CreateManualComponents(IList<System.Action> initializers)
		{
			var orchestrator = new DataViewOrchestrator (this);

			initializers.Add (() => orchestrator.CreateUI (this.Window.Root));
		}

		private void RegisterEventHandlers()
		{
			this.UserManager.AuthenticatedUserChanged += this.HandleAuthenticatedUserChanged;
		}

		private void CreateUI(IEnumerable<System.Action> initializers)
		{
			this.OnCreatingUI ();
			
			this.CreateUIMainWindow ();
			this.CreateUIControllers (initializers);
			
			this.RestoreApplicationState ();
			
			this.OnCreatedUI ();
			this.OnApplicationReady ();
		}

		private void CreateUIMainWindow()
		{
			string path = System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "app.ico");

			this.Window =
				new Window
				{
					ClientSize = new Epsitec.Common.Drawing.Size (600, 400),
					Icon       = Epsitec.Common.Drawing.Bitmap.FromNativeIcon (path, 48, 48)
				};

			if (CoreContext.DatabaseType == CoreDatabaseType.PureTemplateData)
			{
				this.Window.Root.BackColor = Common.Drawing.Color.FromName ("Red");
			}

			this.Window.Root.SizeChanged +=
				delegate
				{
					this.UpdateWindowText ();
				};

			this.PersistenceManager.Register (this.Window);
			this.UpdateWindowText ();
		}

		private void UpdateWindowText()
		{
			var text = FormattedText.FromSimpleText (
				string.Format ("{0} Alpha {1}x{2}{3}",
					this.ShortWindowTitle,
					this.Window.ClientSize.Width, this.Window.ClientSize.Height,
					CoreContext.DatabaseType == CoreDatabaseType.PureTemplateData ? " - EPSITEC" : ""));

			this.Window.Text = text.ToString ();
		}

		private void CreateUIControllers(IEnumerable<System.Action> initializers)
		{
			initializers.ForEach (action => action ());
		}

		private void SaveApplicationState()
		{
			if (this.IsReady)
			{
//-				System.Diagnostics.Debug.WriteLine ("Saving application state.");
				System.DateTime now = System.DateTime.Now.ToUniversalTime ();
				string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

				XDocument doc = new XDocument (
					new XDeclaration ("1.0", "utf-8", "yes"),
					new XComment ("Saved on " + timeStamp),
					new XElement ("store",
					//-						this.StateManager.SaveStates ("stateManager"),
						Library.UI.Services.SaveWindowPositions ("windowPositions"),
						this.PersistenceManager.Save ("uiSettings"),
						this.SettingsManager.Save ("appSettings")));

				doc.Save (CoreApplication.Paths.SettingsPath);
//-				System.Diagnostics.Debug.WriteLine ("Save done.");
			}
		}

		private void RestoreApplicationState()
		{
			var persistenceManager = this.PersistenceManager;
			
			if (System.IO.File.Exists (CoreApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (CoreApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

//-				this.stateManager.RestoreStates (store.Element ("stateManager"));
				Library.UI.Services.RestoreWindowPositions (store.Element ("windowPositions"));
				persistenceManager.Restore (store.Element ("uiSettings"));
				this.SettingsManager.Restore (store.Element ("appSettings"));
			}
			
			persistenceManager.DiscardChanges ();
			persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();

//-			this.UpdateCommandsAfterStateChange ();
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

		private void OnApplicationReady()
		{
			this.IsReady = true;
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
		private PlugIns.PlugInFactory					plugInFactory;
	}
}
