//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationData
	{
		public AmortizationData(decimal rate, AmortizationType type, Periodicity period, decimal rest)
		{
			this.Rate   = rate;
			this.Type   = type;
			this.Period = period;
			this.Rest   = rest;
		}

		public decimal EffectiveRate
		{
			get
			{
				return this.Rate * this.PeriodMonthCount / 12.0m;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Rate   == 0.0m
					&& this.Type   == AmortizationType.Unknown
					&& this.Period == 0
					&& this.Rest   == 0.0m;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Error == ErrorType.Ok;
			}
		}

		public ErrorType Error
		{
			get
			{
				if (this.Rate <= 0.0m || this.Rate > 1.0m)
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


		private int PeriodMonthCount
		{
			get
			{
				return AmortizationData.GetPeriodMonthCount (this.Period);
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


		public static AmortizationData Empty = new AmortizationData (0.0m, AmortizationType.Unknown, 0, 0.0m);

		public readonly decimal				Rate;
		public readonly AmortizationType	Type;
		public readonly Periodicity			Period;
		public readonly decimal				Rest;
	}
}
