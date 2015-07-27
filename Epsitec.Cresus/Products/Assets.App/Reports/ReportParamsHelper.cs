//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Core.Helpers;
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
			if (reportParams == null)
			{
				return null;
			}

			var title    = ReportParamsHelper.GetTag (accessor, reportParams, "<TITLE>");
			var specific = ReportParamsHelper.GetSpecific (accessor, reportParams);

			var tags = new Dictionary<string, string> ();
			ReportParamsHelper.AddTags (tags, accessor, reportParams);
			TagConverters.AddDefaultTags (tags);
			var custom = TagConverters.GetFinalText (tags, reportParams.CustomTitle);

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
						return UniversalLogic.NiceJoin (title, specific);
					}
					else
					{
						return UniversalLogic.NiceJoin (title, custom);
					}

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid type {0}", type));
			}
		}


		private static string GetSpecific(DataAccessor accessor, AbstractReportParams reportParams)
		{
			if (reportParams is MCH2SummaryParams)
			{
				return ReportParamsHelper.GetSpecific (accessor, reportParams as MCH2SummaryParams);
			}
			else if (reportParams is AssetsParams)
			{
				return ReportParamsHelper.GetSpecific (accessor, reportParams as AssetsParams);
			}
			else if (reportParams is PersonsParams)
			{
				return ReportParamsHelper.GetSpecific (accessor, reportParams as PersonsParams);
			}
			else
			{
				return null;
			}
		}

		private static string GetSpecific(DataAccessor accessor, MCH2SummaryParams reportParams)
		{
			//	Retourne le titre du tableau des immobilisations MCH2. Par exemple:
			//	"Tableau des immobilisations MCH2 2014 - Catégories MCH2 (1) - Patrimoine administratif"
			var list = new List<string> ();
			list.Add (ReportParamsHelper.GetTag (accessor, reportParams, "<DATE>"));

			var group = ReportParamsHelper.GetTag (accessor, reportParams, "<GROUP>");
			if (!string.IsNullOrEmpty (group))
			{
				var level = ReportParamsHelper.GetTag (accessor, reportParams, "<LEVEL>");
				if (!string.IsNullOrEmpty (level))
				{
					group = string.Format ("{0} ({1})", group, level);
				}

				list.Add (group);
			}

			var filter = ReportParamsHelper.GetTag (accessor, reportParams, "<FILTER>");
			if (!string.IsNullOrEmpty (filter))
			{
				list.Add (filter);
			}

			return UniversalLogic.NiceJoin (list.ToArray ());
		}

		private static string GetSpecific(DataAccessor accessor, AssetsParams reportParams)
		{
			var list = new List<string> ();
			list.Add (ReportParamsHelper.GetTag (accessor, reportParams, "<DATE>"));

			var group = ReportParamsHelper.GetTag (accessor, reportParams, "<GROUP>");
			if (!string.IsNullOrEmpty (group))
			{
				var level = ReportParamsHelper.GetTag (accessor, reportParams, "<LEVEL>");
				if (!string.IsNullOrEmpty (level))
				{
					group = string.Format ("{0} ({1})", group, level);
				}

				list.Add (group);
			}

			return UniversalLogic.NiceJoin (list.ToArray ());
		}

		private static string GetSpecific(DataAccessor accessor, PersonsParams reportParams)
		{
			return ReportParamsHelper.GetTag (accessor, reportParams, "<FIX>");
		}


		private static void AddTags(Dictionary<string, string> dict, DataAccessor accessor, AbstractReportParams reportParams)
		{
			foreach (var tag in ReportParamsHelper.Tags)
			{
				var text = ReportParamsHelper.GetTag (accessor, reportParams, tag);
				dict.Add (TagConverters.Compile (tag), text);
			}
		}

		private static IEnumerable<string> Tags
		{
			get
			{
				yield return "<TITLE>";
				yield return "<DATE>";
				yield return "<GROUP>";
				yield return "<LEVEL>";
				yield return "<FILTER>";
				yield return "<FIX>";
				yield return "<DIRECTMODE>";
			}
		}


		private static string GetTag(DataAccessor accessor, AbstractReportParams reportParams, string tag)
		{
			if (reportParams is MCH2SummaryParams)
			{
				return ReportParamsHelper.GetTag (accessor, reportParams as MCH2SummaryParams, tag);
			}
			else if (reportParams is AssetsParams)
			{
				return ReportParamsHelper.GetTag (accessor, reportParams as AssetsParams, tag);
			}
			else if (reportParams is PersonsParams)
			{
				return ReportParamsHelper.GetTag (accessor, reportParams as PersonsParams, tag);
			}
			else
			{
				return null;
			}
		}

		private static string GetTag(DataAccessor accessor, MCH2SummaryParams reportParams, string tag)
		{
			switch (tag)
			{
				case "<TITLE>":
					return Res.Strings.ReportParams.MCH2Summary.ToString ();

				case "<DATE>":
					return reportParams.DateRange.ToNiceString ();

				case "<GROUP>":
					return GroupsLogic.GetShortName (accessor, reportParams.RootGuid);

				case "<LEVEL>":
					return reportParams.Level.HasValue ? reportParams.Level.ToString () : "";

				case "<FILTER>":
					return GroupsLogic.GetShortName (accessor, reportParams.FilterGuid);

				case "<DIRECTMODE>":
					return reportParams.DirectMode ? Res.Strings.ReportParams.MCH2Direct.ToString () : Res.Strings.ReportParams.MCH2Indirect.ToString ();

				default:
					return null;
			}
		}

		private static string GetTag(DataAccessor accessor, AssetsParams reportParams, string tag)
		{
			switch (tag)
			{
				case "<TITLE>":
					return Res.Strings.ReportParams.Assets.ToString ();

				case "<DATE>":
					return TypeConverters.DateToString (reportParams.Timestamp.Date);

				case "<GROUP>":
					return GroupsLogic.GetShortName (accessor, reportParams.RootGuid);

				case "<LEVEL>":
					return reportParams.Level.HasValue ? reportParams.Level.ToString () : "";

				default:
					return null;
			}
		}

		private static string GetTag(DataAccessor accessor, PersonsParams reportParams, string tag)
		{
			switch (tag)
			{
				case "<TITLE>":
					return Res.Strings.ReportParams.Persons.ToString ();

				case "<FIX>":
					return Res.Strings.ReportParams.Specific.ToString ();

				default:
					return null;
			}
		}
	}
}
