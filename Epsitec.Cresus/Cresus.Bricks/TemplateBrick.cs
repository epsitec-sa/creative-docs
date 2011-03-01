//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class TemplateBrick<TSource, TField, TParent> : InternalTemplateBrick<TSource, TField, TemplateBrick<TSource, TField, TParent>, TParent>
		where TParent : Brick
	{
		public TemplateBrick(TParent parent)
			: base (parent)
		{
		}
	}
	public class InputBrick<TSource, TField, TParent> : InternalInputBrick<TSource, TField, InputBrick<TSource, TField, TParent>, TParent>
		where TParent : Brick
	{
		public InputBrick(TParent parent)
			: base (parent)
		{
		}
	}
}
