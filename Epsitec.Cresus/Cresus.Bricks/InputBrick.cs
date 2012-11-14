//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class InputBrick<T, TParent> : InternalInputBrick<T, InputBrick<T, TParent>, TParent>
			where TParent : Brick
	{
		public InputBrick(TParent parent)
			: base (parent)
		{
		}
	}
}
