//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AccessorWithDefaultValue</c> is a specialized <see cref="Accessor"/> which
	/// returns a default result when the value is empty.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public class AccessorWithDefaultValue<TResult> : Accessor<TResult>
	{
		public AccessorWithDefaultValue(Accessor<TResult> accessor, TResult defaultResult, System.Predicate<TResult> isEmptyPredicate)
			: base (accessor.Getter)
		{
			this.defaultResult    = defaultResult;
			this.isEmptyPredicate = isEmptyPredicate;
		}

		public override TResult ExecuteGetter()
		{
			TResult result = base.ExecuteGetter ();

			if (this.isEmptyPredicate (result))
			{
				return this.defaultResult;
			}
			else
			{
				return result;
			}
		}

		private readonly TResult defaultResult;
		private readonly System.Predicate<TResult> isEmptyPredicate;
	}
}
