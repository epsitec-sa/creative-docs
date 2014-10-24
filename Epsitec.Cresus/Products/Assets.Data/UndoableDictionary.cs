//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class UndoableDictionary<TKey, TValue>
		where TValue : class
	{
		public UndoableDictionary(UndoManager undoManager)
		{
			this.undoManager = undoManager;

			this.dict = new Dictionary<TKey, TValue> ();
		}


		public int								Count
		{
			get
			{
				return this.dict.Count;
			}
		}


		public void Clear()
		{
			//	On n'utilise pas this.dict.Clear () pour permettre le undo !
			foreach (var key in this.dict.Keys.ToArray ())
			{
				this.Remove (key);
			}
		}


		public void Add(TKey key, TValue item)
		{
			var undoItem = this.UAdd (key, item);

			if (this.undoManager != null)
			{
				this.undoManager.Push (undoItem);
			}
		}

		private UndoItem UAdd(TKey key, TValue item)
		{
			//	Ajoute un item dans le dictionnaire et retourne l'information permettant
			//	de faire l'opération inverse.
			TValue currentItem;
			if (this.TryGetValue (key, out currentItem))
			{
				//	S'il y a déjà un item correspondant à la clé, l'opération d'annulation
				//	devra remettre l'item intitial.
				this.dict.Add (key, item);

				var undoData = new UndoData
				{
					Key  = key,
					Item = currentItem,
				};

				return new UndoItem (delegate (IUndoData d)
				{
					var data = d as UndoData;
					return this.UAdd (data.Key, data.Item);
				},
				undoData, "UndoableDictionary.Add");
			}
			else
			{
				//	Si l'item correspondant à la clé n'est pas déjà dans le dictionnaire,
				//	l'opération d'annulation devra simplement supprimer la clé.
				this.dict.Add (key, item);

				var undoData = new UndoData
				{
					Key  = key,
				};

				return new UndoItem (delegate (IUndoData d)
				{
					var data = d as UndoData;
					return this.URemove (data.Key);
				},
				undoData, "UndoableDictionary.Add");
			}
		}


		public void Remove(TKey key)
		{
			var undoItem = this.URemove (key);

			if (this.undoManager != null)
			{
				this.undoManager.Push (undoItem);
			}
		}

		private UndoItem URemove(TKey key)
		{
			//	Supprime un item du dictionnaire et retourne l'information permettant
			//	de faire l'opération inverse.
			var undoData = new UndoData
			{
				Key  = key,
				Item = this.dict[key],
			};

			this.dict.Remove (key);

			return new UndoItem (delegate (IUndoData d)
			{
				var data = d as UndoData;
				return this.UAdd (data.Key, data.Item);
			},
			undoData, "UndoableDictionary.Remove");
		}


		public bool TryGetValue(TKey key, out TValue item)
		{
			return this.dict.TryGetValue (key, out item);
		}

		public IEnumerable<TKey> Select(System.Func<KeyValuePair<TKey,TValue>,TKey> selector)
		{
			return this.dict.Select (selector);
		}


		public TValue this[TKey key]
		{
			get
			{
				return this.dict[key];
			}
			set
			{
				var undoItem = this.UAdd (key, value);

				if (this.undoManager != null)
				{
					this.undoManager.Push (undoItem);
				}
			}
		}


		private class UndoData : IUndoData
		{
			public TKey							Key;
			public TValue						Item;
		}


		private readonly UndoManager				undoManager;
		private readonly Dictionary<TKey, TValue>	dict;
	}
}
