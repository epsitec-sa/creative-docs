//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe TextTable représente un ensemble de morceaux de texte avec
	/// ses curseurs associés.
	/// </summary>
	internal sealed class TextTable
	{
		public TextTable()
		{
			this.text_chunks = new Internal.TextChunk[1];
			this.text_length = 0;
			
			this.cursors = new Internal.CursorTable ();
			
			//	Alloue un premier morceau :
			
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
			
			cursor.TextChunkId = 1;
			
			this.cursors.WriteCursor (id, cursor);
			
			this.text_chunks[0].AddCursor (id, 0);
			
			return id;
		}
		
		
		public void InsertText(Internal.CursorId cursor_id, ulong[] text)
		{
			Internal.Cursor cursor = this.cursors.ReadCursor (cursor_id);
			
			Debug.Assert.IsTrue (cursor.TextChunkId.IsValid);
			
			Internal.TextChunkId chunk_id = cursor.TextChunkId;
			Internal.TextChunk   chunk    = this.text_chunks[chunk_id-1];
			
			int cursor_position = chunk.GetCursorPosition (cursor_id);
			
			chunk.InsertText (cursor_position, text);
			
			//	Si l'insertion génère un morceau de texte trop gros, on le découpe
			//	en deux parts égales :
			
			if (chunk.TextLength > TextTable.TextChunkSplitSize)
			{
				this.SplitTextChunk (chunk_id, chunk.TextLength / 2);
				this.OptimizeTextChunk (chunk_id);
			}
			
			this.text_length += text.Length;
		}
		
		
		internal void WriteRawText(System.IO.Stream stream)
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
			
			//	Commence par déterminer la place qui sera nécessaire pour stocker
			//	le plus gros morceau de texte :
			
			int max_size = 0;
			
			for (int i = 0; i < this.text_chunks.Length; i++)
			{
				max_size = System.Math.Max (max_size, this.text_chunks[i].TextLength);
			}
			
			byte[] buffer = new byte[8*max_size];
			
			//	Extrait les données pour les envoyer vers le stream :
			
			for (int i = 0; i < this.text_chunks.Length; i++)
			{
				int count = this.text_chunks[i].GetRawText (buffer);
				stream.Write (buffer, 0, count);
			}
		}
		
		internal void ReadRawText(System.IO.Stream stream)
		{
			if ((this.text_length > 0) ||
				(this.text_chunks.Length > 1) ||
				(this.cursors.CursorCount > 0))
			{
				throw new System.InvalidOperationException ("TextTable must be empty.");
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
					int n = (length + TextTable.TextChunkIdealSize - 1) / TextTable.TextChunkIdealSize;
					
					this.text_chunks = new Internal.TextChunk[n];
					this.text_length = length;
					
					int    count  = 8*TextTable.TextChunkIdealSize;
					byte[] buffer = new byte[count];
					
					for (int i = 0; i < n; i++)
					{
						int read = stream.Read (buffer, 0, count);
						
					again:
						if (read < count)
						{
							//	Il se peut que le Read n'ait pas retourné tout ce
							//	qui lui a été demandé, sans pour autant que la fin
							//	ait été atteinte (par ex. lors de décompression).

							//	Dans ce cas, il faut tenter de lire la suite par
							//	petits morceaux :
							
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
						
						//	On décale l'index des morceaux de 1 cran (à cause du
						//	TextChunkId = 0 qui n'est pas valide).
						
						this.text_chunks[i] = new Internal.TextChunk ();
						this.text_chunks[i].SetRawText (buffer, 0, read);
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
					return new Internal.TextChunkId (i+1);
				}
			}
			
			//	Nous avons un problème : la position demandée se trouve hors
			//	des bornes, ce qui est impossible.
			
			throw new Debug.FailureException ();
		}
		
		private int                  FindTextChunkPosition(Internal.TextChunkId id)
		{
			//	Détermine la position du début du morceau spécifié.
			
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.text_chunks.Length-1);
			
			int position = 0;
			int fence    = id - 1;
			
			for (int i = 0; i < fence; i++)
			{
				position += this.text_chunks[i].TextLength;
			}
			
			return position;
		}
		
		
		private void OptimizeTextChunk(Internal.TextChunkId id)
		{
			//	Optimise l'utilisation de la mémoire du morceau de texte.
			
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id, 0, this.text_chunks.Length-1);
			
			this.text_chunks[id-1].OptimizeTextBuffer ();
		}
		
		private void SplitTextChunk(Internal.TextChunkId id, int offset)
		{
			//	Partage un morceau de texte devenu trop grand en deux morceaux
			//	distincts. Les curseurs dans la table globale doivent tous être
			//	vérifiés et éventuellement ajustés.
			
			Debug.Assert.IsTrue (id.IsValid);
			Debug.Assert.IsInBounds (id-1, 0, this.text_chunks.Length-1);
			Debug.Assert.IsInBounds (offset, 0, this.text_chunks[id-1].TextLength);
			
			int id_1 = id - 1;
			int id_2 = id;
			
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
			
			Internal.TextChunk chunk = this.text_chunks[id_2];
			int                count = chunk.GetCursorCount ();
			
			for (int i = 0; i < count; i++)
			{
				this.cursors.ModifyCursorTextChunkId (chunk.GetNthCursorId (i), 1);
			}
		}
		
		
		public const int						TextChunkIdealSize = 10000;
		public const int						TextChunkSplitSize = 15000;
		public const int						TextChunkMergeSize =  8000;
		
		private Internal.TextChunk[]			text_chunks;		//	0..n valides; (prendre index-1 pour l'accès)
		private int								text_length;
		private Internal.CursorTable			cursors;
	}
}
