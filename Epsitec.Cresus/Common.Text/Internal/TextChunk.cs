//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// Summary description for TextChunk.
	/// </summary>
	internal sealed class TextChunk
	{
		public TextChunk()
		{
			this.cursors = new CursorIdArray ();
			this.text    = new ulong[0];
		}
		
		
		public int							TextLength
		{
			get
			{
				return this.length;
			}
		}
		
		public Internal.CursorIdArray		Cursors
		{
			get
			{
				return this.cursors;
			}
		}
		
		public ulong						this[Internal.TextChunkId position]
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
			
			int offset_1 = position;
			int offset_2 = offset_1 + length;
			int count    = this.length - offset_1;
			
			Debug.Assert.IsTrue (offset_1 >= 0);
			Debug.Assert.IsTrue (offset_2+count <= this.text.Length);
			
			//	Creuse un trou pour y mettre le nouveau texte, puis copie le texte
			//	dans le trou et enfin, déplace les curseurs.
			
			System.Buffer.BlockCopy (this.text, 8*offset_1, this.text, 8*offset_2, 8*count);
			System.Buffer.BlockCopy (text, 0, this.text, 8*offset_1, 8*length);
			
			this.length += length;
			
			//	TODO: support pour le undo
			
			this.cursors.ProcessInsertion (position, length);
		}
		
		public void RemoveText(int position, int length)
		{
			Debug.Assert.IsTrue (position >= 0);
			Debug.Assert.IsTrue (length >= 0);
			Debug.Assert.IsTrue (position + length <= this.length);
			
			int offset_1 = position + length;
			int offset_2 = position;
			int count    = this.length - offset_2;
			
			Debug.Assert.IsTrue (offset_2 >= 0);
			Debug.Assert.IsTrue (offset_1+count <= this.length);
			
			System.Buffer.BlockCopy (this.text, 8*offset_1, this.text, 8*offset_2, 8*count);
			
			this.length -= length;
			
			//	TODO: support pour le undo
			
			this.cursors.ProcessRemoval (position, length);
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
		
		
		public void SaveRawText(byte[] buffer, out int length)
		{
			int count = 8*this.length;
			
			Debug.Assert.IsTrue (buffer.Length >= count);
			
			System.Buffer.BlockCopy (this.text, 0, buffer, 0, count);
			
			length = this.length;
		}
		
		public void LoadRawText(byte[] data, int length)
		{
			this.text   = new ulong[length];
			this.length = length;
			
			System.Buffer.BlockCopy (data, 0, this.text, 0, 8*length);
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
			
			a.cursors.ProcessMigration (offset, b.cursors);
		}
		
		
		private void GrowTextBuffer(int length)
		{
			length = System.Math.Max (length, this.text.Length + this.text.Length / 4 + 16);
			
			ulong[] old_text = this.text;
			ulong[] new_text = new ulong[length];
			
			System.Buffer.BlockCopy (old_text, 0, new_text, 0, 8*this.length);
			
			this.text = new_text;
		}
		
		private void OptimizeTextBuffer()
		{
			if (this.text.Length > this.length)
			{
				ulong[] old_text = this.text;
				ulong[] new_text = new ulong[this.length];
				
				System.Buffer.BlockCopy (old_text, 0, new_text, 0, 8*this.length);
				
				this.text = new_text;
			}
			
			Debug.Assert.IsTrue (this.text.Length == this.length);
		}
		
		
		private CursorIdArray				cursors;
		private System.UInt64[]				text;
		private int							length;
	}
}
