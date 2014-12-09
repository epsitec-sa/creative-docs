//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

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


		public static AmortizationDetails DefaultTest
		{
			get
			{
				var def = new AmortizationDefinition (DateRange.Empty, System.DateTime.MinValue, null, null, Periodicity.Annual, 4000.0m);
				var history = new HistoryDetails (5000.0m, 4000.0m, 0, 0);

				return new AmortizationDetails (def, history);
			}
		}

		public static AmortizationDetails Empty = new AmortizationDetails (AmortizationDefinition.Empty, HistoryDetails.Empty);


		public readonly AmortizationDefinition	Def;
		public readonly HistoryDetails			History;
	}
}
