//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class HorizontalGroupBrick<T, TParent> : ChildBrick<T, TParent>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public HorizontalGroupBrick(TParent parent)
			: base (parent)
		{
		}

		public HorizontalGroupBrick<T, TParent> Title(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public HorizontalGroupBrick<T, TParent> Title<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public HorizontalGroupBrick<T, TParent> Field<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Field, expression));
		}

		public HorizontalGroupBrick<T, TParent> Width(int value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Width, value));
		}

		public HorizontalGroupBrick<T, TParent> Height(int value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Height, value));
		}

		public HorizontalGroupBrick<T, TParent> ReadOnly()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.ReadOnly));
		}

		public HorizontalGroupBrick<T, TParent> Password()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Password));
		}
	}
}
