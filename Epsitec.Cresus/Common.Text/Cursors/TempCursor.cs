//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Cursors
{
	/// <summary>
	/// La classe TempCursor d�crit un curseur temporaire (r�serv� � un usage
	/// interne, les modifications d'un curseur temporaire ne sont pas sauv�es
	/// pour le undo/redo).
	/// </summary>
	public class TempCursor : ICursor
	{
		public TempCursor()
		{
		}
		
		
		#region ICursor Members
		public int								CursorId
		{
			get
			{
				return this.cursor_id;
			}
			set
			{
				this.cursor_id = value;
			}
		}
		
		public CursorAttachment					Attachment
		{
			get
			{
				return CursorAttachment.Temporary;
			}
		}
		#endregion
		
		private Internal.CursorId				cursor_id;
	}
}
