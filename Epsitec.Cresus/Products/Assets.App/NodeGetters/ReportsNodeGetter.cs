//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class ReportsNodeGetter : INodeGetter<GuidNode>  // outputNodes
	{
		public ReportsNodeGetter(DataAccessor accessor)
		{
			this.accessor   = accessor;
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
				return this.outputGuids.Length;
			}
		}

		public GuidNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.outputGuids.Length)
				{
					var guid = this.outputGuids[index];
					return new GuidNode (guid);
				}
				else
				{
					return GuidNode.Empty;
				}
			}
		}


		private void Update()
		{
			this.outputGuids = this.inputGuids.OrderBy (x => this.GetNodeDescription (x)).ToArray ();
		}

		private string GetNodeDescription(Guid reportGuid)
		{
			var report = this.accessor.Mandat.Reports[reportGuid];

			if (report == null)
			{
				return "?";
			}
			else
			{
				return ReportParamsHelper.GetTitle (this.accessor, report);
			}
		}


		private readonly DataAccessor			accessor;

		private Guid[]							inputGuids;
		private Guid[]							outputGuids;
	}
}
