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
			
			this.text_chunks[0] = new Internal.TextChunk ();
		}
		
		
		public int								TextLength
		{
			get
			{
				return this.text_length;
			}
		}
		
		
		public int NewCursor()
		{
			Internal.CursorId id     = this.cursors.NewCursor ();
			Internal.Cursor   cursor = this.cursors.ReadCursor (id);
			
			cursor.TextChunkId = 0;
			
			this.cursors.WriteCursor (id, cursor);
			
			this.text_chunks[0].Cursors.Add (id, 0);
			
			return id;
		}
		
		
		public void InsertText(int cursor_id, ulong[] text)
		{
			Internal.Cursor      cursor   = this.cursors.ReadCursor (cursor_id);
			Internal.TextChunkId chunk_id = cursor.TextChunkId;
			Internal.TextChunk   chunk    = this.text_chunks[chunk_id];
			
			int cursor_position = chunk.Cursors.GetCursorPosition (cursor_id);
			
			chunk.InsertText (cursor_position, text);
			
			//	Si l'insertion génère un morceau de texte trop gros, on le découpe en
			//	deux parts égales :
			
			if (chunk.TextLength > TextStory.TextChunkSplitSize)
			{
				this.SplitTextChunk (chunk_id, chunk.TextLength / 2);
			}
			
			this.text_length += text.Length;
		}
		
		
		public void WriteRawText(System.IO.Stream stream)
		{
			byte[] header = new byte[8];
			
			header[0] = (byte) ('T');
			header[1] = (byte) ('X');
			header[2] = (byte) ('T');
			header[3] = (byte) ('1');
			header[4] = (byte) ((this.text_length >> 24) & 0xff);
			header[5] = (byte) ((this.text_length >> 16) & 0xff);
			header[6] = (byte) ((this.text_length >>  8) & 0xff);
			header[7] = (byte) ((this.text_length >>  0) & 0xff);
			
			stream.Write (header, 0, header.Length);
			
			int max_size = 0;
			
			for (int i = 0; i < this.text_chunks.Length; i++)
			{
				max_size = System.Math.Max (max_size, this.text_chunks[i].TextLength);
			}
			
			byte[] buffer = new byte[8*max_size];
			
			for (int i = 0; i < this.text_chunks.Length; i++)
			{
				int length;
				this.text_chunks[i].SaveRawText (buffer, out length);
				stream.Write (buffer, 0, 8*length);
			}
		}
		
		public void ReadRawText(System.IO.Stream stream)
		{
			if ((this.text_length > 0) ||
				(this.text_chunks.Length > 1) ||
				(this.cursors.CursorCount > 0))
			{
				throw new System.InvalidOperationException ("TextStory must be empty.");
			}
			
			byte[] header = new byte[8];
			
			stream.Read (header, 0, header.Length);
			
			if ((header[0] == (byte) ('T')) &&
				(header[1] == (byte) ('X')) &&
				(header[2] == (byte) ('T')) &&
				(header[3] == (byte) ('1')))
			{
				int length = (header[4] << 24) | (header[5] << 16) | (header[6] << 8) | (header[7] << 0);
				
				if (length > 0)
				{
					int n = (length + TextStory.TextChunkIdealSize - 1) / TextStory.TextChunkIdealSize;
					
					this.text_chunks = new Internal.TextChunk[n];
					this.text_length = length;
					
					int    count  = 8*TextStory.TextChunkIdealSize;
					byte[] buffer = new byte[count];
					
					for (int i = 0; i < n; i++)
					{
						int read = stream.Read (buffer, 0, count);
						
					again:
						if (read < count)
						{
							//	Il se peut que le Read n'ait pas retourné tout ce qui lui a été
							//	demandé (en cas de décompression, par exemple). Dans ce cas, il
							//	faut tenter de lire la suite par petits morceaux :
							
							int more = stream.Read (buffer, read, count-read);
							
							if (more != 0)
							{
								read += more;
								goto again;
							}
							
							if ((i < n-1) ||
								((read % 8) != 0))
							{
								throw new System.InvalidOperationException ("Truncated text stream.");
							}
						}
						
						this.text_chunks[i] = new Internal.TextChunk ();
						this.text_chunks[i].LoadRawText (buffer, read/8);
					}
					
					return;
				}
			}
			
			throw new System.InvalidOperationException ("Not a valid text stream.");
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
			
			//	Met à jour tous les curseurs qui se trouvent après les deux morceaux
			//	que nous venons de manipuler (parcours linéaire dans la table des
			//	curseurs pour éviter du "memory trashing") :
			
			foreach (Internal.CursorId cursor_id in this.cursors)
			{
				int text_chunk_id = this.cursors.GetCursorTextChunkId (cursor_id);
				
				if (text_chunk_id > id)
				{
					this.cursors.SetCursorTextChunkId (cursor_id, text_chunk_id + 1);
				}
			}
			
			//	Met à jour tous les curseurs qui se trouvent dans le nouveau morceau
			//	fraîchement créé (ils n'ont pas été affectés par la mise à jour qui
			//	vient juste d'être faite) :
			
			Internal.CursorIdArray curs_2 = this.text_chunks[id_2].Cursors;
			int                    count  = curs_2.GetElementCount ();
			
			for (int i = 0; i < count; i++)
			{
				this.cursors.ModifyCursorTextChunkId (curs_2.GetElementCursorId (i), 1);
			}
		}
		
		
		protected const int						TextChunkIdealSize = 10000;
		protected const int						TextChunkSplitSize = 15000;
		protected const int						TextChunkMergeSize =  8000;
		
		private Internal.TextChunk[]			text_chunks;
		private int								text_length;
		private Internal.CursorTable			cursors;
	}
}
