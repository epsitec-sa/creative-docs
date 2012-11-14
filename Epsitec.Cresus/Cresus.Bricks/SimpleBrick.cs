//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class SimpleBrick<T> : InternalSimpleBrick<T, SimpleBrick<T>>
	{
		public SimpleBrick()
		{
		}

		private SimpleBrick(Brick parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.OfType, this));
		}

		public SimpleBrick<TOutput> OfType<TOutput>()
		{
			var brick = new SimpleBrick<TOutput> (this);

			brick.InheritResolver (this);
			brick.DefineBrickWall (this.BrickWall);

			return brick;
		}
	}
}