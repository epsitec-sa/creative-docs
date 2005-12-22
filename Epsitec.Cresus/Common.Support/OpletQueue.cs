//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.temp_queue = new System.Collections.ArrayList ();
		}
		
		
		public bool								CanUndo
		{
			get
			{
				return this.live_fence > 0;
			}
		}
		
		public bool								CanRedo
		{
			get
			{
				return this.live_fence < this.fence_count;
			}
		}
		
		public int								UndoActionCount
		{
			get
			{
				return this.live_fence;
			}
		}
		
		public int								RedoActionCount
		{
			get
			{
				return this.fence_count - this.live_fence;
			}
		}
		
		public IOplet[]							LastActionOplets
		{
			get
			{
				if ((this.action == null) &&
					(this.live_fence > 0) &&
					(this.live_index > 0))
				{
					int i = this.live_index - 1;
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
					(this.live_fence > 0) &&
					(this.live_index > 0))
				{
					int i = this.live_index - 1;
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
				if (this.live_fence > 0)
				{
					System.Diagnostics.Debug.Assert (this.live_index > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.live_index;
					int n = this.live_fence;
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
				if (this.live_fence < this.fence_count)
				{
					System.Diagnostics.Debug.Assert (this.live_index < this.queue.Count);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.live_index - 1;
					int n = this.fence_count - this.live_fence;
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
				if (this.live_fence > 0)
				{
					System.Diagnostics.Debug.Assert (this.live_index > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
					int i = this.live_index - 1;
				
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
				if (this.live_fence > 0)
				{
					System.Diagnostics.Debug.Assert (this.live_index > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
					int i = this.live_index - 1;
				
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
				if (this.live_fence > 1)
				{
					System.Diagnostics.Debug.Assert (this.live_index > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
					int i = this.live_index;
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
				return this.temp_queue.Count;
			}
		}
		
		public MergeMode						PendingMergeMode
		{
			get
			{
				if (this.action == null)
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
				return (this.disable_count > 0);
			}
		}
		
		public bool								IsEnabled
		{
			get
			{
				return (this.disable_count == 0);
			}
		}
		
		public bool								IsUndoRedoInProgress
		{
			get
			{
				return this.is_undo_redo_in_progress;
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
			
			if (this.is_undo_redo_in_progress)
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
			
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction must be called before any oplets can be inserted into the queue.");
			}
			
			this.temp_queue.Add (oplet);
		}
		
		
		public void DefineActionName(string name)
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/ValidateAction mismatch.");
			}
			
			this.action.Name = name;
		}
		
		public void DefineLastActionName(string name)
		{
			if (this.live_fence > 0)
			{
				System.Diagnostics.Debug.Assert (this.live_index > 0);
				System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
				int i = this.live_index - 1;
				
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
		}
		
		
		public enum MergeMode
		{
			None,
			Automatic,
			Disabled
		}
		
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
			
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/ValidateAction mismatch.");
			}
			
			if (this.action.MergeMode == MergeMode.Disabled)
			{
				mode = MergeMode.Disabled;
			}
			
			this.action.Release ();
			
			if (this.fence_id == 0)
			{
				//	Toutes les actions "ouvertes" ont été validées. On peut donc copier les oplets
				//	(avec leurs frontières) dans la liste officielle.
				
				System.Diagnostics.Debug.Assert (this.action == null);
				
				this.PurgeRedo ();
				
				if (this.temp_queue.Count > 0)
				{
					//	N'insère un élément dans la liste que si des oplets seront effectivement
					//	ajoutés; une insertion vide ne va pas apparaître dans la queue !
					
					this.queue.AddRange (this.temp_queue);
					this.queue.Add (new Fence (this.temp_name, mode));
					this.temp_queue.Clear ();
					
					this.fence_count++;
					
					this.live_fence = this.fence_count;
					this.live_index = this.queue.Count;
				}
			}
		}
		
		public void CancelAction()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/CancelAction mismatch.");
			}
			
			int i = this.temp_queue.Count;
			
			//	Il faut retirer tous les oplets faisant partie de l'action, de la liste
			//	temporaire, et les supprimer proprement :
			
			while (i-- > this.action.Depth)
			{
				IOplet oplet = this.temp_queue[i] as IOplet;
				
				oplet.Dispose ();
				
				this.temp_queue.RemoveAt (i);
			}
			
			this.action.Release ();

			if ((this.action == null) ||
				(this.fence_id <= 0))
			{
				System.Diagnostics.Debug.Assert (this.action == null);
				System.Diagnostics.Debug.Assert (this.fence_id == 0);
				System.Diagnostics.Debug.Assert (this.temp_queue.Count == 0);
			}
		}
		
		
		public bool UndoAction()
		{
			if (this.IsDisabled)
			{
				return false;
			}
			
			if (this.is_undo_redo_in_progress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			try
			{
				this.is_undo_redo_in_progress = true;
				
				if (this.live_fence > 0)
				{
					System.Diagnostics.Debug.Assert (this.live_index > 0);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.live_index - 1;
					
					IOplet oplet = this.queue[i] as IOplet;
					this.live_fence--;
					
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
					
					this.live_index = i;
					
					return true;
				}
				
				return false;
			}
			finally
			{
				this.is_undo_redo_in_progress = false;
			}
		}
		
		public bool RedoAction()
		{
			if (this.IsDisabled)
			{
				return false;
			}
			
			if (this.is_undo_redo_in_progress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			this.is_undo_redo_in_progress = true;
			
			try
			{
				if (this.live_fence < this.fence_count)
				{
					System.Diagnostics.Debug.Assert (this.live_index < this.queue.Count);
					System.Diagnostics.Debug.Assert (this.queue.Count > 0);
					
					int i = this.live_index - 1;
					
					for (;;)
					{
						IOplet oplet = this.queue[++i] as IOplet;
						
						if (oplet.IsFence)
						{
							this.live_index = i + 1;
							this.live_fence++;
							break;
						}
						
						this.queue[i] = oplet.Redo ();
					}
					
					return true;
				}
				
				return false;
			}
			finally
			{
				this.is_undo_redo_in_progress = false;
			}
		}
		
		
		public void MergeLastActions()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.is_undo_redo_in_progress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			if (this.live_fence < 2)
			{
				return;
			}
			
			int i = this.live_index;
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
					
					this.live_fence--;
					this.fence_count--;
				}
				
				n++;
				
				temp.Push (oplet);
				this.queue.RemoveAt (i);
			}
			
			while (temp.Count > 0)
			{
				this.queue.Insert (i++, temp.Pop ());
			}
			
			this.live_index = i;
			
			System.Diagnostics.Debug.Assert (this.live_index >= 0);
			System.Diagnostics.Debug.Assert (this.live_fence >= 0);
		}
		
		public void PurgeSingleUndo()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.is_undo_redo_in_progress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			if (this.live_fence < 1)
			{
				return;
			}
			
			int i = this.live_index;
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
					
					this.live_fence--;
					this.fence_count--;
				}
				
				n++;
				
				this.queue.RemoveAt (i);
				oplet.Dispose ();
			}
			
			this.live_index = (i == 0) ? 0 : (i+1);
			
			System.Diagnostics.Debug.Assert (this.live_index >= 0);
			System.Diagnostics.Debug.Assert (this.live_fence >= 0);
		}
		
		
		public void PurgeUndo()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.is_undo_redo_in_progress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			int i = this.live_index;
			
			while (i > 0)
			{
				i--;
				
				IOplet oplet = this.queue[i] as IOplet;
				
				if (oplet.IsFence)
				{
					this.live_fence--;
					this.fence_count--;
				}
				
				this.queue.RemoveAt (i);
				oplet.Dispose ();
			}
			
			this.live_index = 0;
			
			System.Diagnostics.Debug.Assert (this.live_fence == 0);
		}
		
		public void PurgeRedo()
		{
			if (this.IsDisabled)
			{
				return;
			}
			
			if (this.is_undo_redo_in_progress)
			{
				throw new System.InvalidOperationException ("Undo/redo in progress.");
			}
			if (this.action != null)
			{
				throw new System.InvalidOperationException ("Action definition in progress.");
			}
			
			int i = this.live_index;
			
			while (i < this.queue.Count)
			{
				IOplet oplet = this.queue[i] as IOplet;
				
				if (oplet.IsFence)
				{
					this.fence_count--;
				}
				
				this.queue.RemoveAt (i);
				oplet.Dispose ();
			}
			
			System.Diagnostics.Debug.Assert (this.live_fence == this.fence_count);
			System.Diagnostics.Debug.Assert (this.live_index == this.queue.Count);
		}
		
		
		public void Disable()
		{
			lock (this)
			{
				this.disable_count++;
			}
		}
		
		public void Enable()
		{
			lock (this)
			{
				if (this.disable_count == 0)
				{
					throw new System.InvalidOperationException ("Enable not possible, queue is not disabled.");
				}
				
				this.disable_count--;
			}
		}
		
		
		protected class AutoActionCleanup : System.IDisposable
		{
			public AutoActionCleanup(OpletQueue queue, string name, MergeMode mode)
			{
				queue.fence_id++;
				
				this.queue    = queue;
				this.fence_id = queue.fence_id;
				this.link     = queue.action;
				this.depth    = queue.temp_queue.Count;
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
					(this.queue.fence_id != this.fence_id))
				{
					throw new System.InvalidOperationException ("BeginAction/release mismatch.");
				}
				
				this.queue.fence_id--;
				this.queue.action    = this.link;
				this.queue.temp_name = this.name;
				
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
			protected int						fence_id;
			
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
		
		
		protected System.Collections.ArrayList	queue;
		protected System.Collections.ArrayList	temp_queue;
		protected string						temp_name;
		
		protected AutoActionCleanup				action;
		
		protected int							live_index;
		protected int							live_fence;
		protected int							fence_count;
		protected int							fence_id;
		
		protected bool							is_undo_redo_in_progress;
		
		private int								disable_count;
	}
}
