//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe CursorTable stocke tous les curseurs liés à un texte.
	/// Ces curseurs sont accessibles indirectement au moyen d'un CursorId;
	/// il n'est pas possible de les modifier directement.
	/// </summary>
	internal class CursorTable
	{
		public CursorTable()
		{
			this.cursors = new Internal.Cursor[2];
			
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
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			return new Internal.Cursor (this.cursors[id]);
		}
		
		public void WriteCursor(Internal.CursorId id, Internal.Cursor cursor)
		{
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			//	Copie les champs individuellement; on n'utilise pas l'assignation car cela
			//	écraserait notre indicateur interne d'état du curseur :
			
			this.cursors[id].NextCursorId    = cursor.NextCursorId;
			this.cursors[id].PrevCursorId    = cursor.PrevCursorId;
			this.cursors[id].TextChunkId     = cursor.TextChunkId;
			this.cursors[id].TextChunkOffset = cursor.TextChunkOffset;
		}
		
		
		public Internal.CursorId NewCursor()
		{
			if (this.free_cursor_count < 1)
			{
				this.GrowCursors ();
			}
			
			CursorId free = this.free_cursor_id;
			CursorId next = this.cursors[free].NextCursorId;
			
			Debug.Assert.IsTrue (this.cursors[free].CursorState == Internal.CursorState.Free);
			
			this.free_cursor_id = next;
			this.free_cursor_count--;
			
			this.cursors[free].NextCursorId = 0;
			this.cursors[free].DefineCursorState (Internal.CursorState.Allocated);
			
			Debug.Assert.IsTrue (this.ReadCursor (free) == Internal.Cursor.Empty);
			
			return free;
		}
		
		public void RecycleCursor(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].NextCursorId    = this.free_cursor_id;
			this.cursors[id].PrevCursorId    = 0;
			this.cursors[id].TextChunkId     = 0;
			this.cursors[id].TextChunkOffset = 0;
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
				new_data[i].NextCursorId = i+1;
				new_data[i].DefineCursorState (Internal.CursorState.Free);
			}
			
			this.free_cursor_id    = old_length;
			this.free_cursor_count = new_length - old_length;
			
			this.cursors = new_data;
		}
		
		
		private Internal.Cursor[]				cursors;
		private Internal.CursorId				free_cursor_id;
		private int								free_cursor_count;
	}
}
