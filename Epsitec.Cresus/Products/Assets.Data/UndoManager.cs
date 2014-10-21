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
			this.groups = new List<UndoGroup> ();
			this.Clear ();
		}


		public void Clear()
		{
			this.groups.Clear ();
			this.lastExecuted = -1;
			this.lastSaved = -1;
		}

		public bool IsUndoEnable
		{
			get
			{
				return this.lastExecuted >= 0 && this.groups.Count > 0;
			}
		}

		public bool IsRedoEnable
		{
			get
			{
				return this.lastExecuted+1 < this.groups.Count;
			}
		}

		//?public void Save()
		//?{
		//?	this.lastSaved = this.lastExecuted;
		//?}
		//?
		//?public bool IsModified
		//?{
		//?	get
		//?	{
		//?		return this.lastSaved != this.lastExecuted;
		//?	}
		//?}

		public int Size
		{
			get
			{
				return this.groups.Count;
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
			while (this.groups.Count > numItems)
			{
				this.groups.RemoveAt (0);

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

		public void Start(string description)
		{
			if (this.lastExecuted+1 < this.groups.Count)
			{
				int numCommandsToRemove = this.groups.Count - (this.lastExecuted+1);
				for (int i=0; i<numCommandsToRemove; i++)
				{
					this.groups.RemoveAt (this.lastExecuted+1);
				}
			}

			var group = new UndoGroup (description);
			this.groups.Add (group);

			this.lastExecuted = this.groups.Count-1;
		}

		public void Push(UndoItem item)
		{
			if (this.groups.Any ())
			{
				var group = this.groups.Last ();
				group.Push (item);
			}
		}

		public void Undo()
		{
			if (this.IsUndoEnable)
			{
				this.groups[this.lastExecuted].Undo ();
				this.lastExecuted--;
			}
		}

		public void Redo()
		{
			if (this.IsRedoEnable)
			{
				this.lastExecuted++;
				this.groups[this.lastExecuted].Redo ();
			}
		}


		private readonly List<UndoGroup>		groups;
		private int								lastExecuted;
		private int								lastSaved;
	}
}
