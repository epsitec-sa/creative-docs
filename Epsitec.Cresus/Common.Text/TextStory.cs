//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for TextStory.
	/// </summary>
	public class TextStory
	{
		public TextStory()
		{
			this.text_chunks = new Internal.TextChunk[1];
			this.text_length = 0;
			
			this.cursors = new Internal.CursorTable ();
		}
		
		
		public int NewCursor()
		{
			return -1;
		}
		
		
		private Internal.TextChunkId FindTextChunkId(int position)
		{
			//	D�termine dans quel morceau de texte se trouve la position
			//	indiqu�e. Si la position est � la fronti�re de deux morceaux,
			//	pr�f�re le premier morceau.
			
			Debug.Assert.IsInBounds (position, 0, this.text_length);
			
			for (int i = 0; i < this.text_chunks.Length; i++)
			{
				position -= this.text_chunks[i].TextLength;
				
				if (position <= 0)
				{
					return i;
				}
			}
			
			//	Nous avons un probl�me : la position demand�e se trouve hors
			//	des bornes, ce qui est impossible.
			
			throw new Debug.FailureException ();
		}
		
		private int                  FindTextChunkPosition(Internal.TextChunkId id)
		{
			//	D�termine la position du d�but du morceau sp�cifi�.
			
			Debug.Assert.IsInBounds (id, 0, this.text_chunks.Length-1);
			
			int position = 0;
			
			for (int i = 0; i < id; i++)
			{
				position += this.text_chunks[i].TextLength;
			}
			
			return position;
		}
		
		
		private void SplitTextChunk(Internal.TextChunkId id, int offset)
		{
			//	Partage un morceau de texte devenu trop grand en deux morceaux
			//	distincts. Les curseurs dans la table globale doivent tous �tre
			//	v�rifi�s et �ventuellement ajust�s.
			
			Debug.Assert.IsInBounds (id, 0, this.text_chunks.Length-1);
			Debug.Assert.IsInBounds (offset, 0, this.text_chunks[id].TextLength);
			
			int id_1 = id;
			int id_2 = id + 1;
			
			//	Ajoute un nouveau TextChunk apr�s celui qui doit �tre partag� :
			
			{
				Internal.TextChunk[] old_chunks = this.text_chunks;
				Internal.TextChunk[] new_chunks = new Internal.TextChunk[old_chunks.Length+1];
				
				System.Array.Copy (old_chunks, 0, new_chunks, 0, id_2);
				System.Array.Copy (old_chunks, id_2, new_chunks, id_2+1, old_chunks.Length-id_2);
				
				new_chunks[id_2] = new Internal.TextChunk ();
				
				this.text_chunks = new_chunks;
			}
			
			//	D�place une partie du texte du morceau courant vers le morceau
			//	suivant, tout en d�pla�ant aussi les curseurs :
			
			Internal.TextChunk.ShuffleEnd (this.text_chunks[id_1], this.text_chunks[id_2], offset);
			
			int count;
			
			//	Met � jour tous les curseurs qui se trouvent apr�s les deux morceaux
			//	que nous venons de manipuler (parcours lin�aire dans la table des
			//	curseurs pour �viter du "memory trashing") :
			
			count = this.cursors.CursorCount;
			
			for (int i = 0; i < count; i++)
			{
				int text_chunk_id = this.cursors.GetCursorTextChunkId (i);
				
				if (text_chunk_id > id)
				{
					this.cursors.SetCursorTextChunkId (i, text_chunk_id + 1);
				}
			}
			
			//	Met � jour tous les curseurs qui se trouvent dans le nouveau morceau
			//	fra�chement cr�� (ils n'ont pas �t� affect�s par la mise � jour qui
			//	vient juste d'�tre faite) :
			
			Internal.CursorIdArray curs_2 = this.text_chunks[id_2].Cursors;
			
			count = curs_2.GetElementCount ();
			
			for (int i = 0; i < count; i++)
			{
				this.cursors.ModifyCursorTextChunkId (curs_2.GetElementCursorId (i), 1);
			}
		}
		
		
		private Internal.TextChunk[]			text_chunks;
		private int								text_length;
		private Internal.CursorTable			cursors;
	}
}
