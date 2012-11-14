//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class SimpleBrick<T> : Brick<T, SimpleBrick<T>>
	{
		public SimpleBrick<T> Name(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Name, value));
			return this;
		}

		public SimpleBrick<T> Include<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Include, expression));
			return this;
		}

		public SimpleBrick<T> Attribute<TAttribute>(TAttribute attributeValue)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Attribute, new AttributeValue<TAttribute> (attributeValue)));
			return this;
		}

		public SimpleBrick<TOutput> OfType<TOutput>()
		{
			var brick = new SimpleBrick<TOutput> ();

			this.AddProperty (new BrickProperty (BrickPropertyKey.OfType, brick));

			brick.InheritResolver (this);
			brick.DefineBrickWall (this.BrickWall);

			return brick;
		}

		public TemplateBrick<T, SimpleBrick<T>> Template()
		{
			return new TemplateBrick<T, SimpleBrick<T>> (this);
		}

		public InputBrick<T, SimpleBrick<T>> Input()
		{
			return new InputBrick<T, SimpleBrick<T>> (this);
		}
	}
}
