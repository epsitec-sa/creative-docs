//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>SingleValueEnumerable&lt;T&gt;</c> class can be used to enumerate over a
	/// collection of exactly one element, without having to allocate an array for that.
	/// </summary>
	/// <typeparam name="T">The type of items stored in the collection.</typeparam>
	public sealed class SingleValueEnumerable<T> : IEnumerable<T>
	{
		public SingleValueEnumerable(T value)
		{
			this.value = value;
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			yield return this.value;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			yield return this.value;
		}

		#endregion

		private readonly T						value;
	}
}
