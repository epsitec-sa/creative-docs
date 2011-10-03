//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.DebugService
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 2)
			{
				if (args[0] == "-monitor")
				{
					Program.ProcessMonitor (args[1]);
				}
			}
			if (args.Length == 3)
			{
				if (args[0] == "-monitor")
				{
					Program.ProcessMonitor (args[1], args[2]);
				}
				if (args[0] == "-listen")
				{
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
						while (monitor.Process ())
						{
							//	OK
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
