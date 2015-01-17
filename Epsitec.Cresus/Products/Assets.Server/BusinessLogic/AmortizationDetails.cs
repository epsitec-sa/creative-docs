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
		public AmortizationDetails(AmortizationDefinition def, HistoryDetails history)
		{
			this.Def     = def;
			this.History = history;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Def.IsEmpty;
			}
		}


		public static AmortizationDetails SetMethod(AmortizationDetails model, string arguments, string expression)
		{
			var m = AmortizationDefinition.SetMethod (model.Def, arguments, expression);
			return new AmortizationDetails (m, model.History);
		}


		public static AmortizationDetails Empty = new AmortizationDetails (AmortizationDefinition.Empty, HistoryDetails.Empty);


		public readonly AmortizationDefinition	Def;
		public readonly HistoryDetails			History;
	}
}
