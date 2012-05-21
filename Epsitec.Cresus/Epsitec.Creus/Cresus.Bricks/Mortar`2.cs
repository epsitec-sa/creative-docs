//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class Mortar<TSource, TValue> : Mortar<TSource>
	{
		public Mortar(Expression<System.Func<TSource, TValue>> expression)
		{
			this.textValue = expression.ToString ();
			this.expression = expression;
		}

		public Mortar(string value)
		{
			this.textValue = value;
		}

		public override Expression GetExpression()
		{
			return this.expression;
		}

		public override string GetString()
		{
			return this.expression == null ? this.textValue : null;
		}

		public override string ToString()
		{
			return this.textValue;
		}

		private readonly string textValue;
		private readonly Expression<System.Func<TSource, TValue>> expression;
	}
}
