//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationDefinition
	{
		public AmortizationDefinition(int rank, AmortizationMethod method, decimal rate, AmortizationType type, int yearCount, Periodicity periodicity, ProrataType prorataType, decimal round, decimal residual)
		{
			this.Rank        = rank;
			this.Method      = method;
			this.Rate        = rate;
			this.Type        = type;
			this.YearCount   = yearCount;
			this.Periodicity = periodicity;
			this.ProrataType = prorataType;
			this.Round       = round;
			this.Residual    = residual;
		}

		public decimal							EffectiveRate
		{
			get
			{
				return this.Rate * this.PeriodMonthCount / 12.0m;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Method      == AmortizationMethod.Unknown
					&& this.Rate        == 0.0m
					&& this.Type        == AmortizationType.Unknown
					&& this.YearCount   == 0
					&& this.Periodicity == 0
					&& this.ProrataType == 0
					&& this.Round       == 0.0m
					&& this.Residual    == 0.0m;
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.Error == ErrorType.Ok;
			}
		}

		public ErrorType						Error
		{
			get
			{
				if (this.Rate < 0.0m || this.Rate > 1.0m)
				{
					return ErrorType.AmortizationInvalidRate;
				}

				if (this.Type == AmortizationType.Unknown)
				{
					return ErrorType.AmortizationInvalidType;
				}

				if (this.PeriodMonthCount == -1)
				{
					return ErrorType.AmortizationInvalidPeriod;
				}

				return ErrorType.Ok;
			}
		}


		public int								PeriodMonthCount
		{
			get
			{
				return AmortizationDefinition.GetPeriodMonthCount (this.Periodicity);
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

		public static int GetPeriodMonthCount(Periodicity period)
		{
			switch (period)
			{
				case Periodicity.Annual:
					return 12;

				case Periodicity.Semestrial:
					return 6;

				case Periodicity.Trimestrial:
					return 3;

				case Periodicity.Mensual:
					return 1;

				default:
					return -1;
			}
		}


		public static AmortizationDefinition Empty = new AmortizationDefinition (0, AmortizationMethod.Unknown, 0.0m, AmortizationType.Unknown, 0, 0, 0.0m, 0.0m, 0.0m);

		public readonly int						Rank;
		public readonly AmortizationMethod		Method;
		public readonly decimal					Rate;
		public readonly AmortizationType		Type;
		public readonly int						YearCount;
		public readonly Periodicity				Periodicity;
		public readonly ProrataType				ProrataType;
		public readonly decimal					Round;
		public readonly decimal					Residual;
	}
}
