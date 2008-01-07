//	Copyright � 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>EmptyEnumerable&lt;T&gt;</c> class implements the <c>IEnumerable&lt;T&gt;</c> and
	/// <c>IEnumerable</c> interfaces for an empty collection.
	/// </summary>
	/// <typeparam name="T">Item type</typeparam>
	public sealed class EmptyEnumerable<T> : IEnumerable<T>, IEnumerator<T>
	{
		private EmptyEnumerable()
		{
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this;
		}

		#endregion

		#region IEnumerator<T> Members

		public T Current
		{
			get
			{
				throw new System.InvalidOperationException ("No current value exists");
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		#region IEnumerator Members

		object System.Collections.IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
		}

		#endregion

		public static readonly EmptyEnumerable<T> Instance = new EmptyEnumerable<T> ();
	}
}
