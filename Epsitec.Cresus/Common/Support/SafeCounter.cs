//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

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
			//	NB: the SafeCounter must be a class in order for this to work; turning it into
			//	a struct might be tempting, but doing so will prevent Enter/Release to work
			//	correctly, as they will operate on copies of the SafeCounter, rather than on
			//	the original.

			this.value = 0;
		}


		public bool IsZero
		{
			get
			{
				return this.value == 0;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}


		public System.IDisposable Enter ()
		{
			this.value++;

			return DisposableWrapper.CreateDisposable (this.Release);
		}

		private void Release()
		{
			this.value--;
		}
		
		
		/// <summary>
		/// Executes the action only if the counter value is currently zero.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns><c>true</c> if the action was executed; otherwise, <c>false</c>.</returns>
		public bool IfZero(System.Action action)
		{
			if (this.IsZero)
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


		private int value;
	}
}
