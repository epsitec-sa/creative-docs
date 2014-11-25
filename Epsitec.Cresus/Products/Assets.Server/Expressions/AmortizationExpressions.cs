//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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


		public decimal? Evaluate(string expression, Data data)
		{
			AmortizationExpression ae;
			if (!this.expressions.TryGetValue (expression, out ae))  // pas encore dans le dico ?
			{
				ae = new AmortizationExpression (expression);
				this.expressions.Add (expression, ae);  // on l'ajoute dans le dico
			}

			return ae.Evaluate (data);
		}


		private readonly Dictionary<string, AmortizationExpression> expressions;
	}
}
