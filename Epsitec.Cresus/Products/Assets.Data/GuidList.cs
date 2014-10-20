//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class GuidList<T> : IEnumerable<T>
		where T : class, IGuid
	{
		public GuidList(UndoManager undoManager)
		{
			this.undoManager = undoManager;

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

			if (this.undoManager != null)
			{
				var undoData = new UndoData
				{
					Index = this.list.Count-1,
				};
				this.undoManager.Push (x => this.RemoveAt (x as UndoData), undoData, "Add");
			}
		}

		public void Insert(int index, T item)
		{
			this.dict[item.Guid] = item;
			this.list.Insert (index, item);

			if (this.undoManager != null)
			{
				var undoData = new UndoData
				{
					Index = index,
				};
				this.undoManager.Push (x => this.RemoveAt (x as UndoData), undoData, "Insert");
			}
		}

		public void Remove(T item)
		{
			if (this.undoManager != null)
			{
				var undoData = new UndoData
				{
					Index = this.IndexOf (item),
					Item = item,
				};
				this.undoManager.Push (x => this.Insert (x as UndoData), undoData, "Remove");
			}

			this.dict.Remove (item.Guid);
			this.list.Remove (item);
		}

		public void RemoveAt(int index)
		{
			if (this.undoManager != null)
			{
				var undoData = new UndoData
				{
					Index = index,
					Item = this.list[index],
				};
				this.undoManager.Push (x => this.Insert (x as UndoData), undoData, "RemoveAt");
			}

			this.dict.Remove (this.list[index].Guid);
			this.list.RemoveAt (index);
		}

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}


		private void Insert(UndoData undoData)
		{
			var index = undoData.Index;
			var item = undoData.Item;

			this.dict[item.Guid] = item;
			this.list.Insert (index, item);
		}

		private void RemoveAt(UndoData undoData)
		{
			var index = undoData.Index;

			this.dict.Remove (this.list[index].Guid);
			this.list.RemoveAt (index);
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


		private class UndoData
		{
			public int Index;
			public T Item;
		}


		private readonly UndoManager			undoManager;
		private readonly List<T>				list;
		private readonly Dictionary<Guid, T>	dict;
	}
}
