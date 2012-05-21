//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Bricks
{
	public sealed class BrickPropertyAddedEventArgs : EventArgs
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
