//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe FrameList g�re la liste des cadres utilis�s pour couler le
	/// texte.
	/// </summary>
	public sealed class FrameList
	{
		public FrameList(TextFitter fitter)
		{
			this.fitter     = fitter;
			this.frames     = new System.Collections.ArrayList ();
			this.cursor_map = new System.Collections.Hashtable ();
		}
		
		
		public ITextFrame						this[int index]
		{
			get
			{
				return this.frames[index] as ITextFrame;
			}
		}
		
		public int								Count
		{
			get
			{
				return this.frames.Count;
			}
		}
		
		public TextFitter						TextFitter
		{
			get
			{
				return this.fitter;
			}
		}
		
		public IPageCollection					PageCollection
		{
			get
			{
				return this.fitter.PageCollection;
			}
		}
		
		
		public void ClearCursorMap()
		{
			this.cursor_map.Clear ();
		}
		
		
		public int IndexOf(ITextFrame frame)
		{
			return this.frames.IndexOf (frame);
		}

		public Cursors.FitterCursor FindLastCursor(int index)
		{
			//	Recherche le dernier curseur contenu dans le texte qui d�crit
			//	un paragraphe occupant au moins partiellement le ITextFrame
			//	� trouver :

			TextStory          story  = this.TextFitter.TextStory;
			Internal.TextTable text   = story.TextTable;
			int                length = story.TextLength;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFrameFilter (index);
			CursorInfo[]       infos  = text.FindCursors (0, length, filter, false);

			Debug.Assert.IsInBounds (index, 0, this.frames.Count-1);

			if (infos.Length > 0)
			{
				return text.GetCursorInstance (infos[infos.Length-1].CursorId) as Cursors.FitterCursor;
			}
			else
			{
				return null;
			}
		}
		
		public Cursors.FitterCursor FindFirstCursor(int index)
		{
			//	Trouve le curseur correspondant au paragraphe qui se trouve au
			//	d�but du ITextFrame.
			
			//	Note: un paragraphe peut couvrir plusieurs ITextFrame, ce qui
			//	complique l�g�rement les choses.
			
			ITextFrame frame = this[index];
			
			if (this.cursor_map.Contains (frame))
			{
				return this.cursor_map[frame] as Cursors.FitterCursor;
			}
			
			//	Recherche le premier curseur contenu dans le texte qui d�crit
			//	un paragraphe occupant au moins partiellement le ITextFrame
			//	� trouver :
			
			TextStory          story  = this.TextFitter.TextStory;
			Internal.TextTable text   = story.TextTable;
			int                length = story.TextLength;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFrameFilter (index);
			CursorInfo[]       infos  = text.FindCursors (0, length, filter, true);
			
			Debug.Assert.IsInBounds (index, 0, this.frames.Count-1);
			
			if (infos.Length == 1)
			{
				Cursors.FitterCursor cursor = text.GetCursorInstance (infos[0].CursorId) as Cursors.FitterCursor;
				
				this.cursor_map[frame] = cursor;
				return cursor;
			}
			
			return null;
		}
		
		public Cursors.FitterCursor FindFirstCursor(ITextFrame frame)
		{
			if (this.cursor_map.Contains (frame))
			{
				return this.cursor_map[frame] as Cursors.FitterCursor;
			}
			
			return this.FindFirstCursor (this.IndexOf (frame));
		}
		
		
		public void Add(ITextFrame frame)
		{
			this.InsertAt (this.frames.Count, frame);
		}
		
		
		public void InsertAt(int index, ITextFrame new_frame)
		{
			this.frames.Insert (index, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		public void InsertBefore(ITextFrame existing_frame, ITextFrame new_frame)
		{
			Debug.Assert.IsFalse (this.frames.Contains (new_frame));
			Debug.Assert.IsTrue (this.frames.Contains (existing_frame));
			
			this.frames.Insert (this.frames.IndexOf (existing_frame)+0, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		public void InsertAfter(ITextFrame existing_frame, ITextFrame new_frame)
		{
			Debug.Assert.IsFalse (this.frames.Contains (new_frame));
			Debug.Assert.IsTrue (this.frames.Contains (existing_frame));
			
			this.frames.Insert (this.frames.IndexOf (existing_frame)+1, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		
		public void Remove(ITextFrame frame)
		{
			this.frames.Remove (frame);
			this.HandleRemoval (frame);
		}
		
		
		public void Reset(System.Collections.ICollection frames)
		{
			this.frames.Clear ();
			this.frames.AddRange (frames);
			this.ClearCursorMap ();
		}
		
		
		private void HandleInsertion(ITextFrame frame)
		{
			this.ClearCursorMap ();
		}
		
		private void HandleRemoval(ITextFrame frame)
		{
			this.ClearCursorMap ();
		}
		
		
		
		private TextFitter						fitter;
		private System.Collections.ArrayList	frames;
		private System.Collections.Hashtable	cursor_map;		//	cache: ITextFrame --> FitterCursor
	}
}
