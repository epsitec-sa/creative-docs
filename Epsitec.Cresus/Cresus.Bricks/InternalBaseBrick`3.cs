//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class InternalBaseBrick<TSource, TField, TSelf> : Brick
				where TSelf : InternalBaseBrick<TSource, TField, TSelf>
	{
		public TSelf Text(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Text, value));
			return this as TSelf;
		}

		public TSelf Icon(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Icon, value));
			return this as TSelf;
		}

		public TSelf Title(Expression<System.Func<TSource, string>> func)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Title, func.ToString ()));
			return this as TSelf;
		}
		public TSelf Title(Mortar<TSource> value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Title, value.ToString ()));
			return this as TSelf;
		}
	}

	public class Mortar<T>
	{
		public Mortar(string value)
		{
			this.textValue = value;
		}
		public static implicit operator Mortar<T>(string value)
		{
			return new Mortar<T> (value);
		}
		public static implicit operator Mortar<T>(FormattedText value)
		{
			return new Mortar<T> (value.ToString ());
		}

		public override string ToString()
		{
			return this.textValue;
		}

		private readonly string textValue;
	}
}
