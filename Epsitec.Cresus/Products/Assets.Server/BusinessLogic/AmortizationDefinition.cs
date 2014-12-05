//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationDefinition
	{
		public AmortizationDefinition(string arguments, string expression,
			Periodicity periodicity, decimal startYearAmount)
		{
			this.Arguments       = arguments;
			this.Expression      = expression;
			this.Periodicity     = periodicity;
			this.StartYearAmount = startYearAmount;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Periodicity     == 0
					&& this.StartYearAmount == 0.0m;
			}
		}


		public int								PeriodMonthCount
		{
			get
			{
				return AmortizedAmount.GetPeriodMonthCount (this.Periodicity);
			}
		}

		public System.DateTime GetBeginRangeDate(System.DateTime date)
		{
			//	Retourne la date de début d'une période d'amortissement.
			//	Avec une périodicité Annual (12), c'est le 1er janvier.
			//	Avec une périodicité Semestrial (6), c'est le 1er janvier ou le 1er juillet.
			//	Etc.
			int c = this.PeriodMonthCount;
			if (c > 0)
			{
				int m = date.Year*12 + date.Month-1;

				m = (m/c)*c;

				return new System.DateTime (m/12, m%12+1, 1);
			}
			else
			{
				return date;
			}
		}


		public static AmortizationDefinition SetMethod(AmortizationDefinition model, string arguments, string expression)
		{
			return new AmortizationDefinition (
				arguments,
				expression,
				model.Periodicity,
				model.StartYearAmount);
		}


		public static AmortizationDefinition Empty = new AmortizationDefinition (null, null, Periodicity.Unknown, 0.0m);

		public readonly string					Arguments;
		public readonly string					Expression;
		public readonly Periodicity				Periodicity;
		public readonly decimal					StartYearAmount;
	}
}
