//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The generic DependencyObjectList class implements a generic List which
	/// provides also an ICollation on DependencyObject.
	/// </summary>
	public class DependencyObjectList<T> : List<T>, ICollection<DependencyObject> where T : DependencyObject
	{
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
			this.ToArray ().CopyTo (array, arrayIndex);
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
				return false;
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

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (T item in this)
			{
				yield return item;
			}
		}

		#endregion
	}
}
