using Epsitec.Common.Support.EntityEngine;

using System;

using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public sealed class ActionBrick<T, TParent> : ChildBrick<T, TParent>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public ActionBrick(TParent parent)
			: base (parent)
		{
		}

		public ActionBrick<T, TParent> Icon(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Icon, value));
		}

		public ActionBrick<T, TParent> Title(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public ActionBrick<T, TParent> Title<TResult>(Expression<Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public ActionBrick<T, TParent> Text(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Text, value));
		}

		public ActionBrick<T, TParent> Text<TResult>(Expression<Func<T, TResult>> expression)
		{
			return this.Text (new Mortar<T, TResult> (expression));
		}

		public ActionFieldBrick<T, TField, ActionBrick<T, TParent>> Field<TField>()
		{
			var child = new ActionFieldBrick<T, TField, ActionBrick<T, TParent>> (this);
			return this.AddChild (child, BrickPropertyKey.Field);
		}
	}
}
