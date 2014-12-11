//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Reporting;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEventOfficeReportEntity
	{

		public static AiderEventOfficeReportEntity Create(BusinessContext context, string eventNumber, AiderEventEntity targetEvent, string templateName, IContent content)
		{
			var report = context.CreateAndRegisterEntity<AiderEventOfficeReportEntity> ();

			report.Name			= eventNumber;
			report.DataTemplate = templateName;
			report.CreationDate	= Date.Today;
			report.Office		= targetEvent.Office;
			report.EventNumber  = eventNumber;
			report.Event        = targetEvent;
			report.SetContent (content);

			targetEvent.Office.AddDocumentInternal (report);
			return report;
		}
	}
}
