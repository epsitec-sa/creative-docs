//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class InternalTemplateBrick<TSource, TField, TSelf, TParent> : InternalBaseBrick<TSource, TField, InternalTemplateBrick<TSource, TField, TSelf, TParent>>
			where TSelf : InternalTemplateBrick<TSource, TField, TSelf, TParent>
			where TParent : Brick

	{
		public InternalTemplateBrick(TParent parent)
		{
			parent.AddProperty (new BrickProperty (BrickPropertyKey.Template, this));

			this.parent = parent;
		}

		public TParent End()
		{
			return this.parent;
		}

		readonly TParent parent;
	}
}