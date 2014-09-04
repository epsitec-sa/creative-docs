//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.NodeGetters;
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
			this.outputNodes.Clear ();

			var ordered = this.inputGuids.OrderBy (x => this.GetNodeDescription (x)).ToArray ();

			int index = 0;
			foreach (var guid in ordered)
			{
				var currentReport = this.GetReport (guid);

				int level = 0;

				if (index > 0)
				{
					var previousReport = this.GetReport (ordered[index-1]);

					if (previousReport.GetType () == currentReport.GetType ())
					{
						level = 1;
					}
				}

				if (level == 0)
				{
					var title = ReportParamsHelper.GetTitle (this.accessor, currentReport, ReportTitleType.Title);
					this.outputNodes.Add (new ReportNode (title));
				}

				this.outputNodes.Add (new ReportNode (guid));

				index++;
			}
		}

		private string GetNodeDescription(Guid reportGuid)
		{
			var report = this.GetReport (reportGuid);

			if (report == null)
			{
				return "?";
			}
			else
			{
				return ReportParamsHelper.GetTitle (this.accessor, report, ReportTitleType.Specific);
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
