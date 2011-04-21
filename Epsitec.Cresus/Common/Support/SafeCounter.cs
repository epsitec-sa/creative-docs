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
			this.value = 0;
		}


		public bool IsZero
		{
			get
			{
				return this.value == 0;
			}
		}


		public System.IDisposable Enter ()
		{
			this.value++;

			System.Action action = () => this.value--;

			return DisposableWrapper.Get (action);
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
