//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/04/2004

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
		
		
		public System.IDisposable BeginAction()
		{
			return new AutoActionCleanup (this);
		}
		
		public void Insert(IOplet oplet)
		{
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction must be called before any oplets can be inserted into the queue.");
			}
			
			this.temp_queue.Add (oplet);
		}
		
		public void ValidateAction()
		{
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/ValidateAction mismatch.");
			}
			
			this.action.Release ();
			this.temp_queue.Add (new Fence ());
			
			if (this.fence_id == 0)
			{
				//	Toutes les actions "ouvertes" ont été validées. On peut donc copier les oplets
				//	(avec leurs frontières) dans la liste officielle.
				
				System.Diagnostics.Debug.Assert (this.action == null);
				
				this.PurgeRedo ();
				this.queue.AddRange (this.temp_queue);
				this.temp_queue.Clear ();
				
				this.fence_count++;
				
				this.live_fence = this.fence_count;
				this.live_index = this.queue.Count;
			}
		}
		
		public void CancelAction()
		{
			if ((this.fence_id <= 0) ||
				(this.action == null))
			{
				throw new System.InvalidOperationException ("BeginAction/CancelAction mismatch.");
			}
			
			int i = this.action.Depth;
			
			//	Il faut retirer tous les oplets faisant partie de l'action, de la liste
			//	temporaire, et les supprimer proprement :
			
			while (i < this.temp_queue.Count)
			{
				IOplet oplet = this.temp_queue[i] as IOplet;
				
				oplet.Dispose ();
				
				this.temp_queue.RemoveAt (i);
			}
			
			this.action.Release ();

			if ((this.action == null) ||
				(this.fence_id <= 0) ||
				(this.temp_queue.Count == 0))
			{
				System.Diagnostics.Debug.Assert (this.action == null);
				System.Diagnostics.Debug.Assert (this.fence_id == 0);
				System.Diagnostics.Debug.Assert (this.temp_queue.Count == 0);
			}
		}
		
		
		public bool UndoAction()
		{
			if (this.live_fence > 0)
			{
				System.Diagnostics.Debug.Assert (this.live_index > 0);
				System.Diagnostics.Debug.Assert (this.queue.Count > 0);
				
				int i = this.live_index - 1;
				
				IOplet oplet = this.queue[i] as IOplet;
				
				System.Diagnostics.Debug.Assert (oplet.IsFence);
				
				for (;;)
				{
					oplet = this.queue[--i] as IOplet;
					
					if (oplet.IsFence)
					{
						this.live_index = i + 1;
						this.live_fence--;
						break;
					}
					
					this.queue[i] = oplet.Undo ();
				}
				
				return true;
			}
			
			return false;
		}
		
		public bool RedoAction()
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
		
		public void PurgeUndo()
		{
			int i = this.live_index;
			
			while (i-- > 0)
			{
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
		
		
		protected class AutoActionCleanup : System.IDisposable
		{
			public AutoActionCleanup(OpletQueue queue)
			{
				queue.fence_id++;
				
				this.queue    = queue;
				this.fence_id = queue.fence_id;
				this.link     = queue.action;
				this.depth    = queue.temp_queue.Count;
				
				queue.action = this;
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
			
			
			public void Release()
			{
				if ((this.queue.action != this) ||
					(this.queue.fence_id != this.fence_id))
				{
					throw new System.InvalidOperationException ("BeginAction/release mismatch.");
				}
				
				this.queue.fence_id--;
				this.queue.action = this.link;
				
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
		}
		
		protected class Fence : IOplet
		{
			public Fence()
			{
			}
			
			
			public bool							IsFence
			{
				get
				{
					return true;
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

		}
		
		
		protected System.Collections.ArrayList	queue;
		protected System.Collections.ArrayList	temp_queue;
		
		protected AutoActionCleanup				action;
		
		protected int							live_index;
		protected int							live_fence;
		protected int							fence_count;
		protected int							fence_id;
	}
}
