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
		
		public ulong							this[Internal.CursorId cursor_id]
		{
			get
			{
				ulong[] buffer = { 0 };
				this.ReadText (cursor_id, 1, buffer, 0);
				return buffer[0];
			}
		}
		
		
		
		public Internal.CursorId NewCursor()
		{
			Internal.CursorId id     = this.cursors.NewCursor ();
			Internal.Cursor   cursor = this.cursors.ReadCursor (id);
			
			//	Place (arbitrairement) le curseur au début du texte, donc dans le
			//	morceau de texte numéro 1, à la position 0 :
			
			cursor.TextChunkId = 1;
			
			this.cursors.WriteCursor (id, cursor);
			
			this.text_chunks[0].AddCursor (id, 0);
			
			return id;
		}
		
		public void DeleteCursor(Internal.CursorId id)
		{
			Internal.Cursor cursor = this.cursors.ReadCursor (id);
			
			this.text_chunks[cursor.TextChunkId-1].RemoveCursor (id);
			this.cursors.RecycleCursor (id);
		}
		
		public int MoveCursor(Internal.CursorId id, int distance)
		{
			Internal.Cursor cursor = this.cursors.ReadCursor (id);
			
			if (distance > 0)
			{
				int index = cursor.TextChunkId - 1;
				int moved = 0;
				int pos   = this.text_chunks[index].GetCursorPosition (id);
				
				while (distance > 0)
				{
					int room = this.text_chunks[index].TextLength - pos;
					
					//	Si nous sommes dans le dernier morceau de texte, on ne
					//	peut de toute manière pas aller plus loin :
					
					if (index == this.text_chunks.Length-1)
					{
						distance = System.Math.Min (room, distance);
					}
					
					//	Le déplacement va-t-il se terminer dans le morceau de
					//	texte actuel ?
					
					if (room >= distance)
					{
						this.text_chunks[index].MoveCursor (id, pos + distance);
						
						moved   += distance;
						distance = 0;
						
						cursor.CachedPosition = -1;
						
						this.cursors.WriteCursor (id, cursor);
						
						break;
					}
					
					//	Retire le curseur du morceau de texte actuel pour l'insérer
					//	dans le morceau suivant :
					
					this.text_chunks[index+0].RemoveCursor (id);
					this.text_chunks[index+1].AddCursor (id, 0);
					
					cursor.TextChunkId = index+1 + 1;
					cursor.CachedPosition = -1;
					
					this.cursors.WriteCursor (id, cursor);
					
					moved    += room;
					distance -= room;
					pos       = 0;
					index    += 1;
				}
				
				return moved;
			}
			else if (distance < 0)
			{
				distance = - distance;
				
				int index = cursor.TextChunkId - 1;
				int moved = 0;
				int pos   = this.text_chunks[index].GetCursorPosition (id);
				
				while (distance > 0)
				{
					//	Si nous sommes dans le premier morceau de texte, on ne
					//	peut de toute manière pas aller plus loin :
					
					if (index == 0)
					{
						distance = System.Math.Min (pos, distance);
					}
					
					//	Le déplacement va-t-il se terminer dans le morceau de
					//	texte actuel ?
					
					if (pos >= distance)
					{
						this.text_chunks[index].MoveCursor (id, pos - distance);
						
						moved   += distance;
						distance = 0;
						
						cursor.CachedPosition = -1;
						
						this.cursors.WriteCursor (id, cursor);
						
						break;
					}
					
					//	Retire le curseur du morceau de texte actuel pour l'insérer
					//	dans le morceau précédent :
					
					this.text_chunks[index+0].RemoveCursor (id);
					this.text_chunks[index-1].AddCursor (id, this.text_chunks[index-1].TextLength);
					
					cursor.TextChunkId = index-1 + 1;
					cursor.CachedPosition = -1;
					
					this.cursors.WriteCursor (id, cursor);
					
					moved    += pos;
					distance -= pos;
					pos       = this.text_chunks[index-1].TextLength;
					index    -= 1;
				}
				
				return - moved;
			}
			
			return 0;
		}
		
		public void SetCursorPosition(Internal.CursorId id, int position)
		{
			this.MoveCursor (id, position - this.GetCursorPosition (id));
		}
		
		public CursorInfo[] FindCursors(int position, int length)
		{
			//	Trouve tous les curseurs compris dans la plage indiquée.
			
			if (position > this.text_length)
			{
				return new CursorInfo[0];
			}
			
			int id     = this.FindTextChunkId (position);
			int origin = this.FindTextChunkPosition (id);
			int end    = System.Math.Min (position + length, this.text_length);
			
			int i_org = origin;
			int i_id  = id - 1;
			int i_num = 0;
			int i_pos = origin;
			int i_off = position - origin;
			
			while (i_pos <= end)
			{
				Internal.TextChunk chunk = this.text_chunks[i_id];
				
				int n = chunk.GetCursorCount ();
				int i = (i_off > 0) ? (chunk.GetCursorIndexBeforePosition (i_off) + 1) : 0;
				int j = 0;
				
				while (j < n)
				{
					i_pos += chunk.GetNthCursorOffset (j);
					
					if (i_pos > end)
					{
						goto done_phase_1;
					}
					
					if (j >= i)
					{
						i_num++;
					}
					
					j++;
				}
				
				//	On vient de finir de passer en revue un morceau de texte. Il
				//	faut analyser le suivant, s'il y en a un :
				
				if (++i_id == this.text_chunks.Length)
				{
					break;
				}
				
				i_off  = 0;
				i_org += chunk.TextLength;
				i_pos  = i_org;
			}
			
		done_phase_1:
			
			//	On a trouvé combien il y a de curseurs dans la tranche considérée.
			//	Alloue la table et refait un second parcours :
			
			CursorInfo[] infos = new CursorInfo[i_num];
			
			i_org = origin;
			i_id  = id - 1;
			i_num = 0;
			i_pos = origin;
			i_off = position - origin;
			
			while (i_pos <= end)
			{
				Internal.TextChunk chunk = this.text_chunks[i_id];
				
				int n = chunk.GetCursorCount ();
				int i = (i_off > 0) ? (chunk.GetCursorIndexBeforePosition (i_off) + 1) : 0;
				int j = 0;
				
				while (j < n)
				{
					i_pos += chunk.GetNthCursorOffset (j);
					
					if (i_pos > end)
					{
						goto done_phase_2;
					}
					
					if (j >= i)
					{
						infos[i_num] = new CursorInfo (chunk.GetNthCursorId (j), i_pos);
						i_num++;
					}
					
					j++;
				}
				
				//	On vient de finir de passer en revue un morceau de texte. Il
				//	faut analyser le suivant, s'il y en a un :
				
				if (++i_id == this.text_chunks.Length)
				{
					break;
				}
				
				i_off  = 0;
				i_org += chunk.TextLength;
				i_pos  = i_org;
			}
			
		done_phase_2:
			
			Debug.Assert.IsTrue (i_num == infos.Length);
			
			//	Terminé.
			
			return infos;
		}
		
		
		public int GetCursorPosition(Internal.CursorId id)
		{
			Internal.Cursor cursor = this.cursors.ReadCursor (id);
			
			if (this.cursors.IsPositionCacheValid (id))
			{
				return cursor.CachedPosition;
			}
			
			int offset = this.text_chunks[cursor.TextChunkId-1].GetCursorPosition (id);
			int start  = this.FindTextChunkPosition (cursor.TextChunkId);
			int pos    = start + offset;
			
			//	Puisque nous venons de recalculer la position du curseur, c'est
			//	le bon moment pour en prendre note, afin de pouvoir en bénéficier
			//	la prochaine fois :
			
			cursor.CachedPosition = pos;
			this.cursors.WriteCursor (id, cursor);
			
			return pos;
		}
		
		public int GetCursorDistance(Internal.CursorId id_a, Internal.CursorId id_b)
		{
			//	Retourne la distance de la position du curseur 'a' à la position du curseur
			//	'b' (dans les faits, calcule pos[b] - pos[a]).
			
			int pos_a = this.GetCursorPosition (id_a);
			int pos_b = this.GetCursorPosition (id_b);
			
			return pos_b - pos_a;
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
			
			this.cursors.InvalidatePositionCache ();
		}
		
		
		public void DeleteText(Internal.CursorId cursor_id, int length, out CursorInfo[] infos)
		{
			Internal.Cursor cursor = this.cursors.ReadCursor (cursor_id);
			
			Debug.Assert.IsTrue (cursor.TextChunkId.IsValid);
			Debug.Assert.IsTrue (this.GetCursorPosition (cursor_id) + length <= this.text_length);
			
			System.Collections.ArrayList list = null;
			
			int index  = cursor.TextChunkId - 1;
			int offset = this.text_chunks[index].GetCursorPosition (cursor_id);
			int start  = this.FindTextChunkPosition (cursor.TextChunkId);
			int count  = 0;
			
			bool removal_continuation = false;
			
			while ((length > 0)
				&& (index < this.text_chunks.Length))
			{
				Internal.TextChunk chunk = this.text_chunks[index];
				
				int size = chunk.TextLength;
				int room = System.Math.Min (length, size - offset);
				
				chunk.DeleteText (offset, room, start, removal_continuation, out infos);
				
				if ((infos != null) &&
					(infos.Length > 0))
				{
					if (list == null)
					{
						list = new System.Collections.ArrayList ();
					}
					
					list.AddRange (infos);
				}
				
				if (size > 0)
				{
					removal_continuation = true;
				}
				
				offset  = 0;
				length -= room;
				count  += room;
				index  += 1;
				start  += size;
			}
			
			if (list == null)
			{
				infos = null;
			}
			else
			{
				infos = new CursorInfo[list.Count];
				list.CopyTo (infos);
			}
			
			this.text_length -= count;
			
			this.cursors.InvalidatePositionCache ();
		}
		
		
		public int ReadText(Internal.CursorId cursor_id, int length, ulong[] buffer)
		{
			return this.ReadText (cursor_id, length, buffer, 0);
		}
		
		public int ReadText(Internal.CursorId cursor_id, int length, ulong[] buffer, int offset)
		{
			Internal.TextChunkId chunk_id = this.cursors.ReadCursor (cursor_id).TextChunkId;
			
			int index = chunk_id - 1;
			int read  = 0;
			int pos   = this.text_chunks[index].GetCursorPosition (cursor_id);
			
			while (read < length)
			{
				if (pos == this.text_chunks[index].TextLength)
				{
					index++;
					
					if (index == this.text_chunks.Length)
					{
						break;
					}
					
					pos = 0;
				}
				else
				{
					buffer[read] = this.text_chunks[index][pos];
					
					read += 1;
					pos  += 1;
				}
			}
			
			return read;
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
			Debug.Assert.IsInBounds (id-1, 0, this.text_chunks.Length-1);
			
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
