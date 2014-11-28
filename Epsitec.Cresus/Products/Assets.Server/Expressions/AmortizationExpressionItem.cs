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
			this.Expression  = AmortizationExpression.Format (expressionLines);
		}

		public bool IsEmpty
		{
			get
			{
				return this.Type == AmortizationExpressionType.Unknown;
			}
		}

		public static AmortizationExpressionItem Empty = new AmortizationExpressionItem (AmortizationExpressionType.Unknown, null, null);

		public readonly AmortizationExpressionType	Type;
		public readonly string						Description;
		public readonly string						Expression;
	}
}
