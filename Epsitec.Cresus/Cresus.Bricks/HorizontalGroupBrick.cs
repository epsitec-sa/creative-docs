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
		public HorizontalGroupBrick(BrickWall brickWall, TParent parent)
			: base (brickWall, parent, BrickPropertyKey.HorizontalGroup)
		{
		}

		public HorizontalGroupBrick<T, TParent> Field<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Field, expression));
			return this;
		}

		public HorizontalGroupBrick<T, TParent> Width(int value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Width, value));
			return this;
		}

		public HorizontalGroupBrick<T, TParent> Height(int value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Height, value));
			return this;
		}

		public HorizontalGroupBrick<T, TParent> ReadOnly()
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.ReadOnly));
			return this;
		}
	}
}
