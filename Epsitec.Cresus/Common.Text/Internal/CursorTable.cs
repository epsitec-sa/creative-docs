//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe CursorTable stocke tous les curseurs liés à un texte.
	/// Ces curseurs sont accessibles indirectement au moyen d'un CursorId;
	/// il n'est pas possible de les modifier directement.
	/// </summary>
	internal class CursorTable : System.Collections.IEnumerable
	{
		public CursorTable()
		{
			this.cursors = new Internal.Cursor[2];
			this.gen_id  = 1;
			
			this.free_cursor_id    = 1;
			this.free_cursor_count = 1;
			
			this.cursors[0].DefineCursorState (Internal.CursorState.Fence);
			this.cursors[1].DefineCursorState (Internal.CursorState.Free);
		}
		
		
		public int								CursorCount
		{
			get
			{
				return this.cursors.Length - this.free_cursor_count - 1;
			}
		}
		
		
		public Internal.Cursor ReadCursor(Internal.CursorId id)
		{
			Debug.Assert.IsInBounds (id, 1, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			return new Internal.Cursor (this.cursors[id]);
		}
		
		public void WriteCursor(Internal.CursorId id, Internal.Cursor cursor)
		{
			Debug.Assert.IsInBounds (id, 1, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			Debug.Assert.IsTrue (this.cursors[id].FreeListLink == 0);
			Debug.Assert.IsTrue (cursor.FreeListLink == 0);
			
			//	Copie les champs individuellement; on n'utilise pas l'assignation car cela
			//	écraserait notre indicateur interne d'état du curseur :
			
			this.cursors[id].TextChunkId = cursor.TextChunkId;
		}
		
		
		public Internal.TextChunkId GetCursorTextChunkId(Internal.CursorId id)
		{
			Debug.Assert.IsInBounds (id, 1, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			return this.cursors[id].TextChunkId;
		}
		
		public void SetCursorTextChunkId(Internal.CursorId id, Internal.TextChunkId text_chunk_id)
		{
			Debug.Assert.IsInBounds (id, 1, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].TextChunkId = text_chunk_id;
		}
		
		public void ModifyCursorTextChunkId(Internal.CursorId id, int delta)
		{
			Debug.Assert.IsInBounds (id, 1, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].TextChunkId += delta;
		}
		
		
		public Internal.CursorId NewCursor()
		{
			if (this.free_cursor_count < 1)
			{
				this.GrowCursors ();
			}
			
			CursorId free = this.free_cursor_id;
			CursorId next = this.cursors[free].FreeListLink;
			
			Debug.Assert.IsTrue (this.cursors[free].CursorState == Internal.CursorState.Free);
			
			this.free_cursor_id = next;
			this.free_cursor_count--;
			
			this.cursors[free].FreeListLink = 0;
			this.cursors[free].DefineCursorState (Internal.CursorState.Allocated);
			
			Debug.Assert.IsTrue (this.ReadCursor (free) == Internal.Cursor.Empty);
			
			return free;
		}
		
		public void RecycleCursor(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].FreeListLink = this.free_cursor_id;
			this.cursors[id].TextChunkId  = 0;
			this.cursors[id].DefineCursorState (Internal.CursorState.Free);
			
			this.free_cursor_id = id;
			this.free_cursor_count++;
		}
		
		
		private void GrowCursors()
		{
			Debug.Assert.IsTrue (this.free_cursor_id == 0);
			Debug.Assert.IsTrue (this.free_cursor_count == 0);
			
			int old_length = this.cursors.Length;
			int new_length = old_length + old_length / 4 + 8;
			
			Internal.Cursor[] old_data = this.cursors;
			Internal.Cursor[] new_data = new Internal.Cursor[new_length];
			
			System.Array.Copy (old_data, 0, new_data, 0, old_length);
			
			//	Il faut encore initialiser la liste des curseurs libres :
			
			for (int i = old_length; i < new_length-1; i++)
			{
				new_data[i].FreeListLink = i+1;
				new_data[i].DefineCursorState (Internal.CursorState.Free);
			}
			
			this.free_cursor_id    = old_length;
			this.free_cursor_count = new_length - old_length;
			
			this.cursors = new_data;
		}
		
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator (this);
		}
		#endregion
		
		private class Enumerator : System.Collections.IEnumerator
		{
			public Enumerator(CursorTable table)
			{
				this.cursors      = table.cursors;
				this.table        = table;
				this.table_gen_id = table.gen_id;
				this.index        = -1;
			}
			
			
			#region IEnumerator Members
			public void Reset()
			{
				if (this.table_gen_id != this.table.gen_id)
				{
					throw new System.InvalidOperationException ("CursorTable was modified.");
				}
				
				this.index = -1;
			}
			
			public object						Current
			{
				get
				{
					if ((this.table_gen_id != this.table.gen_id) ||
						(this.index < 0) ||
						(this.index >= this.table.cursors.Length))
					{
						throw new System.InvalidOperationException ("CursorTable was modified.");
					}
					
					return new Internal.CursorId (this.index);
				}
			}
			
			public bool MoveNext()
			{
				if (this.table_gen_id != this.table.gen_id)
				{
					throw new System.InvalidOperationException ("CursorTable was modified.");
				}
				
				while (this.index < this.table.cursors.Length)
				{
					this.index++;
					
					if (this.index == this.table.cursors.Length)
					{
						break;
					}
					
					if (this.table.cursors[this.index].CursorState == Internal.CursorState.Allocated)
					{
						return true;
					}
				}
				
				return false;
			}
			#endregion
			
			
			private Internal.Cursor[]			cursors;
			private CursorTable					table;
			private int							table_gen_id;
			private int							index;
		}

		
		private Internal.Cursor[]				cursors;
		private Internal.CursorId				free_cursor_id;
		private int								free_cursor_count;
		private int								gen_id;
	}
}
