//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Epsitec.Aider.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Processors.Reports
{
	public abstract class AbstractProcessor<T1, T2> : AbstractProcessor<T1>
		where T1 : Epsitec.Aider.Entities.AiderOfficeReportEntity
		where T2 : Pdf.AbstractDocumentWriter<T1>, new ()
	{
		public AbstractProcessor(CoreServer coreServer)
			: base (coreServer)
		{

		}

		protected override string GenerateDocument(System.IO.Stream stream, BusinessContext context, AiderOfficeSenderEntity sender, T1 report)
		{
			var workerApp = WorkerApp.Current;
			var userManager = workerApp.UserManager;

			//	Do something with this entity...
			
			var layout = LabelLayout.Sheet_A4_Simple;
			var doc    = new T2 ();
			
			doc.Setup (context, sender, layout);

			doc.WriteStream (stream, report);
			report.ProcessingDate = System.DateTime.UtcNow;
			context.SaveChanges (LockingPolicy.ReleaseLock);

			return report.Name + ".pdf";
		}
	}
}
