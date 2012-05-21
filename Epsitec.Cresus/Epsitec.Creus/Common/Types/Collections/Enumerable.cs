//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	public static class Enumerable
	{
		public static IEnumerable<T> FromItem<T>(T item)
		{
			yield return item;
		}
	}
}
