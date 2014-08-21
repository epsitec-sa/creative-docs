//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct AmortizedAmount
	{
		public AmortizedAmount
		(
			AmortizationType	amortizationType,
			decimal?			previousAmount,
			decimal?			initialAmount,
			decimal?			baseAmount,
			decimal?			effectiveRate,
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
			this.AmortizationType   = amortizationType;
			this.PreviousAmount     = previousAmount;
			this.InitialAmount      = initialAmount;
			this.BaseAmount         = baseAmount;
			this.EffectiveRate      = effectiveRate;
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

		public readonly AmortizationType		AmortizationType;
		public readonly decimal?				PreviousAmount;
		public readonly decimal?				InitialAmount;
		public readonly decimal?				BaseAmount;
		public readonly decimal?				EffectiveRate;
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
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return 0.0m;
				}
				else
				{
					decimal value;
					if (this.AmortizationType == AmortizationType.Linear)
					{
						value = this.BaseAmount.GetValueOrDefault (0.0m);
					}
					else
					{
						value = this.InitialAmount.GetValueOrDefault (0.0m);
					}

					return value * this.EffectiveRate.GetValueOrDefault (1.0m) * this.Prorata;
				}
			}
		}

		public decimal							Prorata
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


		public static bool operator ==(AmortizedAmount a, AmortizedAmount b)
		{
			return a.AmortizationType   == b.AmortizationType
				&& a.InitialAmount      == b.InitialAmount
				&& a.BaseAmount         == b.BaseAmount
				&& a.EffectiveRate      == b.EffectiveRate
				&& a.ProrataNumerator   == b.ProrataNumerator
				&& a.ProrataDenominator == b.ProrataDenominator
				&& a.RoundAmount        == b.RoundAmount
				&& a.ResidualAmount     == b.ResidualAmount
				&& a.EntryScenario      == b.EntryScenario
				&& a.Date               == b.Date
				&& a.AssetGuid          == b.AssetGuid
				&& a.EventGuid          == b.EventGuid
				&& a.EntryGuid          == b.EntryGuid
				&& a.EntrySeed          == b.EntrySeed;
		}

		public static bool operator !=(AmortizedAmount a, AmortizedAmount b)
		{
			return a.AmortizationType   != b.AmortizationType
				|| a.InitialAmount      != b.InitialAmount
				|| a.BaseAmount         != b.BaseAmount
				|| a.EffectiveRate      != b.EffectiveRate
				|| a.ProrataNumerator   != b.ProrataNumerator
				|| a.ProrataDenominator != b.ProrataDenominator
				|| a.RoundAmount        != b.RoundAmount
				|| a.ResidualAmount     != b.ResidualAmount
				|| a.EntryScenario      != b.EntryScenario
				|| a.Date               != b.Date
				|| a.AssetGuid          != b.AssetGuid
				|| a.EventGuid          != b.EventGuid
				|| a.EntryGuid          != b.EntryGuid
				|| a.EntrySeed          != b.EntrySeed;
		}

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
			return this.AmortizationType  .GetHashCode ()
				 ^ this.InitialAmount     .GetHashCode ()
				 ^ this.BaseAmount        .GetHashCode ()
				 ^ this.EffectiveRate     .GetHashCode ()
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


		#region Constructors helpers
		public static AmortizedAmount SetAmortizationType(AmortizedAmount model, AmortizationType value)
		{
			return new AmortizedAmount
			(
				value,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				value,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				value,
				model.EffectiveRate,
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

		public static AmortizedAmount SetEffectiveRate(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
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

		public static AmortizedAmount SetProrataNumerator(AmortizedAmount model, decimal? value)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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

		public static AmortizedAmount SetPreview(AmortizedAmount model,
			AmortizationType amortizationType, decimal? effectiveRate,
			decimal? prorataNumerator, decimal? prorataDenominator,
			decimal? roundAmount, decimal? residualAmount, EntryScenario entryScenario)
		{
			return new AmortizedAmount
			(
				amortizationType,
				null,
				model.InitialAmount,
				model.BaseAmount,
				effectiveRate,
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
				model.AmortizationType,
				previousAmount,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
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
				model.AmortizationType,
				null,
				initialAmount,
				baseAmount,
				model.EffectiveRate,
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
			AmortizationType amortizationType, decimal? initialAmount, decimal? baseAmount, decimal? effectiveRate)
		{
			return new AmortizedAmount
			(
				amortizationType,
				null,
				initialAmount,
				baseAmount,
				effectiveRate,
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
