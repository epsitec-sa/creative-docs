//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La structure Cursor décrit une marque qui suit le texte et
	/// qui peut être utilisée pour naviguer à travers des instances
	/// de la class TextChunk.
	/// </summary>
	internal struct Cursor
	{
		public Cursor(Internal.Cursor cursor)
		{
			this.free_list_link = cursor.free_list_link;
			this.chunk_id       = cursor.chunk_id;
			
			//	Indique explicitement que ceci est une copie :
			
			this.cursor_state   = Internal.CursorState.Copied;
		}
		
		
		public static Internal.Cursor		Empty = new Internal.Cursor ();
		
		public Internal.CursorId			FreeListLink
		{
			get
			{
				return this.free_list_link;
			}
			set
			{
				this.free_list_link = value;
			}
		}
		
		public Internal.TextChunkId			TextChunkId
		{
			get
			{
				return this.chunk_id;
			}
			set
			{
				this.chunk_id = value;
			}
		}
		
		public Internal.CursorState			CursorState
		{
			get
			{
				return this.cursor_state;
			}
		}
		
		
		public override bool Equals(object obj)
		{
			if (obj is Cursor)
			{
				Cursor that = (Cursor) obj;
				return this == that;
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			return this.free_list_link
				 ^ this.chunk_id;
		}

		
		internal void DefineCursorState(Internal.CursorState state)
		{
			this.cursor_state = state;
		}
		
		
		public static bool operator ==(Cursor a, Cursor b)
		{
			return (a.free_list_link == b.free_list_link)
				&& (a.chunk_id == b.chunk_id);
		}
		
		public static bool operator !=(Cursor a, Cursor b)
		{
			return (a.free_list_link != b.free_list_link)
				|| (a.chunk_id != b.chunk_id);
		}
		
		
		//
		//	ATTENTION:
		//
		//	CursorTable manipule ces champs manuellement; si des nouveaux champs
		//	sont rajoutés ici, il faut mettre à jour la méthode dans les méthodes
		//	suivantes :
		//
		//	- CursorTable.WriteCursor
		//	- CursorTable.RecycleCursor
		//
		
		private Internal.CursorId			free_list_link;
		
		private Internal.TextChunkId		chunk_id;
		
		private Internal.CursorState		cursor_state;
	}
}
