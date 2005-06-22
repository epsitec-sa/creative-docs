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
		
		
		public static System.Collections.IComparer GetPositionComparer(TextStory story)
		{
			return new PositionComparer (story);
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
		
		public virtual CursorAttachment			Attachment
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
		
		#region PositionComparer Class
		private class PositionComparer : System.Collections.IComparer
		{
			public PositionComparer(TextStory story)
			{
				this.story = story;
			}
			
			#region IComparer Members
			public int Compare(object x, object y)
			{
				ICursor cx = x as ICursor;
				ICursor cy = y as ICursor;
				
				int px = story.GetCursorPosition (cx);
				int py = story.GetCursorPosition (cy);
				
				if (px < py) return -1;
				if (px > py) return 1;
				
				return 0;
			}
			#endregion
			
			private TextStory					story;
		}
		#endregion
		
		private Internal.CursorId				cursor_id;
	}
}
