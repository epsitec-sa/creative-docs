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

		public void Push(UndoItem item)
		{
			if (this.lastExecuted+1 < this.items.Count)
			{
				int numCommandsToRemove = this.items.Count - (this.lastExecuted+1);
				for (int i=0; i<numCommandsToRemove; i++)
				{
					this.items.RemoveAt (this.lastExecuted+1);
				}
			}

			this.items.Add (item);

			this.lastExecuted = this.items.Count-1;
		}

		public void Undo()
		{
			if (this.lastExecuted >= 0 && this.items.Count > 0)
			{
				this.Swap ();
				this.lastExecuted--;
			}
		}

		public void Redo()
		{
			if (this.lastExecuted+1 < this.items.Count)
			{
				this.lastExecuted++;
				this.Swap ();
			}
		}

		private void Swap()
		{
			var item = this.items[this.lastExecuted];
			this.items[this.lastExecuted] = item.undoOperation (item.undoData);
		}


		private readonly List<UndoItem>			items;
		private int								lastExecuted;
		private int								lastSaved;
	}
}
