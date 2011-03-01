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
		}

		public Mortar(string value)
		{
			this.textValue = value;
		}

		public override string ToString()
		{
			return this.textValue;
		}

		private readonly string textValue;
	}
}
