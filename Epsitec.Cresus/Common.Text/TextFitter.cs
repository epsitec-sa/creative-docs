//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for TextFitter.
	/// </summary>
	public class TextFitter
	{
		public TextFitter(TextStory story)
		{
			this.story        = story;
			this.cursors      = new System.Collections.ArrayList ();
			this.free_cursors = new System.Collections.Stack ();
		}
		
		
		public TextStory						TextStory
		{
			get
			{
				return this.story;
			}
		}
		
		public int								CursorCount
		{
			get
			{
				return this.cursors.Count - this.free_cursors.Count;
			}
		}
		
		
		
		protected Cursors.FitterCursor NewCursor()
		{
			//	Retourne un curseur tout neuf (ou reprend un curseur qui a été
			//	recyclé précédemment, pour éviter de devoir en allouer à tour
			//	de bras).
			
			if (this.free_cursors.Count > 0)
			{
				return this.free_cursors.Pop () as Cursors.FitterCursor;
			}
			
			Cursors.FitterCursor cursor = new Cursors.FitterCursor ();
			
			this.cursors.Add (cursor);
			
			return cursor;
		}
		
		protected void RecycleCursor(ICursor cursor)
		{
			//	Recycle le curseur passé en entrée. Il est simplement placé
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.free_cursors.Contains (cursor));
			
			this.free_cursors.Push (cursor);
		}
		
		
		
		private TextStory						story;
		private System.Collections.ArrayList	cursors;
		private System.Collections.Stack		free_cursors;
	}
}
