//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
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

	}
}
