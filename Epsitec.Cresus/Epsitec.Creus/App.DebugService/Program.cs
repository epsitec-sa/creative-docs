//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.DebugService
{
	/// <summary>
	/// The <c>Program</c> class for the DebugService is used to both monitor and upload
	/// data from a running system and to listen to incoming requests from the monitor.
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 2)
			{
				if (args[0] == "-monitor")
				{
					//	-monitor "C:\my folder\temp"
					Program.ProcessMonitor (args[1]);
				}
			}
			else if (args.Length == 3)
			{
				if (args[0] == "-monitor")
				{
					//	-monitor "C:\my folder\temp" pid
					Program.ProcessMonitor (args[1], args[2]);
				}
				else if (args[0] == "-listen")
				{
					//	-listen http://+:8081/debugservice/ "C:\my folder\store"
					WebListener.RunServer (args[1], args[2]);
				}
			}
		}

		private static void ProcessMonitor(string dirPath, string processIdValue = null)
		{
			int processId = int.Parse (processIdValue ?? "0", System.Globalization.CultureInfo.InvariantCulture);
			var process   = processId == 0 ? null : System.Diagnostics.Process.GetProcessById (processId);

			if (System.IO.Directory.Exists (dirPath))
			{
				using (var monitor = new FolderMonitor (dirPath))
				{
					do
					{
						bool sleep = true;

						while (monitor.Process ())
						{
							sleep = false;
						}

						if (sleep)
						{
							monitor.Sleep ();
						}
					}
					while (Program.ProbeRunningProcess (process));
				}
			}
		}

		private static bool ProbeRunningProcess(System.Diagnostics.Process process)
		{
			if (process == null)
			{
				return false;
			}

			process.Refresh ();

			return !process.HasExited;
		}
	}
}
