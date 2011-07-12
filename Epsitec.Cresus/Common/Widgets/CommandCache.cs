//	Copyright © 2005-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandCache</c> class maintains the relationship between visuals
	/// and their commands and command states. This is a singleton.
	/// </summary>
	internal sealed class CommandCache
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandCache"/> class.
		/// This is a singleton instance, available throught <see cref="CommandCache.Instance"/>.
		/// </summary>
		private CommandCache()
		{
			this.records = new Record[0];
			this.freeCount = 0;
			this.bunchOfFreeIndexes = new int[20];
			this.bunchOfFreeIndexesCount = 0;
		}

		/// <summary>
		/// Synchronizes every visual in the cache with its command state. If we
		/// know for sure that there is no dirty visual, nothing will be done.
		/// </summary>
		public void Synchronize()
		{
			if (this.dirtyCount > 0)
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

				this.dirtyCount = count;
			}

			this.synchronizationRequired = false;
		}

		/// <summary>
		/// Associates the visual with the command cache.
		/// </summary>
		/// <param name="visual">The visual.</param>
		public void AttachVisual(Visual visual)
		{
			//	This method gets called by Visual when a command is associated
			//	with the visual (Visual.OnCommandChanged).
			
			//	The visual is associated with an entry in the command cache,
			//	through the CommandCacheId.
			
			System.Diagnostics.Debug.Assert (visual.GetCommandCacheId () == -1);
			
			int id = this.FindFreeIndex ();
			
			visual.SetCommandCacheId (id);
			
			this.records[id] = new Record (visual);
			this.dirtyCount += 1;
			
			System.Diagnostics.Debug.Assert (this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (this.records[id].IsDirty);
			
			this.RequestAsyncSynchronization ();
		}

		/// <summary>
		/// Clears the association of the visual with the command cache.
		/// </summary>
		/// <param name="visual">The visual.</param>
		public void DetachVisual(Visual visual)
		{
			//	This method gets called by Visual when a command is removed from
			//	a visual (Visual.OnCommandChanged).
			
			int id = visual.GetCommandCacheId ();

			System.Diagnostics.Debug.Assert (id != -1);
			
			this.RecycleIndex (id);
			
			visual.SetCommandCacheId (-1);
			
			System.Diagnostics.Debug.Assert (! this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (! this.records[id].IsDirty);
		}

		/// <summary>
		/// Invalidates the command association for the specified visual.
		/// </summary>
		/// <param name="visual">The visual.</param>
		public void InvalidateVisual(Visual visual)
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

			this.records[id].Command = visual.CommandObject;
			
			if (this.records[id].ClearCommandState ())
			{
				this.dirtyCount += 1;
			}
			
			this.RequestAsyncSynchronization ();
		}

		/// <summary>
		/// Invalidates the cached information about a command group. Every
		/// command state matching the specified group will be marked as dirty.
		/// </summary>
		/// <param name="group">The command group.</param>
		public void InvalidateGroup(string group)
		{
			//	Called by CommandContext when a group enable changes.

			int count = 0;
			
			for (int i = 0; i < this.records.Length; i++)
			{
				Command command = this.records[i].Command;

				if ((command != null) &&
					(command.Group == group))
				{
					if (this.records[i].ClearCommandState ())
					{
						count++;
						this.dirtyCount += 1;
					}
				}
			}

			if (count > 0)
			{
				this.RequestAsyncSynchronization ();
			}
		}

		/// <summary>
		/// Invalidates the cached information about a command. Every command
		/// state matching the specified command will be marked as dirty.
		/// </summary>
		/// <param name="command">The command.</param>
		public void InvalidateCommand(Command command)
		{
			//	Called by CommandContext when a command enable changes or if a
			//	command state is added or removed to/from the current context.

			int count = 0;

			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Command == command)
				{
					if (this.records[i].ClearCommandState ())
					{
						count++;
						this.dirtyCount += 1;
					}
				}
			}

			if (count > 0)
			{
				this.RequestAsyncSynchronization ();
			}
		}

		/// <summary>
		/// Invalidates the command caption and forces the visual to refresh.
		/// </summary>
		/// <param name="command">The command.</param>
		public void InvalidateCommandCaption(Command command)
		{
			//	Called by Command after the Caption object associated with the
			//	command changed. This is used to notify the visual, so that it
			//	has the opportunity to refresh its representation.
			
			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Command == command)
				{
					Visual visual = this.records[i].Visual;
					
					if (visual != null)
					{
						visual.NotifyCommandCaptionChanged ();
					}
				}
			}
		}

		/// <summary>
		/// Invalidates the cached information about a command. Every command
		/// state matching the specified state will be marked as dirty.
		/// </summary>
		/// <param name="state">The state.</param>
		public void InvalidateState(CommandState state)
		{
			//	Called by CommandState when the command state settings change.

			int count = 0;
			
			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Command == state.Command)
				{
					if (this.records[i].ClearCommandState ())
					{
						count++;
						this.dirtyCount += 1;
					}
				}
			}

			if (count > 0)
			{
				this.RequestAsyncSynchronization ();
			}
		}

		/// <summary>
		/// Invalidates the cached information about a command context. Every
		/// command state matching the specified command context will be marked
		/// as dirty.
		/// </summary>
		/// <param name="context">The command context.</param>
		public void InvalidateContext(CommandContext context)
		{
			//	Called by CommandContext when a context is added or removed from
			//	a visual or a window.

			int count = 0;
			var ids = new HashSet<int> (context.GetLocalEnableSerialIds ());

			for (int i = 0; i < this.records.Length; i++)
			{
				if ((this.records[i].CommandContext == context) ||
					(ids.Contains (this.records[i].SerialId)))
				{
					if (this.records[i].ClearCommandState ())
					{
						count++;
						this.dirtyCount += 1;
					}
				}
			}

			if (this.dirtyCount > 0)
			{
				this.RequestAsyncSynchronization ();
			}
		}

		/// <summary>
		/// Gets the cached command state for the specified visual.
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns>The command state or <c>null</c> if the visual has no associated
		/// information in the cache.</returns>
		public CommandState GetCommandState(Visual visual)
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


		#region Record Struct
		
		/// <summary>
		/// The <c>Record</c> structure is used to store a relationship between a
		/// <c>Visual</c> and a <c>CommandState</c>. Internally, a weak reference
		/// is used to avoid keeping the visual alive uselessly.
		/// </summary>
		private struct Record
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Record"/> struct.
			/// </summary>
			/// <param name="visual">The visual.</param>
			public Record(Visual visual)
			{
				this.visual   = new Types.Weak<Visual> (visual);
				this.state    = null;
				this.command  = visual.CommandObject;
				this.serialId = this.command == null ? -1 : this.command.SerialId;
			}


			/// <summary>
			/// Gets a value indicating whether this instance describes a live
			/// visual. A cleaned up record will always return false.
			/// </summary>
			/// <value><c>true</c> if the visual is alive; otherwise, <c>false</c>.</value>
			public bool							IsAlive
			{
				get
				{
					return this.visual == null ? false : this.visual.IsAlive;
				}
			}

			/// <summary>
			/// Gets a value indicating whether this instance describes a dead
			/// visual. A cleaned up record will always return false.
			/// </summary>
			/// <value><c>true</c> if the visual is dead; otherwise, <c>false</c>.</value>
			public bool							IsDead
			{
				get
				{
					return this.visual == null ? false : ! this.visual.IsAlive;
				}
			}

			/// <summary>
			/// Gets a value indicating whether this instance is dirty. A record
			/// is considered drity if the state does not exist and the visual is
			/// alive.
			/// </summary>
			/// <value><c>true</c> if the state is dirty; otherwise, <c>false</c>.</value>
			public bool							IsDirty
			{
				get
				{
					return (this.state == null) && (this.IsAlive);
				}
			}


			/// <summary>
			/// Gets the visual for this record.
			/// </summary>
			/// <value>The visual.</value>
			public Visual						Visual
			{
				get
				{
					return this.visual == null ? null : this.visual.Target;
				}
			}

			/// <summary>
			/// Gets or sets the command for this record.
			/// </summary>
			/// <value>The command.</value>
			public Command						Command
			{
				get
				{
					return this.command;
				}
				set
				{
					this.command  = value;
					this.serialId = value == null ? -1 : value.SerialId;
				}
			}

			/// <summary>
			/// Gets the command context for this record.
			/// </summary>
			/// <value>The command context.</value>
			public CommandContext				CommandContext
			{
				get
				{
					return this.state == null ? null : this.state.CommandContext;
				}
			}

			/// <summary>
			/// Gets or sets the command state for this record.
			/// </summary>
			/// <value>The command state.</value>
			public CommandState					CommandState
			{
				get
				{
					return this.state;
				}
				set
				{
					System.Diagnostics.Debug.Assert (value != null);
					this.state = value;
				}
			}

			/// <summary>
			/// Gets the serial id of the associated command.
			/// </summary>
			public int							SerialId
			{
				get
				{
					return this.serialId;
				}
			}


			/// <summary>
			/// Clears this instance.
			/// </summary>
			public void Clear()
			{
				this.visual   = null;
				this.state    = null;
				this.command  = null;
				this.serialId = -1;
			}

			/// <summary>
			/// Clears the command state associated with this record.
			/// </summary>
			/// <returns><c>true</c> if the command state was cleared; otherwise, <c>false</c>.</returns>
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
			
			private Types.Weak<Visual>			visual;
			private CommandState				state;
			private Command						command;
			private int							serialId;
		}
		
		#endregion


		/// <summary>
		/// Queue an asynchronous command cache synchronisation. This will
		///	happen when the application returns into the event loop: <see cref="Window"/>
		///	will then call <see cref="CommandCache.Instance.Synchronize"/>.
		/// </summary>
		private void RequestAsyncSynchronization()
		{
			if (!this.synchronizationRequired)
			{
				this.synchronizationRequired = true;
				Platform.Window.SendSynchronizeCommandCache ();
			}
		}
		
		/// <summary>
		/// Synchronizes the information for the record with the specified index.
		/// This will re-associate the <see cref="Visual"/> with its <see cref="CommandState"/>,
		/// if possible.
		/// </summary>
		/// <param name="index">The record index.</param>
		private void SynchronizeIndex(int index)
		{
			Visual  visual  = this.records[index].Visual;
			Command command = this.records[index].Command;
			
			if (visual != null)
			{
				CommandContextChain chain = CommandContextChain.BuildChain (visual);
				
				if ((command != null) &&
					(chain != null))
				{
					//	Find the CommandState in the CommandContext nearest to the
					//	Visual. This requires a command context chain walk :
					
					CommandContext context;
					CommandState   state = chain.GetCommandState (command, out context);

					if ((state != null) &&
						(context != null))
					{
						if ((this.dirtyCount > 0) &&
							(this.records[index].IsDirty))
						{
							this.dirtyCount--;
						}

						System.Diagnostics.Debug.Assert (state.Command == command);
						System.Diagnostics.Debug.Assert (state.CommandContext == context);

						//	Remember the command state for the visual.
						
						this.records[index].CommandState = state;

						bool        enable = state.Enable && chain.GetLocalEnable (command);
						ActiveState active = state.ActiveState;
						string    advanced = state.AdvancedState;

						//	Synchronize the visual with its command state :
						
						visual.Enable      = enable;
						visual.ActiveState = active;

						if (Command.GetHideWhenDisabled (visual))
						{
							visual.Visibility = enable;
						}

						CommandState.SetAdvancedState (visual, advanced);
					}
				}
			}
		}

		/// <summary>
		/// Finds the index of a free record. This method always succeeds.
		/// </summary>
		/// <returns>The index of a free record.</returns>
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

		/// <summary>
		/// Recycles the index, putting the corresponding record back into the
		/// free list.
		/// </summary>
		/// <param name="index">The index.</param>
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

		/// <summary>
		/// Refreshes the miniature table of free indexes.
		/// </summary>
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
					//	Skip this record, it is still in use !
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

		/// <summary>
		/// Grows the array used to store the records.
		/// </summary>
		private void GrowRecords()
		{
			int oldSize = this.records.Length;
			int newSize = oldSize + this.bunchOfFreeIndexes.Length + oldSize / 8;
			
			Record[] oldRecords = this.records;
			Record[] newRecords = new Record[newSize];
			
			System.Array.Copy (oldRecords, 0, newRecords, 0, oldSize);
			
			this.records    = newRecords;
			this.freeCount += newSize - oldSize;
			
			this.RefreshBunchOfFreeIndexes ();
		}
		
		
		public static readonly CommandCache		Instance = new CommandCache ();
		
		private Record[]						records;
		private int								freeCount;
		private int								dirtyCount;
		private int[]							bunchOfFreeIndexes;
		private int								bunchOfFreeIndexesCount;
		private bool							synchronizationRequired;
	}
}
