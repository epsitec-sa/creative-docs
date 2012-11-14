//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class BrickWall<T> : BrickWall
	{
		public SimpleBrick<T> AddBrick()
		{
			return this.AddSimpleBrick<T> (null);
		}

		public SimpleBrick<TField> AddBrick<TField>(Expression<System.Func<T, TField>> expression)
		{
			return this.AddSimpleBrick<TField> (expression);
		}

		public SimpleBrick<TField> AddBrick<TField>(Expression<System.Func<T, IList<TField>>> expression)
		{
			return this.AddSimpleBrick<TField> (expression);
		}

		private SimpleBrick<TBrick> AddSimpleBrick<TBrick>(Expression expression)
		{
			var brick = new SimpleBrick<TBrick> ();

			brick.DefineResolver (expression);

			this.Add (brick);
			this.NotifyBrickAdded (typeof (TBrick), brick);

			return brick;
		}
	}
}
