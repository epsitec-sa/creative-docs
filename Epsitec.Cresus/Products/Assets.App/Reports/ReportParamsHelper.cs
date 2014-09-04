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
		public static AbstractReport CreateReport(DataAccessor accessor, AbstractReportParams reportParams)
		{
			//	Crée le rapport correspondant aux paramètres.
			AbstractReport report = null;

			if (reportParams is MCH2SummaryParams)
			{
				report = new MCH2SummaryReport (accessor);
			}
			else if (reportParams is AssetsParams)
			{
				report = new AssetsReport (accessor);
			}
			else if (reportParams is PersonsParams)
			{
				report = new PersonsReport (accessor);
			}

			if (report != null)
			{
				report.ReportParams = reportParams;
			}

			return report;
		}


		public static string GetTitle(DataAccessor accessor, AbstractReportParams reportParams, ReportTitleType type)
		{
			//	Retourne le titre d'un rapport d'après les paramètres.
			string title    = null;
			string specific = null;

			if (reportParams is MCH2SummaryParams)
			{
				ReportParamsHelper.GetTitle (accessor, reportParams as MCH2SummaryParams, out title, out specific);
			}
			else if (reportParams is AssetsParams)
			{
				ReportParamsHelper.GetTitle (accessor, reportParams as AssetsParams, out title, out specific);
			}
			else if (reportParams is PersonsParams)
			{
				ReportParamsHelper.GetTitle (accessor, reportParams as PersonsParams, out title, out specific);
			}

			switch (type)
			{
				case ReportTitleType.Title:
					return title;

				case ReportTitleType.Specific:
					return specific;

				default:
					return string.Concat (title, " ", specific);
			}
		}

		private static void GetTitle(DataAccessor accessor, MCH2SummaryParams reportParams, out string title, out string specific)
		{
			//	Retourne le titre du tableau des immobilisations MCH2. Par exemple:
			//	"Tableau des immobilisations MCH2 2014 - Catégories MCH2 (1) - Patrimoine administratif"
			title = Res.Strings.ReportParams.MCH2Summary.ToString ();

			var list = new List<string> ();
			list.Add (reportParams.DateRange.ToNiceString ());

			if (!reportParams.RootGuid.IsEmpty)
			{
				var group = GroupsLogic.GetShortName (accessor, reportParams.RootGuid);
				list.Add (string.Format ("{0} ({1})", group, reportParams.Level));
			}

			if (!reportParams.FilterGuid.IsEmpty)
			{
				var filter = GroupsLogic.GetShortName (accessor, reportParams.FilterGuid);
				list.Add (filter);
			}

			specific = string.Join (" — ", list);
		}

		private static void GetTitle(DataAccessor accessor, AssetsParams reportParams, out string title, out string specific)
		{
			title    = Res.Strings.ReportParams.Assets.ToString ();
			specific = TypeConverters.DateToString (reportParams.Timestamp.Date);

			if (!reportParams.RootGuid.IsEmpty)
			{
				var group = GroupsLogic.GetShortName (accessor, reportParams.RootGuid);
				specific = string.Concat (specific, " — ", group);

				if (reportParams.Level.HasValue)
				{
					specific = string.Format ("{0} ({1})", specific, reportParams.Level.Value);
				}
			}
		}

		private static void GetTitle(DataAccessor accessor, PersonsParams reportParams, out string title, out string specific)
		{
			title    = Res.Strings.ReportParams.Persons.ToString ();
			specific = Res.Strings.ReportParams.Specific.ToString ();
		}
	}
}
