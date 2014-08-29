//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Reports
{
	/// <summary>
	/// Cette classe fourni la liste des rapports disponibles.
	/// </summary>
	public static class ReportsList
	{
		public static IEnumerable<ReportType> ReportTypes
		{
			get
			{
				return ReportsList.Reports.Select (x => x.Type);
			}
		}

		public static ReportType GetReportType(int? index)
		{
			//	Retourne le type d'un rapport.
			if (index.HasValue && index.Value != -1)
			{
				return ReportsList.Reports.Select (x => x.Type).ToArray ()[index.Value];
			}
			else
			{
				return ReportType.Unknown;
			}
		}

		public static int GetReportIndex(ReportType type)
		{
			//	Retourne l'index d'un rapport.
			return ReportsList.Reports.Select (x => x.Type).ToList ().IndexOf (type);
		}

		public static string GetReportName(ReportType type)
		{
			//	Retourne le nom d'un rapport.
			return ReportsList.Reports.Where (x => x.Type == type).FirstOrDefault ().Name;
		}


		private static IEnumerable<Report> Reports
		{
			get
			{
				yield return new Report (ReportType.MCH2Summary);
				yield return new Report (ReportType.AssetsList);
				yield return new Report (ReportType.PersonsList);
			}
		}

		private struct Report
		{
			public Report(ReportType type)
			{
				this.Type = type;
				this.Name = ReportsList.GetReportTypeDescription (type);
			}

			public readonly ReportType			Type;
			public readonly string				Name;
		}

		private static string GetReportTypeDescription(ReportType type)
		{
			return EnumKeyValues.GetEnumKeyValue (type).Values.Last ().ToString ();
		}
	}
}