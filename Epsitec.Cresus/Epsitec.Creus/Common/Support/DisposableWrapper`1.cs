//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		/// <summary>
		/// Queues up an action to execute once this disposable has been disposed of.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns>The disposable instance.</returns>
		public DisposableWrapper<T> AndFinally(System.Action action)
		{
			this.EnqueueFinallyAction (action);

			return this;
		}

			
		public static DisposableWrapper<T> CreateDisposable(System.Action action, T value)
		{
			action.ThrowIfNull ("action");
				
			return new DisposableWrapper<T> (action, value);
		}

			
		private readonly T						value;
	}
}
