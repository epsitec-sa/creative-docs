//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	public class HostedDependencyObjectList<T> : HostedList<T>, ICollection<DependencyObject> where T : DependencyObject
	{
		public HostedDependencyObjectList(IListHost<T> host) : base (host)
		{
		}
		
		public HostedDependencyObjectList(Callback insertionCallback, Callback removalCallback) : base (insertionCallback, removalCallback)
		{
		}

		#region ICollection<DependencyObject> Members

		void ICollection<DependencyObject>.Add(DependencyObject item)
		{
			this.Add (item as T);
		}

		void ICollection<DependencyObject>.Clear()
		{
			this.Clear ();
		}

		bool ICollection<DependencyObject>.Contains(DependencyObject item)
		{
			return this.Contains (item as T);
		}

		void ICollection<DependencyObject>.CopyTo(DependencyObject[] array, int arrayIndex)
		{
			T[] temp = this.ToArray ();
			System.Array.Copy (temp, 0, array, arrayIndex, temp.Length);
		}

		int ICollection<DependencyObject>.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection<DependencyObject>.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		bool ICollection<DependencyObject>.Remove(DependencyObject item)
		{
			return this.Remove (item as T);
		}

		#endregion

		#region IEnumerable<DependencyObject> Members

		IEnumerator<DependencyObject> IEnumerable<DependencyObject>.GetEnumerator()
		{
			foreach (T item in this)
			{
				yield return item;
			}
		}

		#endregion
	}
}
