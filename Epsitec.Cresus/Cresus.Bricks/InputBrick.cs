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
		public InputBrick(BrickWall brickWall, TParent parent)
			: base(brickWall, parent, BrickPropertyKey.Input)
		{
		}

		public InputBrick<T, TParent> Title(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Title, value));
			return this;
		}

		public HorizontalGroupBrick<T, InputBrick<T, TParent>> HorizontalGroup(string value)
		{
			var group = this.HorizontalGroup ();
			group.AddProperty (new BrickProperty (BrickPropertyKey.Title, value));
			return group;
		}

		public HorizontalGroupBrick<T, InputBrick<T, TParent>> HorizontalGroup()
		{
			return new HorizontalGroupBrick<T, InputBrick<T, TParent>> (this.BrickWall, this);
		}

		public InputBrick<T, TParent> Field<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Field, expression));
			return this;
		}

		public InputBrick<T, TParent> Width(int value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Width, value));
			return this;
		}

		public InputBrick<T, TParent> Height(int value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Height, value));
			return this;
		}

		public InputBrick<T, TParent> ReadOnly()
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.ReadOnly));
			return this;
		}

		public InputBrick<T, TParent> Password()
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Password));
			return this;
		}

		public InputBrick<T, TParent> WithSpecialController(int mode = 0)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.SpecialController, mode));
			return this;
		}

		public InputBrick<T, TParent> PickFromCollection(System.Collections.IEnumerable value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.FromCollection, value));
			return this;
		}

		public InputBrick<T, TParent> Button(FormattedText title, FormattedText description, System.Action action)
		{
			dynamic expando = new ExpandoObject ();

			expando.ButtonTitle       = title;
			expando.ButtonDescription = description;
			expando.ButtonAction      = action;

			this.AddProperty (new BrickProperty (BrickPropertyKey.Button, expando));
			return this;
		}

		public InputBrick<T, TParent> SearchPanel(FormattedText searchTitle, FormattedText actionTitle, System.Action<Epsitec.Common.Support.EntityEngine.AbstractEntity> action)
		{
			dynamic expando = new ExpandoObject ();

			expando.SearchTitle  = searchTitle;
			expando.ButtonTitle  = actionTitle;
			expando.ButtonAction = action;
			
			this.AddProperty (new BrickProperty (BrickPropertyKey.SearchPanel, expando));
			return this;
		}
	}
}
