//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe CursorIdArray stocke les CursorIds associés à un
	/// TextChunk.
	/// </summary>
	internal class CursorIdArray
	{
		public CursorIdArray()
		{
			this.entries = new Entry[0];
			this.length  = 0;
		}
		
		
		public void AttachCursor(CursorId id, int offset)
		{
		}
		
		
		private void InsertEntry(int index, Entry entry)
		{
			Debug.Assert.IsTrue (this.length > 0);
			Debug.Assert.IsTrue (this.length <= this.entries.Length);
			
			Entry[] old_entries = this.entries;
			Entry[] new_entries = new Entry[this.length+1];
			
			Debug.Assert.IsTrue (index >= 0);
			Debug.Assert.IsTrue (index <= this.length);
			
			int n1 = index;
			int n2 = this.length - index;
			
			System.Array.Copy (old_entries, 0, new_entries, 0, n1);
			System.Array.Copy (old_entries, n1, new_entries, n1+1, n2);
			
			new_entries[index] = entry;
			
			this.entries = new_entries;
			this.length  = new_entries.Length;
		}
		
		private void RemoveEntry(int index)
		{
			Debug.Assert.IsTrue (this.length > 0);
			Debug.Assert.IsTrue (this.length <= this.entries.Length);
			
			Debug.Assert.IsTrue (index >= 0);
			Debug.Assert.IsTrue (index < this.length);
			
			int last = this.entries.Length-1;
			
			for (int i = index; i < last; i++)
			{
				this.entries[i] = this.entries[i+1];
			}
			
			this.length--;
		}
		
		private void MoveEntry(int from_index, int to_index)
		{
			Debug.Assert.IsTrue (this.length > 0);
			Debug.Assert.IsTrue (this.length <= this.entries.Length);
			
			Debug.Assert.IsTrue (from_index >= 0);
			Debug.Assert.IsTrue (from_index < this.length);
			Debug.Assert.IsTrue (to_index >= 0);
			Debug.Assert.IsTrue (to_index <= this.length);
			
			if (from_index < to_index)
			{
				//	Si on décale simplement après soi-même, il n'y a rien à
				//	faire :
				
				if (from_index+1 == to_index)
				{
					return;
				}
				
				Entry entry = this.entries[from_index];
				
				for (int i = from_index; i < to_index-2; i++)
				{
					this.entries[i] = this.entries[i+1];
				}
				
				//	Comme on a supprimé l'élément qui se trouvait avant la
				//	desination, il faut ajuster celle-ci :
				
				to_index--;
				
				//	Termine le déplacement.
				
				this.entries[to_index] = entry;
			}
			else
			{
				if (from_index == to_index)
				{
					return;
				}
				
				Entry entry = this.entries[from_index];
				
				for (int i = from_index; i > to_index; i--)
				{
					this.entries[i] = this.entries[i-1];
				}
				
				//	Termine le déplacement.
				
				this.entries[to_index] = entry;
			}
		}
		
		
		
		private struct Entry
		{
			public Entry(Internal.CursorId id, int offset_to_next)
			{
				this.id             = id;
				this.offset_to_next = offset_to_next;
			}
			
			
			public	Internal.CursorId			id;
			public	int							offset_to_next;
		}
		
		
		private Entry[]							entries;
		private int								length;
	}
}
