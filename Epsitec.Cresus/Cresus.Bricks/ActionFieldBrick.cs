using Epsitec.Common.Support.EntityEngine;

using System;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public sealed class ActionFieldBrick<T, TField, TParent> : ChildBrick<T, TParent>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public ActionFieldBrick(TParent parent)
			: base (parent)
		{
			Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Type, typeof (TField)));
		}

		public ActionFieldBrick<T, TField, TParent> Title(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public ActionFieldBrick<T, TField, TParent> Title<TResult>(Expression<Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public ActionFieldBrick<T, TField, TParent> InitialValue(TField value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Value, value));
		}

		public ActionFieldBrick<T, TField, TParent> InitialValue(Expression<Func<T, TField>> expression)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Value, expression));
		}

		public ActionFieldBrick<T, TField, TParent> Password()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Password));
		}

		public ActionFieldBrick<T, TField, TParent> Multiline()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Multiline));
		}

		public ActionFieldBrick<T, TField, TParent> ReadOnly()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.ReadOnly));
		}

		public ActionFieldBrick<T, TField, TParent> WithSpecialField<TSpecialController>()
		{
			var key = BrickPropertyKey.SpecialFieldController;
			var value = typeof (TSpecialController);

			return Brick.AddProperty (this, new BrickProperty (key, value));
		}
	}
}
