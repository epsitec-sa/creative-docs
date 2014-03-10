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
	public class LetterReportProcessor : Epsitec.Cresus.WebCore.Server.Processors.IReportingProcessor
	{
		public LetterReportProcessor(CoreServer coreServer)
		{
			this.coreServer = coreServer;
		}


		#region IName Members

		public string Name
		{
			get
			{
				return "letter";
			}
		}

		#endregion

		#region IReportingProcessor Members

		public string CreateReport(System.IO.Stream stream, WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			string settingsId = parameters.settings;
			string contactId  = parameters.contact;

			var settings = EntityIO.ResolveEntity (businessContext, settingsId) as AiderOfficeSettingsEntity;
			var contact  = EntityIO.ResolveEntity (businessContext, contactId) as AiderContactEntity;

			return this.GenerateDocument (stream, workerApp, businessContext, settings, contact);
		}

		#endregion

		private string GenerateDocument(System.IO.Stream stream, WorkerApp workerApp, BusinessContext context, AiderOfficeSettingsEntity settings, AiderContactEntity contact)
		{
			var userManager = workerApp.UserManager;

			//	Do something with this entity...
			
			var layout = LabelLayout.Sheet_A4_Simple;
			var doc    = new Pdf.LetterDocumentWriter (contact, layout);

			doc.WriteStream (stream);

			return "letter.pdf";
		}


		private readonly CoreServer				coreServer;
	}
}
