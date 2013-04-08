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
				this.Entries = Executor.GetEntries();
				this.Info = Executor.GetResultInfo();    
		}

		public IList<DirectoriesEntry> Entries;
		public DirectoriesResultInfo Info;
	}
}
