//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>HostedDependencyObjectList</c> stores a list of items derived from the
	/// <see cref="DependencyObject"/> class; it notifies the owner of the list when
	/// items are inserted or removed.
	/// </summary>
	/// <typeparam name="T">A class derived from <see cref="DependencyObject"/>.</typeparam>
	public class HostedDependencyObjectList<T> : HostedList<T>, ICollection<DependencyObject> where T : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HostedDependencyObjectList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="host">The host which must be notified.</param>
		public HostedDependencyObjectList(IListHost<T> host) : base (host)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HostedDependencyObjectList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="insertionCallback">The insertion callback.</param>
		/// <param name="removalCallback">The removal callback.</param>
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
