//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class BrickWall<T>
	{
		public BrickWall()
		{
			this.bricks = new List<Brick> ();
		}

		public SimpleBrick<T, T> AddBrick()
		{
			var brick = new SimpleBrick<T, T> ();

			this.bricks.Add (brick);

			return brick;
		}

		public SimpleBrick<T, TField> AddBrick<TField>(Expression<System.Func<T, TField>> expression)
		{
			var brick = new SimpleBrick<T, TField> ();

			this.bricks.Add (brick);

			return brick;
		}

		public SimpleBrick<T, TField> AddBrick<TField>(Expression<System.Func<T, IList<TField>>> expression)
		{
			var brick = new SimpleBrick<T, TField> ();

			brick.AddProperty (new BrickProperty (BrickPropertyKey.CollectionAnnotation, typeof (TField).FullName));

			this.bricks.Add (brick);

			return brick;
		}

		private readonly List<Brick> bricks;
	}
}
