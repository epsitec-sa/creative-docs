//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct HistoryDetails
	{
		public HistoryDetails(decimal baseAmount, decimal initialAmout,
			decimal yearRank, decimal periodRank)
		{
			this.BaseAmount    = baseAmount;
			this.InitialAmount = initialAmout;
			this.YearRank      = yearRank;
			this.PeriodRank    = periodRank;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.BaseAmount    == 0.0m
					&& this.InitialAmount == 0.0m
					&& this.YearRank      == 0.0m
					&& this.PeriodRank    == 0.0m;
			}
		}

		public static HistoryDetails Empty = new HistoryDetails (0.0m, 0.0m, 0.0m, 0.0m);

		public readonly decimal					BaseAmount;
		public readonly decimal					InitialAmount;
		public readonly decimal					YearRank;
		public readonly decimal					PeriodRank;
	}
}
