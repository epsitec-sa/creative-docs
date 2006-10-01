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
			this.cursors     = new Internal.Cursor[2];
			this.cache_flags = new uint[1];
			
			this.version  = 1;
			
			this.free_cursor_id    = 1;
			this.free_cursor_count = 1;
			
			this.cursors[0].DefineCursorState (Internal.CursorState.Invalid);
			this.cursors[1].DefineCursorState (Internal.CursorState.Free);
			
			Debug.Assert.IsTrue (this.CursorCount == 0);
		}
		
		
		public int								CursorCount
		{
			get
			{
				return this.cursors.Length - this.free_cursor_count - 1;
			}
		}
		
		public ICursor[] GetCursorArray()
		{
			ICursor[] cursors = new ICursor[this.CursorCount];
			
			for (int i = 1, j = 0; i < this.cursors.Length; i++)
			{
				if (this.cursors[i].CursorState == CursorState.Allocated)
				{
					cursors[j++] = this.cursors[i].CursorInstance;
				}
			}
			
			System.Diagnostics.Debug.Assert (cursors.Length == 0 || cursors[cursors.Length-1] != null);
			
			return cursors;
		}
		
		
		public Internal.Cursor ReadCursor(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			if ((this.IsPositionCacheValid (id) == false) &&
				(this.cursors[id].CachedPosition != -1))
			{
				this.cursors[id].CachedPosition = -1;
			}
			
			return new Internal.Cursor (this.cursors[id]);
		}
		
		public void WriteCursor(Internal.CursorId id, Internal.Cursor cursor)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			Debug.Assert.IsTrue (this.cursors[id].FreeListLink == 0);
			Debug.Assert.IsTrue (cursor.FreeListLink == 0);
			
			//	Copie les champs individuellement; on n'utilise pas l'assignation car cela
			//	écraserait notre indicateur interne d'état du curseur :
			
			this.cursors[id].TextChunkId    = cursor.TextChunkId;
			this.cursors[id].CursorInstance = cursor.CursorInstance;
			this.cursors[id].CachedPosition = cursor.CachedPosition;
			
			if (this.cursors[id].CachedPosition == -1)
			{
				this.InvalidateCache (id);
			}
			else
			{
				this.ValidateCache (id);
			}
			
			if (this.cursors[id].CursorInstance != null)
			{
				this.cursors[id].CursorInstance.CursorId = id;
			}
		}
		
		
		public ICursor GetCursorInstance(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			return this.cursors[id].CursorInstance;
		}
		
		
		public int GetCursorTextChunkId(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			return this.cursors[id].TextChunkId;
		}
		
		public void SetCursorTextChunkId(Internal.CursorId id, int text_chunk_id)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].TextChunkId = text_chunk_id;
		}
		
		public void ModifyCursorTextChunkId(Internal.CursorId id, int delta)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].TextChunkId += delta;
		}
		
		
		public Internal.CursorId NewCursor()
		{
			if (this.free_cursor_count < 1)
			{
				this.GrowCursors ();
			}
			
			this.version++;
			
			CursorId free = this.free_cursor_id;
			CursorId next = this.cursors[free].FreeListLink;
			
			Debug.Assert.IsTrue (free.IsValid);
			Debug.Assert.IsTrue (this.cursors[free].CursorState == Internal.CursorState.Free);
			
			this.free_cursor_id = next;
			this.free_cursor_count--;
			
			this.cursors[free].FreeListLink = 0;
			this.cursors[free].DefineCursorState (Internal.CursorState.Allocated);
			
			Debug.Assert.IsTrue (this.ReadCursor (free) == Internal.Cursor.Empty);
			
			Debug.Assert.IsTrue (this.cursors[free].CachedPosition == -1);
			Debug.Assert.IsFalse (this.IsPositionCacheValid (free));
			
			return free;
		}
		
		public void RecycleCursor(Internal.CursorId id)
		{
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.version++;
			
			if (this.cursors[id].CursorInstance != null)
			{
				this.cursors[id].CursorInstance.CursorId = 0;
			}
			
			this.cursors[id].FreeListLink   = this.free_cursor_id;
			this.cursors[id].TextChunkId    = 0;
			this.cursors[id].CursorInstance = null;
			this.cursors[id].CachedPosition = -1;
			this.cursors[id].DefineCursorState (Internal.CursorState.Free);
			
			this.InvalidateCache (id);
			
			this.free_cursor_id = id;
			this.free_cursor_count++;
		}
		
		
		public void InvalidatePositionCache()
		{
			//	Efface tous les bits de validité attachés à tous les curseurs.
			//	Le plus rapide est de mettre à zéro le contenu du tableau :
			
			System.Array.Clear (this.cache_flags, 0, this.cache_flags.Length);
		}
		
		public bool IsPositionCacheValid(Internal.CursorId id)
		{
			//	Vérifie que le cache de position du curseur spécifié est bien
			//	valide :
			
			int index  = id;
			int offset = index / 32;
			int bit    = index & 0x1F;
			
			uint mask = (1u << bit);
			
			return (mask & this.cache_flags[offset]) != 0;
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
			
			new_data[new_length-1].FreeListLink = 0;
			new_data[new_length-1].DefineCursorState (Internal.CursorState.Free);
			
			this.free_cursor_id    = old_length;
			this.free_cursor_count = new_length - old_length;
			
			this.cursors = new_data;
			
			//	Agrandit, au besoin, la table des bits de validité du cache
			//	de position :
			
			int cache_words = (this.cursors.Length+31) / 32;
			
			if (cache_words != this.cache_flags.Length)
			{
				uint[] old_flags = this.cache_flags;
				uint[] new_flags = new uint[cache_words];
				
				System.Array.Copy (old_flags, 0, new_flags, 0, old_flags.Length);
				
				this.cache_flags = new_flags;
			}
		}
		
		private void ValidateCache(Internal.CursorId id)
		{
			//	Prend note que le curseur indiqué contient une position valide
			//	dans son champ CachedPosition :
			
			int index  = id;
			int offset = index / 32;
			int bit    = index & 0x1F;
			
			uint mask = (1u << bit);
			
			this.cache_flags[offset] |= mask;
		}
		
		private void InvalidateCache(Internal.CursorId id)
		{
			//	Prend note que le curseur indiqué contient une position non valide
			//	dans son champ CachedPosition :
			
			int index  = id;
			int offset = index / 32;
			int bit    = index & 0x1F;
			
			uint mask = (1u << bit);
			
			this.cache_flags[offset] &= ~ mask;
		}
		
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator (this);
		}
		#endregion
		
		#region Private Enumerator Class
		private class Enumerator : System.Collections.IEnumerator
		{
			public Enumerator(CursorTable table)
			{
				this.cursors = table.cursors;
				this.table   = table;
				this.version = table.version;
				this.index   = -1;
			}
			
			
			#region IEnumerator Members
			public void Reset()
			{
				if (this.version != this.table.version)
				{
					throw new System.InvalidOperationException ("CursorTable was modified.");
				}
				
				this.index = -1;
			}
			
			public object						Current
			{
				get
				{
					if ((this.version != this.table.version) ||
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
				if (this.version != this.table.version)
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
			private int							version;
			private int							index;
		}
		#endregion
		
		private Internal.Cursor[]				cursors;			//	1..n; prendre index tel quel (zéro = invalide)
		private uint[]							cache_flags;		//	validité du cache: 1 bit par curseur
		private Internal.CursorId				free_cursor_id;
		private int								free_cursor_count;
		private int								version;
	}
}
