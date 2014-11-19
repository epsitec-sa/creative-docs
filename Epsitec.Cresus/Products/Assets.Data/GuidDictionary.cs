//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class GuidDictionary<T> : IEnumerable<T>
		where T : class, IGuid
	{
		public GuidDictionary(UndoManager undoManager)
		{
			this.undoManager = undoManager;

			this.dict = new Dictionary<Guid, T> ();
		}


		public int								Count
		{
			get
			{
				return this.dict.Count;
			}
		}


		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator()
		{
			return this.dict.Values.GetEnumerator ();
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator ();
		}
		#endregion


		public void Clear()
		{
			if (this.undoManager == null)
			{
				this.dict.Clear ();
			}
			else
			{
				//	On n'utilise pas Clear () pour permettre le undo !
				while (this.dict.Any ())
				{
					this.Remove (this.dict.First ().Value);
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

			var undoData = new UndoData
			{
				Item = item,
			};

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.URemove (data.Item);
			},
			undoData, "GuidDictionary.Add");
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
				Item  = item,
			};

			this.dict.Remove (item.Guid);

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.UAdd (data.Item);
			},
			undoData, "GuidDictionary.Remove");
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


		private class UndoData : IUndoData
		{
			public T							Item;
		}


		private readonly UndoManager			undoManager;
		private readonly Dictionary<Guid, T>	dict;
	}
}
