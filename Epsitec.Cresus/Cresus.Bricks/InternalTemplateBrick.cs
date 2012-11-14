//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class InternalTemplateBrick<T, TSelf, TParent> : InternalBaseBrick<T, InternalTemplateBrick<T, TSelf, TParent>>
			where TSelf : InternalTemplateBrick<T, TSelf, TParent>
			where TParent : Brick

	{
		public InternalTemplateBrick(TParent parent)
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