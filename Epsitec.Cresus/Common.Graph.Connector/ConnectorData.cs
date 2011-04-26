//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class ConnectorData
	{
		public ConnectorData(System.IntPtr handle, string path, string meta, string data)
		{
			this.meta = new Dictionary<string, string> ();
			
			this.WindowHandle = handle;
			this.Path         = path ?? "";
			this.Data         = data ?? "";
			this.metaSource   = meta ?? "";
	 		this.Ticks        = System.Environment.TickCount;

			this.meta["Path"] = this.Path;

			foreach (var line in this.metaSource.Split ('\n'))
			{
				int pos = line.IndexOf (':');

				if (pos > 0)
				{
					var key   = line.Substring (0, pos).TrimEnd (' ', '\t');
					var value = line.Substring (pos+1).TrimStart (' ', '\t');

					this.meta[key] = value;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("Invalid meta: <" + line + ">");
				}
			}
		}


		public System.IntPtr WindowHandle
		{
			get;
			private set;
		}

		public string Path
		{
			get;
			private set;
		}

		public string Data
		{
			get;
			private set;
		}

		public int Ticks
		{
			get;
			private set;
		}

		public IDictionary<string, string> Meta
		{
			get
			{
				return this.meta;
			}
		}

		public long Checksum
		{
			get
			{
				if (this.checksum == 0)
                {
					this.checksum = Epsitec.Common.IO.Checksum.ComputeAdler32 (
						x =>
						{
							x.UpdateValue (this.Path);
							x.UpdateValue (this.Data);
							x.UpdateValue (this.metaSource);
						});

					if (this.checksum == 0)
                    {
						this.checksum = 1;
                    }
                }

				return this.checksum;
			}
		}


		private readonly Dictionary<string, string> meta;
		private readonly string					metaSource;
		private long							checksum;
	}
}
