//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Cursors
{
	/// <summary>
	/// La classe TempCursor décrit un curseur temporaire (réservé à un usage
	/// interne, les modifications d'un curseur temporaire ne sont pas sauvées
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
				return this.cursorId;
			}
			set
			{
				this.cursorId = value;
			}
		}
		
		public virtual CursorAttachment			Attachment
		{
			get
			{
				return CursorAttachment.Temporary;
			}
		}
		
		public virtual int						Direction
		{
			get
			{
				return 0;
			}
			set
			{
//				System.Diagnostics.Debug.Assert (value == 0);
			}
		}
		
		
		public virtual void Clear()
		{
		}
		#endregion
		
		private Internal.CursorId				cursorId;
	}
}
