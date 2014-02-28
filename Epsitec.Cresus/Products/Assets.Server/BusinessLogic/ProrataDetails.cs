//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Cette structure contient tous les détails permettant de calculer un
	/// amortissement "au prorata".
	/// </summary>
	public struct ProrataDetails
	{
		private ProrataDetails(DateRange range, System.DateTime? valueDate, decimal? numerator, decimal? denominator, decimal? quotient)
		{
			this.Range       = range;
			this.ValueDate   = valueDate;
			this.Numerator   = numerator;
			this.Denominator = denominator;
			this.Quotient    = quotient;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Range.IsEmpty
					&& !this.ValueDate.HasValue
					&& !this.Numerator.HasValue
					&& !this.Denominator.HasValue
					&& !this.Quotient.HasValue;
			}
		}

		public bool								IsFullPeriod
		{
			get
			{
				return this.Numerator.GetValueOrDefault (0.0m) == 0.0m;
			}
		}

		public static ProrataDetails ComputeProrata(DateRange range, System.DateTime valueDate, ProrataType type)
		{
			//	Retourne le facteur pour le prorata, compris entre 0.0 et 1.0.
			//	Pour une dateValue en début d'année, on retourne 1.0.
			//	Pour une dateValue en milieu d'année, on retourne 0.5.
			//	Pour une dateValue en fin d'année, on retourne 0.0.
			//	Quotient vaut Numerator/Denominator
			int n, d;

			switch (type)
			{
				case ProrataType.None:
					return ProrataDetails.Empty;

				case ProrataType.Prorata12:
					d = ProrataDetails.GetMonthsCount (range.ExcludeTo) - ProrataDetails.GetMonthsCount (range.IncludeFrom);
					n = ProrataDetails.GetMonthsCount (valueDate)       - ProrataDetails.GetMonthsCount (range.IncludeFrom);
					break;

				case ProrataType.Prorata360:
					d = ProrataDetails.GetDaysCount (range.ExcludeTo) - ProrataDetails.GetDaysCount (range.IncludeFrom);
					n = ProrataDetails.GetDaysCount (valueDate)       - ProrataDetails.GetDaysCount (range.IncludeFrom);
					break;

				default:
					d = range.ExcludeTo.Subtract (range.IncludeFrom).Days;
					n = valueDate.Subtract (range.IncludeFrom).Days;
					break;
			}

			n = d - n;
			n = System.Math.Max (n, 0);
			n = System.Math.Min (n, d);
			var quotient = (decimal) n / (decimal) d;
			return new ProrataDetails (range, valueDate, (decimal) n, (decimal) d, quotient);
		}


		private static int GetMonthsCount(System.DateTime date)
		{
			//	Retourne le nombre de mois écoulés depuis le 01.01.0000.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12
				+ (date.Month-1);
		}

		private static int GetDaysCount(System.DateTime date)
		{
			//	Retourne le nombre de jours écoulés depuis le 01.01.0000,
			//	en se basant sur 12 mois à 30 jours par année.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12*30
				+ (date.Month-1)*30
				+ System.Math.Min ((date.Day-1), 30-1);
		}

	
		public static ProrataDetails Empty = new ProrataDetails (DateRange.Empty, null, null, null, null);

		public readonly DateRange				Range;
		public readonly System.DateTime?		ValueDate;
		public readonly decimal?				Numerator;
		public readonly decimal?				Denominator;
		public readonly decimal?				Quotient;
	}
}
