//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public abstract class CoreInteractiveApp : CoreApp
	{
		protected CoreInteractiveApp()
		{
			Library.UI.Services.SetApplication (this);

			this.plugIns = new List<PlugIns.ICorePlugIn> ();
		}

		public bool IsReady
		{
			get;
			private set;
		}

		public CoreData Data
		{
			get
			{
				return this.GetComponent<CoreData> ();
			}
		}

		public CoreCommandDispatcher Commands
		{
			get
			{
				return this.GetComponent<CoreCommandDispatcher> ();
			}
		}

		public UserManager UserManager
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

		public void ShutdownApplication()
		{
			this.OnShutdownStarted ();
			this.DisposePlugIns ();
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		protected abstract void InitializeEmptyDatabase();

		protected abstract void CreateManualComponents(IList<System.Action> initializers);

		protected abstract void SaveApplicationState(XDocument doc);

		protected abstract XDocument LoadApplicationState();


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

		private void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
		}

		private void SetupData()
		{
			if (this.Data.ForceDatabaseCreation)
			{
				this.InitializeEmptyDatabase ();
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
				System.DateTime now = System.DateTime.UtcNow;
				string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

				XDocument doc = new XDocument (
					new XDeclaration ("1.0", "utf-8", "yes"),
					new XComment ("Saved on " + timeStamp),
					new XElement ("store",
					//-						this.StateManager.SaveStates ("stateManager"),
						Library.UI.Services.SaveWindowPositions ("windowPositions"),
						this.PersistenceManager.Save ("uiSettings"),
						this.SettingsManager.Save ("appSettings")));

				this.SaveApplicationState (doc);
			}
		}

		private void RestoreApplicationState()
		{
			var persistenceManager = this.PersistenceManager;

			XDocument doc = this.LoadApplicationState ();

			if (doc != null)
			{
				XElement store = doc.Element ("store");

				Library.UI.Services.RestoreWindowPositions (store.Element ("windowPositions"));
				persistenceManager.Restore (store.Element ("uiSettings"));
				this.SettingsManager.Restore (store.Element ("appSettings"));
			}

			persistenceManager.DiscardChanges ();
			persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();
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
