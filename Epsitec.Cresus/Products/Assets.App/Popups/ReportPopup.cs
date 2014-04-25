//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ReportPopup : StackedPopup
	{
		public ReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Choix d'un rapport";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = ReportPopup.ReportNames,
			});

			this.SetDescriptions (list);
		}


		public ReportType						ReportType
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return ReportPopup.GetReportType (controller.Value);
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = ReportPopup.GetReportIndex (value);
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Voir";
			this.okButton.Enable = this.ReportType != ReportType.Unknown;
		}


		#region Reports manager
		private static ReportType GetReportType(int? index)
		{
			if (index.HasValue && index.Value != -1)
			{
				return ReportPopup.Reports.Select (x => x.Type).ToArray ()[index.Value];
			}
			else
			{
				return ReportType.Unknown;
			}
		}

		private static int GetReportIndex(ReportType type)
		{
			return ReportPopup.Reports.Select (x => x.Type).ToList ().IndexOf (type);
		}

		public static string GetReportName(ReportType type)
		{
			return ReportPopup.Reports.Where (x => x.Type == type).FirstOrDefault ().Name;
		}

		private static string ReportNames
		{
			get
			{
				var list = new List<string> ();

				foreach (var report in ReportPopup.Reports)
				{
					list.Add (report.Name);
				}

				return string.Join ("<br/>", list);
			}
		}
		private static IEnumerable<Report> Reports
		{
			get
			{
				yield return new Report (ReportType.MCH2Summary, "Tableau des immobilisations MCH2");
				yield return new Report (ReportType.AssetsList,  "Liste des objets d'immobilisations");
				yield return new Report (ReportType.PersonsList, "Liste des personnes");
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
		#endregion
	}
}