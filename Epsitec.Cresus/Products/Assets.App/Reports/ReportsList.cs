﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
				yield return new Report (ReportType.MCH2Summary, "Tableau des immobilisations MCH2");
				yield return new Report (ReportType.AssetsList,  "Liste des objets d'immobilisations");
				yield return new Report (ReportType.PersonsList, "Liste des contacts");
			}
		}

		private struct Report
		{
			public Report(ReportType type, string name)
			{
				this.Type = type;
				this.Name = name;
			}

			public readonly ReportType			Type;
			public readonly string				Name;
		}
	}
}