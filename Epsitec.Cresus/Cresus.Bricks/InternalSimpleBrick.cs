//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class InternalSimpleBrick<TSource, TField, TSelf> : InternalBaseBrick<TSource, TField, InternalSimpleBrick<TSource, TField, TSelf>>
			where TSelf : InternalSimpleBrick<TSource, TField, TSelf>
	{
		public TSelf Name(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Name, value));
			return this as TSelf;
		}

		public TSelf Include<TResult>(Expression<System.Func<TSource, TResult>> expression)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Include, expression));
			return this as TSelf;
		}

		public TSelf AutoGroup()
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.AutoGroup));
			return this as TSelf;
		}

		public TemplateBrick<TField, TField, TSelf> Template()
		{
			return new TemplateBrick<TField, TField, TSelf> (this as TSelf);
		}
		
		public InputBrick<TField, TField, TSelf> Input()
		{
			return new InputBrick<TField, TField, TSelf> (this as TSelf);
		}
	}
}
