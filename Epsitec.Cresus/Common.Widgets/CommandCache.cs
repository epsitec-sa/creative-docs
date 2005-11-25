//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandCache permet de réaliser le lien entre des widgets et
	/// leur CommandDispatcher & CommandState associé.
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
		
		
		public void AttachWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (widget.GetCommandCacheId () == -1);
			
			int id = this.FindFreeIndex ();
			
			widget.SetCommandCacheId (id);
			
			this.records[id] = new Record (widget);
			
			System.Diagnostics.Debug.Assert (this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (this.records[id].IsDirty);
		}
		
		public void DetachWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (widget.GetCommandCacheId () != -1);
			
			int id = widget.GetCommandCacheId ();
			
			this.RecycleIndex (id);
			widget.SetCommandCacheId (-1);
			
			System.Diagnostics.Debug.Assert (! this.records[id].IsAlive);
			System.Diagnostics.Debug.Assert (! this.records[id].IsDirty);
		}
		
		
		public void Invalidate(Widget widget)
		{
			int id = widget.GetCommandCacheId ();
			
			if (id == -1)
			{
				return;
			}
			
			if (this.records[id].ClearCommand ())
			{
				this.clear_count += 1;
			}
		}
		
		public void Invalidate(CommandState command)
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
		}
		
		
		public void Synchronize()
		{
		}
		
		#region Record Struct
		private struct Record
		{
			public Record(Widget widget)
			{
				this.widget = new System.WeakReference (widget, false);
				this.command = null;
			}
			
			public Record(Widget widget, CommandState command)
			{
				this.widget = new System.WeakReference (widget, false);
				this.command = null;
			}
			
			
			public bool							IsAlive
			{
				get
				{
					return this.widget.IsAlive;
				}
			}
			
			public bool							IsDead
			{
				get
				{
					return ! this.widget.IsAlive;
				}
			}
			
			public bool							IsDirty
			{
				get
				{
					return (this.command == null) && (this.IsAlive);
				}
			}
			
			
			public Widget						Widget
			{
				get
				{
					return this.widget.Target as Widget;
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
				this.widget.Target = null;
				this.command       = null;
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
			
			
			private System.WeakReference		widget;
			private CommandState				command;
		}
		#endregion
		
		private int FindFreeIndex()
		{
			//	Trouve l'index d'un enregistrement vide, utilisable pour stocker
			//	une information sur une paire widget/commande.
			
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
		
		private void RefreshBunchOfFreeIndexes()
		{
			//	Remplit la mini-table des index libres. L'idée est de ne parcourir
			//	la table des enregistrements qu'une fois pour trouver plusieurs
			//	éléments libres; on évite ainsi du cache trashing.
			
			int free  = this.free_count;
			int index = 0;
			int count = 0;
			
			for (int i = 0; i < this.bunch_of_free_indexes.Length; i++)
			{
				if (free > 0)
				{
					while (this.records[index].IsAlive)
					{
						index++;
					}
					
					this.bunch_of_free_indexes[i] = index;
					
					free  -= 1;
					index += 1;
					count += 1;
				}
				else
				{
					break;
				}
			}
			
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
			
			this.free_count += new_size - old_size;
			this.RefreshBunchOfFreeIndexes ();
		}
		
		
		private Record[]						records;
		private int								free_count;
		private int								clear_count;
		private int[]							bunch_of_free_indexes;
		private int								bunch_of_free_indexes_count;
	}
}
