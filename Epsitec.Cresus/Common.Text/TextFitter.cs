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
			
			this.frame_list      = new FrameList (this);
			this.page_collection = new DefaultPageCollection ();
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
		
		
		public FrameList						FrameList
		{
			get
			{
				return this.frame_list;
			}
		}
		
		public IPageCollection					PageCollection
		{
			get
			{
				return this.page_collection;
			}
			set
			{
				if (this.page_collection != value)
				{
					this.page_collection = value;
				}
			}
		}
		
		
		public void ClearAllMarks()
		{
			this.Process (new Execute (this.ExecuteClear));
			this.frame_list.ClearCursorMap ();
		}
		
		public void GenerateAllMarks()
		{
			this.frame_index = 0;
			this.frame_y     = 0;
			
			this.Process (new Execute (this.ExecuteGenerate));
			
			this.frame_list.ClearCursorMap ();
		}
		
		
		public void RenderParagraph(ICursor cursor, ITextRenderer renderer)
		{
			Cursors.FitterCursor c = cursor as Cursors.FitterCursor;
			
			if (c == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "cursor");
			}
			
			double oy = 0;
			double fence_before = 100;
			double fence_after  = 20;
			
			ulong[] text;
			int length = c.ParagraphLength;
			
			text   = new ulong[length];
			length = this.story.ReadText (c, length, text);
			
			Layout.Context layout = new Layout.Context (this.story.TextContext, text, 0, oy, 14.0, 1000, 0, 0, fence_before, fence_after);
			
			int n = c.Elements.Length;
			
			for (int i = 0; i < n; i++)
			{
				layout.RenderLine (renderer, c.Elements[i].Profile, c.Elements[i].Length, i, i == n-1);
				layout.TextOffset += c.Elements[i].Length;
			}
		}
		
		
		protected void Process(Execute method)
		{
			//	Exécute une méthode pour chaque tout le texte, en procédant par
			//	tranches (exécution itérative).
			
			int pos = 0;
			
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			this.story.NewCursor (cursor);
			
			try
			{
				for (;;)
				{
					//	TODO: lock et détection d'altérations du texte
					
					int max    = this.story.TextLength;
					int length = System.Math.Min (max - pos, 10000);
					
					if (length <= 0)
					{
						break;
					}
					
					method (cursor, pos, ref length);
					
					this.story.MoveCursor (cursor, length);
					pos += length;
				}
			}
			finally
			{
				this.story.RecycleCursor (cursor);
			}
		}
		
		protected void ExecuteClear(Cursors.TempCursor temp_cursor, int pos, ref int length)
		{
			//	Supprime les marques de découpe de lignes représentées par des
			//	curseurs (instances de Cursors.FitterCursor).
			
			CursorInfo[] cursors = this.story.TextTable.FindCursors (pos, length, Cursors.FitterCursor.Filter);
			
			for (int i = 0; i < cursors.Length; i++)
			{
				ICursor cursor = this.story.TextTable.GetCursorInstance (cursors[i].CursorId);
				this.RecycleCursor (cursor);
			}
		}
		
		protected void ExecuteGenerate(Cursors.TempCursor cursor, int pos, ref int length)
		{
			//	Génère les marques de découpe de lignes et insère les curseurs
			//	correspondants.
			
			ulong[] text;
			
			if (pos + length < story.TextLength)
			{
				text = new ulong[length];
				this.story.ReadText (cursor, length, text);
			}
			else
			{
				//	On arrive au bout du texte: il faut donc synthétiser un caractère
				//	supplémentaire de fin de texte pour que l'algorithme de layout
				//	soit satisfait :
				
				text = new ulong[length+1];
				this.story.ReadText (cursor, length, text);
				
				ulong code = text[length-1];
				
				code &= 0xffffffff00000000ul;
				code |= (int) Unicode.Code.EndOfText;
				
				Unicode.Bits.SetBreakInfo (ref code, Unicode.BreakInfo.Yes);
				
				text[length] = code;
			}
			
			Layout.Context         layout = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			Layout.BreakCollection result = new Layout.BreakCollection ();
			
			layout.SelectFrame (this.frame_index, this.frame_y);
			
			int line_count      = 0;
			int line_start      = 0;
			int paragraph_start = 0;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (;;)
			{
				Layout.Status status = layout.Fit (ref result, line_count);
				
				this.frame_index = layout.FrameIndex;
				this.frame_y     = layout.FrameY;
				
				switch (status)
				{
					case Layout.Status.ErrorNeedMoreText:
						length = paragraph_start;
						return;
					
					case Layout.Status.ErrorCannotFit:
						throw new System.InvalidOperationException ("Cannot fit.");
					
					case Layout.Status.Ok:
					case Layout.Status.OkFitEnded:
						break;
					
					default:
						throw new System.InvalidOperationException ("Invalid layout status received.");
				}
				
				//	Le système de layout propose un certain nombre de points de découpe
				//	possibles pour la ligne. Il faut maintenant déterminer lequel est le
				//	meilleur.
				
				int offset;
				int n_breaks = result.Count;
				
				Layout.StretchProfile profile;
				
				if (n_breaks > 1)
				{
					double penalty = Layout.StretchProfile.MaxPenalty;
					int    p_index = -1;
					
					for (int i = 0; i < n_breaks; i++)
					{
						if (result[i].Penalty < penalty)
						{
							penalty = result[i].Penalty;
							p_index = i;
						}
					}
					
					offset  = result[p_index].Offset;
					profile = result[p_index].Profile;
				}
				else
				{
					offset  = result[0].Offset;
					profile = result[0].Profile;
				}
				
				Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
				
				bool end_of_text = false;
				
				if (pos + offset > story.TextLength)
				{
					offset     -= 1;
					end_of_text = true;
				}
				
				element.Length  = offset - line_start;
				element.Profile = profile;
				element.FrameIndex = layout.FrameIndex;
				element.FrameY     = layout.FrameY;
				element.FrameWidth = layout.LineWidth;
				
				list.Add (element);
				
				layout.TextOffset = offset;
				
				if (status == Layout.Status.OkFitEnded)
				{
					Cursors.FitterCursor mark = this.NewCursor ();
					
					mark.AddRange (list);
					list.Clear ();
					
					story.MoveCursor (mark, pos + paragraph_start);
					
					line_start      = offset;
					paragraph_start = offset;
					line_count      = 0;
				}
				else
				{
					line_start = offset;
					line_count++;
				}
				
				if (end_of_text)
				{
					length = paragraph_start;
					return;
				}
			}
		}
		
		
		protected Cursors.FitterCursor NewCursor()
		{
			//	Retourne un curseur tout neuf (ou reprend un curseur qui a été
			//	recyclé précédemment, pour éviter de devoir en allouer à tour
			//	de bras).
			
			Cursors.FitterCursor cursor;
			
			if (this.free_cursors.Count > 0)
			{
				cursor = this.free_cursors.Pop () as Cursors.FitterCursor;
			}
			else
			{
				cursor = new Cursors.FitterCursor ();
				this.cursors.Add (cursor);
			}
			
			this.story.NewCursor (cursor);
			
			return cursor;
		}
		
		protected void RecycleCursor(ICursor cursor)
		{
			//	Recycle le curseur passé en entrée. Il est simplement placé
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.free_cursors.Contains (cursor));
			
			this.story.RecycleCursor (cursor);
			
			this.free_cursors.Push (cursor);
		}
		
		
		protected delegate void Execute(Cursors.TempCursor cursor, int pos, ref int length);
		
		private TextStory						story;
		private System.Collections.ArrayList	cursors;
		private System.Collections.Stack		free_cursors;
		
		private FrameList						frame_list;
		private int								frame_index;
		private double							frame_y;
		
		private IPageCollection					page_collection;
	}
}
