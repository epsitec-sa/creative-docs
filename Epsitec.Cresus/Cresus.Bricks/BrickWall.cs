//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		protected void NotifyBrickAdded(System.Type fieldType, Brick brick)
		{
			brick.DefineBrickWall (this);

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

		public event EventHandler<BrickAddedEventArgs> BrickAdded;
		public event EventHandler<BrickPropertyAddedEventArgs> BrickPropertyAdded;

		private readonly List<Brick> bricks;
	}

	public class BrickAddedEventArgs : EventArgs
	{
		public BrickAddedEventArgs(System.Type fieldType, Brick brick)
		{
			this.fieldType = fieldType;
			this.brick = brick;
		}

		public System.Type FieldType
		{
			get
			{
				return this.fieldType;
			}
		}

		public Brick Brick
		{
			get
			{
				return this.brick;
			}
		}

		private readonly System.Type fieldType;
		private readonly Brick brick;
	}

	public class BrickPropertyAddedEventArgs : EventArgs
	{
		public BrickPropertyAddedEventArgs(Brick brick, BrickProperty property)
		{
			this.brick = brick;
			this.property = property;
		}

		public BrickProperty Property
		{
			get
			{
				return this.property;
			}
		}

		public Brick Brick
		{
			get
			{
				return this.brick;
			}
		}

		private readonly Brick brick;
		private readonly BrickProperty property;
	}
}
