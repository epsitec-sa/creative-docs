//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct AmortizedAmount
	{
		public AmortizedAmount(AmortizationType amortizationType, decimal? initialAmount,
			decimal? baseAmount, decimal? effectiveRate,
			decimal? prorataNumerator, decimal? prorataDenominator,
			decimal? roundAmount, decimal? residualAmount)
		{
			this.AmortizationType   = amortizationType;
			this.InitialAmount      = initialAmount;
			this.BaseAmount         = baseAmount;
			this.EffectiveRate      = effectiveRate;
			this.ProrataNumerator   = prorataNumerator;
			this.ProrataDenominator = prorataDenominator;
			this.RoundAmount        = roundAmount;
			this.ResidualAmount     = residualAmount;
		}

		public AmortizedAmount(decimal? finalAmount)
		{
			//	Initialise un montant initial fixe.
			this.AmortizationType   = AmortizationType.Unknown;
			this.InitialAmount      = finalAmount;
			this.BaseAmount         = null;
			this.EffectiveRate      = null;
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.ResidualAmount     = null;
		}

		public AmortizedAmount(decimal? initialAmount, decimal? finalAmount)
		{
			//	Initialise un montant amorti dégressivement.
			this.AmortizationType   = AmortizationType.Degressive;
			this.InitialAmount      = initialAmount;
			this.BaseAmount         = initialAmount;
			this.EffectiveRate      = 1.0m - (finalAmount / initialAmount);
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.ResidualAmount     = null;
		}


		public decimal? FinalAmortizedAmount
		{
			//	Calcule la valeur amortie finale, en tenant compte de l'arrondi et de la
			//	valeur résiduelle.
			get
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

		public decimal? RoundedAmortizedAmount
		{
			//	Calcule la valeur amortie arrondie, sans tenir compte de la valeur résiduelle.
			get
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

		public decimal? BrutAmortizedAmount
		{
			//	Calcule la valeur amortie, sans tenir compte de l'arrondi ni de la valeur
			//	résiduelle.
			get
			{
				return this.InitialAmount.GetValueOrDefault (0.0m) - this.BrutAmortization;
			}
		}

		public decimal BrutAmortization
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
					return this.BaseAmount.GetValueOrDefault (0.0m)
						 * this.EffectiveRate.GetValueOrDefault (1.0m)
						 * this.Prorata;
				}
			}
		}

		public decimal Prorata
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
			return (a.InitialAmount      == b.InitialAmount)
				&& (a.BaseAmount         == b.BaseAmount)
				&& (a.EffectiveRate      == b.EffectiveRate)
				&& (a.ProrataNumerator   == b.ProrataNumerator)
				&& (a.ProrataDenominator == b.ProrataDenominator)
				&& (a.RoundAmount        == b.RoundAmount)
				&& (a.ResidualAmount     == b.ResidualAmount)
				&& (a.AmortizationType   == b.AmortizationType);
		}

		public static bool operator !=(AmortizedAmount a, AmortizedAmount b)
		{
			return (a.InitialAmount      != b.InitialAmount)
				|| (a.BaseAmount         != b.BaseAmount)
				|| (a.EffectiveRate      != b.EffectiveRate)
				|| (a.ProrataNumerator   != b.ProrataNumerator)
				|| (a.ProrataDenominator != b.ProrataDenominator)
				|| (a.RoundAmount        != b.RoundAmount)
				|| (a.ResidualAmount     != b.ResidualAmount)
				|| (a.AmortizationType   != b.AmortizationType);
		}


		#region Factory
		public static AmortizedAmount CreateType(AmortizedAmount model, AmortizationType type)
		{
			return new AmortizedAmount
			(
				type,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount
			);
		}

		public static AmortizedAmount CreateInitialBase(AmortizedAmount model, decimal? initialAmount, decimal? baseAmount)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				initialAmount.HasValue ? initialAmount : model.InitialAmount,
				baseAmount.HasValue    ? baseAmount    : model.BaseAmount,
				model.EffectiveRate,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount
			);
		}

		public static AmortizedAmount CreateEffectiveRate(AmortizedAmount model, decimal? effectiveRate)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				model.InitialAmount,
				model.BaseAmount,
				effectiveRate,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount
			);
		}

		public static AmortizedAmount CreateProrataNumerator(AmortizedAmount model, decimal? prorataNumerator)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
				prorataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount
			);
		}

		public static AmortizedAmount CreateProrataDenominator(AmortizedAmount model, decimal? prorataDenominator)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
				model.ProrataNumerator,
				prorataDenominator,
				model.RoundAmount,
				model.ResidualAmount
			);
		}

		public static AmortizedAmount CreateRoundAmount(AmortizedAmount model, decimal? roundAmount)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
				model.ProrataNumerator,
				model.ProrataDenominator,
				roundAmount,
				model.ResidualAmount
			);
		}

		public static AmortizedAmount CreateResidualAmount(AmortizedAmount model, decimal? residualAmount)
		{
			return new AmortizedAmount
			(
				model.AmortizationType,
				model.InitialAmount,
				model.BaseAmount,
				model.EffectiveRate,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				residualAmount
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


		public readonly AmortizationType		AmortizationType;
		public readonly decimal?				InitialAmount;
		public readonly decimal?				BaseAmount;
		public readonly decimal?				EffectiveRate;
		public readonly decimal?				ProrataNumerator;
		public readonly decimal?				ProrataDenominator;
		public readonly decimal?				RoundAmount;
		public readonly decimal?				ResidualAmount;
	}
}
