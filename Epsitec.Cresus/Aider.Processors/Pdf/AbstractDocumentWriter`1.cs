//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Processors.Pdf
{
	public abstract class AbstractDocumentWriter<T>
		where T : AiderOfficeReportEntity
	{
		protected AbstractDocumentWriter()
		{
		}

		public void Setup(BusinessContext context, AiderOfficeSenderEntity sender, LabelLayout layout)
		{
			this.context = context;
			this.sender  = sender;
			this.layout  = layout;
		}

		public abstract void WriteStream(System.IO.Stream stream, T officeReport);

		protected BusinessContext				context;
		protected AiderOfficeSenderEntity		sender;
		protected LabelLayout					layout;
	}
}
