//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class InputGroupBrick<TSource, TField, TParent> : InternalInputGroupBrick<TSource, TField, InputGroupBrick<TSource, TField, TParent>, TParent>
				where TParent : Brick
	{
		public InputGroupBrick(TParent parent)
			: base (parent)
		{
		}
	}
}
