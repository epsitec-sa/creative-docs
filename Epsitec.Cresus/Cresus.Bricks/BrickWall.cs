//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Bricks
{
	public abstract class BrickWall
	{
		protected BrickWall()
		{
			this.bricks = new List<Brick> ();
		}

		public IEnumerable<Brick> Bricks
		{
			get
			{
				return this.bricks;
			}
		}

		protected void Add(Brick brick)
		{
			this.bricks.Add (brick);
		}


		private readonly List<Brick> bricks;
	}
}
