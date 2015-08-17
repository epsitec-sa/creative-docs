//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Epsitec.Aider.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Processors.Reports
{
	public class EventOfficeReportProcessor : AbstractProcessor<AiderEventOfficeReportEntity, Pdf.EventWriter>
	{
		public EventOfficeReportProcessor(CoreServer coreServer)
			: base (coreServer)
		{
		}

		protected override string GenerateDocuments(System.IO.Stream stream, BusinessContext businessContext, AiderOfficeSenderEntity settings, IEnumerable<AiderEventOfficeReportEntity> reports)
		{
			throw new System.NotImplementedException ();
		}
	}
}
