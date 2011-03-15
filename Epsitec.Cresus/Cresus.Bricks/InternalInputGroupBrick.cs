//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class InternalInputGroupBrick<TSource, TField, TSelf, TParent> : Brick
		where TSelf : InternalInputGroupBrick<TSource, TField, TSelf, TParent>
		where TParent : Brick
	{
		public InternalInputGroupBrick(TParent parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.InputGroup, this));

			this.DefineBrickWall (parent.BrickWall);
			this.parent = parent;
		}

		public TSelf Title(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Title, value));
			return this as TSelf;
		}

		public TSelf Field<TResult>(Expression<System.Func<TField, TResult>> expression)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Field, expression));
			return this as TSelf;
		}

		public TSelf Width(int value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Width, value));
			return this as TSelf;
		}

		public TParent End()
		{
			return this.parent;
		}

		public override System.Type GetFieldType()
		{
			return typeof (TField);
		}

		readonly TParent parent;
	}
}
