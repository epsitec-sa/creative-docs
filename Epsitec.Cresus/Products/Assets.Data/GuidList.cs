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
			if (this.undoManager == null)
			{
				this.dict.Clear ();
				this.list.Clear ();
			}
			else
			{
				//	On n'utilise pas this.list.Clear () pour permettre le undo !
				while (this.list.Any ())
				{
					this.RemoveAt (0);
				}
			}
		}


		public void Add(T item)
		{
			var undoItem = this.UAdd (item);

			if (this.undoManager != null)
			{
				this.undoManager.Push (undoItem);
			}
		}

		private UndoItem UAdd(T item)
		{
			//	Ajoute un item à la fin de la liste et retourne l'information permettant
			//	de faire l'opération inverse.
			this.dict[item.Guid] = item;
			this.list.Add (item);

			var undoData = new UndoData
			{
				Index = this.list.Count-1,
			};

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.URemoveAt (data.Index);
			},
			undoData, "GuidList.Add");
		}


		public void Insert(int index, T item)
		{
			var undoItem = this.UInsert (index, item);

			if (this.undoManager != null)
			{
				this.undoManager.Push (undoItem);
			}
		}

		private UndoItem UInsert(int index, T item)
		{
			//	Ajoute un item dans la liste et retourne l'information permettant
			//	de faire l'opération inverse.
			this.dict[item.Guid] = item;
			this.list.Insert (index, item);

			var undoData = new UndoData
			{
				Index = index,
			};

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.URemoveAt (data.Index);
			},
			undoData, "GuidList.Insert");
		}


		public void Remove(T item)
		{
			var undoItem = this.URemove (item);

			if (this.undoManager != null)
			{
				this.undoManager.Push (undoItem);
			}
		}

		private UndoItem URemove(T item)
		{
			//	Supprime un item de la liste et retourne l'information permettant
			//	de faire l'opération inverse.
			var undoData = new UndoData
			{
				Index = this.IndexOf (item),
				Item  = item,
			};

			this.dict.Remove (item.Guid);
			this.list.Remove (item);

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.UInsert (data.Index, data.Item);
			},
			undoData, "GuidList.Remove");
		}


		public void RemoveAt(int index)
		{
			var undoItem = this.URemoveAt (index);

			if (this.undoManager != null)
			{
				this.undoManager.Push (undoItem);
			}
		}

		private UndoItem URemoveAt(int index)
		{
			//	Supprime un item de la liste et retourne l'information permettant
			//	de faire l'opération inverse.
			var undoData = new UndoData
			{
				Index = index,
				Item  = this.list[index],
			};

			this.dict.Remove (this.list[index].Guid);
			this.list.RemoveAt (index);

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.UInsert (data.Index, data.Item);
			},
			undoData, "GuidList.RemoveAt");
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


		private class UndoData : IUndoData
		{
			public int							Index;
			public T							Item;
		}


		private readonly UndoManager			undoManager;
		private readonly List<T>				list;
		private readonly Dictionary<Guid, T>	dict;
	}
}
