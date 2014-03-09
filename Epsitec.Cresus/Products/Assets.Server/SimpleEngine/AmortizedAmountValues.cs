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
		public Guid								EntryGuid;
	}
}
