//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class BrickWall<T> : BrickWall
	{
		public BrickWall()
		{
		}

		public SimpleBrick<T> AddBrick()
		{
			var brick = new SimpleBrick<T> ();

			this.Add (brick);
			this.NotifyBrickAdded (typeof (T), brick);

			return brick;
		}

		public SimpleBrick<TField> AddBrick<TField>(Expression<System.Func<T, TField>> expression)
		{
			var brick = new SimpleBrick<TField> ();

			brick.DefineResolver (expression);

			this.Add (brick);
			this.NotifyBrickAdded (typeof (TField), brick);

			return brick;
		}

		public SimpleBrick<TField> AddBrick<TField>(Expression<System.Func<T, IList<TField>>> expression)
		{
			var brick = new SimpleBrick<TField> ();

			brick.DefineResolver (expression);

			this.Add (brick);
			this.NotifyBrickAdded (typeof (TField), brick);

			return brick;
		}
	}
}
