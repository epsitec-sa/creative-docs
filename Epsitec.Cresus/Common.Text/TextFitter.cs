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
		
		
		
		public void ClearAllMarks()
		{
			this.Process (new Execute (this.ExecuteClear));
		}
		
		public void GenerateAllMarks()
		{
			this.Process (new Execute (this.ExecuteGenerate));
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
			
			double oy = 0;
			double mx_left  = 20;
			double mx_right = 1000;
			double fence_before = 100;
			double fence_after  = 20;
			
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
			
			Layout.Context         layout = new Layout.Context (this.story.Context, text, 0, oy, mx_left, mx_right, fence_before, fence_after);
			Layout.BreakCollection result = new Layout.BreakCollection ();
			
			int line_start      = 0;
			int paragraph_start = 0;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (;;)
			{
				Layout.Status status = layout.Fit (ref result);
				
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
				
				element.Length  = offset - line_start;
				element.Profile = profile;
				
				list.Add (element);
				
				layout.TextOffset = offset;
				
				if (status == Layout.Status.OkFitEnded)
				{
					Cursors.FitterCursor mark = this.NewCursor ();
					
					mark.AddRange (list);
					list.Clear ();
					
					story.MoveCursor (mark, pos + line_start);
					
					line_start      = offset;
					paragraph_start = offset;
				}
				else
				{
					line_start = offset;
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
	}
}
