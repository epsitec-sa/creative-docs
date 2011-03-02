//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class SimpleBrick<TSource, TField> : InternalSimpleBrick<TSource, TField, SimpleBrick<TSource, TField>>
	{
		public SimpleBrick()
		{
		}

		private SimpleBrick(Brick parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.AsType, this));
		}

		public SimpleBrick<TOutput, TOutput> AsType<TOutput>()
		{
			var brick = new SimpleBrick<TOutput, TOutput> (this);
			return brick;
		}
	}
}