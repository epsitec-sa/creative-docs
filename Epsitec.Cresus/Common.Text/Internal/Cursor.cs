//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.chunkId   = cursor.chunkId;
			this.cachedPos = cursor.cachedPos;
			this.instance   = cursor.instance;
			
			this.freeLink  = cursor.freeLink;
			
			//	Indique explicitement que ceci est une copie :
			
			this.cursorState = Internal.CursorState.Copied;
		}
		
		
		public static Internal.Cursor		Empty = new Internal.Cursor ();
		
		public int							TextChunkId
		{
			get
			{
				return this.chunkId;
			}
			set
			{
				this.chunkId = value;
			}
		}
		
		public ICursor						CursorInstance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}
		
		public int							CachedPosition
		{
			get
			{
				return this.cachedPos-1;
			}
			set
			{
				this.cachedPos = value+1;
			}
		}
		
		public Internal.CursorId			FreeListLink
		{
			get
			{
				return this.freeLink;
			}
			set
			{
				this.freeLink = value;
			}
		}
		
		public Internal.CursorState			CursorState
		{
			get
			{
				return this.cursorState;
			}
		}
		
		public CursorAttachment				Attachment
		{
			get
			{
				if (this.instance == null)
				{
					return CursorAttachment.Floating;
				}
				else
				{
					return this.instance.Attachment;
				}
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
			return this.freeLink
				 ^ this.chunkId
				 ^ (this.instance == null ? 0 : this.instance.GetHashCode ());
		}

		
		internal void DefineCursorState(Internal.CursorState state)
		{
			this.cursorState = state;
		}
		
		
		public static bool operator ==(Cursor a, Cursor b)
		{
			return (a.freeLink == b.freeLink)
				&& (a.chunkId == b.chunkId)
				&& (a.instance == b.instance);
		}
		
		public static bool operator !=(Cursor a, Cursor b)
		{
			return (a.freeLink != b.freeLink)
				|| (a.chunkId != b.chunkId)
				|| (a.instance != b.instance);
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
		
		private int							chunkId;
		private ICursor						instance;
		private int							cachedPos;
		
		private Internal.CursorId			freeLink;
		private Internal.CursorState		cursorState;
	}
}
