//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// Summary description for TextChunk.
	/// </summary>
	internal class TextChunk
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
		
		
		public void InsertText(int position, uint[] text)
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
			
			this.cursors.ProcessRemoval (position, length);
		}
		
		
		public void CopyTextToBuffer(int position, int length, uint[] buffer)
		{
			Debug.Assert.IsTrue (position >= 0);
			Debug.Assert.IsTrue (length >= 0);
			Debug.Assert.IsTrue (length <= buffer.Length);
			Debug.Assert.IsTrue (position + length <= this.length);
			
			System.Buffer.BlockCopy (this.text, 8*position, buffer, 0, 8*length);
		}
		
		public void CopyTextToBuffer(int position, int length, uint[] buffer, int offset)
		{
			Debug.Assert.IsTrue (position >= 0);
			Debug.Assert.IsTrue (length >= 0);
			Debug.Assert.IsTrue (offset >= 0);
			Debug.Assert.IsTrue (position + length <= this.length);
			Debug.Assert.IsTrue (offset + length <= buffer.Length);
			
			System.Buffer.BlockCopy (this.text, 8*position, buffer, 8*offset, 8*length);
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
