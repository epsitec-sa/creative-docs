//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Cette structure contient tous les détails permettant de calculer un amortissement.
	/// </summary>
	public struct AmortizationDetails
	{
		public AmortizationDetails(AmortizationDefinition def, ProrataDetails prorata,
								   decimal? initialValue, decimal? baseValue)
		{
			this.Def          = def;
			this.Prorata      = prorata;
							
			this.InitialValue = initialValue;
			this.BaseValue    = baseValue;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Def.IsEmpty;
			}
		}


		public static AmortizationDetails Empty = new AmortizationDetails (AmortizationDefinition.Empty, ProrataDetails.Empty, null, null);

		public readonly AmortizationDefinition	Def;
		public readonly ProrataDetails			Prorata;

		public readonly decimal?				InitialValue;
		public readonly decimal?				BaseValue;
	}
}
