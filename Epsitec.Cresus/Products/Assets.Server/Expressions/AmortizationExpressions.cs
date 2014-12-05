//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	/// <summary>
	/// Gère une collection d'expressions. L'idée est que chaque expression n'y soit qu'une
	/// seule fois, afin d'éviter de compiler des tonnes d'expressions identiques, ce qui
	/// ne manquerait pas d'arriver avec les méthodes d'amortissements semblables.
	/// </summary>
	public class AmortizationExpressions
	{
		public AmortizationExpressions()
		{
			this.expressions = new Dictionary<string, AmortizationExpression> ();
		}


		public void Clear()
		{
			this.expressions.Clear ();
		}


		public AbstractCalculator.Result Evaluate(AmortizationDetails details)
		{
			string arguments  = details.Def.Arguments;
			string expression = details.Def.Expression;

			var key = arguments + expression;

			AmortizationExpression ae;
			if (!this.expressions.TryGetValue (key, out ae))  // pas encore dans le dico ?
			{
				ae = new AmortizationExpression (arguments, expression);
				this.expressions.Add (key, ae);  // on l'ajoute dans le dico
			}

			return ae.Evaluate (details);
		}


		private readonly Dictionary<string, AmortizationExpression> expressions;
	}
}
