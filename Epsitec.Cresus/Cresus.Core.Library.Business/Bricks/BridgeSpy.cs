//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
{
	public static class BridgeSpy
	{
		public static void ExecuteSetter<T1, T2>(T1 entity, T2 value, System.Action<T1, T2> setter, System.Func<T1, T2> getter)
		{
			T2 oldValue = getter (entity);
			T2 newValue = value;

			setter (entity, value);
		}
	}
}
