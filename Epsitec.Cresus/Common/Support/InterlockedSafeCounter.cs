namespace Epsitec.Common.Support
{


	// Note This class might be redundant with SafeCounter. I added it only because I wanted to
	// extract that logic away from AbstractEntity and EntityContext and that this logic was similar
	// (but different) from the one in SafeCounter. Because of thread safety stuff that I will do
	// soon, this logic here might change depending of what and how I will need to make things thread
	// safe in those classes. When I'll know exactly what I'll need, I'll decide if I'll merge those
	// two classes together or if I Keep them separate.
	// Marc

	public sealed class InterlockedSafeCounter
	{


		public InterlockedSafeCounter()
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
			System.Threading.Interlocked.Increment (ref this.value);

			System.Action action = () => System.Threading.Interlocked.Decrement (ref this.value);

			return DisposableWrapper.Get (action);
		}


		private int value;


	}


}
