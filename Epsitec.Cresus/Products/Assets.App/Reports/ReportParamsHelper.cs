//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class ReportParamsHelper
	{
		public static string GetTitle(DataAccessor accessor, AbstractReportParams reportParams)
		{
			//	Retourne le titre d'un rapport.
			if (reportParams is MCH2SummaryParams)
			{
				var p = reportParams as MCH2SummaryParams;
				var title = Res.Strings.ReportParams.MCH2Summary.ToString ();
				var date = p.DateRange.ToNiceString ();

				if (p.FilterGuid.IsEmpty)
				{
					return string.Concat (title, " ", date);
				}
				else
				{
					var filter = GroupsLogic.GetShortName (accessor, p.FilterGuid);
					return string.Concat (title, " ", date, " — ", filter);
				}
			}
			else if (reportParams is AssetsParams)
			{
				var p = reportParams as AssetsParams;
				var title = Res.Strings.ReportParams.Assets.ToString ();
				return string.Concat (title, " ", TypeConverters.DateToString (p.Timestamp.Date));
			}
			else if (reportParams is PersonsParams)
			{
				var title = Res.Strings.ReportParams.Persons.ToString ();
				return title;
			}
			else
			{
				return null;
			}
		}


		public static AbstractReport CreateReport(DataAccessor accessor, AbstractReportParams reportParams)
		{
			if (reportParams is MCH2SummaryParams)
			{
				return new MCH2SummaryReport (accessor, reportParams);
			}
			else if (reportParams is AssetsParams)
			{
				return new AssetsReport (accessor, reportParams);
			}
			else if (reportParams is PersonsParams)
			{
				return new PersonsReport (accessor, reportParams);
			}
			else
			{
				return null;
			}
		}
	}
}
