//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public struct AmortizedAmount : System.IEquatable<AmortizedAmount>
	{
		public AmortizedAmount
		(
			AmortizationMethod	amortizationMethod,
			decimal?			rate,
			AmortizationType	amortizationType,
			int					yearRank,
			int					yearCount,
			Periodicity			periodicity,
			decimal?			previousAmount,
			decimal?			initialAmount,
			decimal?			baseAmount,
			decimal?			prorataNumerator,
			decimal?			prorataDenominator,
			decimal?			roundAmount,
			decimal?			residualAmount,
			EntryScenario		entryScenario,
			System.DateTime		date,
			Guid				assetGuid,
			Guid				eventGuid,
			Guid				entryGuid,
			int					entrySeed
		)
		{
			this.AmortizationMethod = amortizationMethod;
			this.Rate               = rate;
			this.AmortizationType   = amortizationType;
			this.YearRank           = yearRank;
			this.YearCount          = yearCount;
			this.Periodicity        = periodicity;
			this.PreviousAmount     = previousAmount;
			this.InitialAmount      = initialAmount;
			this.BaseAmount         = baseAmount;
			this.ProrataNumerator   = prorataNumerator;
			this.ProrataDenominator = prorataDenominator;
			this.RoundAmount        = roundAmount;
			this.ResidualAmount     = residualAmount;
			this.EntryScenario      = entryScenario;
			this.Date               = date;
			this.AssetGuid          = assetGuid;
			this.EventGuid          = eventGuid;
			this.EntryGuid          = entryGuid;
			this.EntrySeed          = entrySeed;
		}

		public AmortizedAmount(System.Xml.XmlReader reader)
		{
			this.AmortizationMethod = (AmortizationMethod) IOHelpers.ReadTypeAttribute (reader, "AmortizationMethod", typeof (AmortizationMethod));
			this.Rate               = IOHelpers.ReadDecimalAttribute (reader, "Rate");
			this.AmortizationType   = (AmortizationType) IOHelpers.ReadTypeAttribute (reader, "AmortizationType", typeof (AmortizationType));
			this.YearRank           = IOHelpers.ReadIntAttribute (reader, "YearRank").GetValueOrDefault ();
			this.YearCount          = IOHelpers.ReadIntAttribute (reader, "YearCount").GetValueOrDefault ();
			this.Periodicity        = (Periodicity) IOHelpers.ReadTypeAttribute (reader, "Periodicity", typeof (Periodicity));
			this.PreviousAmount     = IOHelpers.ReadDecimalAttribute (reader, "PreviousAmount");
			this.InitialAmount      = IOHelpers.ReadDecimalAttribute (reader, "InitialAmount");
			this.BaseAmount         = IOHelpers.ReadDecimalAttribute (reader, "BaseAmount");
			this.ProrataNumerator   = IOHelpers.ReadDecimalAttribute (reader, "ProrataNumerator");
			this.ProrataDenominator = IOHelpers.ReadDecimalAttribute (reader, "ProrataDenominator");
			this.RoundAmount        = IOHelpers.ReadDecimalAttribute (reader, "RoundAmount");
			this.ResidualAmount     = IOHelpers.ReadDecimalAttribute (reader, "ResidualAmount");
			this.EntryScenario      = (EntryScenario) IOHelpers.ReadTypeAttribute (reader, "EntryScenario", typeof (EntryScenario));
			this.Date               = IOHelpers.ReadDateAttribute (reader, "Date").GetValueOrDefault ();
			this.AssetGuid          = IOHelpers.ReadGuidAttribute (reader, "AssetGuid");
			this.EventGuid          = IOHelpers.ReadGuidAttribute (reader, "EventGuid");
			this.EntryGuid          = IOHelpers.ReadGuidAttribute (reader, "EntryGuid");
			this.EntrySeed          = IOHelpers.ReadIntAttribute (reader, "EntrySeed").GetValueOrDefault ();

			reader.Read ();  // on avance plus loin
		}


		public readonly AmortizationMethod		AmortizationMethod;
		public readonly decimal?				Rate;
		public readonly AmortizationType		AmortizationType;
		public readonly int						YearRank;
		public readonly int						YearCount;
		public readonly Periodicity				Periodicity;
		public readonly decimal?				PreviousAmount;
		public readonly decimal?				InitialAmount;
		public readonly decimal?				BaseAmount;
		public readonly decimal?				ProrataNumerator;
		public readonly decimal?				ProrataDenominator;
		public readonly decimal?				RoundAmount;
		public readonly decimal?				ResidualAmount;
		public readonly EntryScenario			EntryScenario;
		public readonly System.DateTime			Date;
		public readonly Guid					AssetGuid;
		public readonly Guid					EventGuid;
		public readonly Guid					EntryGuid;
		public readonly int						EntrySeed;


		public decimal?							EffectiveRate
		{
			get
			{
				if (this.AmortizationMethod == AmortizationMethod.Rate)
				{
					return this.BaseRate * this.ProrataFactor * this.PeriodicityFactor;
				}
				else
				{
					return this.BaseRate;
				}
			}
		}

		public decimal?							BaseRate
		{
			//	Calcule le taux de l'amortissement. C'est ici que se cachent les formules plus
			//	ou moins subtiles de calcul de l'amortissement, qu'il faudra un jour étendre.
			//	Le taux calculé ne tient pas compte du prorata.
			//	Le taux calculé ne tient pas compte de la périodicité en mode AmortizationMethod.Rate.
			get
			{
				var rate = 0.0m;

				if (this.AmortizationMethod == AmortizationMethod.Rate)
				{
					//	Amortissement selon le taux.
					rate = this.Rate.GetValueOrDefault (0.0m);
				}
				else if (this.AmortizationMethod == AmortizationMethod.YearCount)
				{
					//	Amortissement selon le nombre d'années.
					int n = this.YearCountPeriodicity - this.YearRank;  // nb d'années restantes

					if (n <= 0)
					{
						rate = 1.0m;
					}
					else
					{
						if (this.AmortizationType == AmortizationType.Linear)
						{
							rate = 1.0m / (decimal) n;
						}
						else if (this.AmortizationType == AmortizationType.Degressive)
						{
							var x = this.ResidualAmount / this.InitialAmount.GetValueOrDefault (0.0m);
							var y = 1.0m / n;
							rate = 1.0m - (decimal) System.Math.Pow ((double) x, (double) y);
						}
					}
				}

				return rate;
			}
		}

		public string							BaseRateSummary
		{
			//	Retourne un texte expliquant la façon dont est calculé BaseRate.
			get
			{
				if (this.AmortizationMethod == AmortizationMethod.Rate)
				{
					//	Amortissement selon le taux.
					return Res.Strings.AmortizedAmount.BaseRateSummary.Rate.ToString ();
				}
				else if (this.AmortizationMethod == AmortizationMethod.YearCount)
				{
					//	Amortissement selon le nombre d'années.
					int n = this.YearCountPeriodicity - this.YearRank;  // nb d'années restantes

					if (n > 0)
					{
						if (this.AmortizationType == AmortizationType.Linear)
						{
							return string.Format (Res.Strings.AmortizedAmount.BaseRateSummary.YearCount.Linear.ToString (),
								this.YearCountPeriodicity.ToString (System.Globalization.CultureInfo.InvariantCulture),
								this.YearRank.ToString (System.Globalization.CultureInfo.InvariantCulture));
						}
						else if (this.AmortizationType == AmortizationType.Degressive)
						{
							return string.Format (Res.Strings.AmortizedAmount.BaseRateSummary.YearCount.Degressive.ToString (),
								this.ResidualAmount.GetValueOrDefault (0.0m).ToString ("0.00", System.Globalization.CultureInfo.InvariantCulture),
								this.InitialAmount.GetValueOrDefault (0.0m).ToString ("0.00", System.Globalization.CultureInfo.InvariantCulture),
								this.YearCountPeriodicity.ToString (System.Globalization.CultureInfo.InvariantCulture),
								this.YearRank.ToString (System.Globalization.CultureInfo.InvariantCulture));
						}
					}
				}

				return null;
			}
		}

		private int								YearCountPeriodicity
		{
			get
			{
				return this.YearCount * 12 / this.PeriodMonthCount;
			}
		}

		public decimal							ProrataFactor
		{
			//	Retourne le facteur multiplicateur "au prorata", compris entre 0 et 1.
			get
			{
				if (this.ProrataNumerator.HasValue &&
					this.ProrataDenominator.HasValue &&
					this.ProrataDenominator.Value != 0.0m)
				{
					var prorata = this.ProrataNumerator.Value/ this.ProrataDenominator.Value;

					prorata = System.Math.Max (prorata, 0.0m);
					prorata = System.Math.Min (prorata, 1.0m);  // garde-fou

					return prorata;
				}
				else
				{
					return 1.0m;  // 100%
				}
			}
		}

		private decimal							PeriodicityFactor
		{
			//	Retourne le facteur multiplicateur pour la périodicité, compris entre 0 et 1.
			//	Annuel      -> 12/12
			//	Semestriel  ->  6/12
			//	Trimestriel ->  3/12
			//	Mensuel     ->  1/12
			get
			{
				return this.PeriodMonthCount / 12.0m;
			}
		}


		public decimal?							FinalAmortizedAmount
		{
			//	Calcule la valeur amortie finale, en tenant compte de l'arrondi et de la
			//	valeur résiduelle.
			get
			{
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return this.InitialAmount;
				}
				else
				{
					var rounded = this.RoundedAmortizedAmount;

					if (rounded.HasValue && this.ResidualAmount.HasValue)
					{
						return System.Math.Max (rounded.Value, this.ResidualAmount.Value);
					}
					else
					{
						return rounded;
					}
				}
			}
		}

		public decimal?							RoundedAmortizedAmount
		{
			//	Calcule la valeur amortie arrondie, sans tenir compte de la valeur résiduelle.
			get
			{
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return this.InitialAmount;
				}
				else
				{
					var brut = this.BrutAmortizedAmount;

					if (brut.HasValue && this.RoundAmount.HasValue)
					{
						return AmortizedAmount.Round (brut.Value, this.RoundAmount.Value);
					}
					else
					{
						return brut;
					}
				}
			}
		}

		public decimal?							BrutAmortizedAmount
		{
			//	Calcule la valeur amortie, sans tenir compte de l'arrondi ni de la valeur
			//	résiduelle.
			get
			{
				return System.Math.Max (this.InitialAmount.GetValueOrDefault (0.0m) - this.BrutAmortization, 0.0m);
			}
		}

		public decimal							FinalAmortization
		{
			//	Calcule l'amortissement final effectif.
			get
			{
				return this.InitialAmount.GetValueOrDefault (0.0m)
					 - this.FinalAmortizedAmount.GetValueOrDefault (0.0m);
			}
		}

		public decimal							BrutAmortization
		{
			//	Calcule l'amortissement brut, qu'il faudra soustraire à la valeur initiale
			//	pour obtenir la valeur amortie.
			get
			{
				if (this.AmortizationMethod == Data.AmortizationMethod.Rate)
				{
					if (this.AmortizationType == AmortizationType.Linear)
					{
						decimal value = this.BaseAmount.GetValueOrDefault (0.0m);
						return value * this.EffectiveRate.GetValueOrDefault (1.0m);
					}
					else if (this.AmortizationType == AmortizationType.Degressive)
					{
						decimal value = this.InitialAmount.GetValueOrDefault (0.0m);
						return value * this.EffectiveRate.GetValueOrDefault (1.0m);
					}
				}
				else if (this.AmortizationMethod == Data.AmortizationMethod.YearCount)
				{
					decimal value = this.InitialAmount.GetValueOrDefault (0.0m);
					return value * this.EffectiveRate.GetValueOrDefault (1.0m);
				}

				return 0.0m;
			}
		}


		#region IEquatable<AmortizedAmount> Members
		public bool Equals(AmortizedAmount other)
		{
			return this.AmortizationMethod == other.AmortizationMethod
				&& this.Rate               == other.Rate
				&& this.AmortizationType   == other.AmortizationType
				&& this.YearRank           == other.YearRank
				&& this.YearCount          == other.YearCount
				&& this.Periodicity        == other.Periodicity
				&& this.PreviousAmount     == other.PreviousAmount
				&& this.InitialAmount      == other.InitialAmount
				&& this.BaseAmount         == other.BaseAmount
				&& this.ProrataNumerator   == other.ProrataNumerator
				&& this.ProrataDenominator == other.ProrataDenominator
				&& this.RoundAmount        == other.RoundAmount
				&& this.ResidualAmount     == other.ResidualAmount
				&& this.EntryScenario      == other.EntryScenario
				&& this.Date               == other.Date
				&& this.AssetGuid          == other.AssetGuid
				&& this.EventGuid          == other.EventGuid
				&& this.EntryGuid          == other.EntryGuid
				&& this.EntrySeed          == other.EntrySeed;
		}
		#endregion

		public override bool Equals(object obj)
		{
			if (obj is AmortizedAmount)
			{
				return this.Equals ((AmortizedAmount) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.AmortizationMethod.GetHashCode ()
				 ^ this.Rate              .GetHashCode ()
				 ^ this.AmortizationType  .GetHashCode ()
				 ^ this.YearRank          .GetHashCode ()
				 ^ this.YearCount         .GetHashCode ()
				 ^ this.Periodicity       .GetHashCode ()
				 ^ this.PreviousAmount    .GetHashCode ()
				 ^ this.InitialAmount     .GetHashCode ()
				 ^ this.BaseAmount        .GetHashCode ()
				 ^ this.ProrataNumerator  .GetHashCode ()
				 ^ this.ProrataDenominator.GetHashCode ()
				 ^ this.RoundAmount       .GetHashCode ()
				 ^ this.ResidualAmount    .GetHashCode ()
				 ^ this.EntryScenario     .GetHashCode ()
				 ^ this.Date              .GetHashCode ()
				 ^ this.AssetGuid         .GetHashCode ()
				 ^ this.EventGuid         .GetHashCode ()
				 ^ this.EntryGuid         .GetHashCode ()
				 ^ this.EntrySeed         .GetHashCode ();
		}

		public static bool operator ==(AmortizedAmount a, AmortizedAmount b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(AmortizedAmount a, AmortizedAmount b)
		{
			return !(a == b);
		}


		#region Constructors helpers
		public static AmortizedAmount SetAmortizationMethod(AmortizedAmount model, AmortizationMethod method)
		{
			return new AmortizedAmount
			(
				method,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetAmortizationType(AmortizedAmount model, AmortizationType value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				value,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetInitialAmount(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				value,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetBaseAmount(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				value,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetRate(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				value,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetProrataNumerator(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				value,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetProrataDenominator(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				value,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetRoundAmount(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				value,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetResidualAmount(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				value,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetEntryScenario(AmortizedAmount model, EntryScenario value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				value,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetDate(AmortizedAmount model, System.DateTime value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				value,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetAssetGuid(AmortizedAmount model, Guid value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				value,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetEventGuid(AmortizedAmount model, Guid value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				value,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetEntryGuid(AmortizedAmount model, Guid value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				value,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetEntrySeed(AmortizedAmount model, int value)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				value
			);
		}

		public static AmortizedAmount SetEntry(AmortizedAmount model, Guid entryGuid, int entrySeed)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				entryGuid,
				entrySeed
			);
		}

		public static AmortizedAmount SetPreview(AmortizedAmount model, decimal? rate,
			AmortizationType amortizationType,
			decimal? prorataNumerator, decimal? prorataDenominator,
			Periodicity periodicity,
			decimal? roundAmount, decimal? residualAmount, EntryScenario entryScenario)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				rate,
				amortizationType,
				model.YearRank,
				model.YearCount,
				periodicity,
				model.PreviousAmount,
				model.InitialAmount,
				model.BaseAmount,
				prorataNumerator,
				prorataDenominator,
				roundAmount,
				residualAmount,
				entryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetPreviousAmount(AmortizedAmount model, decimal? previousAmount)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				previousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetAmortizedAmount(AmortizedAmount model, decimal? initialAmount, decimal? baseAmount)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				model.AmortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				initialAmount,
				initialAmount,
				baseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetAmortizedAmount(AmortizedAmount model,
			AmortizationType amortizationType, decimal? initialAmount, decimal? baseAmount)
		{
			return new AmortizedAmount
			(
				model.AmortizationMethod,
				model.Rate,
				amortizationType,
				model.YearRank,
				model.YearCount,
				model.Periodicity,
				initialAmount,
				initialAmount,
				baseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}
		#endregion


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteTypeAttribute    (writer, "AmortizationMethod", this.AmortizationMethod);
			IOHelpers.WriteDecimalAttribute (writer, "Rate",               this.Rate);
			IOHelpers.WriteTypeAttribute    (writer, "AmortizationType",   this.AmortizationType);
			IOHelpers.WriteIntAttribute     (writer, "YearRank",           this.YearRank);
			IOHelpers.WriteIntAttribute     (writer, "YearCount",          this.YearCount);
			IOHelpers.WriteTypeAttribute    (writer, "Periodicity",        this.Periodicity);

			IOHelpers.WriteDecimalAttribute (writer, "PreviousAmount",     this.PreviousAmount);
			IOHelpers.WriteDecimalAttribute (writer, "InitialAmount",      this.InitialAmount);
			IOHelpers.WriteDecimalAttribute (writer, "BaseAmount",         this.BaseAmount);
			IOHelpers.WriteDecimalAttribute (writer, "ProrataNumerator",   this.ProrataNumerator);
			IOHelpers.WriteDecimalAttribute (writer, "ProrataDenominator", this.ProrataDenominator);
			IOHelpers.WriteDecimalAttribute (writer, "RoundAmount",        this.RoundAmount);
			IOHelpers.WriteDecimalAttribute (writer, "ResidualAmount",     this.ResidualAmount);

			IOHelpers.WriteTypeAttribute (writer, "EntryScenario", this.EntryScenario);

			IOHelpers.WriteDateAttribute (writer, "Date", this.Date);
			IOHelpers.WriteGuidAttribute (writer, "AssetGuid", this.AssetGuid);
			IOHelpers.WriteGuidAttribute (writer, "EventGuid", this.EventGuid);
			IOHelpers.WriteGuidAttribute (writer, "EntryGuid", this.EntryGuid);
			IOHelpers.WriteIntAttribute  (writer, "EntrySeed", this.EntrySeed);

			writer.WriteEndElement ();
		}


		private int PeriodMonthCount
		{
			get
			{
				return AmortizedAmount.GetPeriodMonthCount (this.Periodicity);
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

		private static decimal Round(decimal value, decimal round)
		{
			//	Retourne un montant arrondi.
			if (round > 0.0m)
			{
				if (value < 0)
				{
					value -= round/2;
				}
				else
				{
					value += round/2;
				}

				return value - (value % round);
			}
			else
			{
				return value;
			}
		}
	}
}
