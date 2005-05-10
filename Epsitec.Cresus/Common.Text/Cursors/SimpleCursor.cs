//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Cursors
{
	/// <summary>
	/// La classe SimpleCursor décrit un curseur tout simple (il ne stocke que
	/// l'identificateur interne du curseur utilisé dans TextStory).
	/// </summary>
	public class SimpleCursor : ICursor
	{
		public SimpleCursor()
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
				return CursorAttachment.Floating;
			}
		}
		
		
		public virtual void Clear()
		{
		}
		#endregion
		
		private Internal.CursorId				cursor_id;
	}
}
