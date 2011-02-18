//	Copyright © 2009-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>Mapper{T1, T2}</c> class can be used to map items iteratively using a
	/// map function built to transform an <see cref="IEnumerable{T1}"/> into an
	/// <see cref="IEnumerable{T2}"/>, such as a LINQ select construct.
	/// </summary>
	/// <typeparam name="T1">Input type.</typeparam>
	/// <typeparam name="T2">Output type.</typeparam>
	public class Mapper<T1,T2> : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mapper{T1, T2}"/> class.
		/// </summary>
		/// <param name="mapFunction">The map function which transforms items of
		/// type <c>T1</c> into items of type <c>T2</c>.</param>
		public Mapper(System.Func<IEnumerable<T1>, IEnumerable<T2>> mapFunction)
		{
			this.mapperEnumerable = mapFunction (this.GetPushCollection ());
			this.mapperEnumerator = this.mapperEnumerable.GetEnumerator ();
		}

		/// <summary>
		/// Transforms a value using the map function.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The transformed value.</returns>
		public T2 Map(T1 value)
		{
			this.value = value;
			this.ready = true;

			//	Fetch the next value from the map function, which will in turn get
			//	its own value back through the virtual collection returned by the
			//	GetPushCollection method.

			if (this.mapperEnumerator.MoveNext ())
			{
				return this.mapperEnumerator.Current;
			}

			//	The map function does no longer map anything... this is a fatal error.
			throw new System.InvalidOperationException ();
		}

		#region IDisposable Members

		/// <summary>
		/// Notifies the mapper that nothing more will happen and that the
		/// last item was provided by the caller.
		/// </summary>
		public void Dispose()
		{
			this.dispose = true;
			bool hasMore = this.mapperEnumerator.MoveNext ();
			System.Diagnostics.Debug.Assert (hasMore == false);
			this.mapperEnumerator.Dispose ();
		}

		#endregion

		/// <summary>
		/// Gets a push collection, i.e. a virtual collection over which can be enumerated
		/// and which gets populated item by item by calling the <see cref="Map"/> method.
		/// </summary>
		/// <returns>A virtual push collection.</returns>
		private IEnumerable<T1> GetPushCollection()
		{
			while (true)
			{
				if (this.dispose)
				{
					//	User called Dispose on the mapper: stop enumerating.
					yield break;
				}

				if (this.ready)
				{
					//	User provided a value through the Map method: yield it.
					this.ready = false;
					yield return this.value;
				}
				else
				{
					//	Consumer needs more values than have been pushed by the
					//	user through the Map method: fatal error.
					throw new System.InvalidOperationException ();
				}
			}
		}

		private readonly IEnumerable<T2> mapperEnumerable;
		private readonly IEnumerator<T2> mapperEnumerator;

		private T1 value;
		private bool dispose;
		private bool ready;
	}
}
