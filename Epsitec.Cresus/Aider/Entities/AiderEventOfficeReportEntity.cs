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

		public static AiderEventOfficeReportEntity Create(BusinessContext context, AiderEventEntity targetEvent)
		{
			var report = context.CreateAndRegisterEntity<AiderEventOfficeReportEntity> ();

			report.CreationDate	= Date.Today;
			report.Office		= targetEvent.Office;
			report.Event        = targetEvent;
			report.Year         = targetEvent.Date.Value.Year;
			report.IsValidated  = true;
			report.EventNumberByYearAndRegistry  = AiderEventOfficeReportEntity.FindNextNumber (context, targetEvent);
			report.Name			= targetEvent.GetRegitryActName () + " " + report.GetEventNumber ();
			targetEvent.Office.AddDocumentInternal (report);
			return report;
		}

		public static AiderEventOfficeReportEntity CreatePreview(BusinessContext context, AiderEventEntity targetEvent)
		{
			var report = context.CreateAndRegisterEntity<AiderEventOfficeReportEntity> ();

			report.CreationDate	= Date.Today;
			report.Office		= targetEvent.Office;
			report.Event        = targetEvent;
			report.IsValidated  = false;
			report.Name			= targetEvent.GetRegitryActName () + " (à valider)";
			return report;
		}

		public string GetEventNumber()
		{
			return StringUtils.Join ("/", this.Year, this.EventNumberByYearAndRegistry);
		}

		partial void GetEventCode(ref string value)
		{
			value = this.GetEventNumber ();
		}

		public static AiderEventOfficeReportEntity GetByEvent (BusinessContext context, AiderEventEntity entity)
		{
			var example = new AiderEventOfficeReportEntity ()
			{
				Event = entity
			};

			return context.GetByExample<AiderEventOfficeReportEntity> (example).FirstOrDefault ();
		}

		private static int FindNextNumber(BusinessContext context, AiderEventEntity eventBase)
		{
			var nextEvent  = 1;
			var eventStyle = new AiderEventEntity ()
			{
				Type = eventBase.Type,
				State = Enumerations.EventState.Validated
			};
			var example   = new AiderEventOfficeReportEntity ()
			{
				Event = eventStyle
			};

			var reports        = context.GetByExample<AiderEventOfficeReportEntity> (example);
			var thisYearEvents = reports.Where (r => r.Year == eventBase.Date.Value.Year);
			if (thisYearEvents.Any ())
			{
				nextEvent = thisYearEvents.Max (r => r.EventNumberByYearAndRegistry) + 1;
			}

			return nextEvent;
		}
	}
}
