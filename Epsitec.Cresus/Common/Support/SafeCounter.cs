//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <see cref="SafeCounter"/> class is used to manage a counter in a safe way; you
	/// call the <see cref="Enter"/> method in a <c>using</c> block to increment/decrement
	/// automatically the <see cref="Value"/>.
	/// </summary>
	public sealed class SafeCounter
	{
		public SafeCounter()
		{
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public System.IDisposable Enter()
		{
			return new State (this);
		}

		/// <summary>
		/// Executes the action only if the counter value is currently zero.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns><c>true</c> if the action was executed; otherwise, <c>false</c>.</returns>
		public bool IfZero(System.Action action)
		{
			if (this.value == 0)
			{
				using (this.Enter ())
				{
					action ();
				}
				
				return true;
			}
			else
			{
				return false;
			}
		}

		private class State : System.IDisposable
		{
			public State(SafeCounter counter)
			{
				this.counter = counter;
				this.counter.value++;
			}

			~State()
			{
				throw new System.InvalidOperationException ("The safe counter sate was not properly disposed of.");
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.GC.SuppressFinalize (this);
				this.counter.value--;
			}

			#endregion

			private readonly SafeCounter counter;
		}

		private int value;
	}
}
