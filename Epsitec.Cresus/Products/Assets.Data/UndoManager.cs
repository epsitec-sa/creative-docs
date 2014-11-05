//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

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
			//	Sp�cifie la fonction a ex�cuter pour obtenir le ViewState.
			this.getViewState = getViewState;
		}


		public void Clear()
		{
			this.groups.Clear ();
			this.lastExecuted = -1;

			this.OnChanged ();
		}

		public bool								IsUndoEnable
		{
			//	Indique s'il y a au moins une action � annuler.
			get
			{
				return this.lastExecuted >= 0 && this.groups.Count > 0;
			}
		}

		public bool								IsRedoEnable
		{
			//	Indique s'il y a au moins une action � r�tablir.
			get
			{
				return this.lastExecuted+1 < this.groups.Count;
			}
		}


		public string							CurrentUndoDescription
		{
			get
			{
				if (this.IsUndoEnable)
				{
					return this.GetDescription (this.lastExecuted, undo: true);
				}
				else
				{
					return null;
				}
			}
		}

		public string							CurrentRedoDescription
		{
			get
			{
				if (this.IsRedoEnable)
				{
					return this.GetDescription (this.lastExecuted+1, undo: false);
				}
				else
				{
					return null;
				}
			}
		}

		
		public IEnumerable<string>				UndoHistory
		{
			//	Retourne les descriptions des actions qu'il est possible d'annuler,
			//	de la plus r�cente � la plus ancienne.
			get
			{
				for (int i=this.lastExecuted; i>=0; i--)
				{
					yield return this.GetDescription (i, undo: true);
				}
			}
		}

		public IEnumerable<string>				RedoHistory
		{
			//	Retourne les descriptions des actions qu'il est possible de r�tablir,
			//	de la plus ancienne � la plus r�cente.
			get
			{
				for (int i=this.lastExecuted+1; i<this.groups.Count; i++)
				{
					yield return this.GetDescription (i, undo: false);
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
			//	Marque le d�but d'une action annulable. On d�marre un nouveau groupe.
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
			//	Sp�cifie la description de l'action annulable.
			if (this.groups.Any ())
			{
				var group = this.groups.Last ();
				group.Description = description;
			}
		}

		public void ReplaceBeforeViewState(IViewState viewState)
		{
			//	Remplace le ViewState initial, avant les modifications.
			if (this.groups.Any ())
			{
				var group = this.groups.Last ();
				group.BeforeViewState = viewState;
			}
		}

		private void SetBeforeViewState()
		{
			//	Sp�cifie le ViewState initial, avant les modifications.
			if (this.groups.Any () && this.getViewState != null)
			{
				var group = this.groups.Last ();
				group.BeforeViewState = this.getViewState ();
			}
		}

		public void SetAfterViewState()
		{
			//	Sp�cifie le ViewState final, apr�s les modifications.
			if (this.groups.Any () && this.getViewState != null)
			{
				var group = this.groups.Last ();
				group.AfterViewState = this.getViewState ();

				this.OnChanged ();
			}
		}

		public void Push(UndoItem item)
		{
			//	Ajoute une action annulable au groupe en cours.
			if (this.groups.Any ())
			{
				var group = this.groups.Last ();
				group.Push (item);
			}
		}


		public IViewState Undo()
		{
			//	Annule la derni�re action et retourne le ViewState initial.
			if (this.IsUndoEnable)
			{
				var group = this.groups[this.lastExecuted--];

				group.Undo ();
				this.OnChanged ();

				return group.BeforeViewState;
			}
			else
			{
				return null;
			}
		}

		public IViewState Redo()
		{
			//	Refait la derni�re action et retourne le ViewState final.
			if (this.IsRedoEnable)
			{
				var group = this.groups[++this.lastExecuted];

				group.Redo ();
				this.OnChanged ();

				return group.AfterViewState;
			}
			else
			{
				return null;
			}
		}


		private string GetDescription(int index, bool undo)
		{
			//	Retourne la description d'une action undo/redo compl�te, par exemple
			//	"R�tablir � Supprimer le contact - Jean Dupond �"
			var description = this.groups[index].Description;

			if (undo)  // undo ?
			{
				return string.Format (Res.Strings.UndoManager.Undo.Description.ToString (), description);
			}
			else  // redo ?
			{
				return string.Format (Res.Strings.UndoManager.Redo.Description.ToString (), description);
			}
		}

		public static string GetDescription(string op, string objectSummary)
		{
			//	Retourne la description d'une action undo/redo, par exemple
			//	"Supprimer le contact - Jean Dupond"
			if (string.IsNullOrEmpty (objectSummary))
			{
				return op;
			}
			else
			{
				return string.Concat (op, " � ", objectSummary);
			}
		}


		#region Events handler
		private void OnChanged()
		{
			this.Changed.Raise (this);
		}

		public event EventHandler Changed;
		#endregion


		private readonly List<UndoGroup>		groups;
		private int								lastExecuted;
		private System.Func<IViewState>			getViewState;
	}
}