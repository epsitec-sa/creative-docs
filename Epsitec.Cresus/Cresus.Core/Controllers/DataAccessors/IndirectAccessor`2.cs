//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>IndirectAccessor</c> class wraps a getter function which requires a source
	/// object; see also the simpler <see cref="Accessor{TResult}"/> class.
	/// </summary>
	/// <typeparam name="T">The type of the source object.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public class IndirectAccessor<T, TResult> : IndirectAccessor
		where T : new ()
	{
		public IndirectAccessor(System.Func<T, TResult> getter)
		{
			this.getter = getter;
		}


		public static Accessor<TResult> GetAccessor(IndirectAccessor<T, TResult> accessor, T source)
		{
			if (accessor == null)
			{
				return null;
			}
			else
			{
				return IndirectAccessor.GetAccessor (source, accessor.getter);
			}
		}

		public static Accessor<TResult> GetAccessor(IndirectAccessor<T, TResult> accessor, T source, TResult defaultResult, System.Predicate<TResult> isEmptyPredicate)
		{
			if (accessor == null)
			{
				return null;
			}
			else
			{
				var result = IndirectAccessor.GetAccessor (source, accessor.getter);
				return new AccessorWithDefaultValue<TResult> (result, defaultResult, isEmptyPredicate);
			}
		}
		
		private readonly System.Func<T, TResult> getter;
	}
}
