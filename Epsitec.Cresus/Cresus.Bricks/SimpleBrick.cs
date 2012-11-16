//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class SimpleBrick<T> : Brick<T>
		where T : AbstractEntity, new ()
	{
		public SimpleBrick(BrickWall brickWall, Expression resolver)
			: base (brickWall, resolver)
		{
		}

		public SimpleBrick<T> Name(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Name, value));
		}

		public SimpleBrick<T> Icon(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Icon, value));
		}

		public SimpleBrick<T> Title(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public SimpleBrick<T> Title<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> TitleCompact(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.TitleCompact, value));
		}

		public SimpleBrick<T> TitleCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TitleCompact (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> Text(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Text, value));
		}

		public SimpleBrick<T> Text<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Text (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> TextCompact(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.TextCompact, value));
		}

		public SimpleBrick<T> TextCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TextCompact (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> Separator()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Separator));
		}

		public SimpleBrick<T> GlobalWarning()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.GlobalWarning));
		}

		public SimpleBrick<T> Attribute<TAttribute>(TAttribute attributeValue)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Attribute, new AttributeValue<TAttribute> (attributeValue)));
		}

		public SimpleBrick<TOutput> OfType<TOutput>()
			where TOutput: AbstractEntity, new()
		{
			var brick = new SimpleBrick<TOutput> (this.BrickWall, this.Resolver);

			this.AddProperty (new BrickProperty (BrickPropertyKey.OfType, brick));

			return brick;
		}

		public SimpleBrick<T> Include<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Include, expression));
		}

		public TemplateBrick<T, SimpleBrick<T>> Template()
		{
			return new TemplateBrick<T, SimpleBrick<T>> (this.BrickWall, this);
		}

		public InputBrick<T, SimpleBrick<T>> Input()
		{
			return new InputBrick<T, SimpleBrick<T>> (this.BrickWall, this);
		}
	}
}