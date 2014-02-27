//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct AmortizedAmount
	{
		public AmortizedAmount(decimal? initialAmount, decimal? baseAmount, decimal? effectiveRate,
			decimal? prorataNumerator, decimal? prorataDenominator,
			decimal? roundAmount, decimal? finalAmount, AmortizationType amortizationType)
		{
			this.InitialAmount      = initialAmount;
			this.BaseAmount         = baseAmount;
			this.EffectiveRate      = effectiveRate;
			this.ProrataNumerator   = prorataNumerator;
			this.ProrataDenominator = prorataDenominator;
			this.RoundAmount        = roundAmount;
			this.FinalAmount        = finalAmount;
			this.AmortizationType   = amortizationType;
		}

		public AmortizedAmount(decimal? finalAmount)
		{
			this.InitialAmount      = null;
			this.BaseAmount         = null;
			this.EffectiveRate      = null;
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.FinalAmount        = finalAmount;
			this.AmortizationType   = AmortizationType.Unknown;
		}

		public AmortizedAmount(decimal? initialAmount, decimal? finalAmount)
		{
			this.InitialAmount      = initialAmount;
			this.BaseAmount         = initialAmount;
			this.EffectiveRate      = 1.0m - (finalAmount / initialAmount);
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.FinalAmount        = finalAmount;
			this.AmortizationType   = AmortizationType.Degressive;
		}


		public static bool operator ==(AmortizedAmount a, AmortizedAmount b)
		{
			return (a.InitialAmount      == b.InitialAmount)
				&& (a.BaseAmount         == b.BaseAmount)
				&& (a.EffectiveRate      == b.EffectiveRate)
				&& (a.ProrataNumerator   == b.ProrataNumerator)
				&& (a.ProrataDenominator == b.ProrataDenominator)
				&& (a.RoundAmount        == b.RoundAmount)
				&& (a.FinalAmount        == b.FinalAmount)
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
				|| (a.FinalAmount        != b.FinalAmount)
				|| (a.AmortizationType   != b.AmortizationType);
		}


		public readonly decimal?				InitialAmount;
		public readonly decimal?				BaseAmount;
		public readonly decimal?				EffectiveRate;
		public readonly decimal?				ProrataNumerator;
		public readonly decimal?				ProrataDenominator;
		public readonly decimal?				RoundAmount;
		public readonly decimal?				FinalAmount;
		public readonly AmortizationType		AmortizationType;
	}
}
