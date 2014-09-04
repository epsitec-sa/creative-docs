//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class ReportsNodeGetter : INodeGetter<ReportNode>  // outputNodes
	{
		public ReportsNodeGetter(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.outputNodes = new List<ReportNode> ();
		}

		public void SetParams(IEnumerable<Guid> inputGuids)
		{
			this.inputGuids = inputGuids.ToArray ();
			this.Update ();
		}


		public int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public ReportNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.outputNodes.Count)
				{
					return this.outputNodes[index];
				}
				else
				{
					return ReportNode.Empty;
				}
			}
		}


		private void Update()
		{
			//	On crée d'abord une liste de ReportNode qui reflète exactement les
			//	Guids donnés en entrée (même nombre et même ordre). Cela demande de
			//	générer les textes de descriptions.
			var reportNodes = new List<ReportNode> ();

			foreach (var guid in this.inputGuids)
			{
				var report = this.GetReport (guid);

				if (report != null)
				{
					var title = ReportParamsHelper.GetTitle (this.accessor, report, ReportTitleType.Title);
					var desc  = ReportParamsHelper.GetTitle (this.accessor, report, ReportTitleType.Specific);
					var sort  = string.Concat (title, "_$$_", desc);

					var node = new ReportNode (desc, sort, guid);
					reportNodes.Add (node);
				}
			}

			//	On crée une liste triée par ordre alphabétique des descriptions.
			var orderedNodes = reportNodes.OrderBy (x => x.SortableDescription).ToArray ();

			//	Avant chaque nouveau type de rapport, on ajoute une ligne de titre.
			AbstractReportParams lastReport = null;
			int index = 0;
			this.outputNodes.Clear ();

			foreach (var node in orderedNodes)
			{
				var currentReport = this.GetReport (node.Guid);

				int level = 0;

				if (index > 0)
				{
					if (lastReport.GetType () == currentReport.GetType ())
					{
						level = 1;
					}
				}

				if (level == 0)
				{
					var title = ReportParamsHelper.GetTitle (this.accessor, currentReport, ReportTitleType.Title);
					this.outputNodes.Add (new ReportNode (title, title, Guid.Empty));
				}

				this.outputNodes.Add (new ReportNode (node.Description, node.SortableDescription, node.Guid));

				lastReport = currentReport;
				index++;
			}
		}

		private AbstractReportParams GetReport(Guid reportGuid)
		{
			return this.accessor.Mandat.Reports[reportGuid];
		}


		private readonly DataAccessor			accessor;
		private readonly List<ReportNode>		outputNodes;

		private Guid[]							inputGuids;
	}
}
