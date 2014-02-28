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


		public decimal? FinalAmount
		{
			//	Calcule la valeur finale après amortissement, selon les paramètres de la structure.
			get
			{
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return this.InitialAmount;
				}
				else
				{
					var amortization = this.BaseAmount.GetValueOrDefault (0.0m) * this.EffectiveRate.GetValueOrDefault (1.0m) * this.Prorata;
					var final = this.InitialAmount.GetValueOrDefault (0.0m) - amortization;
					final = AmortizationDetails.Round (final, this.RoundAmount.GetValueOrDefault (0.0m));
					return System.Math.Max (final, this.ResidualAmount.GetValueOrDefault (0.0m));
				}
			}
		}

		private decimal Prorata
		{
			get
			{
				if (this.ProrataNumerator.HasValue &&
					this.ProrataDenominator.HasValue &&
					this.ProrataDenominator.Value != 0.0m)
				{
					return this.ProrataNumerator.Value/ this.ProrataDenominator.Value;
				}
				else
				{
					return 1.0m;
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
