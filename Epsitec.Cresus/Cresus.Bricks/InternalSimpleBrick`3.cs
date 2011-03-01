//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public class InternalSimpleBrick<TSource, TField, TSelf> : InternalBaseBrick<TSource, TField, InternalSimpleBrick<TSource, TField, TSelf>>
			where TSelf : InternalSimpleBrick<TSource, TField, TSelf>
	{
		public TSelf Name(string value)
		{
			this.AddProperty (new BrickProperty (BrickPropertyKey.Name, value));
			return this as TSelf;
		}

		public TemplateBrick<TField, TField, TSelf> Template()
		{
			return new TemplateBrick<TField, TField, TSelf> (this as TSelf);
		}
	}
}
