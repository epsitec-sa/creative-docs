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
			this.freeCount = 0;
			this.bunchOfFreeIndexes = new int[20];
			this.bunchOfFreeIndexesCount = 0;
		}

		public void Synchronize()
		{
			if (this.clearCount > 0)
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

				this.clearCount = count;
			}

			this.synchronizationRequired = false;
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
			this.clearCount += 1;
			
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
			
			if (this.records[id].ClearCommandState ())
			{
				this.clearCount += 1;
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
					if (this.records[i].ClearCommandState ())
					{
						this.clearCount += 1;
					}
				}
			}

			this.RequestAsyncSynchronization ();
		}

		internal void InvalidateCommand(Command command)
		{
			//	Called by CommandContext when a command enable changes or if a
			//	command state is added or removed to/from the current context.

			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Command == command)
				{
					if (this.records[i].ClearCommandState ())
					{
						this.clearCount += 1;
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
				if (this.records[i].CommandState == state)
				{
					if (this.records[i].ClearCommandState ())
					{
						this.clearCount += 1;
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
				if (this.records[i].CommandContext == context)
				{
					if (this.records[i].ClearCommandState ())
					{
						this.clearCount += 1;
					}
				}
			}

			this.RequestAsyncSynchronization ();
		}
		
		internal CommandState GetCommandState(Visual visual)
		{
			//	Called by Widget to implement property Widget.CommandState.

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
			
			return this.records[id].CommandState;
		}

		
		private void RequestAsyncSynchronization()
		{
			//	Queue an asynchronous command cache synchronisation. This will
			//	happen when the application returns into the event loop: Window
			//	will then call CommandCache.Instance.Synchronize.

			if (!this.synchronizationRequired)
			{
				this.synchronizationRequired = true;
				Platform.Window.SendSynchronizeCommandCache ();
			}
		}
		
		
		#region Record Struct
		
		/// <summary>
		/// The <c>Record</c> structure is used to store a relationship between a
		/// <c>Visual</c> and a <c>CommandState</c>.
		/// </summary>
		private struct Record
		{
			public Record(Visual visual)
			{
				this.visual = new Types.Weak<Visual> (visual);
				this.state  = null;
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
					return (this.state == null) && (this.IsAlive);
				}
			}
			
			
			public Visual						Visual
			{
				get
				{
					return this.visual == null ? null : this.visual.Target;
				}
			}

			public Command						Command
			{
				get
				{
					return this.state == null ? null : this.state.Command;
				}
			}
			
			public CommandContext				CommandContext
			{
				get
				{
					return this.state == null ? null : this.state.CommandContext;
				}
			}
			
			public CommandState					CommandState
			{
				get
				{
					return this.state;
				}
			}
			
			public void Clear()
			{
				this.visual  = null;
				this.state   = null;
			}
			
			public bool ClearCommandState()
			{
				if (this.state != null)
				{
					this.state = null;
					return true;
				}
				else
				{
					return false;
				}
			}
			
			public void SetCommandState(CommandState state)
			{
				System.Diagnostics.Debug.Assert (state != null);
				
				this.state = state;
			}
			
			
			private Types.Weak<Visual>			visual;
			private CommandState				state;
		}
		
		#endregion
		
		private int FindFreeIndex()
		{
			//	Find a free record, which will be used to store a visual/command state
			//	pair.
			
			if (this.freeCount == 0)
			{
				this.GrowRecords ();
			}
			else if (this.bunchOfFreeIndexesCount == 0)
			{
				this.RefreshBunchOfFreeIndexes ();
			}
			
			this.bunchOfFreeIndexesCount -= 1;
			this.freeCount -= 1;
			
			return this.bunchOfFreeIndexes[this.bunchOfFreeIndexesCount];
		}
		
		private void RecycleIndex(int index)
		{
			this.records[index].Clear ();
			this.freeCount += 1;
			
			if (this.bunchOfFreeIndexesCount < this.bunchOfFreeIndexes.Length)
			{
				this.bunchOfFreeIndexes[this.bunchOfFreeIndexesCount] = index;
				this.bunchOfFreeIndexesCount += 1;
			}
		}
		
		private void SynchronizeIndex(int index)
		{
			//	Synchronize the information stored in the specified record. This
			//	will re-associate the Visual with its CommandState, if possible.
			
			Visual visual = this.records[index].Visual;
			
			if (visual != null)
			{
				Command command = Command.Find (visual.CommandName);
				CommandContextChain chain = CommandContextChain.BuildChain (visual);
				
				if ((command != null) &&
					(chain != null))
				{
					//	Find the CommandState in the CommandContext nearest to the
					//	Visual. This requires a command context chain walk :
					
					CommandContext context;
					CommandState state = chain.GetCommandState (command, out context);

					if ((state != null) &&
						(context != null))
					{
						if ((this.clearCount > 0) &&
							(this.records[index].IsDirty))
						{
							this.clearCount--;
						}

						System.Diagnostics.Debug.Assert (state.Command == command);
						System.Diagnostics.Debug.Assert (state.CommandContext == context);

						//	Remember the command state for the visual.
						
						this.records[index].SetCommandState (state);

						bool enable = state.Enable && chain.GetLocalEnable (command);
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
			//	Fill the miniature free index table. The idea here is to avoid
			//	to have to walk through all the records when looking for a free
			//	record. Instead, we walk the records until our miniature free
			//	index table is full, and then use that one.
			
			//	Hint: this avoids CPU cache trashing !
			
			int free  = 0;
			int index = 0;
			int count = 0;
			
			for (int i = 0; i < this.bunchOfFreeIndexes.Length; i++)
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
				
				this.bunchOfFreeIndexes[i] = index;
				
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
			
			this.freeCount                  = free;
			this.bunchOfFreeIndexesCount = count;
			
			System.Diagnostics.Debug.Assert (this.bunchOfFreeIndexesCount == System.Math.Min (this.bunchOfFreeIndexes.Length, this.freeCount));
		}
		
		private void GrowRecords()
		{
			int old_size = this.records.Length;
			int new_size = old_size + this.bunchOfFreeIndexes.Length + old_size / 8;
			
			Record[] old_records = this.records;
			Record[] new_records = new Record[new_size];
			
			System.Array.Copy (old_records, 0, new_records, 0, old_size);
			
			this.records     = new_records;
			this.freeCount += new_size - old_size;
			
			this.RefreshBunchOfFreeIndexes ();
		}
		
		
		public static readonly CommandCache		Instance = new CommandCache ();
		
		private Record[]						records;
		private int								freeCount;
		private int								clearCount;
		private int[]							bunchOfFreeIndexes;
		private int								bunchOfFreeIndexesCount;
		private bool							synchronizationRequired;
	}
}
