//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ExpressionParameterReplacer</c> class is used to replace a parameter in an
	/// expression with another one. This is useful when trying to combine <c>(a => a.X)</c> and
	/// <c>(b => b.Y)</c> into a single expression <c>x => (x => x.X) && (x => x.Y)</c>, for
	/// instance.
	/// </summary>
	internal class ExpressionParameterReplacer : ExpressionVisitor
	{
		// see http://stackoverflow.com/questions/9231569/exception-using-orelse-and-andalso-expression-methods

		private ExpressionParameterReplacer()
		{
			this.replacements = new Dictionary<ParameterExpression, ParameterExpression> ();
		}

		public ExpressionParameterReplacer(ParameterExpression fromParameter, ParameterExpression toParameter)
			: this ()
		{
			this.replacements.Add (fromParameter, toParameter);
		}

		public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
			: this ()
		{
			for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
			{
				this.replacements.Add (fromParameters[i], toParameters[i]);
			}
		}

		
		protected override Expression VisitParameter(ParameterExpression node)
		{
			ParameterExpression replacement;

			if (this.replacements.TryGetValue (node, out replacement))
			{
				node = replacement;
			}

			return base.VisitParameter (node);
		}

		
		private readonly Dictionary<ParameterExpression, ParameterExpression> replacements;
	}
}
