//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			//	Détermine dans quel morceau de texte se trouve la position
			//	indiquée. Si la position est à la frontière de deux morceaux,
			//	préfère le premier morceau.
			
			Debug.Assert.IsInBounds (position, 0, this.text_length);
			
			for (int i = 0; i < this.text_chunks.Length; i++)
			{
				position -= this.text_chunks[i].TextLength;
				
				if (position <= 0)
				{
					return i;
				}
			}
			
			//	Nous avons un problème : la position demandée se trouve hors
			//	des bornes, ce qui est impossible.
			
			throw new Debug.FailureException ();
		}
		
		private int                  FindTextChunkPosition(Internal.TextChunkId id)
		{
			//	Détermine la position du début du morceau spécifié.
			
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
			//	distincts. Les curseurs dans la table globale doivent tous être
			//	vérifiés et éventuellement ajustés.
			
			Debug.Assert.IsInBounds (id, 0, this.text_chunks.Length-1);
			Debug.Assert.IsInBounds (offset, 0, this.text_chunks[id].TextLength);
			
			int id_1 = id;
			int id_2 = id + 1;
			
			//	Ajoute un nouveau TextChunk après celui qui doit être partagé :
			
			{
				Internal.TextChunk[] old_chunks = this.text_chunks;
				Internal.TextChunk[] new_chunks = new Internal.TextChunk[old_chunks.Length+1];
				
				System.Array.Copy (old_chunks, 0, new_chunks, 0, id_2);
				System.Array.Copy (old_chunks, id_2, new_chunks, id_2+1, old_chunks.Length-id_2);
				
				new_chunks[id_2] = new Internal.TextChunk ();
				
				this.text_chunks = new_chunks;
			}
			
			//	Déplace une partie du texte du morceau courant vers le morceau
			//	suivant, tout en déplaçant aussi les curseurs :
			
			Internal.TextChunk.ShuffleEnd (this.text_chunks[id_1], this.text_chunks[id_2], offset);
			
			int count;
			
			//	Met à jour tous les curseurs qui se trouvent après les deux morceaux
			//	que nous venons de manipuler (parcours linéaire dans la table des
			//	curseurs pour éviter du "memory trashing") :
			
			count = this.cursors.CursorCount;
			
			for (int i = 0; i < count; i++)
			{
				int text_chunk_id = this.cursors.GetCursorTextChunkId (i);
				
				if (text_chunk_id > id)
				{
					this.cursors.SetCursorTextChunkId (i, text_chunk_id + 1);
				}
			}
			
			//	Met à jour tous les curseurs qui se trouvent dans le nouveau morceau
			//	fraîchement créé (ils n'ont pas été affectés par la mise à jour qui
			//	vient juste d'être faite) :
			
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
