//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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


		public bool								IsZero
		{
			get
			{
				return this.value == 0;
			}
		}

		public bool								IsNotZero
		{
			get
			{
				return this.value > 0;
			}
		}

		public int								Value
		{
			get
			{
				return this.value;
			}
		}


		public int Increment()
		{
			return System.Threading.Interlocked.Increment (ref this.value);
		}

		public int Decrement()
		{
			return System.Threading.Interlocked.Decrement (ref this.value);
		}
		
		public DisposableWrapper<int> Enter()
		{
			var value = this.Increment ();

			return DisposableWrapper.CreateDisposable (this.Release, value);
		}

		private void Release()
		{
			this.Decrement ();
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
				using (var wrapper = this.Enter ())
				{
					if (wrapper.Value == 1)
					{
						action ();
						return true;
					}
				}
			}
			
			return false;
		}

		public bool IfZero<T>(System.Action<T> action, T arg1)
		{
			if (this.IsZero)
			{
				using (var wrapper = this.Enter ())
				{
					if (wrapper.Value == 1)
					{
						action (arg1);
						return true;
					}
				}
			}

			return false;
		}


		private int								value;
	}
}
