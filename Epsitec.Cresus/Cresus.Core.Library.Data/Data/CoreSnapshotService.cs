//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Data
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
		}
		
		private void CreateDatabaseSnapshot()
		{
			var remoteBackupFolder   = CoreSnapshotService.GetRemoteBackupFolder ();
			var remoteBackupFileName = CoreSnapshotService.GetTimeStampedFileName ("db.firebird-backup");
			var remoteBackupPath     = System.IO.Path.Combine (remoteBackupFolder, remoteBackupFileName);

			CoreData.BackupDatabase (remoteBackupPath, CoreData.GetDatabaseAccess ());
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
	}
}
