//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public class ExportEntriesReport
	{
		public ExportEntriesReport(DateRange range, string filename, int entriesCount)
		{
			this.Range        = range;
			this.Filename     = filename;
			this.EntriesCount = entriesCount;
		}


		public string							Description
		{
			get
			{
				var from = TypeConverters.DateToString (this.Range.IncludeFrom);
				var to   = TypeConverters.DateToString (this.Range.ExcludeTo.AddDays (-1));  // date de fin inclue;

				var filename = System.IO.Path.GetFileName (this.Filename);

				string entries;
				if (this.EntriesCount == 0)
				{
					entries = "Aucune écriture";
				}
				else if (this.EntriesCount == 1)
				{
					entries = "1 écriture";
				}
				else
				{
					entries = string.Format ("{0} écritures", this.EntriesCount);
				}

				return string.Format ("Du {0} au {1} — {2} — {3}", from, to, filename, entries);
			}
		}


		public readonly DateRange				Range;
		public readonly string					Filename;
		public readonly int						EntriesCount;
	}
}
