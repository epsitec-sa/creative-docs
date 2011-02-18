//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe TextTable repr�sente un ensemble de morceaux de texte avec
	/// ses curseurs associ�s.
	/// </summary>
	internal sealed class TextTable
	{
		public TextTable()
		{
			this.textChunks = new Internal.TextChunk[1];
			this.textLength = 0;
			
			this.cursors = new Internal.CursorTable ();
			
			//	Alloue un premier morceau :
			
			this.textChunks[0] = new Internal.TextChunk ();
		}
		
		
		public int								TextLength
		{
			get
			{
				return this.textLength;
			}
		}
		
		public ulong							this[Internal.CursorId cursorId]
		{
			get
			{
				ulong[] buffer = { 0 };
				this.ReadText (cursorId, 1, buffer);
				return buffer[0];
			}
		}
		
		public long								Version
		{
			get
			{
				return this.version;
			}
		}
		
		
		internal CursorTable					CursorTable
		{
			get
			{
				return this.cursors;
			}
		}
		
		
		public Internal.CursorId NewCursor(ICursor cursor)
		{
			Internal.CursorId id     = this.cursors.NewCursor ();
			Internal.Cursor   record = this.cursors.ReadCursor (id);
			
			//	Place (arbitrairement) le curseur au d�but du texte, donc dans le
			//	morceau de texte num�ro 1, � la position 0 :
			
			record.TextChunkId    = 1;
			record.CursorInstance = cursor;
			
			this.cursors.WriteCursor (id, record);
			
			this.textChunks[0].AddCursor (id, 0, record.Attachment);
			
			return id;
		}
		
		public Internal.CursorId NewCursor(ICursor cursor, Internal.CursorId modelId)
		{
			//	TODO: optimiser la cr�ation d'un curseur au m�me endroit
			//	qu'un autre.
			
			Internal.CursorId id = this.NewCursor (cursor);
			
			this.SetCursorPosition (id, this.GetCursorPosition (modelId));
			
			return id;
		}
		
		public void RecycleCursor(Internal.CursorId id)
		{
			Internal.Cursor record = this.cursors.ReadCursor (id);
			
			this.textChunks[record.TextChunkId-1].RemoveCursor (id);
			this.cursors.RecycleCursor (id);
		}
		
		
		public void ChangeVersion()
		{
			this.version++;
		}
		
		
		public int MoveCursor(Internal.CursorId id, int distance)
		{
			Internal.Cursor record = this.cursors.ReadCursor (id);
			
			if (distance > 0)
			{
				int index = record.TextChunkId - 1;
				int moved = 0;
				int pos   = this.textChunks[index].GetCursorPosition (id);
				
				while (distance > 0)
				{
					int room = this.textChunks[index].TextLength - pos;
					
					//	Si nous sommes dans le dernier morceau de texte, on ne
					//	peut de toute mani�re pas aller plus loin :
					
					if (index == this.textChunks.Length-1)
					{
						distance = System.Math.Min (room, distance);
					}
					
					//	Le d�placement va-t-il se terminer dans le morceau de
					//	texte actuel ?
					
					if (room >= distance)
					{
						this.textChunks[index].MoveCursor (id, pos + distance);
						
						moved   += distance;
						distance = 0;
						
						record.CachedPosition = -1;
						
						this.cursors.WriteCursor (id, record);
						
						break;
					}
					
					//	Retire le curseur du morceau de texte actuel pour l'ins�rer
					//	dans le morceau suivant :
					
					this.textChunks[index+0].RemoveCursor (id);
					this.textChunks[index+1].AddCursor (id, 0, record.Attachment);
					
					record.TextChunkId = index+1 + 1;
					record.CachedPosition = -1;
					
					this.cursors.WriteCursor (id, record);
					
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
				
				int index = record.TextChunkId - 1;
				int moved = 0;
				int pos   = this.textChunks[index].GetCursorPosition (id);
				
				while (distance > 0)
				{
					//	Si nous sommes dans le premier morceau de texte, on ne
					//	peut de toute mani�re pas aller plus loin :
					
					if (index == 0)
					{
						distance = System.Math.Min (pos, distance);
					}
					
					//	Le d�placement va-t-il se terminer dans le morceau de
					//	texte actuel ?
					
					if (pos >= distance)
					{
						this.textChunks[index].MoveCursor (id, pos - distance);
						
						moved   += distance;
						distance = 0;
						
						record.CachedPosition = -1;
						
						this.cursors.WriteCursor (id, record);
						
						break;
					}
					
					//	Retire le curseur du morceau de texte actuel pour l'ins�rer
					//	dans le morceau pr�c�dent :
					
					this.textChunks[index+0].RemoveCursor (id);
					this.textChunks[index-1].AddCursor (id, this.textChunks[index-1].TextLength, record.Attachment);
					
					record.TextChunkId = index-1 + 1;
					record.CachedPosition = -1;
					
					this.cursors.WriteCursor (id, record);
					
					moved    += pos;
					distance -= pos;
					pos       = this.textChunks[index-1].TextLength;
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
		
		public void SetCursorDirection(Internal.CursorId id, int direction)
		{
			ICursor cursor = this.GetCursorInstance (id);
			
			if (cursor != null)
			{
				cursor.Direction = direction;
			}
		}
		
		
		public ICursor GetCursorInstance(Internal.CursorId id)
		{
			return this.cursors.GetCursorInstance (id);
		}
		
		
		public CursorInfo[] FindCursorsBefore(int position)
		{
			return this.FindCursorsBefore (position, null);
		}
		
		public CursorInfo[] FindCursorsBefore(int position, CursorInfo.Filter filter)
		{
			//	Trouve les curseurs qui se trouvent avant la position indiqu�e.
			//	Retourne le premier curseur trouv� (s'il y en a plusieurs au m�me
			//	endroit, on les retourne tous en bloc).
			
			if (position > this.textLength)
			{
				position = this.textLength;
			}
			else if (position < 0)
			{
				return new CursorInfo[0];
			}
			
			for (;;)
			{
				int id     = this.FindTextChunkId (position);
				int origin = this.FindTextChunkPosition (id);
				
				int iId  = id - 1;
				int iOff = position - origin;
				
				Internal.TextChunk chunk = this.textChunks[iId];
				
				int cursorIndex = chunk.GetCursorIndexBeforePosition (iOff);
				
				if (cursorIndex > -1)
				{
					//	Trouv� un curseur plac� avant la position indiqu�e. C'est
					//	le dernier d'un paquet s'il y en a plusieurs qui pointent
					//	au m�me endroit.
					
					CursorId cursorId  = chunk.GetNthCursorId (cursorIndex);
					int      cursorPos = this.GetCursorPosition (cursorId);
					
					int count = 1;
					
					for (int i = 1; i <= cursorIndex; i++)
					{
						CursorId cursorIdI  = chunk.GetNthCursorId (cursorIndex - i);
						int      cursorPosI = this.GetCursorPosition (cursorIdI);
						
						if (cursorPosI != cursorPos)
						{
							break;
						}
						
						count++;
					}
					
					CursorInfo[] result = new CursorInfo[count];
					
					result[0] = new CursorInfo (cursorId, cursorPos, this.GetCursorDirection (cursorId));
					
					for (int i = 1; i < count; i++)
					{
						CursorId cursorIdI  = chunk.GetNthCursorId (cursorIndex - i);
						int      cursorPosI = this.GetCursorPosition (cursorIdI);
						
						result[i] = new CursorInfo (cursorIdI, cursorPosI, this.GetCursorDirection (cursorIdI));
					}
					
					//	Filtre le r�sultat, si n�cessaire :
					
					if (result.Length > 0)
					{
						if (filter != null)
						{
							result = this.FilterCursors (result, filter);
							
							if (result.Length == 0)
							{
								//	Arriv� au d�but sans rien n'avoir trouv� ?
								
								if (position == origin)
								{
									break;
								}
								
								//	Il faut continuer la recherche � partir de la
								//	position courante.
								
								position = cursorPos;
								continue;
							}
						}
						
						//	Il y a des r�sultats (apr�s avoir appliqu� le filtre);
						//	on s'arr�te l� :
						
						return result;
					}
				}
				
				//	Aucun curseur trouv� dans le morceau de texte consid�r�; tant
				//	que l'on n'a pas atteint le d�but du texte, on recule et on
				//	relance la recherche depuis la fin du morceau pr�c�dent :
				
				if (position == origin)
				{
					break;
				}
				
				position = origin;
			}
			
			return new CursorInfo[0];
		}
		
		
		public CursorInfo[] FindCursors(int position, int length)
		{
			return this.FindCursors (position, length, null);
		}
		
		public CursorInfo[] FindCursors(int position, int length, CursorInfo.Filter filter)
		{
			return this.FindCursors (position, length, filter, false);
		}
		
		public CursorInfo[] FindCursors(int position, int length, CursorInfo.Filter filter, bool findOnlyFirst)
		{
			//	Trouve tous les curseurs compris dans la plage indiqu�e.
			
			if (position > this.textLength)
			{
				return new CursorInfo[0];
			}
			
			int id     = this.FindTextChunkId (position);
			int origin = this.FindTextChunkPosition (id);
			int end    = System.Math.Min (position + length, this.textLength);
			
			int iOrg = origin;
			int iId  = id - 1;
			int iNum = 0;
			int iPos = origin;
			int iOff = position - origin;
			
			while (iPos <= end)
			{
				Internal.TextChunk chunk = this.textChunks[iId];
				
				int n = chunk.GetCursorCount ();
				int i = (iOff > 0) ? (chunk.GetCursorIndexBeforePosition (iOff) + 1) : 0;
				int j = 0;
				
				while (j < n)
				{
					iPos += chunk.GetNthCursorOffset (j);
					
					if (iPos > end)
					{
						goto done_phase_1;
					}
					
					if (j >= i)
					{
						Internal.CursorId cursorId       = chunk.GetNthCursorId (j);
						ICursor           cursorInstance = this.cursors.GetCursorInstance (cursorId);
						
						if (filter != null)
						{
							if (filter (cursorInstance, iPos))
							{
								if (findOnlyFirst)
								{
									return new CursorInfo[] { new CursorInfo (cursorId, iPos, cursorInstance == null ? 0 : cursorInstance.Direction) };
								}
								
								iNum++;
							}
						}
						else
						{
							if (findOnlyFirst)
							{
								return new CursorInfo[] { new CursorInfo (cursorId, iPos, cursorInstance == null ? 0 : cursorInstance.Direction) };
							}
							
							iNum++;
						}
					}
					
					j++;
				}
				
				//	On vient de finir de passer en revue un morceau de texte. Il
				//	faut analyser le suivant, s'il y en a un :
				
				if (++iId == this.textChunks.Length)
				{
					break;
				}
				
				iOff  = 0;
				iOrg += chunk.TextLength;
				iPos  = iOrg;
			}
			
		done_phase_1:
			
			//	On a trouv� combien il y a de curseurs dans la tranche consid�r�e.
			//	Alloue la table et refait un second parcours :
			
			CursorInfo[] infos = new CursorInfo[iNum];
			
			iOrg = origin;
			iId  = id - 1;
			iNum = 0;
			iPos = origin;
			iOff = position - origin;
			
			while (iPos <= end)
			{
				Internal.TextChunk chunk = this.textChunks[iId];
				
				int n = chunk.GetCursorCount ();
				int i = (iOff > 0) ? (chunk.GetCursorIndexBeforePosition (iOff) + 1) : 0;
				int j = 0;
				
				while (j < n)
				{
					iPos += chunk.GetNthCursorOffset (j);
					
					if (iPos > end)
					{
						goto done_phase_2;
					}
					
					if (j >= i)
					{
						Internal.CursorId cursorId       = chunk.GetNthCursorId (j);
						ICursor           cursorInstance = this.cursors.GetCursorInstance (cursorId);
						
						if (filter != null)
						{
							if (filter (cursorInstance, iPos))
							{
								infos[iNum] = new CursorInfo (cursorId, iPos, cursorInstance == null ? 0 : cursorInstance.Direction);
								iNum++;
							}
						}
						else
						{
							infos[iNum] = new CursorInfo (cursorId, iPos, cursorInstance == null ? 0 : cursorInstance.Direction);
							iNum++;
						}
					}
					
					j++;
				}
				
				//	On vient de finir de passer en revue un morceau de texte. Il
				//	faut analyser le suivant, s'il y en a un :
				
				if (++iId == this.textChunks.Length)
				{
					break;
				}
				
				iOff  = 0;
				iOrg += chunk.TextLength;
				iPos  = iOrg;
			}
			
		done_phase_2:
			
			Debug.Assert.IsTrue (iNum == infos.Length);
			
			//	Termin�.
			
			return infos;
		}
		
		
		public CursorInfo[] FindNextCursor(Internal.CursorId id, CursorInfo.Filter filter)
		{
			int pos = this.GetCursorPosition (id);
			int len = this.textLength - pos;
			
			//	TODO: on pourrait faire nettement mieux ici !
			
			CursorInfo[] infos = this.FindCursors (pos, 1, filter);
			
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i].CursorId == id) &&
					(i+1 < infos.Length))
				{
					return new CursorInfo[] { infos[i+1] };
				}
			}
			
			return this.FindCursors (pos+1, len-1, filter, true);
		}
		
		public CursorInfo[] FindPrevCursor(Internal.CursorId id, CursorInfo.Filter filter)
		{
			int pos = this.GetCursorPosition (id);
			
			//	TODO: on pourrait faire nettement mieux ici !
			
			CursorInfo[] infos = this.FindCursors (pos, 1, filter);
			
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i].CursorId == id) &&
					(i > 0))
				{
					return new CursorInfo[] { infos[i-1] };
				}
			}
			
			CursorInfo[] result = this.FindCursorsBefore (pos, filter);
			
			if (result.Length > 1)
			{
				result = new CursorInfo[1] { result[result.Length-1] };
			}
			
			return result;
		}
		
		
		public CursorInfo[] FilterCursors(CursorInfo[] array, CursorInfo.Filter filter)
		{
			bool[] keep = new bool[array.Length];
			int    num  = 0;
			
			for (int i = 0; i < array.Length; i++)
			{
				ICursor cursor   = this.cursors.GetCursorInstance (array[i].CursorId);
				int     position = array[i].Position;
				
				if (filter (cursor, position))
				{
					keep[i] = true;
					num++;
				}
			}
			
			CursorInfo[] copy  = new CursorInfo[num];
			int          index = 0;
			
			for (int i = 0; i < array.Length; i++)
			{
				if (keep[i])
				{
					copy[index++] = array[i];
				}
			}
			
			return copy;
		}
		
		
		public int GetCursorPosition(Internal.CursorId id)
		{
			Internal.Cursor record = this.cursors.ReadCursor (id);
			
			if (this.cursors.IsPositionCacheValid (id))
			{
				return record.CachedPosition;
			}
			
			int offset = this.textChunks[record.TextChunkId-1].GetCursorPosition (id);
			int start  = this.FindTextChunkPosition (record.TextChunkId);
			int pos    = start + offset;
			
			//	Puisque nous venons de recalculer la position du curseur, c'est
			//	le bon moment pour en prendre note, afin de pouvoir en b�n�ficier
			//	la prochaine fois :
			
			record.CachedPosition = pos;
			this.cursors.WriteCursor (id, record);
			
			return pos;
		}
		
		public int GetCursorDirection(Internal.CursorId id)
		{
			ICursor cursor = this.GetCursorInstance (id);
			
			return (cursor == null) ? 0 : cursor.Direction;
		}
		
		public int GetCursorDistance(Internal.CursorId idA, Internal.CursorId idB)
		{
			//	Retourne la distance de la position du curseur 'a' � la position du curseur
			//	'b' (dans les faits, calcule pos[b] - pos[a]).
			
			int posA = this.GetCursorPosition (idA);
			int posB = this.GetCursorPosition (idB);
			
			return posB - posA;
		}
		
		
		public void InsertText(Internal.CursorId cursorId, ulong[] text)
		{
			Internal.Cursor record = this.cursors.ReadCursor (cursorId);
			
			Debug.Assert.IsTrue (record.TextChunkId > 0);
			
			int chunkId = record.TextChunkId;
			Internal.TextChunk chunk = this.textChunks[chunkId-1];
			
			int cursorPosition = chunk.GetCursorPosition (cursorId);
			
			chunk.InsertText (cursorPosition, text);
			
			//	Si l'insertion g�n�re un morceau de texte trop gros, on le d�coupe
			//	en deux parts �gales :
			
			if (chunk.TextLength > TextTable.TextChunkSplitSize)
			{
				this.SplitTextChunk (chunkId, chunk.TextLength / 2);
				this.OptimizeTextChunk (chunkId);
			}
			
			this.textLength += text.Length;
			this.ChangeVersion ();
			
			this.cursors.InvalidatePositionCache ();
		}
		
		public void DeleteText(Internal.CursorId cursorId, int length, out CursorInfo[] infos)
		{
			Internal.Cursor record = this.cursors.ReadCursor (cursorId);
			
			Debug.Assert.IsTrue (record.TextChunkId > 0);
			Debug.Assert.IsTrue (this.GetCursorPosition (cursorId) + length <= this.textLength);
			
			System.Collections.ArrayList list = null;
			
			int index  = record.TextChunkId - 1;
			int offset = this.textChunks[index].GetCursorPosition (cursorId);
			int start  = this.FindTextChunkPosition (record.TextChunkId);
			int count  = 0;
			
			bool removalContinuation = false;
			
			while ((length > 0)
				&& (index < this.textChunks.Length))
			{
				Internal.TextChunk chunk = this.textChunks[index];
				
				int size = chunk.TextLength;
				int room = System.Math.Min (length, size - offset);
				
				chunk.DeleteText (offset, room, start, removalContinuation, out infos);
				
				if ((infos != null) &&
					(infos.Length > 0))
				{
					if (list == null)
					{
						list = new System.Collections.ArrayList ();
					}
					
					//	Conserve les informations au sujet des curseurs affect�s par la
					//	suppression du texte (cursor, position et direction) :
					
					for (int i = 0; i < infos.Length; i++)
					{
						ICursor iCursor = this.cursors.GetCursorInstance (infos[i].CursorId);
						
						list.Add (new CursorInfo (infos[i].CursorId, infos[i].Position, iCursor.Direction));
					}
				}
				
				if (size > 0)
				{
					removalContinuation = true;
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
			
			this.textLength -= count;
			this.ChangeVersion ();
			
			this.cursors.InvalidatePositionCache ();
		}
		
		
		public ulong ReadChar(Internal.CursorId cursorId)
		{
			ulong[] buffer = new ulong[1];
			int     length = 1;
			
			length = this.ReadText (cursorId, length, buffer);
			
			if (length != 1)
			{
				return 0;
			}
			
			return buffer[0];
		}
		
		public ulong ReadChar(Internal.CursorId cursorId, int offset)
		{
			ulong[] buffer = new ulong[1];
			int     length = 1;
			
			length = this.ReadText (cursorId, offset, length, buffer);
			
			if (length != 1)
			{
				return 0;
			}
			
			return buffer[0];
		}
		
		
		public int ReadText(Internal.CursorId cursorId, int length, ulong[] buffer)
		{
			return this.ReadText (cursorId, 0, length, buffer);
		}
		
		public int ReadText(Internal.CursorId cursorId, int offset, int length, ulong[] buffer)
		{
			int chunkId = this.cursors.ReadCursor (cursorId).TextChunkId;
			
			int index = chunkId - 1;
			int read  = 0;
			int pos   = this.textChunks[index].GetCursorPosition (cursorId);
			
			if (this.AdjustByOffset (ref index, ref pos, offset) == false)
			{
				return 0;
			}
			
			while (read < length)
			{
				if (pos == this.textChunks[index].TextLength)
				{
					index++;
					
					if (index == this.textChunks.Length)
					{
						break;
					}
					
					pos = 0;
				}
				else
				{
					buffer[read] = this.textChunks[index][pos];
					
					read += 1;
					pos  += 1;
				}
			}
			
			return read;
		}
		
		
		public int TraverseText(Internal.CursorId cursorId, int distance, TextStory.CodeCallback callback)
		{
			//	Traverse le texte en commen�ant � partir de la position du
			//	curseur. La distance indique combien de caract�res parcourir
			//	au plus; si elle est n�gative, le parcours se fait en marche
			//	arri�re.
			//
			//	En marche avant, on consid�re l'�l�ment sous le curseur comme
			//	premier �l�ment; en marche arri�re, on consid�re l'�l�ment qui
			//	pr�c�de imm�diatement le curseur comme premier �l�ment.
			//
			//	D�s que le callback retourne 'true', on arr�te et on retourne
			//	le nombre de caract�res qui ont �t� travers�s avec succ�s
			//	(r�sultat positif en cas de succ�s).
			//
			//	Retourne -1 si le callback n'a jamais retourn� 'true'.
			
			int chunkId = this.cursors.ReadCursor (cursorId).TextChunkId;
			
			int index = chunkId - 1;
			int count = 0;
			int pos   = this.textChunks[index].GetCursorPosition (cursorId);
			
			if (distance > 0)
			{
				int length = distance;
				
				while (count < length)
				{
					if (pos == this.textChunks[index].TextLength)
					{
						index++;
						
						if (index == this.textChunks.Length)
						{
							break;
						}
						
						pos = 0;
					}
					else
					{
						ulong code = this.textChunks[index][pos];
						
						if (callback (code))
						{
							return count;
						}
						
						count += 1;
						pos   += 1;
					}
				}
			}
			else if (distance < 0)
			{
				int length = - distance;
				
				while (count < length)
				{
					if (pos == 0)
					{
						index--;
						
						if (index < 0)
						{
							break;
						}
						
						pos = this.textChunks[index].TextLength;
					}
					else
					{
						count += 1;
						pos   -= 1;
						
						ulong code = this.textChunks[index][pos];
						
						if (callback (code))
						{
							return count - 1;
						}
					}
				}
			}
			
			return -1;
		}
		
		
		public int GetRunLength(Internal.CursorId cursorId, int length)
		{
			ulong code;
			ulong next;
			
			return this.GetRunLength (cursorId, length, out code, out next);
		}
		
		public int GetRunLength(Internal.CursorId cursorId, int length, out ulong code, out ulong next)
		{
			return this.GetRunLength (cursorId, 0, length, out code, out next);
		}
		
		public int GetRunLength(Internal.CursorId cursorId, int offset, int length, out ulong code, out ulong next)
		{
			//	Trouve la longueur de texte qui utilise exactement le m�me
			//	style que celui utilis� � la position de d�part.
			//
			//	- 'code' correspond au code du style du morceau de texte mesur�
			//	- 'next' correspond au code du style du prochain morceau de texte
			//
			//	Si on atteint la fin du texte, on aura 'next' = 'code'.
			
			int chunkId = this.cursors.ReadCursor (cursorId).TextChunkId;
			
			code = 0;
			next = 0;
			
			int index = chunkId - 1;
			int count = 0;
			int pos   = this.textChunks[index].GetCursorPosition (cursorId);
			
			if (this.AdjustByOffset (ref index, ref pos, offset) == false)
			{
				return 0;
			}
			
			while (count < length)
			{
				if (pos == this.textChunks[index].TextLength)
				{
					index++;
					
					if (index == this.textChunks.Length)
					{
						break;
					}
					
					pos = 0;
				}
				else
				{
					if (count == 0)
					{
						code = Internal.CharMarker.ExtractCoreAndSettings (this.textChunks[index][pos]);
						next = code;
					}
					else
					{
						next = Internal.CharMarker.ExtractCoreAndSettings (this.textChunks[index][pos]);
						
						if (code != next)
						{
							return count;
						}
					}
					
					count += 1;
					pos   += 1;
				}
			}
			
			return count;
		}
		
		
		public int ChangeMarkers(Internal.CursorId cursorId, int length, ulong marker, bool set)
		{
			int chunkId = this.cursors.ReadCursor (cursorId).TextChunkId;
			
			int  index    = chunkId - 1;
			int  changed  = 0;
			int  pos      = this.textChunks[index].GetCursorPosition (cursorId);
			bool modified = false;
			
			while (changed < length)
			{
				if (pos == this.textChunks[index].TextLength)
				{
					index++;
					
					if (index == this.textChunks.Length)
					{
						break;
					}
					
					pos = 0;
				}
				else
				{
					int count = System.Math.Min (length - changed, this.textChunks[index].TextLength);
					
					modified |= this.textChunks[index].ChangeMarkers (marker, pos, count, set);
					
					changed += count;
					pos     += count;
				}
			}
			
			if (modified)
			{
				this.ChangeVersion ();
			}
			
			return changed;
		}
		
		
		public int WriteText(Internal.CursorId cursorId, int length, ulong[] buffer)
		{
			return this.WriteText (cursorId, 0, length, buffer);
		}
		
		public int WriteText(Internal.CursorId cursorId, int offset, int length, ulong[] buffer)
		{
			int chunkId = this.cursors.ReadCursor (cursorId).TextChunkId;
			
			int  index    = chunkId - 1;
			int  wrote    = 0;
			int  pos      = this.textChunks[index].GetCursorPosition (cursorId);
			bool modified = false;
			
			if (this.AdjustByOffset (ref index, ref pos, offset) == false)
			{
				return 0;
			}
			
			while (wrote < length)
			{
				if (pos == this.textChunks[index].TextLength)
				{
					index++;
					
					if (index == this.textChunks.Length)
					{
						break;
					}
					
					pos = 0;
				}
				else
				{
					if (this.textChunks[index][pos] != buffer[wrote])
					{
						this.textChunks[index][pos] = buffer[wrote];
						modified = true;
					}
					
					wrote += 1;
					pos   += 1;
				}
			}
			
			if (modified)
			{
				this.ChangeVersion ();
			}
			
			return wrote;
		}
		
		
		internal void ReadRawText(System.IO.Stream stream)
		{
			//	Remplit une table de texte en lisant un texte brut � partir d'un stream.
			
			if ((this.textLength > 0) ||
				(this.textChunks.Length > 1) ||
				(this.cursors.CursorCount > 0))
			{
				throw new System.InvalidOperationException ("TextTable must be empty.");
			}
			
			byte[] header = new byte[16];
			
			IO.ReaderHelper.Read (stream, header, 0, header.Length);
			
			if ((header[0] == (byte) ('T')) &&
				(header[1] == (byte) ('X')) &&
				(header[2] == (byte) ('T')) &&
				(header[3] == (byte) ('1')))
			{
				int     length = (header[4] << 24) | (header[5] << 16) | (header[6] << 8) | (header[7] << 0);
				ulong[] magick = new ulong[1];
				
				System.Buffer.BlockCopy (header, 8, magick, 0, 8);
				
				System.Diagnostics.Debug.Assert (magick[0] == 0x0123456789abcdefL);
				
				if (length > 0)
				{
					int n = (length + TextTable.TextChunkIdealSize - 1) / TextTable.TextChunkIdealSize;
					
					this.textChunks = new Internal.TextChunk[n];
					this.textLength = length;
					
					int    count  = 8*TextTable.TextChunkIdealSize;
					byte[] buffer = new byte[count];
					
					for (int i = 0; i < n; i++)
					{
						int read = IO.ReaderHelper.Read (stream, buffer, 0, count);
						
						if (read < count)
						{
							if ((i < n-1) ||
								((read % 8) != 0))
							{
								throw new System.InvalidOperationException ("Truncated text stream.");
							}
						}
						
						//	On d�cale l'index des morceaux de 1 cran (� cause du
						//	TextChunkId = 0 qui n'est pas valide).
						
						this.textChunks[i] = new Internal.TextChunk ();
						this.textChunks[i].SetRawText (buffer, 0, read);
					}
					
					this.ChangeVersion ();
					return;
				}
			}
			
			throw new System.InvalidOperationException ("Not a valid text stream.");
		}
		
		
		internal void WriteRawText(System.IO.Stream stream)
		{
			this.WriteRawText (stream, this.textLength);
		}
		
		internal void WriteRawText(System.IO.Stream stream, int length)
		{
			byte[] header = new byte[16];
			
			header[0] = (byte) ('T');
			header[1] = (byte) ('X');
			header[2] = (byte) ('T');
			header[3] = (byte) ('1');
			header[4] = (byte) ((length >> 24) & 0xff);
			header[5] = (byte) ((length >> 16) & 0xff);
			header[6] = (byte) ((length >>  8) & 0xff);
			header[7] = (byte) ((length >>  0) & 0xff);
			
			ulong[] magick = new ulong[] { 0x0123456789abcdefL };
				
			System.Buffer.BlockCopy (magick, 0, header, 8, 8);
			
			stream.Write (header, 0, header.Length);
			
			length *= 8;
			
			//	Commence par d�terminer la place qui sera n�cessaire pour stocker
			//	le plus gros morceau de texte :
			
			int maxSize = 0;
			
			for (int i = 0; i < this.textChunks.Length; i++)
			{
				maxSize = System.Math.Max (maxSize, this.textChunks[i].TextLength);
			}
			
			byte[] buffer = new byte[8*maxSize];
			
			//	Extrait les donn�es pour les envoyer vers le stream :
			
			for (int i = 0; i < this.textChunks.Length; i++)
			{
				int count = this.textChunks[i].GetRawText (buffer);
				int write = System.Math.Min (count, length);
				
				stream.Write (buffer, 0, write);
				
				length -= write;
				
				if (length == 0)
				{
					break;
				}
			}
		}
		
		
		private bool AdjustByOffset(ref int index, ref int pos, int offset)
		{
			int read = 0;
			
			if (offset > 0)
			{
				while (read < offset)
				{
					if (pos == this.textChunks[index].TextLength)
					{
						index++;
						
						if (index == this.textChunks.Length)
						{
							return false;
						}
						
						pos = 0;
					}
					else
					{
						read += 1;
						pos  += 1;
					}
				}
			}
			else if (offset < 0)
			{
				while (read > offset)
				{
					if (pos == 0)
					{
						index--;
						
						if (index < 0)
						{
							return false;
						}
						
						pos = this.textChunks[index].TextLength;
					}
					else
					{
						read -= 1;
						pos  -= 1;
					}
				}
			}
			
			return true;
		}
		
		
		private int FindTextChunkId(int position)
		{
			//	D�termine dans quel morceau de texte se trouve la position
			//	indiqu�e. Si la position est � la fronti�re de deux morceaux,
			//	pr�f�re le premier morceau.
			
			Debug.Assert.IsInBounds (position, 0, this.textLength);
			
			for (int i = 0; i < this.textChunks.Length; i++)
			{
				position -= this.textChunks[i].TextLength;
				
				if (position <= 0)
				{
					return i+1;
				}
			}
			
			//	Nous avons un probl�me : la position demand�e se trouve hors
			//	des bornes, ce qui est impossible.
			
			throw new Debug.FailureException ();
		}
		
		private int                  FindTextChunkPosition(int id)
		{
			//	D�termine la position du d�but du morceau sp�cifi�.
			
			Debug.Assert.IsTrue (id > 0);
			Debug.Assert.IsInBounds (id-1, 0, this.textChunks.Length-1);
			
			int position = 0;
			int fence    = id - 1;
			
			for (int i = 0; i < fence; i++)
			{
				position += this.textChunks[i].TextLength;
			}
			
			return position;
		}
		
		
		private void OptimizeTextChunk(int id)
		{
			//	Optimise l'utilisation de la m�moire du morceau de texte.
			
			Debug.Assert.IsTrue (id > 0);
			Debug.Assert.IsInBounds (id, 0, this.textChunks.Length-1);
			
			this.textChunks[id-1].OptimizeTextBuffer ();
		}
		
		private void SplitTextChunk(int id, int offset)
		{
			//	Partage un morceau de texte devenu trop grand en deux morceaux
			//	distincts. Les curseurs dans la table globale doivent tous �tre
			//	v�rifi�s et �ventuellement ajust�s.
			
			Debug.Assert.IsTrue (id > 0);
			Debug.Assert.IsInBounds (id-1, 0, this.textChunks.Length-1);
			Debug.Assert.IsInBounds (offset, 0, this.textChunks[id-1].TextLength);
			
			int id1 = id - 1;
			int id2 = id;
			
			//	Ajoute un nouveau TextChunk apr�s celui qui doit �tre partag� :
			
			{
				Internal.TextChunk[] oldChunks = this.textChunks;
				Internal.TextChunk[] newChunks = new Internal.TextChunk[oldChunks.Length+1];
				
				System.Array.Copy (oldChunks, 0, newChunks, 0, id2);
				System.Array.Copy (oldChunks, id2, newChunks, id2+1, oldChunks.Length-id2);
				
				newChunks[id2] = new Internal.TextChunk ();
				
				this.textChunks = newChunks;
			}
			
			//	D�place une partie du texte du morceau courant vers le morceau
			//	suivant, tout en d�pla�ant aussi les curseurs :
			
			Internal.TextChunk.ShuffleEnd (this.textChunks[id1], this.textChunks[id2], offset);
			
			//	Met � jour tous les curseurs qui se trouvent apr�s les deux morceaux
			//	que nous venons de manipuler (parcours lin�aire dans la table des
			//	curseurs pour �viter du "memory trashing") :
			
			foreach (Internal.CursorId cursorId in this.cursors)
			{
				int textChunkId = this.cursors.GetCursorTextChunkId (cursorId);
				
				if (textChunkId > id)
				{
					this.cursors.SetCursorTextChunkId (cursorId, textChunkId + 1);
				}
			}
			
			//	Met � jour tous les curseurs qui se trouvent dans le nouveau morceau
			//	fra�chement cr�� (ils n'ont pas �t� affect�s par la mise � jour qui
			//	vient juste d'�tre faite) :
			
			Internal.TextChunk chunk = this.textChunks[id2];
			int                count = chunk.GetCursorCount ();
			
			for (int i = 0; i < count; i++)
			{
				this.cursors.ModifyCursorTextChunkId (chunk.GetNthCursorId (i), 1);
			}
		}
		
		
		
		public const int						TextChunkIdealSize = 10000;
		public const int						TextChunkSplitSize = 15000;
		public const int						TextChunkMergeSize =  8000;
		
		private Internal.TextChunk[]			textChunks;		//	0..n valides; (prendre index-1 pour l'acc�s)
		private int								textLength;
		private Internal.CursorTable			cursors;
		private long							version = 1;
	}
}
