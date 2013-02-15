using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Database;

using System;

using System.IO;

using System.Linq;

using System.Timers;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	using Timer = System.Timers.Timer;


	/// <summary>
	/// This class takes care of backing up the database at regular intervals. If the database is
	/// on localhost, the backup is zipped and moved to a given directory.
	/// </summary>
	/// <remarks>
	/// If the application use the firebird embedded client, gbak or external tools can't back up
	/// the database while the application is running, because the firebird embedded client locks
	/// the database and only the application can access the database. Therefore, we need to have
	/// the backup made within the application. That's why we have this class.
	/// </remarks>
	public sealed class BackupManager : IDisposable
	{


		/// <summary>
		/// Creates a new instance of BackupManager which will make regular backups of the database
		/// until the Dispose method is called.
		/// </summary>
		/// <param name="directory">
		/// The directory where to put the backup files (usefull only if the database is stored on
		/// the localhost).
		/// </param>
		/// <param name="interval">
		/// The delay between two consecutive backups.
		/// </param>
		/// <param name="start">
		/// The time of the day at which the first backup will take place. This is usefull if you
		/// want the backup to always take place in the middle of the night. In such a case, you
		/// would set up the start time at midnight and the interval to one day. If you provide null
		/// for this argument, the first backup will take place one interval of time after the
		/// application has been launched.
		/// </param>
		public BackupManager(DirectoryInfo directory, TimeSpan interval, Time? start)
		{
			this.backupDirectory = directory;
			this.dbAccess = CoreData.GetDatabaseAccess ();

			this.exclusion = new object ();
			this.safeSectionManager = new SafeSectionManager ();

			this.isRunning = false;

			this.timer = new Timer ();

			if (start.HasValue)
			{
				this.Start (interval, start.Value);
			}
			else
			{
				this.Start (interval);
			}

			Logger.LogToConsole ("Backup manager started");
		}


		private void Start(TimeSpan interval, Time start)
		{
			var delay = (start - Time.Now);

			// If the start time is before the current time, we get a negative timespan. We must
			// convert it to the equivalent positive timespan.
			if (delay < TimeSpan.Zero)
			{
				delay = TimeSpan.FromHours (24) - delay.Duration ();
			}

			// We need to keep a reference on the event handler to remove it from the timer.
			ElapsedEventHandler handler = null;
			
			handler = (o, e) =>
			{
				// Remove this handler from the time, so it is invoked only once. We need to do this
				// because we use the same timer for the initial delay than for the backup schedule.
				this.timer.Elapsed -= handler;

				this.Start (interval);
			};

			this.timer.Interval = delay.TotalMilliseconds;
			this.timer.AutoReset = false;
			this.timer.Elapsed += handler;
			this.timer.Start ();
		}


		private void Start(TimeSpan interval)
		{
			this.timer.Interval = interval.TotalMilliseconds;
			this.timer.AutoReset = true;
			this.timer.Elapsed += (o, e) => this.Backup ();
			this.timer.Start ();
		}


		private void Backup()
		{
			// Try to enter a new safe section to ensure that the backup manager won't be closed
			// while we are executing this method.
			using (var safeSection = this.safeSectionManager.TryCreate ())
			{
				// The safe section could not be created because the Dispose method has been called,
				// thus we exit this function.
				if (safeSection == null)
				{
					Logger.LogToConsole ("Skipping database back up.");
					return;
				}

				lock (this.exclusion)
				{
					// We skip the backup if we are already backing up the database. This might
					// happen if the interval is too small.
					if (this.isRunning)
					{
						Logger.LogToConsole ("Skipping database back up.");
						return;
					}

					// Now we notify the backup manager that we are making a backup, so that if the
					// event is raised again before we have finished, the event will be skiped.
					this.isRunning = true;
				}

				this.MakeBackup ();

				lock (this.exclusion)
				{
					// Now that we have finished, we notify the backup manger that we are done, so
					// when the event is raised again, the backup will be done again.
					this.isRunning = false;
				}
			}
		}


		private void MakeBackup()
		{
			Logger.LogToConsole ("Backing up database...");

			var backupFileName = this.GetBackupFileName ();

			var success = CoreData.BackupDatabase (backupFileName, this.dbAccess);

			if (!success)
			{
				Logger.LogToConsole ("Back up failed");
				return;
			}

			// If the database is not stored on the localhost, we can't process it further and we
			// exit the method.
			if (!this.dbAccess.IsLocalHost)
			{
				Logger.LogToConsole ("Database backed up on server to " + backupFileName);
				return;
			}

			// That's some kind of hacky way to get the directory where the backup has been made.
			var databaseFile = DbFactory.GetDatabaseFilePaths (this.dbAccess).Single ();
			var databaseDirectory = Path.GetDirectoryName (databaseFile);

			var backupFilePath = Path.Combine (databaseDirectory, backupFileName);

			var zippedBackupFileName = backupFileName + ".zip";
			var zippedBackupFilePath = Path.Combine (databaseDirectory, zippedBackupFileName);

			Tools.Zip (backupFilePath, zippedBackupFilePath);
			File.Delete (backupFilePath);

			var finalBackupDirectoryPath = this.backupDirectory.FullName;
			var finalBackupFilePath = Path.Combine (finalBackupDirectoryPath, zippedBackupFileName);

			if (!Directory.Exists (finalBackupDirectoryPath))
			{
				Directory.CreateDirectory (finalBackupDirectoryPath);
			}

			File.Move (zippedBackupFilePath, finalBackupFilePath);

			Logger.LogToConsole ("Database backed up locally to " + finalBackupFilePath);
		}


		private string GetBackupFileName()
		{
			var database = CoreContext.DatabaseName;
			
			var time = DateTime.Now;
			var year = time.Year;
			var month = time.Month;
			var day = time.Day;
			var hour = time.Hour;
			var minute = time.Minute;
			var second = time.Second;

			var template = "{0}-backup-{1:0000}-{2:00}-{3:00}-{4:00}-{5:00}-{6:00}.gbak";

			return string.Format (template, database, year, month, day, hour, minute, second);
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.timer.Stop ();

			this.safeSectionManager.Dispose ();

			this.timer.Dispose ();
		}


		#endregion


		private readonly DirectoryInfo backupDirectory;

		
		private readonly DbAccess dbAccess;


		private readonly Timer timer;


		private readonly object exclusion;


		private readonly SafeSectionManager safeSectionManager;


		private bool isRunning;


	}


}
