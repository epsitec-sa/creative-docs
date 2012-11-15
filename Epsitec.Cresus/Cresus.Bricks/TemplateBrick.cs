//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq.Expressions;
namespace Epsitec.Cresus.Bricks
{
	public class TemplateBrick<T, TParent> : ChildBrick<T, TParent>
			where TParent : Brick
	{
		public TemplateBrick(BrickWall brickWall, TParent parent)
			: base (brickWall, parent, BrickPropertyKey.Template)
		{
		}

		public TemplateBrick<T, TParent> Icon(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Icon, value));
			return this;
		}

		public TemplateBrick<T, TParent> Title(Mortar<T> value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Title, value));
			return this;
		}

		public TemplateBrick<T, TParent> Title<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public TemplateBrick<T, TParent> TitleCompact(Mortar<T> value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.TitleCompact, value));
			return this;
		}

		public TemplateBrick<T, TParent> TitleCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TitleCompact (new Mortar<T, TResult> (expression));
		}

		public TemplateBrick<T, TParent> Text(Mortar<T> value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Text, value));
			return this;
		}

		public TemplateBrick<T, TParent> Text<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Text (new Mortar<T, TResult> (expression));
		}

		public TemplateBrick<T, TParent> TextCompact(Mortar<T> value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.TextCompact, value));
			return this;
		}

		public TemplateBrick<T, TParent> TextCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TextCompact (new Mortar<T, TResult> (expression));
		}
	}
}