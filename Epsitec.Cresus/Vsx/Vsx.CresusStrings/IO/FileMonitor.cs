using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.IO
{
	public class FileMonitor : FileSystemWatcher
	{
		public FileMonitor()
			: base ()
		{
			Trace.WriteLine ("FileMonitor()");
		}
		public FileMonitor(string path)
			: base (path)
		{
			Trace.WriteLine (string.Format ("FileMonitor({0})", path));
		}
		public FileMonitor(string path, string filter)
			: base (path, filter)
		{
			Trace.WriteLine (string.Format ("FileMonitor({0}, {1})", path, filter));
		}

		protected override void Dispose(bool disposing)
		{
			Trace.WriteLine (string.Format ("FileMonitor[{0}\\{1}].Dispose({2})", this.Path, this.Filter, disposing));
			base.Dispose (disposing);
		}
	}
}
