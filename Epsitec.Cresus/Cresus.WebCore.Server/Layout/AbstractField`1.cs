//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	internal class AbstractField<T> : AbstractField
		where T : AbstractField<T>, new ()
	{
		protected override string GetEditionTilePartType()
		{
			return AbstractField<T>.typeName;
		}

		private static readonly string typeName = AbstractField.GetTypeName (typeof (T));
	}
}
