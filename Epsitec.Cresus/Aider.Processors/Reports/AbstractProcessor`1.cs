using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Processors.Reports
{
	public abstract class AbstractProcessor<T> : AbstractProcessor
		where T : Epsitec.Aider.Entities.AiderOfficeReportEntity
	{
		protected AbstractProcessor(CoreServer coreServer)
			: base (coreServer)
		{

		}
		public override string CreateReport(System.IO.Stream stream, WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			string settingsId	= parameters.settings;
			string reportId		= parameters.report;

			var settings = EntityIO.ResolveEntity (businessContext, settingsId) as AiderOfficeSenderEntity;
			var letter  = EntityIO.ResolveEntity (businessContext, reportId) as T;

			return this.GenerateDocument (stream, workerApp, businessContext, settings, letter);
			
		}

		protected abstract string GenerateDocument(System.IO.Stream stream, WorkerApp workerApp, BusinessContext businessContext, AiderOfficeSenderEntity settings, T letter);
	}
}
