//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class InternalSimpleBrick<T, TSelf> : InternalBaseBrick<T, InternalSimpleBrick<T, TSelf>>
			where TSelf : InternalSimpleBrick<T, TSelf>
	{
		public TSelf Name(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Name, value));
			return this as TSelf;
		}

		public TSelf Include<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Include, expression));
			return this as TSelf;
		}

		public TSelf Attribute<TAttribute>(TAttribute attributeValue)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Attribute, new AttributeValue<TAttribute> (attributeValue)));
			return this as TSelf;
		}

		public TemplateBrick<T, TSelf> Template()
		{
			return new TemplateBrick<T, TSelf> (this as TSelf);
		}
		
		public InputBrick<T, T, TSelf> Input()
		{
			return new InputBrick<T, T, TSelf> (this as TSelf);
		}
	}
}
