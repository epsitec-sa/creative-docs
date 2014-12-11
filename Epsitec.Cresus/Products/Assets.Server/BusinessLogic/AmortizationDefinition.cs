//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationDefinition
	{
		public AmortizationDefinition(DateRange range, System.DateTime currentDate,
			string arguments, string expression,
			Periodicity periodicity)
		{
			this.Range           = range;
			this.CurrentDate     = currentDate;
			this.Arguments       = arguments;
			this.Expression      = expression;
			this.Periodicity     = periodicity;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Range.IsEmpty
					|| string.IsNullOrEmpty (this.Expression)
					|| this.Periodicity == Periodicity.Unknown;
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

		public static System.DateTime GetBeginRangeDate(System.DateTime date, int periodMonthCount)
		{
			//	Retourne la date de début d'une période d'amortissement.
			//	Avec une périodicité Annual (12), c'est le 1er janvier.
			//	Avec une périodicité Semestrial (6), c'est le 1er janvier ou le 1er juillet.
			//	Etc.
			int c = periodMonthCount;
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
				model.Range,
				model.CurrentDate,
				arguments,
				expression,
				model.Periodicity);
		}


		public static AmortizationDefinition Empty = new AmortizationDefinition (DateRange.Empty, System.DateTime.MinValue, null, null, Periodicity.Unknown);

		public readonly DateRange				Range;
		public readonly System.DateTime			CurrentDate;
		public readonly string					Arguments;
		public readonly string					Expression;
		public readonly Periodicity				Periodicity;
	}
}
