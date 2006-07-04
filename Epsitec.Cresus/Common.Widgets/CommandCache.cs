//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandCache</c> class maintains the relationship between visuals
	/// and their commands & command states.
	/// </summary>
	public sealed class CommandCache
	{
		private CommandCache()
		{
			this.records = new Record[0];
			this.free_count = 0;
			this.bunch_of_free_indexes = new int[20];
			this.bunch_of_free_indexes_count = 0;
		}
		
		
		internal void AttachVisual(Visual visual)
		{
			//	This method gets called by Visual when a command is associated
			//	with the visual (Visual.OnCommandChanged).
			
			//	The visual is associated with an entry in the command cache,
			//	through the CommandCacheId.
			
			System.Diagnostics.Debug.Assert (visual.GetCommandCacheId () == -1);
			
			int id = this.FindFreeIndex ();
			
			visual.SetCommandCacheId (id);
			
			this.records[id]  = new Record (visual);
			this.clear_count += 1;
			
			System.Diagnostics.Debug.Assert (this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (this.records[id].IsDirty);
			
			this.RequestAsyncSynchronization ();
		}
		
		internal void DetachVisual(Visual visual)
		{
			//	This method gets called by Visual when a command is removed from
			//	a visual (Visual.OnCommandChanged).
			
			System.Diagnostics.Debug.Assert (visual.GetCommandCacheId () != -1);
			
			int id = visual.GetCommandCacheId ();
			
			this.RecycleIndex (id);
			
			visual.SetCommandCacheId (-1);
			
			System.Diagnostics.Debug.Assert (! this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (! this.records[id].IsDirty);
		}
		
		
		internal void InvalidateVisual(Visual visual)
		{
			//	This method gets called by Visual.OnCommandChanged if the visual
			//	just traded one command for another, but also if one of the parents
			//	of the visual changed (indeed, changing a parent might change the
			//	active command context).
			
			int id = visual.GetCommandCacheId ();
			
			if (id == -1)
			{
				return;
			}
			
			if (this.records[id].ClearCommand ())
			{
				this.clear_count += 1;
			}
			
			this.RequestAsyncSynchronization ();
		}

		internal void InvalidateGroup(string group)
		{
			//	Called by CommandContext when a group enable changes.
			
			for (int i = 0; i < this.records.Length; i++)
			{
				Command command = this.records[i].Command;

				if ((command != null) &&
					(command.Group == group))
				{
					if (this.records[i].ClearCommand ())
					{
						this.clear_count += 1;
					}
				}
			}

			this.RequestAsyncSynchronization ();
		}

		public void InvalidateCommand(Command command)
		{
			//	Called by CommandContext when a command enable changes or if a
			//	command state is added or removed to/from the current context.

			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Command == command)
				{
					if (this.records[i].ClearCommand ())
					{
						this.clear_count += 1;
					}
				}
			}

			this.RequestAsyncSynchronization ();
		}

		internal void InvalidateState(CommandState state)
		{
			//	Called by CommandState when the command state settings change.
			
			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].State == state)
				{
					if (this.records[i].ClearCommand ())
					{
						this.clear_count += 1;
					}
				}
			}

			this.RequestAsyncSynchronization ();
		}

		internal void InvalidateContext(CommandContext context)
		{
			//	Called by CommandContext when a context is added or removed from
			//	a visual or a window.
			
			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Context == context)
				{
					if (this.records[i].ClearCommand ())
					{
						this.clear_count += 1;
					}
				}
			}

			this.RequestAsyncSynchronization ();
		}
		
		private void RequestAsyncSynchronization()
		{
			//	Queue an asynchronous command cache synchronisation. This will
			//	happen when the application returns into the event loop: Window
			//	will then call CommandCache.Instance.Synchronize.
			
			if (this.synchronize == false)
			{
				this.synchronize = true;
				Platform.Window.SendSynchronizeCommandCache ();
			}
		}
		
		
		public void Synchronize()
		{
			if (this.clear_count > 0)
			{
				int count = 0;
				
				for (int i = 0; i < this.records.Length; i++)
				{
					if (this.records[i].IsDirty)
					{
						//	We've just found a visual which has no associated CommandState
						//	in the cache :
						
						this.SynchronizeIndex (i);
						
						if (this.records[i].IsDirty)
						{
							count++;
						}
					}
				}
				
				this.clear_count = count;
			}
			
			this.synchronize = false;
		}

		
		public CommandState GetCommandState(Visual visual)
		{
			int id = visual.GetCommandCacheId ();
			
			if (id == -1)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (this.records[id].IsAlive);
			
			if (this.records[id].IsDirty)
			{
				this.SynchronizeIndex (id);
			}
			
			return this.records[id].State;
		}
		
		
		#region Record Struct
		private struct Record
		{
			public Record(Visual visual)
			{
				this.visual  = new System.WeakReference (visual, false);
				
				this.command = null;
				this.context = null;
				this.state   = null;
			}
			
			public Record(Visual visual, Command command, CommandContext context, CommandState state)
			{
				this.visual  = new System.WeakReference (visual, false);
				this.command = command;
				this.context = context;
				this.state   = state;
			}
			
			
			public bool							IsAlive
			{
				get
				{
					return this.visual == null ? false : this.visual.IsAlive;
				}
			}
			
			public bool							IsDead
			{
				get
				{
					//	IsDead n'est pas le contraire de IsAlive ! Si un visual
					//	a déjà été supprimé proprement de l'enregistrement, alors
					//	IsDead sera false.
					
					return this.visual == null ? false : ! this.visual.IsAlive;
				}
			}
			
			public bool							IsDirty
			{
				get
				{
					return (this.command == null) && (this.IsAlive);
				}
			}
			
			
			public Visual						Visual
			{
				get
				{
					return this.visual == null ? null : this.visual.Target as Visual;
				}
			}

			public Command Command
			{
				get
				{
					return this.command;
				}
			}
			
			public CommandContext				Context
			{
				get
				{
					return this.context;
				}
			}
			
			public CommandState					State
			{
				get
				{
					return this.state;
				}
			}
			
			public void Clear()
			{
				this.visual  = null;
				this.command = null;
				this.context = null;
				this.state   = null;
			}
			
			public bool ClearCommand()
			{
				if (this.command != null)
				{
					this.command = null;
					this.state = null;
					return true;
				}
				else
				{
					return false;
				}
			}
			
			public void SetCommand(Command command, CommandContext context, CommandState state)
			{
				System.Diagnostics.Debug.Assert (command != null);
				System.Diagnostics.Debug.Assert (context != null);
				System.Diagnostics.Debug.Assert (state != null);
				
				this.command = command;
				this.context = context;
				this.state   = state;
			}
			
			
			private System.WeakReference		visual;
			private Command						command;
			private CommandContext				context;
			private CommandState				state;
		}
		#endregion
		
		private int FindFreeIndex()
		{
			//	Trouve l'index d'un enregistrement vide, utilisable pour stocker
			//	une information sur une paire visual/commande.
			
			if (this.free_count == 0)
			{
				this.GrowRecords ();
			}
			else if (this.bunch_of_free_indexes_count == 0)
			{
				this.RefreshBunchOfFreeIndexes ();
			}
			
			this.bunch_of_free_indexes_count -= 1;
			this.free_count -= 1;
			
			return this.bunch_of_free_indexes[this.bunch_of_free_indexes_count];
		}
		
		
		private void RecycleIndex(int index)
		{
			this.records[index].Clear ();
			this.free_count += 1;
			
			if (this.bunch_of_free_indexes_count < this.bunch_of_free_indexes.Length)
			{
				this.bunch_of_free_indexes[this.bunch_of_free_indexes_count] = index;
				this.bunch_of_free_indexes_count += 1;
			}
		}
		
		private void SynchronizeIndex(int index)
		{
			Visual visual = this.records[index].Visual;
			
			if (visual != null)
			{
				Command command = Command.Find (visual.CommandName);
				CommandContextChain chain = CommandContextChain.BuildChain (visual);
				
				if ((command != null) &&
					(chain != null))
				{
					CommandContext context;
					CommandState state = chain.GetCommandState (command, out context);

					if ((state != null) &&
						(context != null))
					{
						if ((this.clear_count > 0) &&
							(this.records[index].IsDirty))
						{
							this.clear_count -= 1;
						}

						this.records[index].SetCommand (command, context, state);

						bool enable = state.Enable;
						ActiveState active = state.ActiveState;
						string advanced = state.AdvancedState;

						this.UpdateVisual (visual, command, enable, active, advanced);
					}
				}
			}
		}
		
		private void UpdateVisual(Visual visual, Command command, bool enable, ActiveState active, string advanced)
		{
			if (visual != null)
			{
				visual.Enable      = enable;
				visual.ActiveState = active;
				
				CommandState.SetAdvancedState (visual, advanced);
			}
		}
		
		private void RefreshBunchOfFreeIndexes()
		{
			//	Remplit la mini-table des index libres. L'idée est de ne parcourir
			//	la table des enregistrements qu'une fois pour trouver plusieurs
			//	éléments libres; on évite ainsi du cache trashing.
			
			int free  = 0;
			int index = 0;
			int count = 0;
			
			for (int i = 0; i < this.bunch_of_free_indexes.Length; i++)
			{
				while ((index < this.records.Length)
					&& (this.records[index].IsAlive))
				{
					index++;
				}
				
				if (index == this.records.Length)
				{
					break;
				}
				
				this.bunch_of_free_indexes[i] = index;
				
				if (this.records[index].IsDead)
				{
					this.records[index].Clear ();
				}
				
				index += 1;
				count += 1;
				free  += 1;
			}
			
			while (index < this.records.Length)
			{
				if (this.records[index].IsAlive)
				{
					//	Rien à faire : l'enregistrement est encore utilisé.
				}
				else
				{
					if (this.records[index].IsDead)
					{
						this.records[index].Clear ();
					}
					
					free += 1;
				}
				
				index++;
			}
			
			this.free_count                  = free;
			this.bunch_of_free_indexes_count = count;
			
			System.Diagnostics.Debug.Assert (this.bunch_of_free_indexes_count == System.Math.Min (this.bunch_of_free_indexes.Length, this.free_count));
		}
		
		private void GrowRecords()
		{
			int old_size = this.records.Length;
			int new_size = old_size + this.bunch_of_free_indexes.Length + old_size / 8;
			
			Record[] old_records = this.records;
			Record[] new_records = new Record[new_size];
			
			System.Array.Copy (old_records, 0, new_records, 0, old_size);
			
			this.records     = new_records;
			this.free_count += new_size - old_size;
			
			this.RefreshBunchOfFreeIndexes ();
		}
		
		
		public static readonly CommandCache		Instance = new CommandCache ();
		
		private Record[]						records;
		private int								free_count;
		private int								clear_count;
		private int[]							bunch_of_free_indexes;
		private int								bunch_of_free_indexes_count;
		private bool							synchronize;
	}
}
