//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryParams : AbstractParams
	{
		public MCH2SummaryParams(Timestamp initialTimestamp, Timestamp finalTimestamp, Guid rootGuid, int? level)
		{
			this.InitialTimestamp = initialTimestamp;
			this.FinalTimestamp   = finalTimestamp;
			this.RootGuid         = rootGuid;
			this.Level            = level;
		}

		public MCH2SummaryParams()
		{
			var year = Timestamp.Now.Date.Year;  // année en cours
			this.InitialTimestamp = new Timestamp (new System.DateTime (year,  1,  1), 0);  // 1 janvier
			this.FinalTimestamp   = new Timestamp (new System.DateTime (year, 12, 31), 0);  // 31 décembre
			this.RootGuid         = Guid.Empty;
			this.Level            = 1;
		}

		public override bool StrictlyEquals(AbstractParams other)
		{
			if (other is MCH2SummaryParams)
			{
				var o = other as MCH2SummaryParams;

				return this.InitialTimestamp == o.InitialTimestamp
					&& this.FinalTimestamp   == o.FinalTimestamp
					&& this.RootGuid         == o.RootGuid;
			}

			return false;
		}

		public readonly Timestamp				InitialTimestamp;
		public readonly Timestamp				FinalTimestamp;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
	}
}
