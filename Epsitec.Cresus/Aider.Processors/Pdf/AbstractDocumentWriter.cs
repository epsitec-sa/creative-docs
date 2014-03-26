using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

namespace Epsitec.Aider.Processors.Pdf
{
	public abstract class AbstractDocumentWriter<T>
		where T : Epsitec.Aider.Entities.AiderOfficeReportEntity
	{
		public AbstractDocumentWriter()
		{

		}

		public void Setup(BusinessContext context, AiderOfficeSenderEntity sender, LabelLayout layout)
		{
			this.context = context;
			this.sender  = sender;
			this.layout  = layout;
		}

		public abstract void WriteStream(System.IO.Stream stream, T officeReport);

		protected BusinessContext			context;
		protected AiderOfficeSenderEntity	sender;
		protected LabelLayout				layout;
	}
}
