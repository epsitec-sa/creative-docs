//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Entities;

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
			CoreProgram.Application = this;
			this.persistenceManager = new PersistenceManager ();

			this.data = new CoreData (forceDatabaseCreation: true);

			this.exceptionManager = new ExceptionManager ();
			this.commands = new CoreCommandDispatcher (this);

			this.mainWindowOrchestrator = new DataViewOrchestrator (this.data, this.CommandContext);
			this.mainWindowController = new MainWindowController (this.data, this.CommandContext, this.mainWindowOrchestrator);

			this.attachedDialogs = new List<Dialogs.IAttachedDialog> ();

			this.userManager = new Business.UserManagement.UserManager (this.data);
			this.userManager.AuthenticatedUserChanged += sender => this.data.ConnectionManager.ReopenConnection ();
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

		public BusinessSettingsEntity			BusinessSettings
		{
			get
			{
				if (this.businessSettings == null)
				{
					this.businessSettings = this.data.GetAllEntities<BusinessSettingsEntity> ().First ();
				}

				return this.businessSettings;
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

		public CoreCommandDispatcher			Commands
		{
			get
			{
				return this.commands;
			}
		}

		public MainWindowController				MainWindowController
		{
			get
			{
				return this.mainWindowController;
			}
		}

		public DataViewOrchestrator				MainWindowOrchestrator
		{
			get
			{
				return this.mainWindowOrchestrator;
			}
		}

		public List<Dialogs.IAttachedDialog>	AttachedDialogs
		{
			get
			{
				return this.attachedDialogs;
			}
		}

		public Business.UserManagement.UserManager UserManager
		{
			get
			{
				return this.userManager;
			}
		}

		public void AttachDialog(Dialogs.IAttachedDialog dialog)
		{
			if (!this.attachedDialogs.Contains (dialog))
			{
				this.attachedDialogs.Add (dialog);
			}
		}

		public void DetachDialog(Dialogs.IAttachedDialog dialog)
		{
			if (this.attachedDialogs.Contains (dialog))
			{
				this.attachedDialogs.Remove (dialog);
			}
		}


		public static T GetController<T>(CommandContext context)
			where T : CoreController
		{
			var root = CoreProgram.Application.MainWindowController;

			if (root is T)
			{
				return root as T;
			}

			return root.GetAllSubControllers ().Where (item => item is T).FirstOrDefault () as T;
		}


		#region Settings
		public static string GetSettings(string key)
		{
			//	Donne le contenu d'un réglage global.
			if (CoreApplication.settings.ContainsKey (key))
			{
				return CoreApplication.settings[key];
			}

			return null;
		}

		public static void SetSettings(string key, string value)
		{
			//	Modifie un réglage global.
			CoreApplication.settings[key] = value;
		}

		public static Dictionary<string, string> ExtractSettings(string startKey)
		{
			//	Extrait tous les réaglages d'une catégorie donnée.
			Dictionary<string, string> dict = new Dictionary<string, string> ();

			foreach (var pair in CoreApplication.settings)
			{
				if (pair.Key.StartsWith (startKey))
				{
					dict.Add (pair.Key, pair.Value);
				}
			}

			return dict;
		}

		public static void MergeSettings(string startKey, Dictionary<string, string> dict)
		{
			//	Met à jour tous les réaglages d'une catégorie donnée.
			var keys = new List<string> ();
			foreach (var key in CoreApplication.settings.Keys)
			{
				keys.Add (key);
			}

			foreach (var key in keys)
			{
				if (key.StartsWith (startKey))
				{
					CoreApplication.settings.Remove (key);
				}
			}

			foreach (var pair in dict)
			{
				CoreApplication.settings.Add (pair.Key, pair.Value);
			}
		}

		public static void SaveSettings()
		{
			try
			{
				string[] lines = CoreApplication.GetDataToSerialize ();
				System.IO.File.WriteAllLines (CoreApplication.GetSettingsPath (), lines);
			}
			catch
			{
			}

		}

		public static void LoadSettings()
		{
			try
			{
				string[] lines = System.IO.File.ReadAllLines (CoreApplication.GetSettingsPath ());
				CoreApplication.SetDeserializedData (lines);
			}
			catch
			{
			}
		}

		private static string GetSettingsPath()
		{
			return System.IO.Path.Combine (Globals.Directories.UserAppData, "Cresus.Core.settings.data");
		}

		private static string[] GetDataToSerialize()
		{
			var lines = new List<string> ();

			foreach (var pair in CoreApplication.settings)
			{
				lines.Add (string.Concat (pair.Key, "=", pair.Value));
			}

			return lines.ToArray ();
		}

		private static void SetDeserializedData(string[] lines)
		{
			foreach (var line in lines)
			{
				int i = line.IndexOf ("=");
				if (i != -1)
				{
					string key   = line.Substring (0, i);
					string value = line.Substring (i+1);

					CoreApplication.settings.Add (key, value);
				}
			}
		}
		#endregion


		internal void CreateUI()
		{
			this.CreateUIMainWindow ();
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
			this.data.SetupDatabase ();
		}

		internal void Shutdown()
		{
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.mainWindowOrchestrator.Dispose ();
				this.mainWindowController.Dispose ();

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
			string path = System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "app.ico");
			
			Window window = new Window
			{
				Text = this.ShortWindowTitle,
				ClientSize = new Epsitec.Common.Drawing.Size (600, 400),
				Icon = Epsitec.Common.Drawing.Bitmap.FromNativeIcon (path, 48, 48)
			};

			this.Window = window;
			this.Window.Root.SizeChanged +=
				delegate
				{
					this.Window.Text = string.Format ("{0} {1}x{2}", this.ShortWindowTitle, this.Window.ClientSize.Width, this.Window.ClientSize.Height);
				};
		}

		private void CreateUIControllers()
		{
			this.mainWindowController.CreateUI (this.Window.Root);
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


		private static Dictionary<string, string>		settings = new Dictionary<string, string> ();

		private PersistenceManager						persistenceManager;
		private CoreData								data;
		private Business.UserManagement.UserManager		userManager;
		private ExceptionManager						exceptionManager;
		private CoreCommandDispatcher					commands;
		private DataViewOrchestrator					mainWindowOrchestrator;
		private MainWindowController					mainWindowController;
		private List<Dialogs.IAttachedDialog>			attachedDialogs;
		private BusinessSettingsEntity					businessSettings;
	}
}
