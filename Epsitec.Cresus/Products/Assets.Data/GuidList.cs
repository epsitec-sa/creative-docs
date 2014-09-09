//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class GuidList<T> : IEnumerable<T>
		where T : class, IGuid
	{
		public GuidList()
		{
			this.list = new List<T> ();
			this.dict = new Dictionary<Guid, T> ();
		}


		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}


		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion

		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion


		public void Clear()
		{
			this.dict.Clear ();
			this.list.Clear ();
		}

		public void Add(T item)
		{
			this.dict[item.Guid] = item;
			this.list.Add (item);
		}

		public void Insert(int index, T item)
		{
			this.dict[item.Guid] = item;
			this.list.Insert (index, item);
		}

		public void Remove(T item)
		{
			this.dict.Remove (item.Guid);
			this.list.Remove (item);
		}

		public void RemoveAt(int index)
		{
			this.dict.Remove (this.list[index].Guid);
			this.list.RemoveAt (index);
		}

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}


		public T this[Guid key]
		{
			get
			{
				if (this.dict.ContainsKey (key))
				{
					return this.dict[key];
				}
				else
				{
					return null;
				}
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= 0 && index < this.list.Count)
				{
					return this.list[index];
				}
				else
				{
					return null;
				}
			}
		}


		private readonly List<T>				list;
		private readonly Dictionary<Guid, T>	dict;
	}
}
