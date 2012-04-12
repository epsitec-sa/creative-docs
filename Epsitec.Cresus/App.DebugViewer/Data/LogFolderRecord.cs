//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DebugViewer.Data
{
	public class LogFolderRecord
	{
		public LogFolderRecord(string path)
			{
				this.path = path;

				var name = System.IO.Path.GetFileName (path);
				var args = name.Split ('-', '.');

				int n = args.Length;
				long ticks = 10000 * long.Parse (args[n-2]);

				this.timeStamp = new System.DateTime (ticks);
				this.machine = string.Join ("-", args.Take (n-2));
			}

		public System.DateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		public string Machine
		{
			get
			{
				return this.machine;
			}
		}

		public string GetMessage(string rootPath)
		{
			return string.Format ("{0} {1} {2}", this.timeStamp.ToShortDateString (), this.timeStamp.ToShortTimeString (), this.machine);
		}

	
		private readonly string path;
		private readonly System.DateTime timeStamp;
		private readonly string machine;
	}
}
