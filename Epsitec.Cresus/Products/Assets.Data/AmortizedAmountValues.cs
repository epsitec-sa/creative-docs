//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	/// <summary>
	/// Il y a ici toute l'information permettant de calculer un amortissement,
	/// ainsi que l'information permettant de générer l'écriture correspondante.
	/// </summary>
	public class AmortizedAmountValues
	{
		public AmortizationType					AmortizationType;
		public decimal?							InitialAmount;
		public decimal?							BaseAmount;
		public decimal?							EffectiveRate;
		public decimal?							ProrataNumerator;
		public decimal?							ProrataDenominator;
		public decimal?							RoundAmount;
		public decimal?							ResidualAmount;
		public EntryScenario					EntryScenario;
		public System.DateTime					Date;
		public Guid								AssetGuid;
		public Guid								EventGuid;
		public Guid								EntryGuid;
		public int								EntrySeed;

		public static bool operator ==(AmortizedAmountValues a, AmortizedAmountValues b)
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

		public static bool operator !=(AmortizedAmountValues a, AmortizedAmountValues b)
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
			if (obj is AmortizedAmountValues)
			{
				return this.Equals ((AmortizedAmountValues) obj);
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
	}
}
