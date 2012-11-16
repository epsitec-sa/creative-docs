//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;

namespace Epsitec.Cresus.Bricks
{
	public class InputBrick<T, TParent> : ChildBrick<T, TParent>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public InputBrick(TParent parent)
			: base(parent)
		{
		}

		public InputBrick<T, TParent> Title(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public InputBrick<T, TParent> Field<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Field, expression));
		}

		public InputBrick<T, TParent> Width(int value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Width, value));
		}

		public InputBrick<T, TParent> Height(int value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Height, value));
		}

		public InputBrick<T, TParent> ReadOnly()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.ReadOnly));
		}

		public InputBrick<T, TParent> Password()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Password));
		}

		public InputBrick<T, TParent> WithSpecialController(int mode = 0)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.SpecialController, mode));
		}

		public InputBrick<T, TParent> PickFromCollection(System.Collections.IEnumerable value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.FromCollection, value));
		}

		public InputBrick<T, TParent> Button(FormattedText title, FormattedText description, System.Action action)
		{
			dynamic expando = new ExpandoObject ();

			expando.ButtonTitle       = title;
			expando.ButtonDescription = description;
			expando.ButtonAction      = action;

			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Button, expando));
		}

		public InputBrick<T, TParent> SearchPanel(FormattedText searchTitle, FormattedText actionTitle, System.Action<Epsitec.Common.Support.EntityEngine.AbstractEntity> action)
		{
			dynamic expando = new ExpandoObject ();

			expando.SearchTitle  = searchTitle;
			expando.ButtonTitle  = actionTitle;
			expando.ButtonAction = action;

			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.SearchPanel, expando));
		}

		public HorizontalGroupBrick<T, InputBrick<T, TParent>> HorizontalGroup()
		{
			var child = new HorizontalGroupBrick<T, InputBrick<T, TParent>> (this);
			return this.AddChild (child, BrickPropertyKey.HorizontalGroup);
		}
	}
}
