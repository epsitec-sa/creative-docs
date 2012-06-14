//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>EmptyEnumerable&lt;T&gt;</c> class implements the <c>IEnumerable&lt;T&gt;</c> and
	/// <c>IEnumerable</c> interfaces for an empty collection.
	/// </summary>
	/// <typeparam name="T">Item type</typeparam>
	public sealed class EmptyEnumerable<T> : IEnumerable<T>
	{
		private EmptyEnumerable()
		{
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return EmptyEnumerator<T>.Instance;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return EmptyEnumerator<T>.Instance;
		}

		#endregion

		public static readonly EmptyEnumerable<T> Instance = new EmptyEnumerable<T> ();
	}
}
