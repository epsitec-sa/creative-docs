//	Copyright � 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

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

		internal Brick GetLastBrick()
		{
			int index = this.bricks.Count - 1;

			if (index < 0)
			{
				return null;
			}

			return this.bricks[index];
		}

		internal void Remove(Brick brick)
		{
			this.bricks.Remove (brick);
		}

		internal void NotifyBrickAdded(System.Type fieldType, Brick brick)
		{
			var handler = this.BrickAdded;

			if (handler != null)
			{
				handler (this, new BrickAddedEventArgs (fieldType, brick));
			}
		}
		
		internal void NotifyBrickPropertyAdded(Brick brick, BrickProperty property)
		{
			var handler = this.BrickPropertyAdded;

			if (handler != null)
			{
				handler (this, new BrickPropertyAddedEventArgs (brick, property));
			}
		}

		public event EventHandler<BrickAddedEventArgs>			BrickAdded;
		public event EventHandler<BrickPropertyAddedEventArgs>	BrickPropertyAdded;

		private readonly List<Brick> bricks;
	}
}
