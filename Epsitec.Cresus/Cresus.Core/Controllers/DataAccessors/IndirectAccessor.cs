//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class IndirectAccessor
	{
	}
	
	public class IndirectAccessor<T> : IndirectAccessor
		where T : new ()
	{
		public static IndirectAccessor<T, TResult> Create<TResult>(System.Func<T, TResult> action)
		{
			return new IndirectAccessor<T, TResult> (action);
		}
	}

	public class IndirectAccessor<T, TResult> : IndirectAccessor<T>
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
				return accessor.GetAccessor (source);
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
				var result = accessor.GetAccessor (source);
				return new AccessorWithDefaultValue<TResult> (result.Getter, defaultResult, isEmptyPredicate);
			}
		}

		public Accessor<TResult> GetAccessor(T source)
		{
			if (source == null)
			{
				return null;
			}
			else
			{
				return new Accessor<TResult> (() => this.getter (source));
			}
		}

		private readonly System.Func<T, TResult> getter;
	}
}
