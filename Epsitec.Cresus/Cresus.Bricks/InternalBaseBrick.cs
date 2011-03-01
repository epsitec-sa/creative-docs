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

		public TSelf Title<TResult>(Expression<System.Func<TSource, TResult>> expression)
		{
			return this.Title (new Mortar<TSource, TResult> (expression));
		}
		
		public TSelf Title(Mortar<TSource> value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Title, value.ToString ()));
			return this as TSelf;
		}
		
		public TSelf Separator()
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Separator));
			return this as TSelf;
		}
	}
}