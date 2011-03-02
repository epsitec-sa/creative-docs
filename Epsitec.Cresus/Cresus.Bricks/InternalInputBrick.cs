//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class InternalInputBrick<TSource, TField, TSelf, TParent> : Brick
		where TSelf : InternalInputBrick<TSource, TField, TSelf, TParent>
		where TParent : Brick
	{
		public InternalInputBrick(TParent parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.Input, this));

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
