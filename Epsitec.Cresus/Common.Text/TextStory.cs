//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStory représente un texte complet, avec tous ses attributs
	/// typographiques, ses curseurs, sa gestion du undo, etc.
	/// </summary>
	public class TextStory
	{
		public TextStory()
		{
			this.text = new Internal.TextTable ();
		}
		
		
		public int								TextLength
		{
			get
			{
				return this.text.TextLength;
			}
		}
		
		
		public int NewCursor()
		{
			return 0;
		}
		
		
		public void InsertText(int cursor_id, ulong[] text)
		{
		}
		
		
		private Internal.TextTable				text;
	}
}
