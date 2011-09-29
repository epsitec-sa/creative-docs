//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Orchestrators;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Cresus.Core.Library.Business
{
	/// <summary>
	/// The <c>CoreSnapshotService</c> class is responsible for the creation of the
	/// database snapshots.
	/// </summary>
	public class CoreSnapshotService
	{
		public CoreSnapshotService()
		{
			this.CreateDatabaseSnapshot ();
		}

		public void NotifyApplicationStarted(CoreApp app)
		{
			var dataViewOrchestrator = app.FindActiveComponent<DataViewOrchestrator> ();
			var navigationOrchestrator = dataViewOrchestrator.Navigator;

			navigationOrchestrator.NodeAdded += sender => this.RecordEvent ("NAV", navigationOrchestrator.GetTopNavigationPath ().ToString ());

			CommandDispatcher.CommandDispatching       += this.HandleCommandDispatcherCommandDispatching;
			CommandDispatcher.CommandDispatched        += this.HandleCommandDispatcherCommandDispatchFinished;
			CommandDispatcher.CommandDispatchCancelled += this.HandleCommandDispatcherCommandDispatchFinished;
			CommandDispatcher.CommandDispatchFailed    += this.HandleCommandDispatcherCommandDispatchFinished;

			BridgeSpy.ExecutingSetter += this.HandleBridgeSpyExecutingSetter;
		}
		
		private void CreateDatabaseSnapshot()
		{
			var remoteBackupFolder   = CoreSnapshotService.GetRemoteBackupFolder ();
			var remoteBackupFileName = CoreSnapshotService.GetTimeStampedFileName ("db.firebird-backup");
			var remoteBackupPath     = System.IO.Path.Combine (remoteBackupFolder, remoteBackupFileName);

			CoreData.BackupDatabase (remoteBackupPath, CoreData.GetDatabaseAccess ());
		}

		private void RecordEvent(string eventName, string eventArg)
		{
			System.Diagnostics.Debug.WriteLine (string.Concat (">>> ", eventName, ": ", eventArg ?? "-"));
		}


		private void HandleCommandDispatcherCommandDispatching(object sender, CommandEventArgs e)
		{
			this.RecordEvent ("CMD", string.Format ("{0} ({1})", e.Command.CommandId, e.Command.Name));
			this.commandDispatchDepth++;
		}

		private void HandleCommandDispatcherCommandDispatchFinished(object sender, CommandEventArgs e)
		{
			this.commandDispatchDepth--;
		}

		private void HandleBridgeSpyExecutingSetter(object sender, BridgeSpyEventArgs e)
		{
			var entity = e.Entity;

			var dataContext = Epsitec.Cresus.DataLayer.Context.DataContextPool.GetDataContext (entity);
			var entityKey   = dataContext.GetNormalizedEntityKey (entity);

			if (entityKey.HasValue)
			{
				this.RecordEvent ("SET_DB_FIELD", string.Format ("{0}/{1} : {2} -> {3}", entityKey.Value, e.FieldCaption.Name, e.OldValue ?? "<null>", e.NewValue ?? "<null>"));
			}
			else
			{
				this.RecordEvent ("SET_LIVE_FIELD", string.Format ("{0}/{1} : {2} -> {3}", e.Entity.GetEntityStructuredTypeId (), e.FieldCaption.Name, e.OldValue ?? "<null>", e.NewValue ?? "<null>"));
			}
		}

		private static string GetRemoteBackupFolder()
		{
			return "Snapshots";
		}

		private static string GetTimeStampedFileName(string name)
		{
			var userName   = System.Environment.UserName.ToLowerInvariant ();
			var hostName   = System.Environment.MachineName.ToLowerInvariant ();
			var totalTicks = System.DateTime.UtcNow.Ticks / 10000;

			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}@{1}-{2:0000000000000000}-{3}", userName, hostName, totalTicks, name);
		}


		private int commandDispatchDepth;
	}
}
