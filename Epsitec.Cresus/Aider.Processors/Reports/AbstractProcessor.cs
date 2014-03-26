using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Processors;

namespace Epsitec.Aider.Processors.Reports
{
	public abstract class AbstractProcessor : IReportingProcessor
	{
		protected AbstractProcessor(CoreServer coreServer)
		{
			this.coreServer = coreServer;
		}

		#region IReportingProcessor Members

		public abstract string CreateReport(System.IO.Stream stream, Cresus.WebCore.Server.Core.WorkerApp workerApp, Cresus.Core.Business.BusinessContext businessContext, dynamic parameters);

		#endregion

		#region IName Members

		public string Name
		{
			get
			{
				return this.GetType ().Name.StripSuffix ("Processor").ToLowerInvariant ();
			}
		}

		#endregion


		protected readonly CoreServer			coreServer;
	}
}
