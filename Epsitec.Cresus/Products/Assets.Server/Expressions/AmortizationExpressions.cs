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
	/// ne manquerait pas d'arriver avec les amortissements successifs sans changement des
	/// arguments (taux, valeur résiduelle et arrondi identiques par exemple).
	/// Comme les arguments sont injectés dans le source. Il suffit que le taux d'amortissement
	/// change pour qu'il soit nécessaire de compiler une nouvelle expression. D'où la
	/// nécessité d'un cache.
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


		public bool Check(string arguments, string expression)
		{
			//	Indique si une expression est correcte. Si nécessaire, elle est compilée
			//	et insérée dans le dictionnaire.
			var details = Amortizations.GetDefaultDetails (arguments, expression);
			var result = this.Evaluate (details);
			return !result.HasError;
		}

		public ExpressionResult Evaluate(AmortizationDetails details)
		{
			//	Evalue une expression. Si nécessaire, elle est compilée et insérée dans
			//	le dictionnaire. Si elle est déjà dans le dictionnaire, elle est juste
			//	exécutée, ce qui est super rapide (c'est du code C# natif).
			string arguments  = details.Def.Arguments;
			string expression = details.Def.Expression;

			var key = arguments + expression;

			AmortizationExpression ae;
			if (!this.expressions.TryGetValue (key, out ae))  // pas encore dans le dico ?
			{
				ae = new AmortizationExpression (arguments, expression);
				this.expressions.Add (key, ae);  // on l'ajoute dans le dico
			}

			if (ae.HasError)
			{
				return new ExpressionResult (null, null, ae.Error);
			}
			else
			{
				return ae.Evaluate (details);
			}
		}


		private readonly Dictionary<string, AmortizationExpression> expressions;
	}
}
