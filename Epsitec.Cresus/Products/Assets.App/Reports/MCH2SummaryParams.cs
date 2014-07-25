//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryParams : AbstractParams
	{
		public MCH2SummaryParams(DateRange dateRange, Guid rootGuid, int? level)
		{
			this.DateRange = dateRange;
			this.RootGuid  = rootGuid;
			this.Level     = level;
		}

		public MCH2SummaryParams()
		{
			var year = Timestamp.Now.Date.Year;  // année en cours

			var i = new System.DateTime (year,   1, 1);  // 1 janvier
			var f = new System.DateTime (year+1, 1, 1);  // 1 janvier de l'année suivante

			this.DateRange = new DateRange (i, f);
			this.RootGuid  = Guid.Empty;
			this.Level     = 1;
		}

		public override bool StrictlyEquals(AbstractParams other)
		{
			if (other is MCH2SummaryParams)
			{
				var o = other as MCH2SummaryParams;

				return this.DateRange == o.DateRange
					&& this.RootGuid  == o.RootGuid;
			}

			return false;
		}

		public readonly DateRange				DateRange;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
	}
}
