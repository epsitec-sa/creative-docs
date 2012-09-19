//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>AutoCache</c> class caches results of function calls. That is, when requested for a
	/// result, it computes it, caches it and never computes it back.	
	/// </summary>
	/// <remarks>
	/// There are some restrictions with the functions that can be used with this class.
	/// - A function must be stable, that is, it must have the same result if called several times
	///   with the same argument.
	/// - It might only have a single input argument and a single result.
	/// 
	/// This class is thread safe. By saying that, I mean that this class by itself will not cause
	/// any deadlock and that calls to the computer function, their results and to the clear function
	/// will always be consistent. However, this class cannot make any guarantees about the thread
	/// safety of the computer function.
	/// </remarks>
	/// <typeparam name="TKey">The type of the function argument.</typeparam>
	/// <typeparam name="TValue">The type of the function result.</typeparam>
	public sealed class AutoCache<TKey, TValue> : System.IDisposable
	{
		/*
		 * As the Dictionary<TKey,TValue> class cannot contain a value for the null key, we have to
		 * work around this with the fields resultOfCallWithNull and resultOfCallWithNullComputed
		 * and the function GetResultForNullArgument in order to store the result of a call to the
		 * computer function with a null argument.
		 * Marc
		 */


		/// <summary>
		/// Builds a new instance of the <c>AutoCache</c> class.
		/// </summary>
		/// <param name="computer">The function whose results must be cached.</param>
		/// <exception cref=""
		public AutoCache(System.Func<TKey, TValue> computer)
		{
			computer.ThrowIfNull ("computer");

			this.rwLock = new ReaderWriterLockWrapper ();

			this.resultOfCallWithNull = default (TValue);
			this.resultOfCallWithNullComputed = false;

			this.computer = computer;
			this.cache = new Dictionary<TKey, TValue> ();
		}


		/// <summary>
		/// Computes the result of the computer function if it has not been computed yet and gives
		/// the result back.
		/// </summary>
		/// <param name="key">The value that must be passed as an argument to the computer function.</param>
		/// <returns>The (possibly cached) result of the computer function.</returns>
		public TValue							this[TKey key]
		{
			get
			{
				if (key == null)
				{
					return this.GetResultForNullArgument (key);
				}
				else
				{
					return this.GetResultForRegularArgument (key);
				}
			}
		}
		
		/// <summary>
		/// Clears the cache used by this instance.
		/// </summary>
		public void Clear()
		{
			using (this.rwLock.LockWrite ())
			{
				this.cache.Clear ();

				this.resultOfCallWithNull = default (TValue);
				this.resultOfCallWithNullComputed = false;
			}
		}
		
		
		#region IDisposable Members


		public void Dispose()
		{
			this.rwLock.Dispose ();
		}


		#endregion
	

		private TValue GetResultForNullArgument(TKey key)
		{
			TValue result = default (TValue);
			bool done = false;

			using (this.rwLock.LockRead ())
			{
				done = this.resultOfCallWithNullComputed;

				if (done)
				{
					result = this.resultOfCallWithNull;
				}
			}

			if (!done)
			{
				using (this.rwLock.LockWrite ())
				{
					if (!this.resultOfCallWithNullComputed)
					{
						this.resultOfCallWithNull = this.computer (key);
						this.resultOfCallWithNullComputed = true;
					}

					result = this.resultOfCallWithNull;
				}
			}

			return result;
		}

		private TValue GetResultForRegularArgument(TKey key)
		{
			TValue result = default (TValue);
			bool done = false;

			using (this.rwLock.LockRead ())
			{
				done = this.cache.TryGetValue (key, out result);
			}

			if (!done)
			{
				using (this.rwLock.LockWrite ())
				{
					done = this.cache.TryGetValue (key, out result);

					if (!done)
					{
						result = this.computer (key);

						this.cache[key] = result;
					}
				}
			}

			return result;
		}


		private readonly ReaderWriterLockWrapper rwLock;
		private readonly System.Func<TKey, TValue> computer;
		private readonly IDictionary<TKey,TValue> cache;

		private TValue							resultOfCallWithNull;
		private bool							resultOfCallWithNullComputed;
	}
}
