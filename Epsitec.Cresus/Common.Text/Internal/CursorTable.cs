//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.cacheFlags = new uint[1];
			
			this.version  = 1;
			
			this.freeCursorId    = 1;
			this.freeCursorCount = 1;
			
			this.cursors[0].DefineCursorState (Internal.CursorState.Invalid);
			this.cursors[1].DefineCursorState (Internal.CursorState.Free);
			
			Debug.Assert.IsTrue (this.CursorCount == 0);
		}
		
		
		public int								CursorCount
		{
			get
			{
				return this.cursors.Length - this.freeCursorCount - 1;
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
		
		public void SetCursorTextChunkId(Internal.CursorId id, int textChunkId)
		{
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.cursors.Length-1);
			Debug.Assert.IsTrue (this.cursors[id].CursorState == Internal.CursorState.Allocated);
			
			this.cursors[id].TextChunkId = textChunkId;
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
			if (this.freeCursorCount < 1)
			{
				this.GrowCursors ();
			}
			
			this.version++;
			
			CursorId free = this.freeCursorId;
			CursorId next = this.cursors[free].FreeListLink;
			
			Debug.Assert.IsTrue (free.IsValid);
			Debug.Assert.IsTrue (this.cursors[free].CursorState == Internal.CursorState.Free);
			
			this.freeCursorId = next;
			this.freeCursorCount--;
			
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
			
			this.cursors[id].FreeListLink   = this.freeCursorId;
			this.cursors[id].TextChunkId    = 0;
			this.cursors[id].CursorInstance = null;
			this.cursors[id].CachedPosition = -1;
			this.cursors[id].DefineCursorState (Internal.CursorState.Free);
			
			this.InvalidateCache (id);
			
			this.freeCursorId = id;
			this.freeCursorCount++;
		}
		
		
		public void InvalidatePositionCache()
		{
			//	Efface tous les bits de validité attachés à tous les curseurs.
			//	Le plus rapide est de mettre à zéro le contenu du tableau :
			
			System.Array.Clear (this.cacheFlags, 0, this.cacheFlags.Length);
		}
		
		public bool IsPositionCacheValid(Internal.CursorId id)
		{
			//	Vérifie que le cache de position du curseur spécifié est bien
			//	valide :
			
			int index  = id;
			int offset = index / 32;
			int bit    = index & 0x1F;
			
			uint mask = (1u << bit);
			
			return (mask & this.cacheFlags[offset]) != 0;
		}
		
		
		private void GrowCursors()
		{
			Debug.Assert.IsTrue (this.freeCursorId == 0);
			Debug.Assert.IsTrue (this.freeCursorCount == 0);
			
			int oldLength = this.cursors.Length;
			int newLength = oldLength + oldLength / 4 + 8;
			
			Internal.Cursor[] oldData = this.cursors;
			Internal.Cursor[] newData = new Internal.Cursor[newLength];
			
			System.Array.Copy (oldData, 0, newData, 0, oldLength);
			
			//	Il faut encore initialiser la liste des curseurs libres :
			
			for (int i = oldLength; i < newLength-1; i++)
			{
				newData[i].FreeListLink = i+1;
				newData[i].DefineCursorState (Internal.CursorState.Free);
			}
			
			newData[newLength-1].FreeListLink = 0;
			newData[newLength-1].DefineCursorState (Internal.CursorState.Free);
			
			this.freeCursorId    = oldLength;
			this.freeCursorCount = newLength - oldLength;
			
			this.cursors = newData;
			
			//	Agrandit, au besoin, la table des bits de validité du cache
			//	de position :
			
			int cacheWords = (this.cursors.Length+31) / 32;
			
			if (cacheWords != this.cacheFlags.Length)
			{
				uint[] oldFlags = this.cacheFlags;
				uint[] newFlags = new uint[cacheWords];
				
				System.Array.Copy (oldFlags, 0, newFlags, 0, oldFlags.Length);
				
				this.cacheFlags = newFlags;
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
			
			this.cacheFlags[offset] |= mask;
		}
		
		private void InvalidateCache(Internal.CursorId id)
		{
			//	Prend note que le curseur indiqué contient une position non valide
			//	dans son champ CachedPosition :
			
			int index  = id;
			int offset = index / 32;
			int bit    = index & 0x1F;
			
			uint mask = (1u << bit);
			
			this.cacheFlags[offset] &= ~ mask;
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
		private uint[]							cacheFlags;		//	validité du cache: 1 bit par curseur
		private Internal.CursorId				freeCursorId;
		private int								freeCursorCount;
		private int								version;
	}
}
