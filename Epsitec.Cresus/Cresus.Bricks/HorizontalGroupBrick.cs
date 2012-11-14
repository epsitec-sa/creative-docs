//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class HorizontalGroupBrick<T, TParent> : Brick
		where TParent : Brick
	{
		public HorizontalGroupBrick(TParent parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.HorizontalGroup, this));

			this.DefineBrickWall (parent.BrickWall);
			this.parent = parent;
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

		public TParent End()
		{
			return this.parent;
		}

		public override System.Type GetFieldType()
		{
			return typeof (T);
		}

		readonly TParent parent;
	}
}
