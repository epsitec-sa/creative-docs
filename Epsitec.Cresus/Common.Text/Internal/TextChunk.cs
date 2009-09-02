//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe TextChunk stocke un morceau de texte sous la forme d'un
	/// tableau de mots de 64 bits.
	/// </summary>
	internal sealed class TextChunk
	{
		public TextChunk()
		{
			this.cursors = new CursorIdArray ();
			this.text    = new ulong[0];
			
			this.accMarkers       = 0;
			this.accMarkersValid = true;
		}
		
		
		public int							TextLength
		{
			get
			{
				return this.length;
			}
		}
		
		
		public ulong						this[int position]
		{
			get
			{
				if ((position < 0) ||
					(position >= this.length))
				{
					throw new System.ArgumentOutOfRangeException ("position", position, "Index out of range.");
				}
				
				return this.text[position];
			}
			set
			{
				if ((position < 0) ||
					(position >= this.length))
				{
					throw new System.ArgumentOutOfRangeException ("position", position, "Index out of range.");
				}
				
				ulong mOld = Internal.CharMarker.GetMarker (this.text[position]);
				ulong mNew = Internal.CharMarker.GetMarker (value);
				
				this.text[position] = value;
				
				if ((mOld != mNew) &&
					(this.accMarkersValid))
				{
					ulong mClr = (mOld & ~mNew);
					
					if (mClr != 0)
					{
						//	On a supprimé certains bits; comme c'était peut-être
						//	les derniers, marque que l'accumulation actuelle doit
						//	être recalculée :
						
						this.accMarkersValid = false;
					}
					else
					{
						this.accMarkers |= mNew;
					}
				}
			}
		}
		
		
		public ulong						AccumulatedCharMarkers
		{
			get
			{
				//	Retourne l'union de tous les marqueurs de caractères (cf.
				//	la classe CharMarker) contenus dans ce morceau de texte.
				
				if (this.accMarkersValid == false)
				{
					this.accMarkersValid = true;
					this.accMarkers = Internal.CharMarker.Accumulate (this.text, 0, this.length);
				}
				
				return this.accMarkers;
			}
		}
		
		
		public void InsertText(int position, ulong[] text)
		{
			int length = text.Length;
			
			if (this.length + length > this.text.Length)
			{
				//	Il n'y a plus assez de place dans le buffer actuel. Il faut donc
				//	agrandir celui-ci.
				
				this.GrowTextBuffer (this.length + length);
			}
			
			if (this.accMarkersValid)
			{
				this.accMarkers |= Internal.CharMarker.Accumulate (text, 0, text.Length);
			}
			
			int offset1 = position;
			int offset2 = offset1 + length;
			int count    = this.length - offset1;
			
			Debug.Assert.IsTrue (offset1 >= 0);
			Debug.Assert.IsTrue (offset2+count <= this.text.Length);
			
			//	Creuse un trou pour y mettre le nouveau texte, puis copie le texte
			//	dans le trou et enfin, déplace les curseurs.
			
			System.Buffer.BlockCopy (this.text, 8*offset1, this.text, 8*offset2, 8*count);
			System.Buffer.BlockCopy (text, 0, this.text, 8*offset1, 8*length);
			
			this.length += length;
			
			this.cursors.ProcessInsertion (position, length);
		}
		
		public void DeleteText(int position, int length, int absOrigin, bool removalContinuation, out CursorInfo[] infos)
		{
			//	Supprime le texte commençant à la position indiquée et comprenant
			//	'length' caractères.
			
			//	L'appelant doit indiquer dans 'absOrigin' l'origine absolue du
			//	début du morceau de texte, afin que la table d'information sur
			//	les curseurs supprimés puisse contenir des positions absolues.
			
			Debug.Assert.IsTrue (position >= 0);
			Debug.Assert.IsTrue (length >= 0);
			Debug.Assert.IsTrue (position + length <= this.length);
			
			int offset1 = position + length;
			int offset2 = position;
			int count    = this.length - offset1;
			
			Debug.Assert.IsTrue (offset2 >= 0);
			Debug.Assert.IsTrue (offset1+count <= this.length);
			
			System.Buffer.BlockCopy (this.text, 8*offset1, this.text, 8*offset2, 8*count);
			
			this.length -= length;
			
			this.cursors.ProcessRemoval (position, length, absOrigin, removalContinuation, out infos);
			
			if (this.accMarkers != 0)
			{
				this.accMarkersValid = false;
			}
		}
		
		
		public void CopyTextToBuffer(int position, int length, ulong[] buffer)
		{
			Debug.Assert.IsTrue (position >= 0);
			Debug.Assert.IsTrue (length >= 0);
			Debug.Assert.IsTrue (length <= buffer.Length);
			Debug.Assert.IsTrue (position + length <= this.length);
			
			System.Buffer.BlockCopy (this.text, 8*position, buffer, 0, 8*length);
		}
		
		public void CopyTextToBuffer(int position, int length, ulong[] buffer, int offset)
		{
			Debug.Assert.IsTrue (position >= 0);
			Debug.Assert.IsTrue (length >= 0);
			Debug.Assert.IsTrue (offset >= 0);
			Debug.Assert.IsTrue (position + length <= this.length);
			Debug.Assert.IsTrue (offset + length <= buffer.Length);
			
			System.Buffer.BlockCopy (this.text, 8*position, buffer, 8*offset, 8*length);
		}
		
		
		public bool ChangeMarkers(ulong marker, int position, int length, bool set)
		{
			//	Change les marqueurs pour le fragment de texte considéré.
			//	Retourne 'true' si un changement a eu lieu.
			
			bool changed = false;
			
			if (set)
			{
				changed = Internal.CharMarker.SetMarkers (marker, this.text, position, length);
				
				if (this.accMarkersValid)
				{
					this.accMarkers |= marker;
				}
			}
			else
			{
				if (Internal.CharMarker.ClearMarkers (marker, this.text, position, length))
				{
					changed = true;
					this.accMarkersValid = false;
				}
			}
			
			return changed;
		}
		
		
		public int GetRawText(byte[] buffer)
		{
			int count = 8*this.length;
			
			Debug.Assert.IsNotNull (buffer);
			Debug.Assert.IsTrue (buffer.Length >= count);
			
			System.Buffer.BlockCopy (this.text, 0, buffer, 0, count);
			
			return count;
		}
		
		public void SetRawText(byte[] data, int offset, int count)
		{
			Debug.Assert.IsNotNull (data);
			Debug.Assert.IsTrue (offset >= 0);
			Debug.Assert.IsTrue ((count % 8) == 0);
			
			int length = count/8;
			
			this.text   = new ulong[length];
			this.length = length;
			
			System.Buffer.BlockCopy (data, offset, this.text, 0, count);
			
			this.accMarkers       = 0;
			this.accMarkersValid = false;
		}
		
		
		public static void ShuffleEnd(TextChunk a, TextChunk b, int offset)
		{
			//	Réorganisation de la fin du texte de 'a' : déplace tout ce qui
			//	dépasse l'offset spécifié de 'a' vers 'b' (texte et curseurs).
			
			if (offset >= a.length)
			{
				return;
			}
			
			//	Copie le texte de la fin de 'a' vers le début de 'b' :
			
			int length = a.length - offset;
			
			Debug.Assert.IsTrue (length > 0);
			
			if (b.length + length > b.text.Length)
			{
				//	Il n'y a plus assez de place dans le buffer actuel de 'b'.
				//	Il faut donc agrandir celui-ci.
				
				b.GrowTextBuffer (b.length + length);
			}
			
			System.Buffer.BlockCopy (b.text, 0, b.text, 8*length, 8*b.length);
			System.Buffer.BlockCopy (a.text, 8*offset, b.text, 0, 8*length);
			
			a.length  = offset;
			b.length += length;
			
			//	Déplace aussi les curseurs. Commence par ajuster la position du
			//	premier curseur dans 'b' (s'il y en a un) :
			
			b.cursors.ProcessInsertion (0, length);
			
			//	Ensuite, il faut déplacer les curseurs de 'a' situés après l'offset
			//	vers 'b' :
			
			a.cursors.ProcessMigration (offset, ref b.cursors);
			
			if (a.accMarkers != 0)
			{
				a.accMarkersValid = false;
				b.accMarkersValid = false;
			}
		}
		
		
		public void GrowTextBuffer(int length)
		{
			length = System.Math.Max (length, this.text.Length + this.text.Length / 4 + 16);
			
			ulong[] oldText = this.text;
			ulong[] newText = new ulong[length];
			
			System.Buffer.BlockCopy (oldText, 0, newText, 0, 8*this.length);
			
			this.text = newText;
		}
		
		public void OptimizeTextBuffer()
		{
			int delta = this.text.Length - this.length;
			
			Debug.Assert.IsTrue (delta >= 0);
			
			if (delta > 4)
			{
				ulong[] oldText = this.text;
				ulong[] newText = new ulong[this.length];
				
				System.Buffer.BlockCopy (oldText, 0, newText, 0, 8*this.length);
				
				this.text = newText;
			}
		}
		
		
		public void AddCursor(Internal.CursorId id, int position, CursorAttachment attachment)
		{
			this.cursors.Add (id, position, attachment);
		}
		
		public void RemoveCursor(Internal.CursorId id)
		{
			this.cursors.Remove (id);
		}
		
		public void MoveCursor(Internal.CursorId id, int position)
		{
			Debug.Assert.IsInBounds (position, 0, this.length);
			
			this.cursors.Move (id, position);
		}
		
		
		public int GetCursorPosition(Internal.CursorId id)
		{
			return this.cursors.GetCursorPosition (id);
		}
		
		public int GetCursorCount()
		{
			return this.cursors.ElementCount;
		}
		
		public int GetCursorIndexBeforePosition(int position)
		{
			return this.cursors.GetCursorElementBeforePosition (position);
		}
		
		public Internal.CursorId GetNthCursorId(int index)
		{
			return this.cursors.GetElementCursorId (index);
		}
		
		public int GetNthCursorOffset(int index)
		{
			return this.cursors.GetElementCursorOffset (index);
		}
		
		
		private CursorIdArray				cursors;
		private System.UInt64[]				text;
		private int							length;
		
		private ulong						accMarkers;
		private bool						accMarkersValid;
	}
}
