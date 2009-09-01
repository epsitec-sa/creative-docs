//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe OpletQueue implémente l'infrastructure pour le UNDO/REDO
	/// généralisé; chaque action se décompose en "oplets" (cf. IOplet).
	/// </summary>
	public class OpletQueue
	{
		public OpletQueue()
		{
			this.queue      = new System.Collections.ArrayList ();
			this.tempQueue  = new System.Collections.ArrayList ();
		}
		
		
		public bool								CanUndo
		{
			get
			{
				return this.liveFence > 0;
			}
		}
		
		public bool								CanRedo
		{
			get
			{
				return this.liveFence < this.fenceCount;
			}
		}
		
		public int								UndoActionCount
		{
			get
			{
				return this.liveFence;
			}
		}
		
		public int								RedoActionCount
		{
			get
			{
				return this.fenceCount - this.liveFence;
			}
		}
		
		public IOplet[]							LastActionOplets
		{
			get
			{
				if ((this.action == null) &&
					(this.liveFence > 0) &&
					(this.liveIndex > 0))
				{
					int i = this.liveIndex - 1;
					int n = 0;
					
					IOplet oplet = this.queue[i] as IOplet;
					
					System.Diagnostics.Debug.Assert (oplet.IsFence);
					
					i--;
					
					while (i >= 0)
					{
						oplet = this.queue[i] as IOplet;
						
						if (oplet.IsFence)
						{
							break;
						}
						
						i--;
						n++;
					}
					
					IOplet[] oplets = new IOplet[n];
					
					int j = 0;
					i++;
					
					while (n > 0)
					{
						oplets[j] = this.queue[i] as IOplet;
						
						i++;
						j++;
						
						n--;
					}
					
					return oplets;
				}
				
				return new IOplet[0];
			}
		}
		
		public IOplet[]							LastActionMinusOneOplets
		{
			get
			{
				if ((this.action == null) &&
					(this.liveFence > 0) &&
					(this.liveIndex > 0))
				{
					int i = this.liveIndex - 1;
					int n = 0;
					
					IOplet oplet = this.queue[i] as IOplet;
					
					System.Diagnostics.Debug.Assert (oplet.IsFence);
					
					i--;
					
					while (i >= 0)
					{
						oplet = this.queue[i] as IOplet;
						
						if (oplet.IsFence)
						{
							break;
						}
						
						i--;
					}
					
					i--;
					
					while (i >= 0)
					{
						oplet = this.queue[i] as IOplet;
						
						if (oplet.IsFence)
						{
							break;
						}
						
						i--;
						n++;
					}
					
					IOplet[] oplets = new IOplet[n];
					
					int j = 0;
					i++;
					
					while (n > 0)
					{
						oplets[j] = this.queue[i] as IOplet;
						
						i++;
						j++;
						
						n--;
					}
					
					return oplets;
				}
				
				return new IOplet[0];
			}
		}
		
		public string[]							UndoActionNames
		{
			get
			{
				if (this.liveFence > 0)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.liveIndex;
					int n = this.liveFence;
					int j = 0;
					
					string[] names = new string[n];
					
					while (--i > 0)
					{
						IOplet oplet = this.queue[i] as IOplet;
						
						if (oplet.IsFence)
						{
							Types.IName fence = oplet as Types.IName;
							names[j++] = (fence == null) ? "" : fence.Name;
						}
					}
					
					return names;
				}
				
				return new string[0];
			}
		}
		
		public string[]							RedoActionNames
		{
			get
			{
				if (this.liveFence < this.fenceCount)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex < this.queue.Count);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.liveIndex - 1;
					int n = this.fenceCount - this.liveFence;
					int j = 0;
					
					string[] names = new string[n];
					
					while (++i < this.queue.Count)
					{
						IOplet oplet = this.queue[i] as IOplet;
						
						if (oplet.IsFence)
						{
							Types.IName fence = oplet as Types.IName;
							names[j++] = (fence == null) ? "" : fence.Name;
						}
					}
					
					return names;
				}
				
				return new string[0];
			}
		}
		
		
		public string							LastActionName
		{
			get
			{
				if (this.liveFence > 0)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
					int i = this.liveIndex - 1;
				
					IOplet      oplet = this.queue[i] as IOplet;
					Types.IName fence = oplet as Types.IName;
				
					System.Diagnostics.Debug.Assert (oplet.IsFence);
					System.Diagnostics.Debug.Assert (fence != null);
				
					return fence.Name;
				}
			
				return null;
			}
		}
		
		public MergeMode						LastActionMergeMode
		{
			get
			{
				if (this.liveFence > 0)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
					int i = this.liveIndex - 1;
				
					IOplet oplet = this.queue[i] as IOplet;
					Fence  fence = oplet as Fence;
				
					System.Diagnostics.Debug.Assert (oplet.IsFence);
				
					return fence == null ? MergeMode.Automatic : fence.MergeMode;
				}
			
				return MergeMode.None;
			}
		}
		
		public MergeMode						LastActionMinusOneMergeMode
		{
			get
			{
				if (this.liveFence > 1)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
					int i = this.liveIndex;
					int n = 0;
				
					while (i > 0)
					{
						i--;
					
						IOplet oplet = this.queue[i] as IOplet;
					
						if (oplet.IsFence)
						{
							if (n++ == 1)
							{
								Fence  fence = oplet as Fence;
								return fence == null ? MergeMode.Automatic : fence.MergeMode;
							}
						}
					}
				}
			
				return MergeMode.None;
			}
		}
		
		
		public int								PendingOpletCount
		{
			get
			{
				return this.tempQueue.Count;
			}
		}
		
		public IOplet[]							PendingOplets
		{
			get
			{
				return (IOplet[]) this.tempQueue.ToArray (typeof (IOplet));
			}
		}
		
		public MergeMode						PendingMergeMode
		{
			get
			{
				if (this.disableMerge)
				{
					return MergeMode.Disabled;
				}
				else if (this.action == null)
				{
					return MergeMode.None;
				}
				else
				{
					return this.action.MergeMode;
				}
			}
		}
		
		
		public bool								IsDisabled
		{
			get
			{
				return (this.disableCount > 0);
			}
		}
		
		public bool								IsEnabled
		{
			get
			{
				return (this.disableCount == 0);
			}
		}
		
		public bool								IsUndoRedoInProgress
		{
			get
			{
				return this.isUndoRedoInProgress;
			}
		}
		
		public bool								IsActionDefinitionInProgress
		{
			get
			{
				if ((this.fenceId <= 0) ||
					(this.action == null))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		
		
		public System.IDisposable BeginAction()
		{
			return this.BeginAction (null);
		}
		
		public System.IDisposable BeginAction(string name)
		{
			return this.BeginAction (name, MergeMode.Automatic);
		}

		public System.IDisposable BeginAction(string name, MergeMode mode)
		{
			if (this.IsDisabled)
			{
				return null;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			
			if (this.action == null)
			{
				this.PurgeRedo ();
			}
			
			System.Diagnostics.Debug.Assert (this.CanRedo == false);
			
			return new AutoActionCleanup (this, name, mode);
		}
		
		
		public void Insert(IOplet oplet)
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if ((this.fenceId <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction must be called before any oplets can be inserted into the queue.");
			}
			
			this.tempQueue.Add (oplet);
		}
		
		
		public void DefineActionName(string name)
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if ((this.fenceId <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/ValidateAction mismatch.");
			}
			
			this.action.Name = name;
		}
		
		public void ChangeLastActionName(string name)
		{
			if (this.liveFence > 0)
			{
				System.Diagnostics.Debug.Assert (this.liveIndex > 0);
				System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
				int i = this.liveIndex - 1;
				
				IOplet oplet = this.queue[i] as IOplet;
				Fence  fence = oplet as Fence;
				
				System.Diagnostics.Debug.Assert (oplet.IsFence);
				
				this.queue[i] = new Fence (name, fence == null ? MergeMode.Automatic : fence.MergeMode);
				
				oplet.Dispose ();
			}
		}
		
		
		public void DisableMerge()
		{
			if (this.action != null)
			{
				this.action.DefineMergeMode (MergeMode.Disabled);
			}
			else
			{
				this.disableMerge = true;
			}
		}
		
		
		#region MergeMode Enumeration
		public enum MergeMode
		{
			None,
			Automatic,
			Disabled
		}
		#endregion
		
		public void ValidateAction()
		{
			this.ValidateAction (MergeMode.Automatic);
		}
		
		public void ValidateAction(MergeMode mode)
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if ((this.fenceId <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/ValidateAction mismatch.");
			}
			
			if (this.action.MergeMode == MergeMode.Disabled)
			{
				mode = MergeMode.Disabled;
			}
			
			this.action.Release ();
			
			if (this.fenceId == 0)
			{
				//	Toutes les actions "ouvertes" ont été validées. On peut donc copier les oplets
				//	(avec leurs frontières) dans la liste officielle.
				
				System.Diagnostics.Debug.Assert (this.action == null);
				
				this.PurgeRedo ();
				
				if (this.tempQueue.Count > 0)
				{
					//	N'insère un élément dans la liste que si des oplets seront effectivement
					//	ajoutés; une insertion vide ne va pas apparaître dans la queue !
					
					if (this.disableMerge)
					{
						if (this.liveIndex > 0)
						{
							//	Empêche la fusion de cette série d'oplets avec la série précédente;
							//	il faut donc marquer la séquence précédente :
							
							Fence fence = this.queue[this.liveIndex-1] as Fence;
							
							if (fence != null)
							{
								fence.MergeMode = MergeMode.Disabled;
							}
						}
					}
					
					this.queue.AddRange (this.tempQueue);
					this.queue.Add (new Fence (this.tempName, mode));
					this.tempQueue.Clear ();
					
					this.fenceCount++;
					
					this.liveFence = this.fenceCount;
					this.liveIndex = this.queue.Count;
				}
				
				this.OnActionValidated ();
				this.disableMerge = false;
			}
		}
		
		public void CancelAction()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if ((this.fenceId <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/CancelAction mismatch.");
			}
			
			int i = this.tempQueue.Count;
			
			//	Il faut retirer tous les oplets faisant partie de l'action, de la liste
			//	temporaire, et les supprimer proprement :
			
			while (i-- > this.action.Depth)
			{
				IOplet oplet = this.tempQueue[i] as IOplet;

				oplet.Undo ();
				oplet.Dispose ();
				
				this.tempQueue.RemoveAt (i);
			}
			
			this.action.Release ();

			if ((this.action == null) ||
				(this.fenceId <= 0))
			{
				System.Diagnostics.Debug.Assert (this.action == null);
				System.Diagnostics.Debug.Assert (this.fenceId == 0);
				System.Diagnostics.Debug.Assert (this.tempQueue.Count == 0);
				
				this.OnActionCanceled ();
			}
		}
		
		
		public bool UndoAction()
		{
			if (this.IsDisabled)
			{
				return false;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			try
			{
				this.isUndoRedoInProgress = true;
				
				if (this.liveFence > 0)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.liveIndex - 1;
					
					IOplet oplet = this.queue[i] as IOplet;
					this.liveFence--;
					
					System.Diagnostics.Debug.Assert (oplet.IsFence);
					
					while (i > 0)
					{
						oplet = this.queue[--i] as IOplet;
						
						if (oplet.IsFence)
						{
							i++;
							break;
						}
						
						this.queue[i] = oplet.Undo ();
					}
					
					this.liveIndex = i;
					this.OnUndoExecuted ();
					
					return true;
				}
				
				return false;
			}
			finally
			{
				this.isUndoRedoInProgress = false;
			}
		}
		
		public bool RedoAction()
		{
			if (this.IsDisabled)
			{
				return false;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			this.isUndoRedoInProgress = true;
			
			try
			{
				if (this.liveFence < this.fenceCount)
				{
					System.Diagnostics.Debug.Assert (this.liveIndex < this.queue.Count);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.liveIndex - 1;
					
					for (;;)
					{
						IOplet oplet = this.queue[++i] as IOplet;
						
						if (oplet.IsFence)
						{
							this.liveIndex = i + 1;
							this.liveFence++;
							break;
						}
						
						this.queue[i] = oplet.Redo ();
					}

					this.OnRedoExecuted ();
					
					return true;
				}
				
				return false;
			}
			finally
			{
				this.isUndoRedoInProgress = false;
			}
		}
		
		
		public void MergeLastActions()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			if (this.liveFence < 2)
			{
				return;
			}
			
			int i = this.liveIndex;
			int n = 0;
			
			System.Collections.Stack temp = new System.Collections.Stack ();
			
			while (i > 0)
			{
				i--;
				
				IOplet oplet = this.queue[i] as IOplet;
				
				if (oplet.IsFence)
				{
					if (n > 0)
					{
						this.queue.RemoveAt (i);
						break;
					}
					
					this.liveFence--;
					this.fenceCount--;
				}
				
				n++;
				
				temp.Push (oplet);
				this.queue.RemoveAt (i);
			}
			
			while (temp.Count > 0)
			{
				this.queue.Insert (i++, temp.Pop ());
			}
			
			this.liveIndex = i;
			
			System.Diagnostics.Debug.Assert (this.liveIndex >= 0);
			System.Diagnostics.Debug.Assert (this.liveFence >= 0);
		}
		
		public void PurgeSingleUndo()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			if (this.liveFence < 1)
			{
				return;
			}
			
			int i = this.liveIndex;
			int n = 0;
			
			while (i > 0)
			{
				i--;
				
				IOplet oplet = this.queue[i] as IOplet;
				
				if (oplet.IsFence)
				{
					if (n > 0)
					{
						break;
					}
					
					this.liveFence--;
					this.fenceCount--;
				}
				
				n++;
				
				this.queue.RemoveAt (i);
				oplet.Dispose ();
			}
			
			this.liveIndex = (i == 0) ? 0 : (i+1);
			
			System.Diagnostics.Debug.Assert (this.liveIndex >= 0);
			System.Diagnostics.Debug.Assert (this.liveFence >= 0);
		}
		
		
		public void PurgeUndo()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			int i = this.liveIndex;
			
			while (i > 0)
			{
				i--;
				
				IOplet oplet = this.queue[i] as IOplet;
				
				if (oplet.IsFence)
				{
					this.liveFence--;
					this.fenceCount--;
				}
				
				this.queue.RemoveAt (i);
				oplet.Dispose ();
			}
			
			this.liveIndex = 0;
			
			System.Diagnostics.Debug.Assert (this.liveFence == 0);
		}
		
		public void PurgeRedo()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.isUndoRedoInProgress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			int i = this.liveIndex;
			
			while (i < this.queue.Count)
			{
				IOplet oplet = this.queue[i] as IOplet;
				
				if (oplet.IsFence)
				{
					this.fenceCount--;
				}
				
				this.queue.RemoveAt (i);
				oplet.Dispose ();
			}
			
			System.Diagnostics.Debug.Assert (this.liveFence == this.fenceCount);
			System.Diagnostics.Debug.Assert (this.liveIndex == this.queue.Count);
		}
		
		
		public void Disable()
		{
			lock (this)
			{
				this.disableCount++;
			}
		}
		
		public void Enable()
		{
			lock (this)
			{
				if (this.disableCount == 0)
				{
					throw new System.InvalidOperationException ("Enable not possible, queue is not disabled.");
				}
				
				this.disableCount--;
			}
		}
		
		
		protected class AutoActionCleanup : System.IDisposable
		{
			public AutoActionCleanup(OpletQueue queue, string name, MergeMode mode)
			{
				queue.fenceId++;
				
				this.queue    = queue;
				this.fenceId = queue.fenceId;
				this.link     = queue.action;
				this.depth    = queue.tempQueue.Count;
				this.name     = name;
				this.mode     = mode;
				
				queue.action = this;
			}
			
			
			public string						Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}
			
			public AutoActionCleanup			Link
			{
				get
				{
					return this.link;
				}
			}
			
			public int							Depth
			{
				get
				{
					return this.depth;
				}
			}
			
			public MergeMode					MergeMode
			{
				get
				{
					if (this.link == null)
					{
						return this.mode;
					}
					else
					{
						if (this.link.MergeMode == MergeMode.Disabled)
						{
							return MergeMode.Disabled;
						}
						else
						{
							return this.mode;
						}
					}
				}
			}
			
			
			public void DefineMergeMode(MergeMode mode)
			{
				this.mode = mode;
			}
			
			public void Release()
			{
				if ((this.queue.action != this) ||
					(this.queue.fenceId != this.fenceId))
				{
					throw new System.InvalidOperationException ("BeginAction/release mismatch.");
				}
				
				this.queue.fenceId--;
				this.queue.action    = this.link;
				this.queue.tempName = this.name;
				
				this.queue = null;
				this.link  = null;
			}
			
			
			#region IDisposable Members
			public void Dispose()
			{
				if (this.queue != null)
				{
					this.queue.CancelAction ();
				}
			}
			#endregion
			
			protected OpletQueue				queue;
			protected int						fenceId;
			
			protected AutoActionCleanup			link;
			protected int						depth;
			protected string					name;
			protected MergeMode					mode;
		}
		
		protected class Fence : IOplet, Types.IName
		{
			public Fence(string name) : this (name, MergeMode.Automatic)
			{
			}
			
			public Fence(string name, MergeMode mode)
			{
				this.name = name;
				this.mode = mode;
			}
			
			
			public bool							IsFence
			{
				get
				{
					return true;
				}
			}
			
			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			
			public MergeMode					MergeMode
			{
				get
				{
					return this.mode;
				}
				set
				{
					this.mode = value;
				}
			}

			
			public IOplet Undo()
			{
				return this;
			}

			public IOplet Redo()
			{
				return this;
			}

			public void Dispose()
			{
			}
			
			
			protected string					name;
			protected MergeMode					mode;
		}
		
		
		protected virtual void OnActionValidated()
		{
			if (this.ActionValidated != null)
			{
				this.ActionValidated (this);
			}
		}
		
		protected virtual void OnActionCanceled()
		{
			if (this.ActionCanceled != null)
			{
				this.ActionCanceled (this);
			}
		}

		protected virtual void OnUndoExecuted()
		{
			if (this.UndoExecuted != null)
			{
				this.UndoExecuted (this);
			}
		}

		protected virtual void OnRedoExecuted()
		{
			if (this.RedoExecuted != null)
			{
				this.RedoExecuted (this);
			}
		}
		
		public event EventHandler				ActionValidated;
		public event EventHandler				ActionCanceled;
		public event EventHandler				UndoExecuted;
		public event EventHandler				RedoExecuted;
		
		protected System.Collections.ArrayList	queue;
		protected System.Collections.ArrayList	tempQueue;
		protected string						tempName;
		
		protected AutoActionCleanup				action;
		
		protected int							liveIndex;
		protected int							liveFence;
		protected int							fenceCount;
		protected int							fenceId;
		
		protected bool							isUndoRedoInProgress;
		protected bool							disableMerge;
		
		private int								disableCount;
	}
}
