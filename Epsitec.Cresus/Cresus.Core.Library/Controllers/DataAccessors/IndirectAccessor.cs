//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>IndirectAccessor</c> class is the base class for <see cref="IndirectAccessor{T, TResult}"/>.
	/// </summary>
	public abstract class IndirectAccessor
	{
		public static IndirectAccessor<T, TResult> Create<T, TResult>(System.Func<T, TResult> action)
			where T : new ()
		{
			return new IndirectAccessor<T, TResult> (action);
		}
		
		protected static Accessor<TResult> GetAccessor<T, TResult>(T source, System.Func<T, TResult> getter)
		{
			if (source == null)
			{
				return null;
			}
			else
			{
				return new Accessor<TResult> (() => getter (source));
			}
		}

	}
}
