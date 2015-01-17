//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public struct AmortizationExpressionItem
	{
		public AmortizationExpressionItem(AmortizationExpressionType type, string description,
			string[] expressionLines)
		{
			this.Type        = type;
			this.Description = description;
			this.Expression  = AmortizationExpressionItem.Format (expressionLines);
		}

		public bool IsEmpty
		{
			get
			{
				return this.Type == AmortizationExpressionType.Unknown;
			}
		}

		private static string Format(params string[] lines)
		{
			//	Le code ne doit pas contenir de tabulateurs. Il faut les remplacer
			//	systématiquement par 4 espaces. En effet, lorsque l'utilisateur
			//	édite l'expression, il ne peut pas insérer de tabulateur, car la touche
			//	Tab passe au champ suivant !

			if (lines == null)
			{
				return null;
			}
			else
			{
				return AmortizationExpression.ConvertToTaggedText (string.Join ("\n", lines))
					.Replace ("<tab/>", "    ");
			}
		}

		public static AmortizationExpressionItem Empty = new AmortizationExpressionItem (AmortizationExpressionType.Unknown, null, null);

		public readonly AmortizationExpressionType	Type;
		public readonly string						Description;
		public readonly string						Expression;
	}
}
