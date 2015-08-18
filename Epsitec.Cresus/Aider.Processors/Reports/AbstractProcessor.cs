//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
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

		public abstract string CreateReport(System.IO.Stream stream, Cresus.Core.Business.BusinessContext businessContext, dynamic parameters);
		public abstract string CreateReports(System.IO.Stream stream, Cresus.Core.Business.BusinessContext businessContext, IEnumerable<AbstractEntity> entities, dynamic parameters);
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
