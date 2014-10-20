//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class UndoManager
	{
		public UndoManager()
		{
			this.items = new List<UndoItem> ();
			this.Clear ();
		}


		public void Clear()
		{
			this.items.Clear ();
			this.lastExecuted = -1;
			this.lastSaved = -1;
		}

		public void Save()
		{
			this.lastSaved = this.lastExecuted;
		}

		public bool IsModified
		{
			get
			{
				return this.lastSaved != this.lastExecuted;
			}
		}

		public int Size
		{
			get
			{
				return this.items.Count;
			}
		}

		public int LastExecuted
		{
			get
			{
				return this.lastExecuted;
			}
		}

		public void Limit(int numItems)
		{
			while (this.items.Count > numItems)
			{
				this.items.RemoveAt (0);

				if (this.lastExecuted >= 0)
				{
					this.lastExecuted--;
				}

				if (this.lastSaved >= 0)
				{
					this.lastSaved--;
				}
			}
		}

		//?public void Add(UndoItem item, bool execute)
		//?{
		//?	if (this.lastExecuted+1 < this.items.Count)
		//?	{
		//?		int numCommandsToRemove = this.items.Count - (this.lastExecuted+1);
		//?		for (int i=0; i<numCommandsToRemove; i++)
		//?		{
		//?			this.items.RemoveAt (this.lastExecuted+1);
		//?		}
		//?	}
		//?
		//?	if (execute)
		//?	{
		//?		item.Do ();
		//?	}
		//?
		//?	this.items.Add (item);
		//?	this.lastExecuted = this.items.Count-1;
		//?}

		public void Push(System.Action<object> undoOperation, object undoData, string description)
		{
			if (this.lastExecuted+1 < this.items.Count)
			{
				int numCommandsToRemove = this.items.Count - (this.lastExecuted+1);
				for (int i=0; i<numCommandsToRemove; i++)
				{
					this.items.RemoveAt (this.lastExecuted+1);
				}
			}

			var item = new UndoItem (undoOperation, undoData, description);
			this.items.Add (item);

			this.lastExecuted = this.items.Count-1;
		}

		public void Undo()
		{
			if (this.lastExecuted >= 0)
			{
				if (this.items.Count > 0)
				{
					var item = this.items[this.lastExecuted];
					item.undoOperation (item.undoData);
					this.lastExecuted--;
				}
			}
		}

		public void Redo()
		{
			if (this.lastExecuted+1 < this.items.Count)
			{
				var item = this.items[this.lastExecuted+1];
				item.undoOperation (item.undoData);
				this.lastExecuted++;
			}
		}


		private struct UndoItem
		{
			public UndoItem(System.Action<object> undoOperation, object undoData, string description)
			{
				this.undoOperation = undoOperation;
				this.undoData = undoData;
				this.description = description;
			}

			//?public System.Action Do;
			//?public System.Action Undo;

			public readonly System.Action<object> undoOperation;
			public readonly object undoData;
			public readonly string description;
		}


		private readonly List<UndoItem> items;
		private int lastExecuted;
		private int lastSaved;
	}
}
