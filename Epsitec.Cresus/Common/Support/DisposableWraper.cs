using Epsitec.Common.Support.Extensions;

using System;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The <see cref="DisposableWrapper"/> class provides an easy way to wrap a call to an
	/// <see cref="Action"/> into a <see cref="IDisposable"/> object that will ensure that it will
	/// be called exactly once if the object is disposed and that an
	/// <see cref="InvalidOperationException"/> will be thrown if the object has not been disposed.
	/// </summary>
	public sealed class DisposableWrapper : IDisposable
	{


		private DisposableWrapper(Action action)
		{
			this.action = action;

			this.actionDone = false;
		}


		~DisposableWrapper()
		{
			if (!this.actionDone)
			{
				throw new InvalidOperationException ("Caller forgot to call Dispose");
			}
		}


		#region IDisposable Members


		public void Dispose()
		{
			if (!this.actionDone)
			{
				this.action ();

				this.actionDone = true;
			}

			GC.SuppressFinalize (this);
		}


		#endregion


		private readonly Action action;


		private bool actionDone;


		/// <summary>
		/// Gets an instance of <see cref="IDisposable"/> that will call <paramref name="action"/>
		/// when disposed for the first time and that will throw an <see cref="InvalidOperationException"/>
		/// if not disposed.
		/// </summary>
		/// <param name="action">The <see cref="Action"/> to execute when disposed.</param>
		/// <returns>The <see cref="IDisposable"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
		public static IDisposable Get(Action action)
		{
			action.ThrowIfNull ("action");

			return new DisposableWrapper (action);
		}


		/// <summary>
		/// Gets an instance of <see cref="IDisposable"/> that will dispose <paramref name="d1"/>
		/// and the <paramref name="d2"/> when disposed and that will throw an
		/// <see cref="InvalidOperationException"/> if not disposed.
		/// </summary>
		/// <param name="d1">The first object to dispose.</param>
		/// <param name="d2">The second object to dispose.</param>
		/// <returns>The <see cref="IDisposable"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
		public static IDisposable Combine(IDisposable d1, IDisposable d2)
		{
			d1.ThrowIfNull ("d1");
			d2.ThrowIfNull ("d2");

			Action initializer = () => { };
			
			Action finalizer = () =>
			{
				d1.Dispose ();
				d2.Dispose ();
			};

			return DisposableWrapper.Get (finalizer);
		}


	}


}
