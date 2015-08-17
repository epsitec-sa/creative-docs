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
	public class OfficeLetterProcessor : AbstractProcessor<AiderOfficeLetterReportEntity, Pdf.OfficeLetterDocumentWriter>
	{
		public OfficeLetterProcessor(CoreServer coreServer)
			: base (coreServer)
		{
		}

		protected override string GenerateDocuments(System.IO.Stream stream, BusinessContext businessContext, AiderOfficeSenderEntity settings, IEnumerable<AiderOfficeLetterReportEntity> reports)
		{
			throw new System.NotImplementedException ();
		}
	}
}
