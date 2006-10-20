//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
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
