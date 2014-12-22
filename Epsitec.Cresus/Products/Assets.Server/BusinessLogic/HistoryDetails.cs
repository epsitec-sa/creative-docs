//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct HistoryDetails
	{
		public HistoryDetails(System.DateTime firstDate, decimal firstAmount,
			System.DateTime baseDate, decimal baseAmount, decimal baseYearCount, decimal inputAmout)
		{
			this.FirstDate     = firstDate;
			this.FirstAmount   = firstAmount;
			this.BaseDate      = baseDate;
			this.BaseAmount    = baseAmount;
			this.BaseYearCount = baseYearCount;
			this.InputAmount   = inputAmout;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.FirstAmount == 0.0m
					&& this.BaseAmount  == 0.0m
					&& this.InputAmount == 0.0m;
			}
		}

		public static HistoryDetails Empty = new HistoryDetails (System.DateTime.MinValue, 0.0m, System.DateTime.MinValue, 0.0m, 0.0m, 0.0m);

		public readonly System.DateTime			FirstDate;
		public readonly decimal					FirstAmount;
		public readonly System.DateTime			BaseDate;
		public readonly decimal					BaseAmount;
		public readonly decimal					BaseYearCount;
		public readonly decimal					InputAmount;
	}
}
