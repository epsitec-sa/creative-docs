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


		public void SetViewStateGetter(System.Func<IViewState> getViewState)
		{
			//	Spécifie la fonction a exécuter pour obtenir le ViewState.
			this.getViewState = getViewState;
		}


		public void Clear()
		{
			this.groups.Clear ();
			this.lastExecuted = -1;
		}

		public bool								IsUndoEnable
		{
			get
			{
				return this.lastExecuted >= 0 && this.groups.Count > 0;
			}
		}

		public bool								IsRedoEnable
		{
			get
			{
				return this.lastExecuted+1 < this.groups.Count;
			}
		}

		
		public IEnumerable<string>				UndoList
		{
			get
			{
				for (int i=this.lastExecuted; i>=0; i--)
				{
					yield return this.groups[i].Description;
				}
			}
		}

		public IEnumerable<string>				RedoList
		{
			get
			{
				for (int i=this.lastExecuted+1; i<this.groups.Count; i++)
				{
					yield return this.groups[i].Description;
				}
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
			}
		}


		public void Start()
		{
			if (this.lastExecuted+1 < this.groups.Count)
			{
				int numCommandsToRemove = this.groups.Count - (this.lastExecuted+1);
				for (int i=0; i<numCommandsToRemove; i++)
				{
					this.groups.RemoveAt (this.lastExecuted+1);
				}
			}

			var group = new UndoGroup ();
			this.groups.Add (group);

			this.lastExecuted = this.groups.Count-1;

			this.SetBeforeViewState ();
		}

		public void SetDescription(string description)
		{
			if (this.groups.Any ())
			{
				var group = this.groups.Last ();
				group.Description = description;
			}
		}

		private void SetBeforeViewState()
		{
			if (this.groups.Any () && this.getViewState != null)
			{
				var group = this.groups.Last ();
				group.BeforeViewState = this.getViewState ();
			}
		}

		public void SetAfterViewState()
		{
			if (this.groups.Any () && this.getViewState != null)
			{
				var group = this.groups.Last ();
				group.AfterViewState = this.getViewState ();
			}
		}

		public void Push(UndoItem item)
		{
			if (this.groups.Any ())
			{
				var group = this.groups.Last ();
				group.Push (item);
			}
		}


		public IViewState Undo()
		{
			if (this.IsUndoEnable)
			{
				var group = this.groups[this.lastExecuted--];

				group.Undo ();
				return group.BeforeViewState;
			}
			else
			{
				return null;
			}
		}

		public IViewState Redo()
		{
			if (this.IsRedoEnable)
			{
				var group = this.groups[++this.lastExecuted];

				group.Redo ();
				return group.AfterViewState;
			}
			else
			{
				return null;
			}
		}


		private readonly List<UndoGroup>		groups;
		private int								lastExecuted;
		private System.Func<IViewState>			getViewState;
	}
}
