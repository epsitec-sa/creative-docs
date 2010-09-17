//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public abstract class IndirectAccessor<T> : IndirectAccessor
		where T : new ()
	{
		public static IndirectAccessor<T, TResult> Create<TResult>(System.Func<T, TResult> action)
		{
			return new IndirectAccessor<T, TResult> (action);
		}
	}
}
