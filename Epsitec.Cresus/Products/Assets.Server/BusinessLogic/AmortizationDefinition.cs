//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationDefinition
	{
		public AmortizationDefinition(AmortizationMethod method, string expression,
			decimal rate, decimal yearCount,
			Periodicity periodicity, ProrataType prorataType,
			decimal round, decimal residual, decimal startYearAmount)
		{
			this.Method          = method;
			this.Expression      = expression;
			this.Rate            = rate;
			this.YearCount       = yearCount;
			this.Periodicity     = periodicity;
			this.ProrataType     = prorataType;
			this.Round           = round;
			this.Residual        = residual;
			this.StartYearAmount = startYearAmount;
		}

		public bool								None
		{
			//	Retourne true s'il ne faut pas générer d'amortissement.
			get
			{
				return this.Rate == 0.0m;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Method          == AmortizationMethod.Unknown
					&& this.Rate            == 0.0m
					&& this.YearCount       == 0.0m
					&& this.Periodicity     == 0
					&& this.ProrataType     == 0
					&& this.Round           == 0.0m
					&& this.Residual        == 0.0m
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


		public static AmortizationDefinition SetMethod(AmortizationDefinition model, AmortizationMethod method)
		{
			return new AmortizationDefinition (
				method,
				model.Expression,
				model.Rate,
				model.YearCount,
				model.Periodicity,
				model.ProrataType,
				model.Round,
				model.Residual,
				model.StartYearAmount);
		}


		public static AmortizationDefinition Empty = new AmortizationDefinition (AmortizationMethod.Unknown, null, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0, 0.0m);

		public readonly AmortizationMethod		Method;
		public readonly string					Expression;
		public readonly decimal					Rate;
		public readonly decimal					YearCount;
		public readonly Periodicity				Periodicity;
		public readonly ProrataType				ProrataType;
		public readonly decimal					Round;
		public readonly decimal					Residual;
		public readonly decimal					StartYearAmount;
	}
}
