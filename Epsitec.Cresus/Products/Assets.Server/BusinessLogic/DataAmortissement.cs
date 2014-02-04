//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct DataAmortissement
	{
		public DataAmortissement(decimal rate, TypeAmortissement type, Périodicité period, decimal rest)
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
					&& this.Type   == TypeAmortissement.Unknown
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
					return ErrorType.AmortissementInvalidRate;
				}

				if (this.Type == TypeAmortissement.Unknown)
				{
					return ErrorType.AmortissementInvalidType;
				}

				if (this.PeriodMonthCount == -1)
				{
					return ErrorType.AmortissementInvalidPeriod;
				}

				return ErrorType.Ok;
			}
		}


		private int PeriodMonthCount
		{
			get
			{
				return DataAmortissement.GetPeriodMonthCount (this.Period);
			}
		}

		public static int GetPeriodMonthCount(Périodicité period)
		{
			switch (period)
			{
				case Périodicité.Annuel:
					return 12;

				case Périodicité.Semestriel:
					return 6;

				case Périodicité.Trimestriel:
					return 3;

				case Périodicité.Mensuel:
					return 1;

				default:
					return -1;
			}
		}


		public static DataAmortissement Empty = new DataAmortissement (0.0m, TypeAmortissement.Unknown, 0, 0.0m);

		public readonly decimal				Rate;
		public readonly TypeAmortissement	Type;
		public readonly Périodicité			Period;
		public readonly decimal				Rest;
	}
}
