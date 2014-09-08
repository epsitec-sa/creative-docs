//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Export.Helpers;
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


		public static AbstractReportParams Search(DataAccessor accessor, string customTitle)
		{
			if (string.IsNullOrEmpty (customTitle))
			{
				return null;
			}
			else
			{
				return accessor.Mandat.Reports
					.Where (x => x.CustomTitle == customTitle)
					.FirstOrDefault ();
			}
		}


		public static string GetTitle(DataAccessor accessor, AbstractReportParams reportParams, ReportTitleType type)
		{
			//	Retourne le titre d'un rapport d'après les paramètres.
			string title    = null;
			string specific = null;
			string custom   = reportParams.CustomTitle;

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

			var tags = new Dictionary<string, string> ();
			ReportParamsHelper.AddTags (tags, accessor, reportParams);
			TagConverters.AddDefaultTags (tags);

			title    = TagConverters.GetFinalText (tags, title   );
			specific = TagConverters.GetFinalText (tags, specific);
			custom   = TagConverters.GetFinalText (tags, custom  );

			switch (type)
			{
				case ReportTitleType.Title:
					return title;

				case ReportTitleType.Specific:
					return specific;

				case ReportTitleType.Custom:
					return custom;

				case ReportTitleType.Sortable:
					return string.Concat (title, "_$$_", specific);

				case ReportTitleType.Full:
					if (string.IsNullOrEmpty (custom))
					{
						return string.Concat (title, " — ", specific);
					}
					else
					{
						return string.Concat (title, " — ", custom);
					}

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid type {0}", type));
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



		private static void AddTags(Dictionary<string, string> dict, DataAccessor accessor, AbstractReportParams reportParams)
		{
			if (reportParams is MCH2SummaryParams)
			{
				ReportParamsHelper.AddTags (dict, accessor, reportParams as MCH2SummaryParams);
			}
			else if (reportParams is AssetsParams)
			{
				ReportParamsHelper.AddTags (dict, accessor, reportParams as AssetsParams);
			}
			else if (reportParams is PersonsParams)
			{
				ReportParamsHelper.AddTags (dict, accessor, reportParams as PersonsParams);
			}
		}

		private static void AddTags(Dictionary<string, string> dict, DataAccessor accessor, MCH2SummaryParams reportParams)
		{
			var date = reportParams.DateRange.ToNiceString ();
			dict.Add (TagConverters.Compile ("<DATE>"), date);

			var group = GroupsLogic.GetShortName (accessor, reportParams.RootGuid);
			dict.Add (TagConverters.Compile ("<GROUP>"), group);

			var level = reportParams.Level.HasValue ? reportParams.Level.ToString () : "";
			dict.Add (TagConverters.Compile ("<LEVEL>"), level);

			var filter = GroupsLogic.GetShortName (accessor, reportParams.FilterGuid);
			dict.Add (TagConverters.Compile ("<FILTER>"), filter);
		}

		private static void AddTags(Dictionary<string, string> dict, DataAccessor accessor, AssetsParams reportParams)
		{
			var date = TypeConverters.DateToString (reportParams.Timestamp.Date);
			dict.Add (TagConverters.Compile ("<DATE>"), date);

			var group = GroupsLogic.GetShortName (accessor, reportParams.RootGuid);
			dict.Add (TagConverters.Compile ("<GROUP>"), group);

			var level = reportParams.Level.HasValue ? reportParams.Level.ToString () : "";
			dict.Add (TagConverters.Compile ("<LEVEL>"), level);
		}

		private static void AddTags(Dictionary<string, string> dict, DataAccessor accessor, PersonsParams reportParams)
		{
		}
	}
}
