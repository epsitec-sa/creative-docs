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
			this.RenderParagraphInTextFrame (cursor, renderer, null);
		}
		
		public void RenderParagraphInTextFrame(ICursor cursor, ITextRenderer renderer, ITextFrame frame)
		{
			Cursors.FitterCursor c = cursor as Cursors.FitterCursor;
			
			if (c == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "cursor");
			}
			
			ulong[] text;
			int length = c.ParagraphLength;
			
			text   = new ulong[length];
			length = this.story.ReadText (c, length, text);
			
			Layout.Context layout = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			
			int n = c.Elements.Length;
			
			for (int i = 0; i < n; i++)
			{
				int index = c.Elements[i].FrameIndex;
				int count = c.Elements[i].Length;
				
				layout.SelectFrame (index, 0);
				
				if ((frame == null) ||
					(layout.Frame == frame))
				{
					if (count > 0)
					{
						double ox    = c.Elements[i].LineBaseX;
						double oy    = c.Elements[i].LineBaseY;
						double width = c.Elements[i].LineWidth;
						double asc   = c.Elements[i].LineAscender;
						double desc  = c.Elements[i].LineDescender;
						
						layout.Frame.MapToView (ref ox, ref oy);
						
						if (renderer.IsFrameAreaVisible (layout.Frame, ox, oy+desc, width, asc+desc))
						{
							Layout.StretchProfile profile = c.Elements[i].Profile;
							layout.RenderLine (renderer, profile, count, ox, oy, width, i, i == n-1);
						}
					}
				}
				
				layout.TextOffset += count;
			}
		}
		
		public void RenderTextFrame(ITextFrame frame, ITextRenderer renderer)
		{
			int index = this.frame_list.IndexOf (frame);
			
			if (index < 0)
			{
				throw new System.ArgumentException ("Not a valid ITextFrame.", "frame");
			}
			
			Cursors.FitterCursor cursor = this.frame_list.FindFirstCursor (index);
			Internal.TextTable   text   = this.story.TextTable;
			
			while (cursor != null)
			{
				if (cursor.ContainsFrameIndex (index) == false)
				{
					break;
				}
				
				this.RenderParagraphInTextFrame (cursor, renderer, frame);
				
				//	Trouve le curseur du paragraphe suivant :
				
				CursorInfo[] cursors = text.FindNextCursor (cursor.CursorId, Cursors.FitterCursor.Filter);
				
				if (cursors.Length == 1)
				{
					cursor = text.GetCursorInstance (cursors[0].CursorId) as Cursors.FitterCursor;
				}
				else
				{
					break;
				}
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
					//	TODO: lock et détection d'altérations du texte (et de la liste des
					//	ITextFrame liés à ce TextFitter).
					
					int max    = this.story.TextLength;
					int step   = 10000;
					
				again:
					int length = System.Math.Min (max - pos, step);
					
					if (length <= 0)
					{
						break;
					}
					
					method (cursor, pos, ref length);
					
					if (length == 0)
					{
						//	La méthode de traitement n'a rien pu faire avec le texte passé
						//	en entrée, vraisemblablement parce que le paragraphe mesure plus
						//	de 'step' caractères. Il faut donc augmenter cette limite et
						//	tenter un nouvel essai.
						
						Debug.Assert.IsTrue (length < max-pos);
						
						step += 10000;
						goto again;
					}
					
					if (length < 0)
					{
						//	Avortement demandé par la méthode de traitement.
						
						break;
					}
					
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
				this.RecycleFitterCursor (cursor);
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
			
			bool continuation = false;
			bool reset_line_h = true;
			
			int def_line_count = line_count;
			int def_line_start = line_start;
			int def_list_count = list.Count;
			
			int    def_frame_index = layout.FrameIndex;
			double def_frame_y     = layout.FrameY;
			
			for (;;)
			{
				if (reset_line_h)
				{
					layout.ResetLineHeight ();
					reset_line_h = false;
				}
				
				Properties.TabProperty tab_property;
				Layout.Status status = layout.Fit (ref result, line_count, continuation);
				
				bool tab_new_line = false;
				bool end_of_text  = false;
				
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
						continuation = false;
						break;
					
					case Layout.Status.OkFitEnded:
						continuation = false;
						reset_line_h = true;
						break;
					
					case Layout.Status.RestartLayout:
						
						Debug.Assert.IsTrue (continuation);
						
						continuation = false;
						line_count   = def_line_count;
						line_start   = def_line_start;
						
						layout.MoveTo (0, line_start);
						layout.SelectFrame (def_frame_index, def_frame_y);
						
						list.RemoveRange (def_list_count, list.Count - def_list_count);
						
						continue;
					
					case Layout.Status.OkTabReached:
						
						//	On vient de trouver une marque de tabulation. Il faut
						//	l'analyser et déterminer si elle peut être placée sur
						//	la même ligne que le texte qui précède :
						
						layout.TextContext.GetTab (text[layout.TextOffset-1], out tab_property);
						
						Debug.Assert.IsNotNull (tab_property);
						
						double tab_x;
						double tab_dx;
						
						TabStatus tab_status = this.MeasureTabTextWidth (layout, tab_property, line_count, out tab_x, out tab_dx);
						
						System.Console.Out.WriteLine ("Suggested start for tabbed text: {0:0.00}, status: {1}, width: {2:0.00}", tab_x, tab_status, tab_dx);
						
						if (tab_status == TabStatus.ErrorNeedMoreText)
						{
							length = paragraph_start;
							return;
						}
						else if (tab_status == TabStatus.ErrorNeedMoreRoom)
						{
							//	Le tabulateur ne tient plus sur cette ligne. Force un passage
							//	à la ligne.
							
							tab_new_line = true;
							
							layout.MoveTo (tab_x, layout.TextOffset);
						}
						else if (tab_status == TabStatus.Ok)
						{
							//	Le tabulateur occupe la même ligne que le texte qui précède.
							
							layout.MoveTo (tab_x, layout.TextOffset);
						}
						else
						{
							throw new System.NotImplementedException ();
						}
						
						continuation = true;
						
						break;
					
					case Layout.Status.ErrorNeedMoreRoom:
						end_of_text = true;
						break;
					
					default:
						throw new System.InvalidOperationException (string.Format ("Invalid layout status received: {0}.", status));
				}
				
				//	Le système de layout propose un certain nombre de points de découpe
				//	possibles pour la ligne. Il faut maintenant déterminer lequel est le
				//	meilleur.
				
				int offset   = line_start;
				int n_breaks = result.Count;
				
				Layout.StretchProfile profile = null;
				
				if (n_breaks > 1)
				{
					double penalty = Layout.StretchProfile.MaxPenalty;
					int    p_index = -1;
					
					for (int i = 0; i < n_breaks; i++)
					{
						double computed_penalty = this.ComputePenalty (result[i].SpacePenalty, result[i].BreakPenalty);
						
						if (computed_penalty < penalty)
						{
							penalty = computed_penalty;
							p_index = i;
						}
					}
					
					offset  = result[p_index].Offset;
					profile = result[p_index].Profile;
				}
				else if (n_breaks == 1)
				{
					offset  = result[0].Offset;
					profile = result[0].Profile;
				}
				
				if (n_breaks > 0)
				{
					Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
					
					if (pos + offset > story.TextLength)
					{
						Debug.Assert.IsTrue (status == Layout.Status.OkFitEnded);
						
						offset     -= 1;
						end_of_text = true;
					}
					
					element.Length     = offset - line_start;
					element.Profile    = profile;
					element.FrameIndex = layout.FrameIndex;
					element.LineBaseX  = layout.StartX;
					element.LineBaseY  = layout.Y;
					element.LineWidth  = continuation ? result[result.Count-1].Profile.TotalWidth : layout.AvailableWidth;
					
					element.LineAscender  = layout.LineAscender;
					element.LineDescender = layout.LineDescender;
					
					list.Add (element);
				}
				
				layout.TextOffset = offset;
				
				if (status == Layout.Status.OkTabReached)
				{
					line_start = offset;
					
					if (tab_new_line)
					{
						line_count++;
					}
					else
					{
						layout.SelectFrame (def_frame_index, def_frame_y);
					}
				}
				else
				{
					if (status == Layout.Status.ErrorNeedMoreRoom)
					{
						//	Le paragraphe a été tronqué, car il n'y a plus assez de place dans
						//	les ITextFrame pour y placer le texte. Il faut générer un Element
						//	décrivant la fin du paragraphe :
						
						Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
						
						int paragraph_length = this.ComputeParagraphLength (text, line_start);
						
						if (paragraph_length < 0)
						{
							//	La fin du paragraphe n'a pas pu être trouvée; demande à l'appelant
							//	une nouvelle passe avec plus de texte.
							
							length = paragraph_start;
							return;
						}
						
						element.Length     = paragraph_length;
						element.FrameIndex = -1;
						
						list.Add (element);
					}
					
					if ((status == Layout.Status.OkFitEnded) ||
						(status == Layout.Status.ErrorNeedMoreRoom))
					{
						Debug.Assert.IsTrue (list.Count > 0);
						
						Cursors.FitterCursor mark = this.NewFitterCursor ();
						
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
					
					def_line_count  = line_count;
					def_line_start  = line_start;
					def_list_count  = list.Count;
					
					def_frame_index = layout.FrameIndex;
					def_frame_y     = layout.FrameY;
				}

				if (end_of_text)
				{
					length = -1;
					return;
				}
			}
		}
		
		
		protected enum TabStatus
		{
			Ok,
			ErrorNeedMoreText,
			ErrorNeedMoreRoom,
			ErrorCannotFit
		}
		
		protected TabStatus MeasureTabTextWidth(Layout.Context layout, Properties.TabProperty tab_property, int line_count, out double tab_x, out double width)
		{
			tab_x = 0;
			width = 0;
			
			double d = tab_property.Disposition;
			
			double x1 = layout.X;
			double x2 = tab_property.Position;
			double x3 = layout.LineWidth - layout.RightMargin;
			
			double x_before = x2 - x1;
			double x_after  = x3 - x2;
			
			TabStatus              status  = TabStatus.Ok;
			Layout.BreakCollection result  = new Layout.BreakCollection ();
			Layout.Context         scratch = new Layout.Context (layout.TextContext, layout.Text, 0, null);
			
			scratch.ResetLineHeight ();
			scratch.RecordAscender (layout.LineAscender);
			scratch.RecordDescender (layout.LineDescender);
			scratch.RecordLineHeight (layout.LineHeight);
			
			Debug.Assert.IsTrue (layout.Disposition == 0);
			Debug.Assert.IsTrue (layout.Justification == 0);
			
			if ((x_before <= 0) ||
				(x_after <= 0))
			{
				//	Tabulateur mal placé... Demande un saut de ligne ! Mais on
				//	calcule encore au préalable la position qu'occupera le texte
				//	tabulé sur la ligne suivante :
				
				scratch.SelectMargins (line_count);
				
				x1 = scratch.LeftMargin;
				
				x_before = x2 - x1;
				x_after  = x3 - x2;
				
				status = TabStatus.ErrorNeedMoreRoom;
			}
			
			double room;
			double room_after;
			double room_before;

			
			//	Détermine la place disponible entre le texte qui se trouve
			//	avant le tabulateur et la marge droite, en tenant compte de
			//	la manière dont le texte est disposé.
			
			if (d < 0.5)
			{
				double ratio = d / (1-d);				//	plutôt tabulateur aligné à gauche
				
				room_after  = x_after;
				room_before = x_after * ratio;
				
				if (x_before < room_before)
				{
					room_before = x_before;
					room_after  = x_before / ratio;
				}
			}
			else
			{
				double ratio = (1-d) / d;				//	plutôt tabulateur aligné à droite
				
				room_before = x_before;
				room_after  = x_before * ratio;
				
				if (x_after < room_after)
				{
					room_after  = x_after;
					room_before = x_after / ratio;
				}
			}
			
			room = room_before + room_after;
			
			scratch.DefineAvailableWidth (room);
			scratch.MoveTo (0, layout.TextOffset);
			
			Layout.Status fit_status = scratch.Fit (ref result, line_count, true);
			
			if (fit_status == Layout.Status.ErrorNeedMoreText)
			{
				return TabStatus.ErrorNeedMoreText;
			}
			
			if ((fit_status == Layout.Status.Ok) ||
				(fit_status == Layout.Status.OkFitEnded) ||
				(fit_status == Layout.Status.OkTabReached))
			{
				//	TODO: sélectionner le résultat optimal
				
				width = result[result.Count-1].Profile.TotalWidth;
				tab_x = x2 - d * width;
				
				return status;
			}
			
			return TabStatus.ErrorCannotFit;
		}
		
		protected double ComputePenalty(double space_penalty, double break_penalty)
		{
			return space_penalty + break_penalty;
		}
		
		protected int ComputeParagraphLength(ulong[] text, int offset)
		{
			//	Détermine la longueur du paragraphe dans le texte passé en entrée, en
			//	commençant à l'offset indiqué. Une marque de fin de texte ne fait pas
			//	partie du paragraphe, alors qu'une marque de fin de paragraphe si.
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			for (int i = offset; i < text.Length; i++)
			{
				int code = Unicode.Bits.GetCode (text[i]);
				
				if (code == (int) Unicode.Code.EndOfText)
				{
					return i - offset;
				}
				
				if (analyzer.IsBreak (code))
				{
					return i - offset + 1;
				}
			}
			
			return -1;
		}
		
		
		protected Cursors.FitterCursor NewFitterCursor()
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
		
		protected void RecycleFitterCursor(ICursor cursor)
		{
			//	Recycle le curseur passé en entrée. Il est simplement placé
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.free_cursors.Contains (cursor));
			
			this.story.RecycleCursor (cursor);
			
			cursor.Clear ();
			
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
