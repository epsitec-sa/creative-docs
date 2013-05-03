//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Override
{
	/// <summary>
	/// The <c>AiderActivityLogger</c> class logs user access statistics to a file. The
	/// logging is very lightweight and done asynchronously (as 10-minute chunks).
	/// </summary>
	public sealed class AiderActivityLogger
	{
		private AiderActivityLogger()
		{
			this.exclusion   = new object ();
			this.users       = new HashSet<string> ();
			this.accessCount = 0;

			this.timer = new System.Timers.Timer (60*1000)
			{
				AutoReset = true,
				Enabled = true
			};

			this.timer.Elapsed += this.HandleTimerElapsed;
			this.timer.Start ();
		}

		
		public static AiderActivityLogger		Current
		{
			get
			{
				return AiderActivityLogger.current;
			}
		}


		/// <summary>
		/// Records the user access (remembers the user and increment the total
		/// access count).
		/// </summary>
		/// <param name="user">The user.</param>
		public void RecordAccess(AiderUserEntity user)
		{
			if (user != null)
			{
				var code = user.Code;

				lock (this.exclusion)
				{
					this.users.Add (code);
					this.accessCount++;
				}
			}
		}

		
		private void HandleTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			var stamp = this.GetDateStamp ();

			if (this.dateStamp == null)
			{
				this.dateStamp = stamp;
			}
			if (this.dateStamp == stamp)
			{
				return;
			}

			int userCount   = 0;
			int accessCount = 0;

			lock (this.exclusion)
			{
				userCount   = this.users.Count;
				accessCount = this.accessCount;

				this.users.Clear ();
				this.accessCount = 0;
			}

			var path = this.GetLogFilePath ();
			var contents = string.Format ("{0}\t{1}\t{2}\r\n", this.dateStamp, userCount, accessCount);

			System.IO.File.AppendAllText (path, contents);

			this.dateStamp = stamp;
		}

		private string GetDateStamp()
		{
			var now = System.DateTime.UtcNow;
			return string.Format ("{0:00}:{1:00}", now.Hour, (now.Minute / 10)*10);
		}

		private string GetLogFilePath()
		{
			var now = System.DateTime.UtcNow;
			var fileName = string.Format ("{0:0000}-{1:00}-{2:00} user access.log", now.Year, now.Month, now.Day);
			return System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.CommonAppData, fileName);
		}


		private static readonly AiderActivityLogger current = new AiderActivityLogger ();

		private readonly object					exclusion;
		private readonly System.Timers.Timer	timer;
		private readonly HashSet<string>		users;
		private int								accessCount;
		private string							dateStamp;
	}
}

