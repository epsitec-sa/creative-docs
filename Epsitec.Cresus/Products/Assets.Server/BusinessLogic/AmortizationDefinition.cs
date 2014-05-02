//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationDefinition
	{
		public AmortizationDefinition(decimal rate, AmortizationType type, Periodicity periodicity, ProrataType prorataType, decimal round, decimal residual)
		{
			this.Rate        = rate;
			this.Type        = type;
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
				return this.Rate        == 0.0m
					&& this.Type        == AmortizationType.Unknown
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


		public string GetFullName()
		{
			var stringTaux     = TypeConverters.RateToString (this.Rate);
			var stringType     = EnumDictionaries.GetAmortizationTypeName (this.Type);
			var stringPeriod   = EnumDictionaries.GetPeriodicityName (this.Periodicity);
			var stringRound    = TypeConverters.AmountToString (this.Round);
			var stringResidual = TypeConverters.AmountToString (this.Residual);
			var stringProrata  = EnumDictionaries.GetProrataTypeName (this.ProrataType);

			return string.Format ("{0} {1} — Périodicité {2} — Arrondi {3} — Valeur résiduelle {4} — Au prorata {5}",
				stringTaux, stringType, stringPeriod, stringRound, stringResidual, stringProrata);
		}


		public static AmortizationDefinition Empty = new AmortizationDefinition (0.0m, AmortizationType.Unknown, 0, 0.0m, 0.0m, 0.0m);

		public readonly decimal					Rate;
		public readonly AmortizationType		Type;
		public readonly Periodicity				Periodicity;
		public readonly ProrataType				ProrataType;
		public readonly decimal					Round;
		public readonly decimal					Residual;
	}
}
