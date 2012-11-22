//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class TemplateBrick<T, TParent> : ChildBrick<T, TParent>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public TemplateBrick(TParent parent)
			: base (parent, true)
		{
		}

		public TemplateBrick<T, TParent> Icon(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Icon, value));
		}

		public TemplateBrick<T, TParent> Title(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public TemplateBrick<T, TParent> Title<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public TemplateBrick<T, TParent> TitleCompact(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.TitleCompact, value));
		}

		public TemplateBrick<T, TParent> TitleCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TitleCompact (new Mortar<T, TResult> (expression));
		}

		public TemplateBrick<T, TParent> Text(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Text, value));
		}

		public TemplateBrick<T, TParent> Text<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Text (new Mortar<T, TResult> (expression));
		}

		public TemplateBrick<T, TParent> TextCompact(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.TextCompact, value));
		}

		public TemplateBrick<T, TParent> TextCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TextCompact (new Mortar<T, TResult> (expression));
		}
	}
}