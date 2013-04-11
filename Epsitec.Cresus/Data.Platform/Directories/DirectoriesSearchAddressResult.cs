using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Data.Platform.Directories.Entity;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesSearchAddressResult
	{
		public DirectoriesSearchAddressResult(DirectoriesSearchAddressExecutor Executor)
		{
				this.RowEntries = Executor.GetEntries();
				this.Info = Executor.GetResultInfo();    
		}

		public List<DirectoriesEntryAdd> GetEntries()
		{
			List<DirectoriesEntryAdd> EntryAdds = new List<DirectoriesEntryAdd>();
			foreach (DirectoriesEntry en in this.RowEntries)
			{
				EntryAdds.AddRange (en.EntryAdds);
			}
			return EntryAdds;
		}

		public IList<DirectoriesEntry> RowEntries;
		public DirectoriesResultInfo Info;
	}
}
