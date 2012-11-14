//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class TemplateBrick<T, TParent> : InternalBaseBrick<T, TemplateBrick<T, TParent>>
			where TParent : Brick
	{
		public TemplateBrick(TParent parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.Template, this));

			this.DefineBrickWall (parent.BrickWall);
			this.parent = parent;
		}

		public TParent End()
		{
			return this.parent;
		}

		readonly TParent parent;
	}
}