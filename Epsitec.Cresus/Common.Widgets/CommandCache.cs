//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandCache permet de réaliser le lien entre des visuals et
	/// leur CommandState associé.
	/// </summary>
	public sealed class CommandCache
	{
		public CommandCache()
		{
			this.records = new Record[0];
			this.free_count = 0;
			this.bunch_of_free_indexes = new int[20];
			this.bunch_of_free_indexes_count = 0;
		}
		
		
		public void AttachVisual(Visual visual)
		{
			System.Diagnostics.Debug.Assert (visual.GetCommandCacheId () == -1);
			
			int id = this.FindFreeIndex ();
			
			visual.SetCommandCacheId (id);
			
			this.records[id]  = new Record (visual);
			this.clear_count += 1;
			
			System.Diagnostics.Debug.Assert (this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (this.records[id].IsDirty);
			
//			System.Diagnostics.Debug.WriteLine (string.Format ("Command {0} attached ({1}/{2}; {3} free)", visual.CommandName, id, this.records.Length - this.free_count, this.free_count));
			
			this.RequestAsyncSynchronization ();
		}
		
		public void DetachVisual(Visual visual)
		{
			System.Diagnostics.Debug.Assert (visual.GetCommandCacheId () != -1);
			
			int id = visual.GetCommandCacheId ();
			
			this.RecycleIndex (id);
			
			visual.SetCommandCacheId (-1);
			
			System.Diagnostics.Debug.Assert (! this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (! this.records[id].IsDirty);
			
//			System.Diagnostics.Debug.WriteLine (string.Format ("Command detached ({0})", id));
		}
		
		
		public void InvalidateVisual(Visual visual)
		{
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
		
		public void InvalidateCommand(CommandState command)
		{
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

		public void InvalidateGroup(string group)
		{
			for (int i = 0; i < this.records.Length; i++)
			{
				CommandState command = this.records[i].Command;
				
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
		
		public void RequestAsyncSynchronization()
		{
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
						//	Nous avons trouvé un visual qui n'a pas encore de
						//	CommandState attaché dans le cache.
						
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
		
		public void UpdateWidgets(CommandState command)
		{
			if (this.synchronize)
			{
				this.Synchronize ();
			}
			
			bool        enable   = command.Enable;
			ActiveState active   = command.ActiveState;
			string      advanced = command.AdvancedState;
			
			int count = 0;
			
			for (int i = 0; i < this.records.Length; i++)
			{
				if (this.records[i].Command == command)
				{
					this.UpdateWidget (this.records[i].Visual as Widget, command, enable, active, advanced);
					
					count++;
				}
			}
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
			
			return this.records[id].Command;
		}
		
		
		#region Record Struct
		private struct Record
		{
			public Record(Visual visual)
			{
				this.visual = new System.WeakReference (visual, false);
				this.command = null;
			}
			
			public Record(Visual visual, CommandState command)
			{
				this.visual = new System.WeakReference (visual, false);
				this.command = null;
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
			
			public CommandState					Command
			{
				get
				{
					return this.command;
				}
			}
			
			
			public void Clear()
			{
				this.visual  = null;
				this.command = null;
			}
			
			public bool ClearCommand()
			{
				if (this.command != null)
				{
					this.command = null;
					return true;
				}
				else
				{
					return false;
				}
			}
			
			public void SetCommand(CommandState command)
			{
				this.command = command;
			}
			
			
			private System.WeakReference		visual;
			private CommandState				command;
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
				CommandState command = Helpers.VisualTree.GetCommandState (visual);
				
				if ((this.clear_count > 0) &&
					(this.records[index].Command == null))
				{
					this.clear_count -= 1;
				}
				
				if (command == null)
				{
//-					System.Diagnostics.Debug.WriteLine (string.Format ("Command '{0}' does not exist (yet).", visual.CommandName));
				}
				
				if (command != null)
				{
					this.records[index].SetCommand (command);
					
					bool        enable   = command.Enable;
					ActiveState active   = command.ActiveState;
					string      advanced = command.AdvancedState;
					
					this.UpdateWidget (this.records[index].Visual as Widget, command, enable, active, advanced);
				}
			}
		}
		
		private void UpdateWidget(Widget widget, CommandState command, bool enable, ActiveState active, string advanced)
		{
			if (widget != null)
			{
				if (enable)
				{
					CommandContext context = Helpers.VisualTree.GetCommandContext (widget);

					if ((context != null) &&
					(context.GetLocalEnable (command) == false))
					{
						enable = false;
					}
				}
				
				widget.Enable      = enable;
				widget.ActiveState = active;
				
				if (advanced != null)
				{
					CommandState.SetAdvancedState (widget, advanced);
				}
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
		
		
		public static readonly CommandCache		Default = new CommandCache ();
		
		private Record[]						records;
		private int								free_count;
		private int								clear_count;
		private int[]							bunch_of_free_indexes;
		private int								bunch_of_free_indexes_count;
		private bool							synchronize;
	}
}
