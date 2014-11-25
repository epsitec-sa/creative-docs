//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public struct Data
	{
		//??public Data(
		//??	decimal? forcedAmount,
		//??	decimal baseAmount,
		//??	decimal initialAmount,
		//??	decimal residualAmount,
		//??	decimal roundAmount,
		//??	decimal rate,
		//??	decimal periodicityFactor,
		//??	decimal prorataNumerator,
		//??	decimal prorataDenominator,
		//??	decimal yearCount,
		//??	int yearRank)
		//??{
		//??	this.ForcedAmount       = forcedAmount;
		//??	this.BaseAmount         = baseAmount;
		//??	this.InitialAmount      = initialAmount;
		//??	this.ResidualAmount     = residualAmount;
		//??	this.RoundAmount        = roundAmount;
		//??	this.Rate               = rate;
		//??	this.PeriodicityFactor  = periodicityFactor;
		//??	this.ProrataNumerator   = prorataNumerator;
		//??	this.ProrataDenominator = prorataDenominator;
		//??	this.YearCount          = yearCount;
		//??	this.YearRank           = yearRank;
		//??}

		public Data(AmortizedAmount amount)
		{
			this.ForcedAmount       = amount.ForcedAmount;
			this.BaseAmount         = amount.BaseAmount.GetValueOrDefault ();
			this.InitialAmount      = amount.InitialAmount.GetValueOrDefault ();
			this.ResidualAmount     = amount.ResidualAmount.GetValueOrDefault ();
			this.RoundAmount        = amount.RoundAmount.GetValueOrDefault ();
			this.Rate               = amount.Rate.GetValueOrDefault ();
			this.PeriodicityFactor  = amount.PeriodicityFactor;
			this.ProrataNumerator   = amount.ProrataNumerator.GetValueOrDefault ();
			this.ProrataDenominator = amount.ProrataDenominator.GetValueOrDefault ();
			this.YearCount          = amount.YearCount;
			this.YearRank           = amount.YearRank;
		}

		public decimal Round(decimal value)
		{
			if (this.RoundAmount > 0.0m)
			{
				if (value < 0.0m)
				{
					value -= this.RoundAmount/2.0m;
				}
				else
				{
					value += this.RoundAmount/2.0m;
				}

				value -= (value % this.RoundAmount);
			}

			return value;
		}

		public decimal Residual(decimal value)
		{
			return System.Math.Max (value, this.ResidualAmount);
		}

		public readonly decimal?				ForcedAmount;		// valeur forcée facultative
		public readonly decimal					BaseAmount;			// valeur d'achat
		public readonly decimal					InitialAmount;		// valeur avant amortissement
		public readonly decimal					ResidualAmount;		// valeur résiduelle
		public readonly decimal					RoundAmount;		// arrondi
		public readonly decimal					Rate;				// taux d'amortissement
		public readonly decimal					PeriodicityFactor;	// facteur lié à la périodicité (0.25 si trimestriel par exemple)
		public readonly decimal					ProrataNumerator;	// numérateur du prorata
		public readonly decimal					ProrataDenominator;	// dénominateur du prorata
		public readonly decimal					YearCount;			// nombre total d'années
		public readonly int						YearRank;			// rang de l'année (0..YearCount-1)
	}
}
