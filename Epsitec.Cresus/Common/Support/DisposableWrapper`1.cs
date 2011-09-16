//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	public sealed class DisposableWrapper<T> : DisposableWrapper
		{
			private DisposableWrapper(System.Action action, T value)
				: base (action)
			{
				this.value = value;
			}


			public T								Value
			{
				get
				{
					return this.value;
				}
			}

			public static DisposableWrapper<T> CreateDisposable(System.Action action, T value)
			{
				action.ThrowIfNull ("action");
				
				return new DisposableWrapper<T> (action, value);
			}

			private readonly T						value;
		}
}
